using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using Unity.Profiling;
#endif

public class DataManager : MonoBehaviour
{
    public static DataManager instance = null;

#if UNITY_EDITOR
    private static readonly ProfilerMarker s_ProfileSaveRunTotal = new ProfilerMarker("DataManager.SaveRunData");
    private static readonly ProfilerMarker s_ProfileSaveRunToSaveData = new ProfilerMarker("DataManager.SaveRunData.ToSaveData");
    private static readonly ProfilerMarker s_ProfileSaveRunToJson = new ProfilerMarker("DataManager.SaveRunData.ToJson");
    private static readonly ProfilerMarker s_ProfileSaveRunWriteAtomic = new ProfilerMarker("DataManager.SaveRunData.WriteAtomic");
#endif

    private string path;

    private PlayerData playerData;

    // --- RunData 비동기 저장 파이프라인 ---
    private volatile int runSaveGeneration;              // 저장 요청 세대 번호 (lock 안에서 쓰고, 워커에서 volatile 읽기)
    private CancellationTokenSource runSaveCts;          // 진행 중인 워커 취소용
    private Task runSaveTask;                            // 현재 백그라운드 저장 Task (메인 스레드용)
    private readonly object runSaveLock = new object();  // 세대·CTS 갱신 동기화

    private const int SAVE_RETRY_COUNT = 3;              // 디스크 쓰기 최대 재시도 횟수
    private const int SAVE_RETRY_BASE_DELAY_MS = 50;     // 재시도 간 기본 대기(지수 백오프 적용)
    private const int FLUSH_TIMEOUT_MS = 3000;           // pause/quit 시 저장 완료 대기 한도

    private void Awake()
    {
        // singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        path = Application.dataPath + "/Data/";

        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.WebGLPlayer)
        {
            path = Application.persistentDataPath;
        }
    }

    // 해금, 통계
    public void SavePlayerData(PlayerData playerData)
    {
        this.playerData = playerData;

        string savedJson = JsonUtility.ToJson(playerData, true);
        string dataPath = Path.Combine(path, "PlayerData.json");
        File.WriteAllText(dataPath, savedJson);
    }

    public void OnStatisticsPlayerDataRequested()
    {
        GameUIManager.instance.OnStatisticsPlayerDataCallback(playerData);
    }

    public void OnDeckSelectionPlayerDataRequested()
    {
        GameUIManager.instance.OnDeckSelectionPlayerDataCallback(playerData);
    }

    /// <summary>
    /// <c>RunData.json</c> 파일 존재 여부만 본다. JSON 파싱·버전·마이그레이션은 하지 않는다(오버헤드·메뉴 표시용).
    /// 실제 복원 가능 여부는 <see cref="TryLoadRunData"/>에서 판단한다.
    /// </summary>
    public bool HasValidRunSaveData()
    {
        string dataPath = Path.Combine(path, "RunData.json");
        return File.Exists(dataPath);
    }

    /// <summary>
    /// 저장된 런이 패배 상태인지 확인한다. 파일이 없으면 false.
    /// </summary>
    public bool IsRunDefeated()
    {
        string dataPath = Path.Combine(path, "RunData.json");
        if (!File.Exists(dataPath)) return false;

        string json = File.ReadAllText(dataPath);
        RunSaveData saveData = JsonUtility.FromJson<RunSaveData>(json);
        return saveData != null && saveData.isDefeated;
    }

    // 이어하기 — 메인 스레드에서 직렬화 후, 디스크 I/O는 백그라운드로 넘긴다.
    public void SaveRunData(RunData runData)
    {
        RunSaveData saveData;
        string jsonData;

#if UNITY_EDITOR
        using (s_ProfileSaveRunTotal.Auto())
        {
            using (s_ProfileSaveRunToSaveData.Auto())
                saveData = RunSaveMapper.ToSaveData(runData);

            using (s_ProfileSaveRunToJson.Auto())
                jsonData = JsonUtility.ToJson(saveData, true);
        }
#else
        saveData = RunSaveMapper.ToSaveData(runData);
        jsonData = JsonUtility.ToJson(saveData, true);
#endif

        EnqueueRunSave(jsonData);
    }

    /// <summary>직렬화된 JSON을 백그라운드 워커에 넘긴다. 이전 저장이 있으면 취소(coalesce).</summary>
    /// <remarks>메인 스레드 전용. 백그라운드에서 호출하지 않는다.</remarks>
    private void EnqueueRunSave(string jsonData)
    {
        int generation;
        CancellationToken token;

        lock (runSaveLock)
        {
            // 세대 증가 — 이후 stale 판별의 기준이 된다 
            generation = ++runSaveGeneration;

            // 기존 작업이 있다면 취소 신호만 보낸다
            if (runSaveCts != null)
                runSaveCts.Cancel();

            // 새 토큰 발급
            runSaveCts = new CancellationTokenSource();
            token = runSaveCts.Token;
        }

        // 클로저 캡처 시점의 경로를 고정하여 워커에 전달
        string directory = path;
        runSaveTask = Task.Run(() => RunSaveWorker(directory, jsonData, generation, token), token);
    }

    /// <summary>백그라운드 스레드에서 원자적 쓰기를 시도하고, 실패 시 지수 백오프로 재시도한다.</summary>
    private void RunSaveWorker(string directory, string jsonData, int generation, CancellationToken token)
    {
        for (int attempt = 0; attempt <= SAVE_RETRY_COUNT; attempt++)
        {
            // 새 세대가 이미 요청됐다면 즉시 종료
            if (token.IsCancellationRequested)
                return;

            try
            {
                WriteRunDataJsonAtomic(directory, jsonData, generation, token);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.Log("Finish Saving RunData");
#endif
                return;
            }
            catch (Exception exception)
            {
                // 최대 재시도 횟수 초과 — 로그 남기고 포기 (디스크엔 직전 성공본이 유지됨)
                if (attempt == SAVE_RETRY_COUNT)
                {
                    Debug.LogError($"[DataManager] RunData 저장 최종 실패 (generation={generation}): {exception}");
                    Firebase.Crashlytics.Crashlytics.LogException(exception);
                    return;
                }

                // 지수 백오프 후 재시도 (50ms → 100ms → 200ms)
                Debug.LogWarning($"[DataManager] RunData 저장 재시도 ({attempt + 1}/{SAVE_RETRY_COUNT}): {exception.Message}");
                Thread.Sleep(SAVE_RETRY_BASE_DELAY_MS * (1 << attempt));
            }
        }
    }

    /// <summary>
    /// 임시 파일에 쓴 뒤 세대·취소를 확인하고 원자적 교체를 수행한다.
    /// stale 세대이거나 취소된 경우 임시 파일만 정리하고 조용히 반환한다.
    /// </summary>
    private void WriteRunDataJsonAtomic(string directory, string jsonData, int generation, CancellationToken token)
    {
        string finalPath = Path.Combine(directory, "RunData.json");
        // 세대별 고유 경로 — 워커끼리 같은 파일을 공유하지 않아 락 없이도 충돌하지 않는다
        string tempPath = Path.Combine(directory, $"RunData.json.{generation}.tmp");
        string backupPath = Path.Combine(directory, $"RunData.json.{generation}.bak");

#if UNITY_EDITOR
        using (s_ProfileSaveRunWriteAtomic.Auto())
#endif
        {
            // 임시 파일에 JSON 기록
            File.WriteAllText(tempPath, jsonData);

            // tmp 기록 후 stale/취소 판정 — 새 세대가 요청됐으면 교체 생략
            if (runSaveGeneration != generation || token.IsCancellationRequested)
            {
                TryDeleteFile(tempPath);
                return;
            }

            // 원자적 교체: tmp → final (기존 파일이 있으면 backup 경유)
            try
            {
                if (File.Exists(finalPath))
                {
                    if (File.Exists(backupPath))
                        File.Delete(backupPath);
                    File.Replace(tempPath, finalPath, backupPath);
                }
                else
                {
                    File.Move(tempPath, finalPath);
                }
            }
            catch
            {
                TryDeleteFile(tempPath);
                throw;
            }

            // 교체 성공 후 백업 정리
            TryDeleteFile(backupPath);
        }
    }

    private static void TryDeleteFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        catch
        {
            // best effort
        }
    }

    /// <summary>이어하기용 런 저장 파일(<c>RunData.json</c>)을 삭제한다. 파일이 없으면 아무 것도 하지 않는다.</summary>
    public void DeleteRunSaveData()
    {
        // 진행 중인 저장이 있으면 취소 후 완료 대기 — 삭제와 쓰기가 충돌하지 않도록
        CancelRunSave();

        string dataPath = Path.Combine(path, "RunData.json");
        if (!File.Exists(dataPath))
            return;

        File.Delete(dataPath);
    }

    // --- 비동기 저장 라이프사이클 ---

    /// <summary>진행 중인 저장을 취소하고 완료될 때까지 대기한다.</summary>
    /// <remarks>메인 스레드 전용. 백그라운드에서 호출하지 않는다.</remarks>
    private void CancelRunSave()
    {
        lock (runSaveLock)
        {
            if (runSaveCts != null)
                runSaveCts.Cancel();
        }

        FlushRunSave();
    }

    /// <summary>현재 백그라운드 저장 Task가 끝날 때까지 메인 스레드에서 대기한다.</summary>
    /// <remarks>메인 스레드 전용. 백그라운드에서 호출하지 않는다.</remarks>
    private void FlushRunSave()
    {
        Task task = runSaveTask;
        if (task == null || task.IsCompleted)
            return;

        try
        {
            if (!task.Wait(FLUSH_TIMEOUT_MS))
                Debug.LogWarning($"[DataManager] RunData 저장 flush 타임아웃 ({FLUSH_TIMEOUT_MS}ms)");
        }
        catch (AggregateException)
        {
            // 취소 또는 실패 — 워커에서 이미 로그를 남김
        }
    }

    /// <summary>앱이 백그라운드로 내려갈 때 저장 완료를 보장한다.</summary>
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            FlushRunSave();
    }

    /// <summary>앱 종료 직전 저장 완료를 보장한다.</summary>
    private void OnApplicationQuit()
    {
        FlushRunSave();
    }

    public PlayerData LoadPlayerData()
    {
        string dataPath = Path.Combine(path, "PlayerData.json");
        if (!File.Exists(dataPath))
        {
            Debug.Log("There doesn't exist Player Data");
            return null;
        }

        string loadedJson = File.ReadAllText(dataPath);
        playerData = JsonUtility.FromJson<PlayerData>(loadedJson);

        return playerData;
    }

    /// <summary>
    /// 디스크에서 JSON 역직렬화 → <see cref="RunSaveData.TryMigrate"/> → <see cref="RunSaveMapper.FromSaveData"/>로 <see cref="RunData"/>를 만든다.
    /// 실패 시 빈 런을 만들지 않고 false를 반환한다(구버전·손상·레지스트리 불일치 등).
    /// </summary>
    public bool TryLoadRunData(out RunData runData)
    {
        runData = null;

        string dataPath = Path.Combine(path, "RunData.json");
        if (!File.Exists(dataPath))
        {
            Debug.LogWarning("TryLoadRunData: 런 저장 파일이 없습니다.");
            return false;
        }

        string loadedJson = File.ReadAllText(dataPath);
        RunSaveData saveData = JsonUtility.FromJson<RunSaveData>(loadedJson);
        if (saveData == null || !RunSaveData.TryMigrate(ref saveData))
        {
            Debug.LogWarning("TryLoadRunData: 런 저장이 없거나 마이그레이션할 수 없습니다.");
            return false;
        }

        runData = RunSaveMapper.FromSaveData(saveData);
        return runData != null;
    }

    // 덱 해금 설정
    public void SetDeckUnlocked(string deckName)
    {
        int index = playerData.decks.FindIndex(x => x.deckName == deckName);

        if (index == -1)
        {
            Debug.LogError("해금하려는 덱이 존재하지 않음: " + deckName);
            return;
        }

        if (playerData.decks[index].isUnlocked)
        {
            Debug.LogError("해금하려는 덱이 이미 해금됨: " + deckName);
            return;
        }

        playerData.decks[index].isUnlocked = true;
    }

    // 덱 레벨 업데이트
    public void UpdateDeckLevel(DeckType deckType, int clearedLevel)
    {
        int index = playerData.decks.FindIndex(x => x.deckType == deckType);

        if (index == -1)
        {
            Debug.LogError("레벨 업데이트하려는 덱이 존재하지 않음" + deckType);
            return;
        }

        DeckInfo deckInfo = playerData.decks[index];

        if (deckInfo.level == clearedLevel && deckInfo.level < DeckInfo.MAX_LEVEL)
        {
            deckInfo.level++;
        }
    }

    // 아이템 해금 추가
    public void AddUnlockedItem(string itemID)
    {
        playerData.unlockedItems.Add(itemID);
    }

    // 데이터 업데이트 함수 ------------------------------------------------
    public void SetTutorialValue(bool value)
    {
        playerData.tutorialValue = value;
    }
    
    public void UpdateBlockPlaceCount(Block block)
    {
        switch (block.Type)
        {
            case BlockType.I:
                playerData.placeCountI++;
                UnlockManager.instance.onPlaceCountIUpdate?.Invoke(playerData.placeCountI);
                break;
            case BlockType.O:
                playerData.placeCountO++;
                UnlockManager.instance.onPlaceCountOUpdate?.Invoke(playerData.placeCountO);
                break;
            case BlockType.Z:
                playerData.placeCountZ++;
                UnlockManager.instance.onPlaceCountZUpdate?.Invoke(playerData.placeCountZ);
                break;
            case BlockType.S:
                playerData.placeCountS++;
                UnlockManager.instance.onPlaceCountSUpdate?.Invoke(playerData.placeCountS);
                break;
            case BlockType.J:
                playerData.placeCountJ++;
                UnlockManager.instance.onPlaceCountJUpdate?.Invoke(playerData.placeCountJ);
                break;
            case BlockType.L:
                playerData.placeCountL++;
                UnlockManager.instance.onPlaceCountLUpdate?.Invoke(playerData.placeCountL);
                break;
            case BlockType.T:
                playerData.placeCountT++;
                UnlockManager.instance.onPlaceCountTUpdate?.Invoke(playerData.placeCountT);
                break;
        }

        switch (block.Type)
        {
            case BlockType.I:
            case BlockType.O:
                UnlockManager.instance.onPlaceCountIOUpdate?.Invoke(playerData.placeCountI + playerData.placeCountO);
                break;
            case BlockType.Z:
            case BlockType.S:
                UnlockManager.instance.onPlaceCountZSUpdate?.Invoke(playerData.placeCountZ + playerData.placeCountS);
                break;
            case BlockType.J:
            case BlockType.L:
            case BlockType.T:
                UnlockManager.instance.onPlaceCountJLUpdate?.Invoke(playerData.placeCountJ + playerData.placeCountL);
                break;
        }
    }
    public void UpdateRerollCount()
    {
        playerData.rerollCount++;
        UnlockManager.instance.onRerollCountUpdate?.Invoke(playerData.rerollCount);
    }
    public void UpdateItemPurchaseCount()
    {
        playerData.itemPurchaseCount++;
        UnlockManager.instance.onItemPurchaseCountUpdate?.Invoke(playerData.itemPurchaseCount);
    }
    public void UpdateShopRerollCount()
    {
        playerData.shopRerollCount++;
        UnlockManager.instance.onShopRerollCountUpdate?.Invoke(playerData.shopRerollCount);
    }
    public void UpdateMaxScore(int score)
    {
        if (score > playerData.maxScore)
        {

            playerData.maxScore = score;
            UnlockManager.instance.onMaxScoreUpdate?.Invoke(playerData.maxScore);
        }
    }
    public void UpdateMaxChapterStage(int chapter, int stage)
    {
        if ((chapter > playerData.maxChapter) ||
            (chapter == playerData.maxChapter &&
             stage > playerData.maxStage))
        {
            playerData.maxChapter = chapter;
            playerData.maxStage = stage;
            UnlockManager.instance.onMaxChapterUpdate?.Invoke(chapter);
        }
    }
    public void UpdateWinCount()
    {
        playerData.winCount++;
        UnlockManager.instance.onWinCountUpdate?.Invoke(playerData.winCount);
    }
    public void UpdateBlockRerollCount(BlockData block)
    {
        switch (block.type)
        {
            case BlockType.I:
            case BlockType.O:
                playerData.rerollCountIO++;
                UnlockManager.instance.onRerollCountIOUpdate?.Invoke(playerData.rerollCountIO);
                break;
            case BlockType.Z:
            case BlockType.S:
                playerData.rerollCountZS++;
                UnlockManager.instance.onRerollCountZSUpdate?.Invoke(playerData.rerollCountZS);
                break;
            case BlockType.J:
            case BlockType.L:
            case BlockType.T:
                playerData.rerollCountJLT++;
                UnlockManager.instance.onRerollCountJLTUpdate?.Invoke(playerData.rerollCountJLT);
                break;
        }
    }
    public void UpdateMaxBaseMultiplier(int value)
    {
        if (value > playerData.maxBaseMultiplier)
        {
            playerData.maxBaseMultiplier = value;
            UnlockManager.instance.onMaxBaseMultiplierUpdate?.Invoke(playerData.maxBaseMultiplier);
        }
    }
    public void UpdateMaxMultiplier(int value)
    {
        if (value > playerData.maxMultiplier)
        {
            playerData.maxMultiplier = value;
            UnlockManager.instance.onMaxMultiplierUpdate?.Invoke(playerData.maxMultiplier);
        }
    }
    public void UpdateMaxBaseRerollCount(int value)
    {
        if (value > playerData.maxBaseRerollCount)
        {
            playerData.maxBaseRerollCount = value;
            UnlockManager.instance.onMaxBaseRerollCountUpdate?.Invoke(playerData.maxBaseRerollCount);
        }
    }
    public void UpdateMaxGold(int value)
    {
        if (value > playerData.maxGold)
        {
            playerData.maxGold = value;
            UnlockManager.instance.onMaxGoldUpdate?.Invoke(playerData.maxGold);
        }
    }
    public void UpdateHasOnlySpecificBlock(List<BlockData> deck)
    {
        if (deck.All(block => block.type == BlockType.I || block.type == BlockType.O))
        {
            playerData.hasOnlyIO++;
            UnlockManager.instance.onHasOnlyIOUpdate?.Invoke(playerData.hasOnlyIO);
        }
        else if (deck.All(block => block.type == BlockType.Z || block.type == BlockType.S))
        {
            playerData.hasOnlyZS++;
            UnlockManager.instance.onHasOnlyZSUpdate?.Invoke(playerData.hasOnlyZS);
        }
        else if (deck.All(block => block.type == BlockType.J || block.type == BlockType.L))
        {
            playerData.hasOnlyJL++;
            UnlockManager.instance.onHasOnlyJLUpdate?.Invoke(playerData.hasOnlyJL);
        }
    }

    public void UpdateDeckWinCount(DeckType deckType)
    {
        switch (deckType)
        {
            case DeckType.Default:
                playerData.defaultDeckWinCount++;
                UnlockManager.instance.onDefaultDeckWinCountUpdate?.Invoke(playerData.defaultDeckWinCount);
                break;
            case DeckType.YoYo:
                playerData.yoyoDeckWinCount++;
                UnlockManager.instance.onYoYoDeckWinCountUpdate?.Invoke(playerData.yoyoDeckWinCount);
                break;
            case DeckType.Dice:
                playerData.diceDeckWinCount++;
                UnlockManager.instance.onDiceDeckWinCountUpdate?.Invoke(playerData.diceDeckWinCount);
                break;
            case DeckType.Telescope:
                playerData.telescopeDeckWinCount++;
                UnlockManager.instance.onTelescopeDeckWinCountUpdate?.Invoke(playerData.telescopeDeckWinCount);
                break;
            case DeckType.Mirror:
                playerData.mirrorDeckWinCount++;
                UnlockManager.instance.onMirrorDeckWinCountUpdate?.Invoke(playerData.mirrorDeckWinCount);
                break;
            case DeckType.Bomb:
                playerData.bombDeckWinCount++;
                UnlockManager.instance.onBombDeckWinCountUpdate?.Invoke(playerData.bombDeckWinCount);
                break;
        }
    }

    public void UpdateClearedMaxLevel(int value)
    {
        if (value > playerData.clearedMaxLevel)
        {
            playerData.clearedMaxLevel = value;
            UnlockManager.instance.onClearedMaxLevelUpdate?.Invoke(playerData.clearedMaxLevel);
        }
    }

    public void UpdateDeckAdWatchCount(DeckType deckType)
    {
        int index = playerData.decks.FindIndex(x => x.deckType == deckType);

        if (index == -1)
        {
            Debug.LogError("광고 시청 횟수 업데이트하려는 덱이 존재하지 않음: " + deckType);
            return;
        }

        playerData.decks[index].adWatchCount++;

        int count = playerData.decks[index].adWatchCount;
        switch (deckType)
        {
            case DeckType.YoYo:
                UnlockManager.instance.onYoYoAdWatchCountUpdate?.Invoke(count);
                break;
            case DeckType.Dice:
                UnlockManager.instance.onDiceAdWatchCountUpdate?.Invoke(count);
                break;
            case DeckType.Telescope:
                UnlockManager.instance.onTelescopeAdWatchCountUpdate?.Invoke(count);
                break;
            case DeckType.Mirror:
                UnlockManager.instance.onMirrorAdWatchCountUpdate?.Invoke(count);
                break;
            case DeckType.Bomb:
                UnlockManager.instance.onBombAdWatchCountUpdate?.Invoke(count);
                break;
        }

        SavePlayerData(playerData);
    }
    // ---------------------------------------------------------------------
}

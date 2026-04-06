using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance = null;

    private string path;

    private PlayerData playerData;

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

    // 이어하기
    public void SaveRunData(RunData runData)
    {
        RunSaveData saveData = RunSaveMapper.ToSaveData(runData);
        string jsonData = JsonUtility.ToJson(saveData, true);
        WriteRunDataJsonAtomic(path, jsonData);

        Debug.Log("Finish Saving RunData");
    }

    /// <summary>
    /// <c>RunData.json</c>을 임시 파일에 쓴 뒤 교체해, 쓰기 중단 시 기존 파일이 깨지지 않게 한다.
    /// </summary>
    private static void WriteRunDataJsonAtomic(string directory, string jsonData)
    {
        string finalPath = Path.Combine(directory, "RunData.json");
        string tempPath = Path.Combine(directory, "RunData.json.tmp");
        string backupPath = Path.Combine(directory, "RunData.json.bak");

        File.WriteAllText(tempPath, jsonData);

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
            if (File.Exists(tempPath))
            {
                try
                {
                    File.Delete(tempPath);
                }
                catch
                {
                    // best effort
                }
            }

            throw;
        }

        if (File.Exists(backupPath))
        {
            try
            {
                File.Delete(backupPath);
            }
            catch
            {
                // best effort; next save may delete it
            }
        }
    }

    /// <summary>이어하기용 런 저장 파일(<c>RunData.json</c>)을 삭제한다. 파일이 없으면 아무 것도 하지 않는다.</summary>
    public void DeleteRunSaveData()
    {
        string dataPath = Path.Combine(path, "RunData.json");
        if (!File.Exists(dataPath))
            return;

        File.Delete(dataPath);
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

    // 덱 해금 추가
    public void AddUnlockedDeck(string deckName)
    {
        int index = playerData.unlockedDecks.FindIndex(x => x.deckName == deckName);

        if (index != -1)
        {
            Debug.LogError("해금하려는 덱이 이미 존재함: " + deckName); ;
            return;
        }

        DeckType deckType = Enums.GetEnumByString<DeckType>(deckName);
        DeckInfo deckInfo = new DeckInfo(deckType, 0);
        playerData.unlockedDecks.Add(deckInfo);
    }

    // 덱 레벨 업데이트
    public void UpdateDeckLevel(DeckType deckType, int clearedLevel)
    {
        int index = playerData.unlockedDecks.FindIndex(x => x.deckType == deckType);

        if (index == -1)
        {
            Debug.LogError("레벨 업데이트하려는 덱이 존재하지 않음" + deckType);
            return;
        }

        DeckInfo deckInfo = playerData.unlockedDecks[index];

        if (deckInfo.level == clearedLevel && deckInfo.level < DeckInfo.MAX_LEVEL)
        {
            deckInfo.level++;
            playerData.unlockedDecks[index] = deckInfo;
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
    // ---------------------------------------------------------------------
}

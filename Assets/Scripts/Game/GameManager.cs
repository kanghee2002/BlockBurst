using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public ApplicationType applicationType;

    public PlayerData playerData;
    public GameData gameData;
    public RunData runData;
    public BlockGameData blockGame;

    public DeckManager deckManager;
    public ShopManager shopManager;
    public StageManager stageManager;
    public AnimationManager animationManager;
    public TutorialManager tutorialManager;

    public Board board;

    private const int STAGE_CHOICE_COUNT = 2;
    private const int CLEAR_CHAPTER = 5;
    private const int INFINITE_CHAPTER = -1;
    private int currentClearChapter = 5;

    public List<BlockData> handBlocksData = new List<BlockData>();
    public List<Block> handBlocks = new List<Block>();

    private Dictionary<ItemType, ItemData[]> shopItems = new();

    private int blockId = 0;

    private StageData[] nextStageChoices = new StageData[STAGE_CHOICE_COUNT];
    private List<Match> currentMatches;
    private float SCORE_ANIMATION_DELAY;  // 블록 점수 간의 딜레이

    [SerializeField] private bool isTutorial = true;

    private bool isClearStage = false; // 중복 방지 플래그

    // ------------------------------
    // GAME LAYER - start
    // ------------------------------

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
            return;
        }

        playerData = null;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120; // for mobile build
        SetApplicationType();

        if (applicationType == ApplicationType.Windows)
        {
            SceneTransitionManager.instance.TransitionToScene("NewLogoScene");
        }
    }

    // ------------------------------------------------------------------------
    // TEST (에디터 플레이 모드에서만 동작)

    private void Update()
    {
#if UNITY_EDITOR
        // 디버그: 런 저장 테스트 (R 키)
        if (Input.GetKeyDown(KeyCode.R))
        {
            DataManager.instance.SaveRunData(runData);
            Debug.Log("[Editor] DataManager.SaveRunData(runData) 호출됨.");
        }

        // 디버그: 플레이어 데이터 저장 테스트 (T 키)
        if (Input.GetKeyDown(KeyCode.T))
        {
            DataManager.instance.SavePlayerData(playerData);
            Debug.Log("[Editor] DataManager.SaveRunData(runData) 호출됨.");
        }

        // 디버그: 플레이 중 스테이지 즉시 종료 (기존 SKIP_BUTTON과 동일 동작, S 키)
        if (Input.GetKeyDown(KeyCode.S))
            EndStage();

        // 디버그: 골드 5000 추가 (G 키)
        if (Input.GetKeyDown(KeyCode.G))
            DebugAddGold();
#endif
    }

    /// <summary>디버그용. 현재 런에 골드 추가.</summary>
    public void DebugAddGold()
    {
        UpdateGold(5000);
    }

    public void TEST_BUTTON()
    {
        shopManager.AddFirstItem(new List<string>() { "Wheel" });
    }

    public void TEST_TEXT(string text)
    {
        if (GameObject.Find("TEST_TEXT"))
        {
            GameObject.Find("TEST_TEXT").GetComponent<TextMeshProUGUI>().text += text;
        }
    }

    // ------------------------------------------------------------------------

    private void LoadPlayerData()
    {
        if (playerData != null)
        {
            return;
        }

        playerData = DataManager.instance.LoadPlayerData();

        if (playerData == null)
        {
            playerData = new PlayerData();

            DataManager.instance.SavePlayerData(playerData);
        }
    }

    private void SetApplicationType()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.OSXEditor)
        {
            return;
        }

        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.WebGLPlayer)
        {
            applicationType = ApplicationType.Mobile;
        }
        else
        {
            applicationType = ApplicationType.Windows;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (ScriptableDataManager.instance == null)
        {
            Debug.LogError("ScriptableDataManager: instance is null — adding component to GameManager GameObject.");
            gameObject.AddComponent<ScriptableDataManager>();
        }

        LoadPlayerData();
        SetTutorialValue(playerData.tutorialValue);

        UnlockManager.instance.Initialize(playerData);

        if (scene.name == "VerticalGameScene" || scene.name == "HorizontalGameScene")
        {
            deckManager = FindObjectOfType<DeckManager>();
            shopManager = FindObjectOfType<ShopManager>();
            stageManager = FindObjectOfType<StageManager>();
            animationManager = FindObjectOfType<AnimationManager>();
            tutorialManager = FindObjectOfType<TutorialManager>();

            GameUIManager.instance.SetTutorialToggleValue(isTutorial);

            StartCoroutine(WaitUntilAudioManagerIsReadyAndPlay());
        }
    }

    IEnumerator WaitUntilAudioManagerIsReadyAndPlay()
    {
        while (AudioManager.instance == null)
        {
            yield return null;
        }
        while (!AudioManager.instance.isInitialized)
        {
            yield return null;
        }
        AudioManager.instance.BeginBackgroundMusic();
    }

    // 지금 안 씀
    public void GoToGameScene()
    {
        AudioManager.instance.SFXSelectMenu();
        SceneTransitionManager.instance.TransitionToScene("VerticalGameScene");
    }

    public void SetTutorialValue(bool isTutorial)
    {
        this.isTutorial = isTutorial;
        DataManager.instance.SetTutorialValue(isTutorial);
    }

    public bool GetTutorialValue()
    {
        return isTutorial;
    }

    public void StartNewGame(DeckType deckType, int level)
    {
        // 각종 초기화
        Debug.Log("Game Start");

        currentClearChapter = CLEAR_CHAPTER;
        
        SCORE_ANIMATION_DELAY = 0.01f;
        currentMatches = new List<Match>();

        shopItems = new Dictionary<ItemType, ItemData[]>()
        {
            { ItemType.ITEM, new ItemData[2] },
            { ItemType.BOOST, new ItemData[2] },
            { ItemType.ADD_BLOCK, new ItemData[2] },
        };

        DeckData deckData = ScriptableDataManager.instance.deckTemplates.FirstOrDefault(deck => deck.type == deckType);
        deckData.Initialize();

        LevelData levelData = ScriptableDataManager.instance.levelTemplates.FirstOrDefault(data => data.level == level);

        gameData = new GameData();
        gameData.Initialize(ScriptableDataManager.instance.blockTemplates, deckData, levelData);

        StartNewRun();
    }

    /// <summary>
    /// 런 저장으로 이어하기. 성공 시 true. 로드·덱/레벨·저장된 스테이지 id로 복원 불가 시 false.
    /// </summary>
    public bool ContinueGame()
    {
        ScriptableDataManager sdManager = ScriptableDataManager.instance;
        if (sdManager == null)
        {
            Debug.LogError("ContinueGame: ScriptableDataManager.instance is null.");
            return false;
        }

        // 디스크에서만 복원한다. 실패 시 빈 런으로 대체하지 않고 중단한다.
        if (!DataManager.instance.TryLoadRunData(out RunData loadedRun))
        {
            Debug.LogWarning("ContinueGame: 런 저장을 불러오지 못했습니다.");
            return false;
        }

        // 매퍼가 채운 currentDeck / currentLevel로 스테이지 점수·보상 등에 쓸 GameData를 구성한다.
        if (loadedRun.currentDeck == null || loadedRun.currentLevel == null)
        {
            Debug.LogError("ContinueGame: 저장의 덱·레벨 id를 레지스트리에서 찾지 못했습니다.");
            return false;
        }

        // 이어하기는 저장 시점의 스테이지 id가 있어야 하고, 레지스트리에서 풀 수 있어야 한다.
        if (string.IsNullOrEmpty(loadedRun.currentStageId))
        {
            Debug.LogError("ContinueGame: 저장에 currentStageId가 없어 이어하기를 할 수 없습니다.");
            return false;
        }

        if (!sdManager.TryGetStage(loadedRun.currentStageId, out StageData resumeStage))
        {
            Debug.LogError($"ContinueGame: currentStageId '{loadedRun.currentStageId}'에 해당하는 스테이지를 찾을 수 없습니다.");
            return false;
        }

        runData = loadedRun;

        gameData = new GameData();
        gameData.Initialize(Array.Empty<BlockData>(), runData.currentDeck, runData.currentLevel);

        // StartNewGame과 동일한 런 진입 전 필드 초기화(히스토리는 저장본 유지).
        currentClearChapter = CLEAR_CHAPTER;
        SCORE_ANIMATION_DELAY = 0.01f;
        currentMatches = new List<Match>();

        shopItems = new Dictionary<ItemType, ItemData[]>()
        {
            { ItemType.ITEM, new ItemData[2] },
            { ItemType.BOOST, new ItemData[2] },
            { ItemType.ADD_BLOCK, new ItemData[2] },
        };

        // 보드 셀 효과·효과 매니저를 저장된 activeEffects 기준으로 맞춘다.
        CellEffectManager.instance.Initialize();
        CellEffectManager.instance.ApplyActiveUpgradeItems(runData.activeUpgrades);
        EffectManager.instance.Initialize(ref runData);

        // 상점 풀: ShopManager.Initialize로 해금 베이스를 채운 뒤 ApplyActiveInventoryToPool로 인벤/부스트만큼 맞춤.
        ItemData[] unlockedShopItems = UnlockManager.instance.GetUnlockedItems(sdManager.itemTemplates);
        shopManager.Initialize(ref runData, unlockedShopItems);
        shopManager.ApplyActiveItemToPool(runData);

        animationManager.InitializeRunData(ref runData);

        UpdateDeckCount(runData.availableBlocks.Count, runData.availableBlocks.Count);

        // 스테이지 선택 단계를 건너뛰므로 selecting UI는 열지 않음(OnStageStart가 playing 전환).
        GameUIManager.instance.InitializeContinueGame(runData);

        StartStage(resumeStage);
        string[] playingDebuffNames = resumeStage.constraints.Select(c => c.effectName).ToArray();
        GameUIManager.instance.OnStageStart(
            runData.currentChapterIndex,
            runData.currentStageIndex,
            playingDebuffNames,
            blockGame.clearRequirement,
            blockGame);
        GameUIManager.instance.BlockCells(blockGame.inactiveCells);

        return true;
    }

    public void InitializeHistory()
    {
        runData.history.startTime = Time.time;
        runData.history.blockHistory = new int[Enum.GetNames(typeof(BlockType)).Length];
        runData.history.rerollCount = 0;
        runData.history.itemPurchaseCount = 0;
        runData.history.maxScore = 0;
    }
    
    public void EndGame(bool isWin, string loseReason = "")
    {
        if (!isWin && DataManager.instance != null)
            DataManager.instance.DeleteRunSaveData();

        BlockType mostPlacedBlockType = (BlockType)runData.history.blockHistory.ToList().IndexOf(runData.history.blockHistory.Max());
        GameUIManager.instance.OnGameEnd(isWin, runData.currentChapterIndex, runData.currentStageIndex, runData.history, mostPlacedBlockType, loseReason: loseReason);
    }

    public void InfiniteMode()
    {
        currentClearChapter = INFINITE_CHAPTER;
        EndStage();
    }

    public void BackToMain()
    {
        // 메인 화면으로 돌아가기
        Debug.Log("Back to Main");
        //SceneManager.LoadScene("NewLogoScene");
        AudioManager.instance.StopBackgroundMusic();
        
        if (applicationType == ApplicationType.Windows)
        {
            SceneTransitionManager.instance.TransitionToScene("NewLogoScene");
        }
        else
        {
            SceneTransitionManager.instance.TransitionToScene("LogoVerti");
        }
    }

    public void MakeNewRun()
    {
        SceneTransitionManager.instance.TransitionToScene("VerticalGameScene");
    }

    // ------------------------------
    // GAME LAYER - end
    // ------------------------------

    // ------------------------------
    // RUN LAYER - start
    // ------------------------------

    public void StartNewRun()
    {
        // 초기화
        runData = new RunData();
        runData.Initialize(gameData);

        InitializeHistory();

        CellEffectManager.instance.Initialize();
        EffectManager.instance.Initialize(ref runData);

        ItemData[] unlockedItems = UnlockManager.instance.GetUnlockedItems(ScriptableDataManager.instance.itemTemplates);

        shopManager.Initialize(ref runData, unlockedItems);

        animationManager.InitializeRunData(ref runData);

        UpdateDeckCount(runData.availableBlocks.Count, runData.availableBlocks.Count);

        GameUIManager.instance.Initialize(runData);

        if (isTutorial)
        {
            isTutorial = false;
            DataManager.instance.SetTutorialValue(isTutorial);
            tutorialManager.Initialize();
        }

        StartStageSelection();
    }

    public void OnRunInfoRequested()
    {
        // 최대로 사용된 블록 찾기
        int maxCount = runData.history.blockHistory.Max();
        BlockType? mostPlacedBlockType =  maxCount > 0 ? (BlockType)runData.history.blockHistory.ToList().IndexOf(maxCount) : null;

        // Run 정보 UI 열기
        GameUIManager.instance.OnRunInfoCallback(runData, runData.history.startTime, mostPlacedBlockType);
    }

    public void OnDeckInfoRequested()
    {
        // Deck 정보 UI 열기
        GameUIManager.instance.OnDeckInfoCallback(runData, blockGame);
    }

    public void OnBoostInfoRequested(int index)
    {
        ItemData boostData = runData.activeBoosts[index];
        GameUIManager.instance.OnBoostInfoCallback(boostData, index);
    }

    public void OnItemInfoRequested(int index)
    {
        ItemData itemData = runData.activeItems[index];
        GameUIManager.instance.OnItemInfoCallback(itemData, index);
    }

    public void OnShopItemInfoRequested(int index)
    {
        List<ItemData> shopItemList = GetShopItemList();
        ItemData itemData = shopItemList[index];

        GameUIManager.instance.OnShopItemInfoCallback(itemData, index);
    }

    public void OnDeckLevelInfoRequested()
    {
        GameUIManager.instance.OnDeckLevelInfoCallback(
            ScriptableDataManager.instance.deckTemplates,
            ScriptableDataManager.instance.levelTemplates);
    }

    private int GetStageClearRequirement(StageData stage)
    {
        if (runData.currentChapterIndex <= gameData.stageBaseScoreList.Count)
        {
            int stageBaseScore = gameData.stageBaseScoreList[runData.currentChapterIndex - 1];
            float scoreMultiplier = gameData.stageScoreMultiplier[runData.currentStageIndex - 1];
            float stageBaseMultiplier = stage.baseScoreMultiplier;
            return (int)(stageBaseScore * (scoreMultiplier + stageBaseMultiplier));
        }

        int lastBaseScore = gameData.stageBaseScoreList[gameData.stageBaseScoreList.Count - 1];
        int powExponent = 3 * (runData.currentChapterIndex - 1 - gameData.stageBaseScoreList.Count) + runData.currentStageIndex + 1;
        float powResult = Mathf.Pow(1.5f, powExponent);
        float combinedScoreMultiplier = gameData.stageScoreMultiplier[runData.currentStageIndex - 1] + stage.baseScoreMultiplier;
        return (int)(lastBaseScore * powResult * combinedScoreMultiplier);
    }

    private int GetStageGoldReward(StageData stage)
    {
        return gameData.stageBaseReward + (int)MathF.Pow(runData.currentChapterIndex + 2, 0.5f) * 3 + stage.additionalGold;
    }

    public void StartStageSelection()
    {
        // 상점 아이템을 다시 리스트에 추가
        foreach (ItemData item in GetShopItemList())
        {
            if (item != null)
            {
                shopManager.AddItem(item);
            }
        }

        // stage Template에서 stagetype이 맞는 것을 랜덤하게 추출
        StageType stageType = runData.currentStageIndex == 3 ? StageType.BOSS : StageType.NORMAL;
        var templates = ScriptableDataManager.instance.stageTemplates.Where(stage => stage.type == stageType).ToArray();
        
        // 사용할 인덱스들을 미리 섞어서 준비
        List<int> indices = Enumerable.Range(0, templates.Length).ToList();
        for (int i = indices.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            int temp = indices[i];
            indices[i] = indices[j];
            indices[j] = temp;
        }

        // 섞인 인덱스에서 필요한 만큼만 가져와서 사용
        for (int i = 0; i < nextStageChoices.Length; i++)
        {
            nextStageChoices[i] = templates[indices[i]];
        }

        // 디버깅 / 튜토리얼: firstStageList 키는 StageData.resourceKey와 대소문자 무시하고 일치하면 인정
        List<string> stageNames = stageManager.firstStageList;
        if (stageNames.Count > 0)
        {
            for (int i = 0; i < nextStageChoices.Length; i++)
            {
                if (stageNames.Count == 0) break;
                string wantedKey = stageNames[0];
                stageNames.RemoveAt(0);
                StageData stage = templates.FirstOrDefault(x =>
                    string.Equals(x.resourceKey, wantedKey, StringComparison.OrdinalIgnoreCase));
                if (stage != null)
                    nextStageChoices[i] = stage;
                else
                    Debug.LogError($"[DEBUG] firstStageList: resourceKey가 '{wantedKey}'와 일치하는 스테이지 없음 (대소문자 무시). 랜덤 선택 유지.");
            }
        }

        // UI용 목표/보상 계산 (SO에 쓰지 않음)
        string[][] choiceDebuffNames = new string[nextStageChoices.Length][];
        int[] choiceClearRequirements = new int[nextStageChoices.Length];
        int[] choiceGoldRewards = new int[nextStageChoices.Length];
        for (int i = 0; i < nextStageChoices.Length; i++)
        {
            StageData stage = nextStageChoices[i];
            choiceDebuffNames[i] = stage.constraints.Select(c => c.effectName).ToArray();
            choiceClearRequirements[i] = GetStageClearRequirement(stage);
            choiceGoldRewards[i] = GetStageGoldReward(stage);
        }

        GameUIManager.instance.OnStageSelection(
            runData.currentChapterIndex,
            runData.currentStageIndex,
            choiceDebuffNames[0], choiceClearRequirements[0], choiceGoldRewards[0],
            choiceDebuffNames[1], choiceClearRequirements[1], choiceGoldRewards[1]);
    }

    public void OnStageSelection(int choiceIndex)
    {
        // 선택된 스테이지로 진행
        StageData selectedStage = nextStageChoices[choiceIndex];

        StartStage(selectedStage);
        string[] playingDebuffNames = selectedStage.constraints.Select(c => c.effectName).ToArray();
        GameUIManager.instance.OnStageStart(
            runData.currentChapterIndex,
            runData.currentStageIndex,
            playingDebuffNames,
            blockGame.clearRequirement,
            blockGame);
        GameUIManager.instance.BlockCells(blockGame.inactiveCells);
    }

    HashSet<Vector2Int> SetInactiveBlockCells(BlockGameData data)
    {
        if (data.isCornerBlocked)
        {
            data.inactiveCells.Add(new Vector2Int(0, 0));
            data.inactiveCells.Add(new Vector2Int(0, data.boardColumns - 1));
            data.inactiveCells.Add(new Vector2Int(data.boardRows - 1, 0));
            data.inactiveCells.Add(new Vector2Int(data.boardRows - 1, data.boardColumns - 1));
        }
        data.inactiveCells.UnionWith(GetRandomBlockCells(data));

        return data.inactiveCells;
    }

    private HashSet<Vector2Int> GetRandomBlockCells(BlockGameData data)
    {
        HashSet<Vector2Int> result = new HashSet<Vector2Int>();

        for (int i = 0; i < 10000; i++)
        {
            // isCornerBlocked일 경우 break 다르게 구현
            if (result.Count == data.inactiveCellCount) break;
            int x = Random.Range(0, data.boardRows);
            int y = Random.Range(0, data.boardColumns);
            result.Add(new Vector2Int(y, x));
        }
        return result;
    }

    public void StartStage(StageData stage)
    {
        blockGame = new BlockGameData();
        blockGame.Initialize(runData);
        
        deckManager.Initialize(ref blockGame, ref runData);
        animationManager.InitializeBlockGameData(ref blockGame);

        EffectManager.instance.InitializeBlockGameData(ref blockGame);
        CellEffectManager.instance.InitializeBlockGame(ref blockGame);

        blockGame.clearRequirement = GetStageClearRequirement(stage);
        blockGame.goldReward = GetStageGoldReward(stage);

        runData.currentStageId = stage.id;

        DataManager.instance.SaveRunData(runData);

        stageManager.StartStage(stage, blockGame);

        board = new Board();
        board.Initialize(blockGame);

        // 보드와 프론트를 막음
        SetInactiveBlockCells(blockGame);
        board.BlockCells(blockGame.inactiveCells);

        DrawBlocks();

        PlayActivatedItemAnimation();

        isClearStage = false;
    }

    public void EndStage()
    {
        if (runData.currentChapterIndex == currentClearChapter && stageManager.currentStage.type == StageType.BOSS)
        {
            EndGame(true);
            DataManager.instance.UpdateWinCount();

            DataManager.instance.UpdateDeckWinCount(runData.currentDeck.type);
            DataManager.instance.UpdateDeckLevel(runData.currentDeck.type, runData.currentLevel.level);
            DataManager.instance.UpdateClearedMaxLevel(runData.currentLevel.level);
        }
        else
        {
            EffectManager.instance.TriggerEffects(TriggerType.ON_END_STAGE);

            EffectManager.instance.EndTriggerEffect();

            stageManager.GrantReward();
            if (stageManager.currentStage.type == StageType.BOSS)
            {
                runData.currentChapterIndex++;
                runData.currentStageIndex = 1;
            }
            else 
            {
                runData.currentStageIndex++;
            }
            DataManager.instance.UpdateMaxChapterStage(runData.currentChapterIndex, runData.currentStageIndex);

            StartShop(true);

            // 게임 관련 UI 초기화
            UpdateDeckCount(runData.availableBlocks.Count, runData.availableBlocks.Count);
            UpdateBaseMultiplier();

            GameUIManager.instance.StopAllItemShakeAnimation();
            GameUIManager.instance.StopWarningStageEffectAnimation(true);
            GameUIManager.instance.StopWarningStageEffectAnimation(false);
        }
        DataManager.instance.SavePlayerData(playerData);
    }

    IEnumerator DelayedEndStage()
    {
        yield return new WaitForSeconds(1.5f);

        GameUIManager.instance.PlayStageClearAnimation(SCORE_ANIMATION_DELAY);

        AudioManager.instance.SFXStageClear();

        yield return new WaitForSeconds(1.5f);

        EndStage();

        yield return new WaitForSeconds(0.5f);

        // 튜토리얼 진행 시 호출
        ProcessTutorialStep("EndStage");
    }

    public void StartShop(bool isFirst = false)
    {
        foreach ((ItemType itemType, int count) in runData.shopItemCounts)
        {
            // 부스트 리롤 X
            if (!isFirst && itemType == ItemType.BOOST)
            {
                continue;
            }

            for (int i = 0; i < 2; i++)
            {
                if (i < count)
                {
                    shopItems[itemType][i] = shopManager.PopItem(itemType);
                }
                else
                {
                    shopItems[itemType][i] = null;
                }
            }
        }

        List<ItemData> shopItemList = GetShopItemList();

        if (isFirst) shopManager.InitializeRerollCost();
        GameUIManager.instance.OnShopStart(shopItemList, shopManager.currentRerollCost, runData.currentChapterIndex, runData.currentStageIndex, isFirst);   
    }

    private List<ItemData> GetShopItemList()
    {
        List<ItemData> shopItemList = new();

        foreach ((ItemType itemType, ItemData[] items) in shopItems)
        {
            shopItemList.AddRange(items);
        }

        return shopItemList;
    }

    public void OnItemDiscard(int index)
    {
        ItemData soldItem = runData.activeItems[index];
        runData.activeItems.RemoveAt(index);

        shopManager.AddItem(soldItem);

        foreach (EffectData effect in soldItem.effects)
        {
            EffectManager.instance.RemoveEffect(effect);
        }
        GameUIManager.instance.DisplayItemSet(runData.activeItems, runData.maxItemCount, index);
    }

    public int OnItemPurchased(int index)
    {
        List<ItemData> shopItemList = GetShopItemList();
        ItemData shopItem = shopItemList[index];

        int res = shopManager.PurchaseItem(shopItem);
        if (res != -1)
        {
            if (index < 2)
            {
                shopItems[ItemType.ITEM][index] = null;
            }
            else if (index < 4)
            {
                shopItems[ItemType.BOOST][index - 2] = null;
            }
            else
            {
                shopItems[ItemType.ADD_BLOCK][index - 4] = null;
            }
            GameUIManager.instance.DisplayItemSet(runData.activeItems, runData.maxItemCount);

            DataManager.instance.UpdateItemPurchaseCount();
            runData.history.itemPurchaseCount++;  // 아이템 히스토리 업데이트

            EffectManager.instance.TriggerEffects(TriggerType.ON_ITEM_PURCHASE);
        }
        else
        {
            UpdateGold(0);
        }
        EffectManager.instance.EndTriggerEffect();

        // 아이템 구매 시 시각 효과
        if (res != -1 && shopItem.type == ItemType.ITEM)
        {
            if (shopItem.effects.All(effect => effect.trigger != TriggerType.ON_ACQUIRE))
            {
                GameUIManager.instance.PlayItemEffectAnimation("", runData.activeItems.Count - 1, 1f);
            }
        }
        return res;
    }

    public void OnShopReroll()
    {
        if (runData.gold < shopManager.currentRerollCost)
        {
            UpdateGold(0);
            return;
        }

        UpdateGold(-shopManager.currentRerollCost);
        List<ItemData> remains = GetShopItemList().Where(item => item != null).ToList();

        // 부스트는 제외 삭제
        remains.RemoveAll(item => item.type == ItemType.BOOST);

        shopItems[ItemType.ITEM] = new ItemData[2];
        shopItems[ItemType.ADD_BLOCK] = new ItemData[2];

        StartShop();
        shopManager.RerollShop(remains);
        UpdateShopRerollCost(0);
        DataManager.instance.UpdateShopRerollCount();

        EffectManager.instance.TriggerEffects(TriggerType.ON_SHOP_REROLL);
        EffectManager.instance.EndTriggerEffect();
    }

    public void UpdateShopRerollCost(int addingValue)
    {
        shopManager.currentRerollCost += addingValue;
        GameUIManager.instance.UpdateShopRerollCost(shopManager.currentRerollCost);
    }

    public void RemoveBlockFromRunDeck(BlockType blockType)
    {
        deckManager.RemoveBlockFromRunDeck(blockType);
    }

    public void UpdateGold(int value, bool isStageReward = false)
    {
        int prevGold = runData.gold;

        runData.gold += value;
        if (runData.gold < 0) runData.gold = 0;

        int currentGold = runData.gold;

        GameUIManager.instance.UpdateGold(runData.gold);

        // 골드를 얻을 때, 스테이지 보상이 아닐 때만 실행
        if (currentGold > prevGold && !isStageReward)
        {
            AudioManager.instance.SFXGold();
        }
    }

    public void UpdateDeckCount(int deckCount, int maxDeckCount)
    {
        GameUIManager.instance.DisplayDeckCount(deckCount, maxDeckCount);
    }

    public void PlayItemFullAnimation()
    {
        GameUIManager.instance.PlayItemFullAnimation();
    }

    public void PlayNotEnoughGoldAnimation()
    {
        GameUIManager.instance.PlayNotEnoughGoldAnimation();
    }

    public void ProcessTutorialStep(string sign)
    {
        tutorialManager.ProceedNextStep(sign);
    }

    public void AddUnlockedItem(string itemID)
    {
        ItemData itemData = ScriptableDataManager.instance.itemTemplates.FirstOrDefault(item => item.resourceKey == itemID);
        
        if (itemData == null)
        {
            Debug.LogError("추가하려는 해금 아이템이 존재하지 않음: " + itemID);
            return;
        }

        DataManager.instance.AddUnlockedItem(itemID);

        shopManager.AddItem(itemData);
    }

    public void PlayUnlockAnimation(UnlockInfo unlockInfo)
    {
        GameUIManager.instance.PlayUnlockAnimation(unlockInfo);
    }

    public int GetCurrentStageIndex()
    {
        return runData.currentStageIndex;
    }

    // ------------------------------
    // RUN LAYER - end
    // ------------------------------

    // ------------------------------
    // BLOCKGAME LAYER - start
    // ------------------------------

    public void DrawBlocks()
    {
        handBlocksData.Clear();
        handBlocks.Clear();
        for (int i = 0; i < blockGame.drawBlockCount; i++)
        {
            BlockData blockData = deckManager.DrawBlock();
            if (!blockData)
            {
                Debug.Log("Deck is empty");
                break;
            }
            handBlocksData.Add(blockData);
            Block block = new Block();
            block.Score = blockGame.blockScores[blockData.type];
            handBlocks.Add(block);
            handBlocks[i].Initialize(handBlocksData[i], blockId++);
        }
        GameUIManager.instance.OnBlocksDrawn(handBlocks);
        UpdateDeckCount(blockGame.deck.Count, runData.availableBlocks.Count);
        GameOverCheck();
    }

    public void OnRerolled()
    {
        int handCount = handBlocks.Count(block => block != null);

        if (deckManager.RerollDeck(handBlocksData))
        {
            if (handCount == blockGame.drawBlockCount)
            {
                EffectManager.instance.TriggerEffects(TriggerType.ON_REROLL_WITHOUT_PLACE);
            }

            DataManager.instance.UpdateRerollCount();
            EffectManager.instance.TriggerEffects(TriggerType.ON_REROLL);

            foreach (BlockData blockData in handBlocksData)
            {
                if (blockData != null)
                {
                    DataManager.instance.UpdateBlockRerollCount(blockData);
                    EffectManager.instance.TriggerEffects(TriggerType.ON_REROLL_SPECIFIC_BLOCK, blockTypes: new BlockType[] { blockData.type });
                }
            }

            DrawBlocks();

            runData.history.rerollCount++;    // 리롤 히스토리 업데이트
        }
        EffectManager.instance.EndTriggerEffect();
    }

    public void OnRotateBlock(int idx)
    {
        handBlocks[idx].RotateShape();
        GameUIManager.instance.OnBlockRotateCallback(idx, handBlocks[idx]);

        EffectManager.instance.TriggerEffects(TriggerType.ON_ROTATE_BLOCK, blockTypes: new BlockType[] { handBlocks[idx].Type });
        EffectManager.instance.EndTriggerEffect();

        GameManager.instance.ProcessTutorialStep("RotateBlock");
    }

    public void OnBeginDragBlock(Block block)
    {
        for (int i = 0; i < runData.activeItems.Count; i++) 
        {
            // 아이템의 효과가 드래그하는 블록과 관련이 있다면
            if (runData.activeItems[i].effects
                .Any(effect => 
                effect.blockTypes.Contains(block.Type) &&
                effect.trigger != TriggerType.ON_ACQUIRE
                ))
            {
                GameUIManager.instance.StartItemShakeAnimation(i, isBlockRelated: true);
            }
        }

        foreach (EffectState state in runData.activeEffects)
        {
            EffectData effect = ScriptableDataManager.instance.GetEffect(state.effectDataId);
            if (effect == null)
                continue;
            if (effect.scope == EffectScope.Stage && effect.blockTypes != null && effect.blockTypes.Contains(block.Type))
            {
                GameUIManager.instance.StartWarningStageEffectAnimation(isBlockRelated: true);
            }
        }
    }

    public void OnEndDragBlock(Block block)
    {
        GameUIManager.instance.StopItemShakeAnimation(isBlockRelated: true);
        GameUIManager.instance.StopWarningStageEffectAnimation(isBlockRelated: true);
    }

    public void PlayBoardRelatedAnimation(int matchCount)
    {
        /*GameUIManager.instance.StopItemShakeAnimation(isBlockRelated: false);

        // 첫 줄 지우기 효과 관련
        for (int i = 0; i < runData.activeItems.Count; i++)
        {
            if (runData.activeItems[i].effects
                .Any(effect => effect.trigger == TriggerType.ON_FIRST_LINE_CLEAR))
            {
                if (matchCount == 0) GameUIManager.instance.StartItemShakeAnimation(i, isBlockRelated: false);
            }
        }*/

        matchCount++;

        GameUIManager.instance.StopWarningStageEffectAnimation(isBlockRelated: false);
        foreach (EffectState state in runData.activeEffects)
        {
            EffectData effect = ScriptableDataManager.instance.GetEffect(state.effectDataId);
            if (effect == null)
                continue;
            if (effect.scope == EffectScope.Stage && effect.triggerMode == TriggerMode.Interval)
            {
                if (effect.triggerValue == state.triggerCount + 1)
                {
                    GameUIManager.instance.StartWarningStageEffectAnimation(isBlockRelated: false);
                }
            }
        }
    }

    public void PlayStageEffectAnimation()
    {
        GameUIManager.instance.PlayStageEffectAnimation();
    }

    public void PlayItemEffectAnimation(List<AnimationData> animations, bool isMatching = false)
    {
        // 아이템 애니메이션
        for (int i = 0; i < runData.activeItems.Count; i++)
        {
            ItemData currentItem = runData.activeItems[i];

            List<string> itemEffectIDList = currentItem.effects.Select(effect => effect.resourceKey).ToList();

            for (int j = animations.Count - 1; j >= 0; j--)
            {
                AnimationData itemEffectAnim = animations[j];

                if (itemEffectIDList.Contains(itemEffectAnim.effect.resourceKey))
                {
                    itemEffectAnim.animationType = AnimationType.ItemEffect;
                    itemEffectAnim.index = i;

                    AnimationData subAnimation = animationManager.GetAnimationData(itemEffectAnim.effect, itemEffectAnim.value);
                    itemEffectAnim.subAnimations = new();
                    itemEffectAnim.subAnimations.Add(subAnimation);

                    animationManager.EnqueueAnimation(itemEffectAnim);

                    animations.RemoveAt(j);
                }
            } 
        }
        
        // 아이템 이외 시각 효과 처리 (블록 강화 등)
        foreach (AnimationData animation in animations)
        {
            if (animation.animationType != AnimationType.None)
            {
                animationManager.ProcessAnimation(animation);

            }
        }

        // 점수 계산 애니메이션
        if (isMatching)
        {
            int lastScore = ScoreCalculator.instance.GetLastScore();

            AnimationData delayAnim = new AnimationData();
            delayAnim.animationType = AnimationType.Delay;
            delayAnim.delayMultiplier = 30f;
            delayAnim.resetSpeed = true;
            animationManager.EnqueueAnimation(delayAnim);

            AnimationData resetChipAnim = new AnimationData();
            resetChipAnim.animationType = AnimationType.UpdateChip;
            resetChipAnim.value = 0;
            animationManager.EnqueueAnimation(resetChipAnim);

            AnimationData resetMultiplierAnim = new AnimationData();
            resetMultiplierAnim.animationType = AnimationType.UpdateMultiplier;
            resetMultiplierAnim.value = runData.baseMatchMultipliers[MatchType.ROW];
            animationManager.EnqueueAnimation(resetMultiplierAnim);

            AnimationData scoreAnim = new AnimationData();
            scoreAnim.animationType = AnimationType.UpdateScore;
            scoreAnim.value = lastScore;
            animationManager.EnqueueAnimation(scoreAnim);
        }
    }
    public bool TryPlaceBlock(int idx, Vector2Int pos, GameObject blockObj) {
        if (isClearStage) return false;
        Block block = handBlocks[idx];
        bool success = board.PlaceBlock(block, pos);
        if (success) {
            runData.history.blockHistory[(int)handBlocksData[idx].type]++;  // 블록 히스토리 업데이트
            
            // 손에서 블록 제거
            handBlocks[idx] = null;
            handBlocksData[idx] = null;
            
            // UI 업데이트 트리거
            GameUIManager.instance.OnBlockPlaced(blockObj, block, pos);

            // Match 처리된 결과 가져오기 및 애니메이션 실행
            currentMatches = board.GetLastMatches();
            if (currentMatches.Count > 0) {
                AnimationData lineClearAnim = new AnimationData();
                lineClearAnim.animationType = AnimationType.LineClear;
                lineClearAnim.matches = currentMatches;
                animationManager.EnqueueAnimation(lineClearAnim);
            }

            // 아이템 시각 효과 실행
            EffectManager.instance.EndTriggerEffectOnPlace(currentMatches);

            if (!isClearStage && stageManager.CheckStageClear(blockGame))
            {
                isClearStage = true;
                if (blockGame.currentScore >= runData.history.maxScore)
                {
                    runData.history.maxScore = blockGame.currentScore;
                }
                StartCoroutine(DelayedEndStage());

                DataManager.instance.UpdateMaxScore(blockGame.currentScore);
            }

            DataManager.instance.UpdateBlockPlaceCount(block);

            GameManager.instance.ProcessTutorialStep("PlaceBlock");
        }

        // 손패 다 쓰면 드로우
        if (handBlocks.All(block => block == null)) {
            DrawBlocks();
        }
        else if (!isClearStage)
        {
            GameOverCheck();
        }
        return success;
    }

    public void GameOverCheck()
    {
        bool noMoreBlocks = blockGame.deck.Count == 0 && handBlocks.All(b => b == null);
        bool cannotPlaceAny = handBlocksData.All(data => data == null || !board.CanPlaceSome(data));

        HashSet<BlockType> checkedBlockTypes = new();

        foreach (BlockData blockData in handBlocksData)
        {
            if (blockData != null)
            {
                checkedBlockTypes.Add(blockData.type);
            }
        }

        // 덱에 대해 블록 배치 검사
        bool cannotPlayAnyDeck = true;
        if (blockGame.rerollCount > 0)
        {
            foreach (BlockData blockData in blockGame.deck)
            {
                if (!checkedBlockTypes.Contains(blockData.type))
                {
                    if (board.CanPlaceSome(blockData))
                    {
                        cannotPlayAnyDeck = false;
                        break;
                    }
                    checkedBlockTypes.Add(blockData.type);
                }
            }
        }

        cannotPlaceAny = cannotPlaceAny && cannotPlayAnyDeck;

        bool isGameOver = !isClearStage && (noMoreBlocks || cannotPlaceAny);

        if (isGameOver)
        {
            if (noMoreBlocks)
            {
                EndGame(false, loseReason: "덱의 블록을 모두 사용했으나\r\n점수 달성 실패");
            }
            else if (cannotPlaceAny)
            {
                EndGame(false, loseReason: "보드에 블록을\r\n배치할 곳이 없음");
            }
        }
    }

    public void ForceLineClearBoard(MatchType matchType, List<int> indices)
    {
        board.ForceMatches(matchType, indices);
    }

    public void UpdateMultiplierByAdd(int addingValue)
    {
        GameUIManager.instance.UpdateMultiplierByAdd(addingValue);
    }
    
    public void UpdateBaseMultiplier()
    {
        GameUIManager.instance.UpdateMultiplier(runData.baseMatchMultipliers[MatchType.ROW]);
    }

    /// <summary>UI 등에서 SO 대신 런타임 트리거 누적값을 표시할 때 사용.</summary>
    public int GetTriggerCountForTemplate(EffectData template)
    {
        if (template == null)
            return 0;

        EffectState matchedState = runData.activeEffects.FirstOrDefault(s => s.effectDataId == template.id);
        if (matchedState == null)
            return 0;

        return matchedState.triggerCount;
    }

    /// <summary>UI 설명 등에 쓸 현재 수치. 대응 <see cref="EffectState"/>가 없으면 <see cref="EffectData.baseEffectValue"/>.</summary>
    public int GetDisplayEffectValue(EffectData template)
    {
        if (template == null)
            return 0;

        if (runData == null || runData.activeEffects == null)
            return template.baseEffectValue;

        EffectState matchedState = runData.activeEffects.FirstOrDefault(s => s.effectDataId == template.id);
        if (matchedState == null)
            return template.baseEffectValue;

        return matchedState.effectValue;
    }

    public void UpdateItemTriggerCount(EffectState state)
    {
        int index = runData.activeItems.FindIndex(item => item.effects.Any(e => e.id == state.effectDataId));

        if (index == -1) return;

        GameUIManager.instance.StopItemShakeAnimation(isBlockRelated: false);

        ItemData item = runData.activeItems[index];
        if (item.effects.Any(effectData =>
        {
            EffectState matchedState = runData.activeEffects.FirstOrDefault(s => s.effectDataId == effectData.id);
            if (matchedState == null)
                return false;
            return effectData.triggerValue == matchedState.triggerCount + 1;
        }))
        {
            GameUIManager.instance.StartItemShakeAnimation(index, isBlockRelated: false);
        }

        GameUIManager.instance.UpdateItemTriggerCount(index, state.triggerCount);
    }

    public void PlayActivatedItemAnimation()
    {
        for (int i = 0; i < runData.activeItems.Count; i++)
        {
            ItemData item = runData.activeItems[i];
            if (item.effects.Any(effectData =>
            {
                EffectState matchedState = runData.activeEffects.FirstOrDefault(s => s.effectDataId == effectData.id);
                if (matchedState == null)
                    return false;
                return effectData.triggerValue == matchedState.triggerCount + 1;
            }))
            {
                GameUIManager.instance.StartItemShakeAnimation(i, isBlockRelated: false);
            }
        }
    }

    // ------------------------------
    // BLOCKGAME LAYER - end
    // ------------------------------

    // 코드로 ScriptableObject의 값을 변경한 걸 Git의 변경사항으로 적용함
    public void SaveScriptableObjectChanges(ScriptableObject scriptableObject)
    {
        #if UNITY_EDITOR
            EditorUtility.SetDirty(scriptableObject); // 변경 사항을 Dirty 상태로 만듦
            AssetDatabase.SaveAssets(); // 에셋 저장
            AssetDatabase.Refresh(); // Unity 에디터 새로고침
        #endif
    }
}
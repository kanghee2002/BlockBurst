using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using UnityEditor;
using TMPro;

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
    public TutorialManager tutorialManager;

    public DeckData[] deckTemplates;
    public LevelData[] levelTemplates;
    public StageData[] stageTemplates;
    public ItemData[] itemTemplates;
    public BlockData[] blockTemplates;

    public Board board;

    private const int STAGE_CHOICE_COUNT = 2;
    private int CLEAR_CHAPTER = 4;

    public List<BlockData> handBlocksData = new List<BlockData>();
    public List<Block> handBlocks = new List<Block>();

    public Dictionary<ItemType, ItemData[]> shopItems = new();

    private int blockId = 0;

    private StageData[] nextStageChoices = new StageData[STAGE_CHOICE_COUNT];
    private float[] difficulties = new float[STAGE_CHOICE_COUNT];

    private List<Match> currentMatches;
    private float scoreAnimationDelay;  // 블록 점수 간의 딜레이
    private float scoreDelayedTime;     // 점수 계산하는데 걸린 시간

    [SerializeField] private bool isTutorial = true;

    bool isClearStage = false; // 중복 방지 플래그

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
        }

        playerData = null;

        LoadTemplates();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120; // for mobile build
        SetApplicationType();

        if (applicationType == ApplicationType.Windows)
        {
            SceneTransitionManager.instance.TransitionToScene("NewLogoScene");
        }
    }

    // ------------------------------------------------------------------------
    // TEST

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {

            //dataManager.SaveRunData(runData);
        }
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

    private void LoadTemplates()
    {
        // 경로에서 scriptable objects를 로드
        deckTemplates = Resources.LoadAll<DeckData>("ScriptableObjects/Deck");
        levelTemplates = Resources.LoadAll<LevelData>("ScriptableObjects/Level");
        stageTemplates = Resources.LoadAll<StageData>("ScriptableObjects/Stage");
        itemTemplates = Resources.LoadAll<ItemData>("ScriptableObjects/Item");
        blockTemplates = Resources.LoadAll<BlockData>("ScriptableObjects/Block");
    }

    private void LoadPlayerData()
    {
        if (playerData != null)
        {
            TEST_TEXT("ALREADY EXIST PlayerData\n");
            return;
        }

        playerData = DataManager.instance.LoadPlayerData();

        
        TEST_TEXT("LOAD PlayerData\n");

        if (playerData == null)
        {
            playerData = new PlayerData();

            TEST_TEXT("SAVE PlayerData\n");

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
        LoadPlayerData();
        SetTutorialValue(playerData.tutorialValue);

        UnlockManager.instance.Initialize(playerData);

        if (scene.name == "VerticalGameScene" || scene.name == "HorizontalGameScene")
        {
            deckManager = FindObjectOfType<DeckManager>();
            shopManager = FindObjectOfType<ShopManager>();
            stageManager = FindObjectOfType<StageManager>();
            tutorialManager = FindObjectOfType<TutorialManager>();

            GameUIManager.instance.SetTutorialToggleValue(isTutorial);

            AudioManager.instance.BeginBackgroundMusic();
        }
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

        CLEAR_CHAPTER = 4;

        shopItems = new Dictionary<ItemType, ItemData[]>()
        {
            { ItemType.ITEM, new ItemData[2] },
            { ItemType.BOOST, new ItemData[2] },
            { ItemType.ADD_BLOCK, new ItemData[2] },
        };

        DeckData deckData = deckTemplates.FirstOrDefault(deck => deck.type == deckType);
        deckData.Initialize();

        LevelData levelData = levelTemplates.FirstOrDefault(data => data.level == level);

        gameData = new GameData();
        gameData.Initialize(blockTemplates, deckData, levelData);
        
        scoreAnimationDelay = 0.01f;
        scoreDelayedTime = 0f;
        currentMatches = new List<Match>();

        StartNewRun();
        
        //ContinueGame();
    }

    public void ContinueGame()
    {
        /*foreach (BlockData blockData in blockTemplates)
        {
            if (Enums.IsSpecialBlockType(blockData.type))
            {
                continue;
            }
            for (int i = 0; i < gameData.defaultBlockCount; i++)
            {
                gameData.defaultBlocks.Add(blockData);
            }
        }
        runData = DataManager.instance.LoadRunData(gameData);

        CLEAR_CHAPTER = 3;

        InitializeHistory();

        scoreAnimationDelay = 0.01f;
        scoreDelayedTime = 0f;
        currentMatches = new List<Match>();

        CellEffectManager.instance.Initialize();
        EffectManager.instance.Initialize(ref runData);

        stageManager.Initialize(ref runData);
        shopManager.Initialize(ref runData, itemTemplates);

        UpdateDeckCount(runData.availableBlocks.Count, runData.availableBlocks.Count);

        GameUIManager.instance.Initialize(runData);

        StartStageSelection();*/
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
        BlockType mostPlacedBlockType = (BlockType)runData.history.blockHistory.ToList().IndexOf(runData.history.blockHistory.Max());
        GameUIManager.instance.OnGameEnd(isWin, runData.currentChapterIndex, runData.currentStageIndex, runData.history, mostPlacedBlockType, loseReason: loseReason);
    }

    public void InfiniteMode()
    {
        CLEAR_CHAPTER = -1;
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

        ItemData[] unlockedItems = UnlockManager.instance.GetUnlockedItems(itemTemplates);

        stageManager.Initialize(ref runData);

        shopManager.Initialize(ref runData, unlockedItems);

        UpdateDeckCount(runData.availableBlocks.Count, runData.availableBlocks.Count);

        GameUIManager.instance.Initialize(runData);

        StartStageSelection();

        if (isTutorial)
        {
            isTutorial = false;
            DataManager.instance.SetTutorialValue(isTutorial);
            tutorialManager.Initialize();
        }
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
        GameUIManager.instance.OnDeckLevelInfoCallback(deckTemplates, levelTemplates);
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
        var templates = stageTemplates.Where(stage => stage.type == stageType).ToArray();
        
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
            templates[indices[i]].constraints.ForEach(constraint => constraint.triggerCount = 0);
            nextStageChoices[i] = templates[indices[i]];
        }

        //디버깅, 튜토리얼 시에만 실행
        List<string> stageNames = stageManager.firstStageList;
        if (stageNames.Count > 0) 
        {
            for (int i = 0; i < nextStageChoices.Length; i++)
            {
                if (stageNames.Count == 0) break;
                StageData stage = templates.FirstOrDefault(x => x.id == stageNames[0]);
                stage.constraints.ForEach(constraint => constraint.triggerCount = 0);
                stageNames.RemoveAt(0);
                nextStageChoices[i] = stage;
            }
        }

        // 스테이지 난이도 설정
        for (int i = 0; i < nextStageChoices.Length; i++)
        {
            StageData stage = nextStageChoices[i];

            if (runData.currentChapterIndex <= gameData.stageBaseScoreList.Count)
            {
                int stageBaseScore = gameData.stageBaseScoreList[runData.currentChapterIndex - 1];
                float scoreMultiplier = gameData.stageScoreMultiplier[runData.currentStageIndex - 1];
                float stageScoreMultiplier = stage.baseScoreMultiplier;
                stage.clearRequirement = (int)(stageBaseScore * (scoreMultiplier + stageScoreMultiplier));
            }
            else
            {
                stage.clearRequirement = (int)(gameData.stageBaseScoreList[gameData.stageBaseScoreList.Count - 1] * Mathf.Pow(2f, 3 * (runData.currentChapterIndex - 9) + runData.currentStageIndex + 2) * (gameData.stageScoreMultiplier[runData.currentStageIndex - 1] + stage.baseScoreMultiplier));
            }
            stage.goldReward = gameData.stageBaseReward + (int)MathF.Pow(runData.currentChapterIndex + 2, 0.5f) * 3 + Random.Range(0, 2);
        }

        // UI에 전달
        GameUIManager.instance.OnStageSelection(nextStageChoices, runData.currentChapterIndex, runData.currentStageIndex);
    }

    public void OnStageSelection(int choiceIndex)
    {
        // 선택된 스테이지로 진행
        StageData selectedStage = nextStageChoices[choiceIndex];

        StartStage(selectedStage);
        GameUIManager.instance.OnStageStart(runData.currentChapterIndex, runData.currentStageIndex, selectedStage, blockGame);
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

        EffectManager.instance.InitializeBlockGameData(ref blockGame);
        CellEffectManager.instance.InitializeBlockGame(ref blockGame);

        // 스테이지 시작
        stageManager.StartStage(stage);

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
        if (runData.currentChapterIndex == CLEAR_CHAPTER && stageManager.currentStage.type == StageType.BOSS)
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

        GameUIManager.instance.PlayStageClearAnimation(scoreAnimationDelay);

        AudioManager.instance.SFXStageClear();

        yield return new WaitForSeconds(1.5f);

        EndStage();

        yield return new WaitForSeconds(0.5f);

        // 튜토리얼 진행 시 호출
        if (runData.currentChapterIndex == 1 &&
            (runData.currentStageIndex == 2 ||
             runData.currentStageIndex == 3))
        {
            ProcessTutorialStep("EndStage");
        }
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

        // 부스트 구매 시 시각 효과
        if (res != - 1 && shopItem.type == ItemType.BOOST)
        {
            if (shopItem.effects.Any(effect =>
            effect.type == EffectType.BASEMULTIPLIER_MODIFIER ||
            effect.type == EffectType.BASEMULTIPLIER_MULTIPLIER))
            {
                UpdateBaseMultiplier();
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

    public void UpdateGold(int value, bool isMultiplying = false, bool isStageReward = false)
    {
        int prevGold = runData.gold;

        if (isMultiplying)
        {
            runData.gold *= value;
        }
        else
        {
            runData.gold += value;
            if (runData.gold < 0) runData.gold = 0;
        }

        int currentGold = runData.gold;

        GameUIManager.instance.UpdateGold(runData.gold);

        // 골드를 얻을 때, 스테이지 보상이 아닐 때만 실행
        if (currentGold > prevGold && !isStageReward)
        {
            AudioManager.instance.SFXGold();
        }
    }

    public void UpdateRerollCount(int value, bool isMultiplying = false)
    {
        // RunData.RerollCount는 플레이 중 드러나지 않으므로 처리 X
        if (isMultiplying)
        {
            blockGame.rerollCount *= value;
        }
        else
        {
            blockGame.rerollCount += value;
            if (blockGame.rerollCount < 0) blockGame.rerollCount = 0;
        }

        GameUIManager.instance.DisplayRerollCount(blockGame.rerollCount);
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
        ItemData itemData = itemTemplates.FirstOrDefault(item => item.id == itemID);
        
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

        foreach (EffectData effect in runData.activeEffects)
        {
            if (effect.scope == EffectScope.Stage && effect.blockTypes.Contains(block.Type))
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
        foreach (EffectData effect in runData.activeEffects)
        {
            if (effect.scope == EffectScope.Stage && effect.triggerMode == TriggerMode.Interval)
            {
                if (effect.triggerValue == effect.triggerCount + 1)
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

    public void PlayItemEffectAnimation(List<string> effectIdList, float matchAnimationTime = 0f)
    {
        StartCoroutine(ItemEffectAnimationCoroutine(effectIdList, matchAnimationTime));
    }

    private IEnumerator ItemEffectAnimationCoroutine(List<string> effectIdList, float matchAnimationTime)
    {
        yield return new WaitForSeconds(matchAnimationTime);

        // 임시 구현 (블록 강화 시각 효과)
        foreach (string effectId in effectIdList)
        {
            bool isItemEffect = runData.activeItems
                                .Any(item => item.effects
                                .Any(effect => effect.id == effectId));
            if (!isItemEffect)
            {
                EffectData effect = runData.activeEffects.Find(x => x.id == effectId);

                if (effect != null)
                {
                    if (effect.type == EffectType.MULTIPLIER_MODIFIER)
                    {
                        UpdateMultiplierByAdd(effect.effectValue);
                    }
                }
            }
        }

        int delayCount = 0;
        float delay = 0.25f;    // 아이템 효과 간의 딜레이
        for (int i = 0; i < runData.activeItems.Count; i++)
        {
            ItemData currentItem = runData.activeItems[i];

            foreach (EffectData effect in currentItem.effects)
            {
                if (effectIdList.Contains(effect.id))
                {
                    ProcessItemEffectAnimation(effect, i, delay);

                    delayCount++;

                    yield return new WaitForSeconds(delay);
                }
            }
        }

        // 매칭 진행 중이라면
        if (matchAnimationTime > 0f)
        {
            yield return new WaitForSeconds(0.3f);

            StartCoroutine(CalculateScoreAnimationCoroutine());
        }
    }

    private IEnumerator CalculateScoreAnimationCoroutine()
    {
        int lastScore = ScoreCalculator.instance.GetLastScore();
        GameUIManager.instance.UpdateProduct(lastScore);
        GameUIManager.instance.UpdateChip(0);
        UpdateBaseMultiplier();

        // 계산된 점수가 얻은 점수에 더해지기 전 딜레이
        float delay = 0.2f;
        yield return new WaitForSeconds(delay);

        GameUIManager.instance.UpdateProduct(0);
        GameUIManager.instance.UpdateScore(blockGame.currentScore);
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
                Dictionary<Match, List<int>> scores = GetScoreDictionary(currentMatches);
                GameUIManager.instance.PlayMatchAnimation(currentMatches, scores, scoreAnimationDelay);
            }

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

            tutorialManager.TriggerNotificationAnimation();
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
        bool cannotPlaceAny = blockGame.rerollCount == 0 &&
                              handBlocksData.All(data => data == null || !board.CanPlaceSome(data));

        bool isGameOver = !isClearStage && (noMoreBlocks || cannotPlaceAny);

        if (isGameOver)
        {
            if (noMoreBlocks)
            {
                EndGame(false, loseReason: "점수 부족");
            }
            else if (cannotPlaceAny)
            {
                EndGame(false, loseReason: "더 이상 둘 곳이 없음");
            }
        }
    }

    public void ForceLineClearBoard(MatchType matchType, List<int> indices)
    {
        board.ForceMatches(matchType, indices);
    }

    public float GetMatchAnimationTime(List<Match> matches)
    {
        float result = 0f;
        float lastDelay = 0.3f;     // 매칭 이후 아이템 애니메이션 진행 전 딜레이

        foreach (Match match in matches)
        {
            if (match.matchType == MatchType.ROW)
            {
                result += scoreAnimationDelay * blockGame.boardRows;
            }
            else if (match.matchType == MatchType.COLUMN)
            {
                result += scoreAnimationDelay * blockGame.boardColumns;
            }
        }

        if (matches.Count > 0)
        {
            result += lastDelay;
        }

        return result;
    }
    
    public void UpdateMultiplierByAdd(int addingValue)
    {
        GameUIManager.instance.UpdateMultiplierByAdd(addingValue);
    }

    public void UpdateMultiplier()
    {
        GameUIManager.instance.UpdateMultiplier(blockGame.matchMultipliers[MatchType.ROW]);
    }
    
    public void UpdateBaseMultiplier()
    {
        GameUIManager.instance.UpdateMultiplier(runData.baseMatchMultipliers[MatchType.ROW]);
    }

    public void UpdateItemTriggerCount(EffectData effect)
    {
        int index = runData.activeItems.FindIndex(item => item.effects.Contains(effect));

        if (index == -1) return;

        GameUIManager.instance.StopItemShakeAnimation(isBlockRelated: false);

        // 발동 직전 횟수라면
        if (runData.activeItems[index].effects.Any(effect => effect.triggerValue == effect.triggerCount + 1))
        {
            GameUIManager.instance.StartItemShakeAnimation(index, isBlockRelated: false);
        }

        GameUIManager.instance.UpdateItemTriggerCount(index, effect.triggerCount);
    }

    public void PlayActivatedItemAnimation()
    {
        for (int i = 0; i < runData.activeItems.Count; i++)
        {
            ItemData item = runData.activeItems[i];
            if (item.effects.Any(effect => effect.triggerValue == effect.triggerCount + 1))
            {
                GameUIManager.instance.StartItemShakeAnimation(i, isBlockRelated: false);
            }
        }
    }

    // Match에 해당하는 점수 리스트 만들기
    private Dictionary<Match, List<int>> GetScoreDictionary(List<Match> matches)
    {
        Dictionary<Match, List<int>> dictionary = new Dictionary<Match, List<int>>();

        foreach (Match match in matches)
        {
            List<int> scores = new List<int>();

            int blockIndex = 0;
            if (match.matchType == MatchType.ROW)
            {
                for (int x = 0; x < blockGame.boardColumns; x++)
                {
                    if (match.validIndices.Contains(x))
                    {
                        BlockType blockType = match.blocks[blockIndex].Item1;
                        int score = blockGame.blockScores[blockType];
                        scores.Add(score);
                        blockIndex++;
                    }
                    else
                    {
                        scores.Add(0);
                    }
                }
            }
            else if (match.matchType == MatchType.COLUMN)
            {
                for (int y = 0; y < blockGame.boardRows; y++)
                {
                    if (match.validIndices.Contains(y))
                    {
                        BlockType blockType = match.blocks[blockIndex].Item1;
                        int score = blockGame.blockScores[blockType];
                        scores.Add(score);
                        blockIndex++;
                    }
                    else
                    {
                        scores.Add(0);
                    }
                }
            }
            dictionary.Add(match, scores);
        }
        return dictionary;
    }

    private void ProcessItemEffectAnimation(EffectData effect, int index, float delay)
    {
        string description = "";

        string value = effect.effectValue > 0 ? "+" + effect.effectValue : effect.effectValue.ToString();
        
        switch (effect.type)
        {
            case EffectType.SCORE_MODIFIER:
                description = "<color=#0088FF>" + value + "</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.SCORE_MULTIPLIER:
                description = "<color=#0088FF>X" + effect.effectValue + "</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.MULTIPLIER_MODIFIER:
                description = "<color=red>" + value + "</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                UpdateMultiplierByAdd(effect.effectValue);
                break;
            case EffectType.BASEMULTIPLIER_MODIFIER:
                description = "<color=red>" + value + "</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                UpdateMultiplier();
                break;
            case EffectType.BASEMULTIPLIER_MULTIPLIER:
                description = "<color=red>X" + effect.effectValue + "</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                UpdateMultiplier();
                break;
            case EffectType.REROLL_MODIFIER:
            case EffectType.BASEREROLL_MODIFIER:
                description = "<color=white>" + value + "</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.REROLL_MULTIPLIER:
            case EffectType.BASEREROLL_MULTIPLIER:
                description = "<color=white>X" + effect.effectValue + "</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.GOLD_MODIFIER:
                description = "<color=yellow>" + value + "</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.GOLD_MULTIPLIER:
                description = "<color=yellow>X" + effect.effectValue + "</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.BOARD_SIZE_MODIFIER:
                description = "<color=white>보드 크기" + value + "</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.BOARD_CORNER_BLOCK:
                description = "<color=white>보드 가장자리\n막힘!</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.BOARD_RANDOM_BLOCK:
                description = "<color=white>보드 무작위\n" + effect.effectValue +"칸 막힘!</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.DECK_MODIFIER:
                // TODO
                break;
            case EffectType.BLOCK_REUSE_MODIFIER:
                // TODO
                break;
            case EffectType.SQUARE_CLEAR:
                // TODO
                break;
            case EffectType.BLOCK_MULTIPLIER:
                description = "<color=white>블록 " + effect.effectValue + "배!</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.BLOCK_DELETE:
                description = "<color=white>블록 모두\n삭제!</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.BLOCK_DELETE_WITH_COUNT:
                // TODO
                break;
            case EffectType.ROW_LINE_CLEAR:
            case EffectType.RANDOM_ROW_LINE_CLEAR:
                description = "<color=white>가로\n지움!</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.COLUMN_LINE_CLEAR:
            case EffectType.RANDOM_COLUMN_LINE_CLEAR:
                description = "<color=white>세로\n지움!</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.BOARD_CLEAR:
                description = "<color=white>보드\n지움!</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.DRAW_BLOCK_COUNT_MODIFIER:
                description = "<color=white>선택지가\n" + effect.effectValue +"개로!</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.RANDOM_LINE_CLEAR:
                description = "<color=white>무작위\n한 줄 지움!</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.EFFECT_VALUE_MODIFIER:
                if (effect.modifyingEffect.type == EffectType.GOLD_MODIFIER)
                {
                    description = "<color=yellow>" + value + "</color>";
                }
                else
                {
                    description = "<color=red>" + value + "</color>";
                }
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.MULTIPLIER_MULTIPLER:
                description = "<color=red>X" + effect.effectValue + "</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                UpdateMultiplier();
                break;

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
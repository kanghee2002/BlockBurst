using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public GameData gameData;
    public RunData runData;
    public BlockGameData blockGame;

    public DeckManager deckManager;
    public ShopManager shopManager;
    public StageManager stageManager;

    public StageData[] stageTemplates;
    public ItemData[] itemTemplates;
    public BlockData[] blockTemplates;

    const int STAGE_CHOICE_COUNT = 2;
    public int currentChapterIndex = 1;
    public int currentStageIndex = 1;

    public List<BlockData> handBlocksData = new List<BlockData>();
    public List<Block> handBlocks = new List<Block>();

    public List<ItemData> shopItems = new List<ItemData>();
    const int SHOP_ITEM_COUNT = 3;

    private int blockId = 0;

    public Board board;

    StageData[] nextStageChoices = new StageData[STAGE_CHOICE_COUNT];
    float[] difficulties = new float[STAGE_CHOICE_COUNT];

    private float scoreAnimationDelay;

    public float startTime; // 게임 시작 시간
    public int[] blockHistory;

    bool isClearStage = false; // 중복 방지 플래그

    // ------------------------------
    // GAME LAYER - start
    // ------------------------------

    void Awake()
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
        LoadTemplates();
    }

    void LoadTemplates()
    {
        // 경로에서 scriptable objects를 로드
        stageTemplates = Resources.LoadAll<StageData>("ScriptableObjects/Stage");
        itemTemplates = Resources.LoadAll<ItemData>("ScriptableObjects/Item");
        blockTemplates = Resources.LoadAll<BlockData>("ScriptableObjects/Block");
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
        if (scene.name == "GameScene")
        {
            deckManager = FindObjectOfType<DeckManager>();
            shopManager = FindObjectOfType<ShopManager>();
            stageManager = FindObjectOfType<StageManager>();
            StartNewGame();
        }
    }

    public void GoToGameScene()
    {
        AudioManager.instance.SFXSelectMenu();
        SceneManager.LoadScene("GameScene");
    }

    public void StartNewGame()
    {
        // 각종 초기화
        Debug.Log("Game Start");

        currentChapterIndex = 1;
        currentStageIndex = 1;

        gameData = new GameData();
        gameData.Initialize();
        
        foreach (BlockData blockData in blockTemplates)
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
        startTime = Time.time;
        scoreAnimationDelay = 0.025f;

        blockHistory = new int[Enum.GetValues(typeof(BlockType)).Length];

        StartNewRun();
    }
    
    public void EndGame(bool isWin)
    {
        // 게임 종료 처리
        if (isWin)
        {
            Debug.Log("Game Win");
        }
        else
        {
            Debug.Log("Game Lose");
        }
    }

    public void BackToMain()
    {
        // 메인 화면으로 돌아가기
        Debug.Log("Back to Main");
        SceneManager.LoadScene("NewLogoScene");
    }

    public void MakeNewRun()
    {
        SceneManager.LoadScene("GameScene");
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

        EffectManager.instance.Initialize(ref runData);

        stageManager.Initialize(ref runData);

        shopManager.Initialize(ref runData, itemTemplates);

        UpdateDeckCount(runData.availableBlocks.Count, runData.availableBlocks.Count);

        GameUIManager.instance.Initialize(runData);

        StartStageSelection();
    }

    public void OnRunInfoRequested()
    {
        // 최대로 사용된 블록 찾기
        BlockType mostPlacedBlockType = (BlockType)blockHistory.ToList().IndexOf(blockHistory.Max());
        // Run 정보 UI 열기
        GameUIManager.instance.OnRunInfoCallback(runData, startTime, mostPlacedBlockType);
    }

    public void OnDeckInfoRequested()
    {
        // Deck 정보 UI 열기
        GameUIManager.instance.OnDeckInfoCallback(runData, blockGame);
    }

    public void StartStageSelection()
    {
        // stage Template에서 stagetype이 맞는 것을 랜덤하게 추출
        StageType stageType = currentStageIndex == 3 ? StageType.BOSS : StageType.NORMAL;
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
            nextStageChoices[i] = templates[indices[i]];
        }

        // 스테이지 난이도 설정
        for (int i = 0; i < nextStageChoices.Length; i++)
        {
            StageData stage = nextStageChoices[i];
            difficulties[i] = gameData.difficulty * UnityEngine.Random.Range(gameData.stageBaseScoreMultipliers[0], gameData.stageBaseScoreMultipliers[1]);
            stage.clearRequirement = (int)(gameData.stageBaseScores * difficulties[i] / 10f) * 10;
            stage.goldReward = (int)(gameData.stageBaseReward * Mathf.Sqrt(difficulties[i]));
        }

        // UI에 전달
        GameUIManager.instance.OnStageSelection(nextStageChoices, currentChapterIndex, currentStageIndex);
    }

    public void OnStageSelection(int choiceIndex)
    {
        // 선택된 스테이지로 진행
        StageData selectedStage = nextStageChoices[choiceIndex];

        gameData.difficulty = difficulties[choiceIndex];

        StartStage(selectedStage);
        GameUIManager.instance.OnStageStart(currentChapterIndex, currentStageIndex, selectedStage, blockGame);
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

        EffectManager.instance.InitializeBlockGameData(ref blockGame);

        // 스테이지 시작
        stageManager.StartStage(stage);

        board = new Board();
        board.Initialize(blockGame);

        // 보드와 프론트를 막음
        SetInactiveBlockCells(blockGame);
        board.BlockCells(blockGame.inactiveCells);

        deckManager.Initialize(ref blockGame, ref runData);

        DrawBlocks();

        isClearStage = false;
    }

    public void EndStage(bool cleared)
    {
        if (cleared)
        {
            stageManager.GrantReward();
            if (stageManager.currentStage.type == StageType.BOSS)
            {
                currentChapterIndex++;
                currentStageIndex = 1;
                
            }
            else 
            {
                currentStageIndex++;
            }
            StartShop(true);
            UpdateDeckCount(runData.availableBlocks.Count, runData.availableBlocks.Count);
            UpdateBaseMultiplier();
        } 
        else 
        {
            EndGame(false);
        }
    }

    IEnumerator DelayedEndStage(bool cleared)
    {
        yield return new WaitForSeconds(2.0f);
        EndStage(cleared);
    }

    public void StartShop(bool isFirst = false)
    {
        // 아이템 랜덤하게 뽑아서 UI에 전달
        shopItems.Clear();
        for (int i = 0; i < SHOP_ITEM_COUNT; i++)
        {
            shopItems.Add(shopManager.PopItem());
        }
        GameUIManager.instance.OnShopStart(shopItems, shopManager.rerollCost, isFirst);   
    }

    public int OnItemPurchased(int index)
    {
        ItemData shopItem = shopItems[index];
        shopItems[index] = null;
        int res = shopManager.PurchaseItem(shopItem);
        if (res != -1)
        {
            GameUIManager.instance.DisplayItemSet(runData.activeItems);
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
        if (runData.gold < shopManager.rerollCost)
        {
            UpdateGold(0);
            return;
        }

        UpdateGold(-shopManager.rerollCost);
        List<ItemData> remains = shopItems.Where(item => item != null).ToList();
        StartShop();
        shopManager.RerollShop(remains);
    }

    public void UpdateGold(int value, bool isMultiplying = false)
    {
        if (isMultiplying)
        {
            runData.gold *= value;
        }
        else
        {
            runData.gold += value;
            if (runData.gold < 0) runData.gold = 0;
        }
        GameUIManager.instance.UpdateGold(runData.gold);
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
            handBlocks.Add(new Block());
            handBlocks[i].Initialize(handBlocksData[i], blockId++);
        }
        GameUIManager.instance.OnBlocksDrawn(handBlocks);
        UpdateDeckCount(blockGame.deck.Count, runData.availableBlocks.Count);
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
            EffectManager.instance.TriggerEffects(TriggerType.ON_REROLL);
            DrawBlocks();
        }
        EffectManager.instance.EndTriggerEffect();
    }

    public void PlayItemEffectAnimation(List<string> effectIdList, float matchAnimationTIme = 0f)
    {
        StartCoroutine(ItemEffectAnimationCoriotine(effectIdList, matchAnimationTIme));
    }

    private IEnumerator ItemEffectAnimationCoriotine(List<string> effectIdList, float matchAnimationTIme = 0f)
    {
        yield return new WaitForSeconds(matchAnimationTIme);

        // 임시 구현 (블록 강화 시각 효과)
        foreach (string effectId in effectIdList)
        {
            bool isItemEffect = runData.activeItems
                                .Any(item => item.effects
                                .Any(effect => effect.id == effectId));
            if (!isItemEffect)
            {
                EffectData effect = runData.activeEffects.Find(x => x.id == effectId);

                if (effect.type == EffectType.MULTIPLIER_MODIFIER)
                {
                    UpdateMultiplierByAdd(effect.effectValue);
                }
            }
        }


        int delayCount = 0;
        float delay = 0.5f;
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
        if (matchAnimationTIme > 0f)
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

        yield return new WaitForSeconds(1.2f);

        GameUIManager.instance.UpdateProduct(0);
        GameUIManager.instance.UpdateScore(blockGame.currentScore);
    }

    public bool TryPlaceBlock(int idx, Vector2Int pos, GameObject blockObj) {
        Block block = handBlocks[idx];
        bool success = board.PlaceBlock(block, pos);
        if (success) {
            blockHistory[(int)handBlocksData[idx].type]++;  // 블록 히스토리 업데이트

            // 손에서 블록 제거
            handBlocks[idx] = null;
            handBlocksData[idx] = null;
            
            // UI 업데이트 트리거
            GameUIManager.instance.OnBlockPlaced(blockObj, block, pos);

            // Match 처리된 결과 가져오기 및 애니메이션 실행
            List<Match> matches = board.GetLastMatches();
            if (matches.Count > 0) {
                Dictionary<Match, List<int>> scores = GetScoreDictionary(matches);
                GameUIManager.instance.PlayMatchAnimation(matches, scores, scoreAnimationDelay);
            }

            if (!isClearStage && stageManager.CheckStageClear(blockGame))
            {
                isClearStage = true;
                StartCoroutine(DelayedEndStage(true));
            }
        }

        // 손패 다 쓰면 드로우
        if (handBlocks.All(block => block == null)) {
            DrawBlocks();
        }
        return success;
    }

    public void ForceLineClear(List<(MatchType, List<int>)> lines)
    {
        foreach ((MatchType matchType, List<int> indices) in lines) 
        {
            if (matchType == MatchType.ROW)
            {
                board.ForceRowMatches(indices);
            }
            else if (matchType == MatchType.COLUMN)
            {
                board.ForceColumnMatches(indices);
            }
        }

        // Match 처리된 결과 가져오기 및 애니메이션 실행
        List<Match> matches = board.GetLastMatches();
        if (matches.Count > 0)
        {
            Dictionary<Match, List<int>> scores = GetScoreDictionary(matches);
            GameUIManager.instance.PlayMatchAnimation(matches, scores, scoreAnimationDelay);
        }

        if (stageManager.CheckStageClear(blockGame))
        {
            StartCoroutine(DelayedEndStage(true));
        }
    }

    public float GetMatchAnimationTime(List<Match> matches)
    {
        float result = 0f, lastDelay = 0.5f;

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

        if (result != 0f)
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
                description = "<color=white>가로 한 줄\n지움!</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
            case EffectType.COLUMN_LINE_CLEAR:
                description = "<color=white>세로 한 줄\n지움!</color>";
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
                description = "<color=white>무작위 한 줄\n지움!</color>";
                GameUIManager.instance.PlayItemEffectAnimation(description, index, delay);
                break;
        }
        
    }

    // ------------------------------
    // BLOCKGAME LAYER - end
    // ------------------------------

}
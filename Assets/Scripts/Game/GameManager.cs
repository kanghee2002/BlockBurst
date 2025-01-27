using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

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

    private float scoreAnimationDelay;

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

    void Start()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        // 각종 초기화
        Debug.Log("Game Start");
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

        StartNewRun();

        scoreAnimationDelay = 0.05f;
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

    public void StartStageSelection()
    {
        // stage Template에서 stagetype이 맞는 것을 랜덤하게 추출
        StageType stageType = currentStageIndex == 3 ? StageType.BOSS : StageType.NORMAL;
        var templates = stageTemplates.Where(stage => stage.type == stageType).ToArray();
        for (int i = 0; i < nextStageChoices.Length; i++)
        {
            nextStageChoices[i] = templates[Random.Range(0, templates.Length)];
        }

        // 스테이지 목표 점수 설정
        for (int i = 0; i < nextStageChoices.Length; i++)
        {
            StageData stage = nextStageChoices[i];
            gameData.difficulty *= gameData.stageBaseScoreMultipliers[Random.Range(0, gameData.stageBaseScoreMultipliers.Length)];
            stage.clearRequirement = (int)(gameData.stageBaseScores * gameData.difficulty);
            stage.goldReward = (int)(gameData.stageBaseReward * gameData.difficulty);
        }

        // UI에 전달
        GameUIManager.instance.OnStageSelection(nextStageChoices, currentChapterIndex, currentStageIndex);
    }

    public void OnStageSelection(int choiceIndex)
    {
        // 선택된 스테이지로 진행
        StageData selectedStage = nextStageChoices[choiceIndex];

        StartStage(selectedStage);
        GameUIManager.instance.OnStageStart(currentChapterIndex, currentStageIndex, selectedStage, blockGame);
        GameUIManager.instance.BlockCells(GetInactiveBlockCells(blockGame));
    }

    HashSet<Vector2Int> GetInactiveBlockCells(BlockGameData data)
    {
        HashSet<Vector2Int> inactiveBlockCells = new HashSet<Vector2Int>(data.inactiveBlockCells);
        if (data.isCornerBlocked)
        {
            inactiveBlockCells.Add(new Vector2Int(0, 0));
            inactiveBlockCells.Add(new Vector2Int(0, data.boardColumns - 1));
            inactiveBlockCells.Add(new Vector2Int(data.boardRows - 1, 0));
            inactiveBlockCells.Add(new Vector2Int(data.boardRows - 1, data.boardColumns - 1));
        }
        return inactiveBlockCells;
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
        HashSet<Vector2Int> inactiveBlockCells = GetInactiveBlockCells(blockGame);
        board.BlockCells(inactiveBlockCells);

        deckManager.Initialize(ref blockGame, ref runData);

        DrawBlocks();
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

        UpdateBaseMultiplier();

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

    public RunData GetDeckInfo()
    {
        return runData;
    }

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

            if (stageManager.CheckStageClear(blockGame))
            {
                StartCoroutine(DelayedEndStage(true));
            }
        }

        // 손패 다 쓰면 드로우
        if (handBlocks.All(block => block == null)) {
            DrawBlocks();
        }
        return success;
    }

    public void ForceLineClear(MatchType matchType, List<int> indices)
    {
        if (matchType == MatchType.ROW)
        {
            board.ForceRowMatches(indices);
        }
        else if (matchType == MatchType.COLUMN)
        {
            board.ForceColumnMatches(indices);
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
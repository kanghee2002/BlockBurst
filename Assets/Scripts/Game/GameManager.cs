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
        
        // 템플릿에서 몇개 랜덤으로 뽑아 추가
        for (int i = 0; i < gameData.defaultBlockCount; i++)
        {
            gameData.defaultBlocks.Add(blockTemplates[Random.Range(0, blockTemplates.Length)]);
        }

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
        GameUIManager.instance.DisplayDeckCount(gameData.defaultBlockCount, gameData.defaultBlockCount);

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

        // UI에 전달
        GameUIManager.instance.OnStageSelection(nextStageChoices, currentChapterIndex, currentStageIndex);
    }

    public void OnStageSelection(int choiceIndex)
    {
        // 선택된 스테이지로 진행
        StageData selectedStage = nextStageChoices[choiceIndex];
        StartStage(selectedStage);
        GameUIManager.instance.OnStageStart(currentChapterIndex, currentStageIndex, selectedStage, blockGame);
    }

    public void StartStage(StageData stage)
    {
        blockGame = new BlockGameData();
        blockGame.Initialize(runData);

        board = new Board();
        board.Initialize(blockGame);

        deckManager.Initialize(ref blockGame, runData.availableBlocks);

        EffectManager.instance.InitializeBlockGameData(ref blockGame);
        
        // 스테이지 시작
        stageManager.StartStage(stage);
        DrawBlocks();
    }

    public void EndStage(bool cleared)
    {
        if (cleared)
        {
            stageManager.GrantReward();
            GameUIManager.instance.UpdateGold(runData.gold);
            if (stageManager.currentStage.type == StageType.BOSS)
            {
                currentChapterIndex++;
                currentStageIndex = 1;
                
            }
            else 
            {
                currentStageIndex++;
            }
            StartShop();
        } 
        else 
        {
            EndGame(false);
        }
    }

    public void StartShop()
    {
        // 아이템 랜덤하게 뽑아서 UI에 전달
        shopItems.Clear();
        for (int i = 0; i < SHOP_ITEM_COUNT; i++)
        {
            shopItems.Add(shopManager.PopItem());
        }
        GameUIManager.instance.OnShopStart(shopItems);
        
    }

    public int OnItemPurchased(int index)
    {
        ItemData shopItem = shopItems[index];
        shopItems[index] = null;
        return shopManager.PurchaseItem(shopItem);
    }

    public void OnShopReroll()
    {
        List<ItemData> remains = shopItems.Where(item => item != null).ToList();
        StartShop();
        shopManager.RerollShop(remains);
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
        GameUIManager.instance.DisplayDeckCount(blockGame.deck.Count, gameData.defaultBlockCount);
    }

    public void OnRerolled()
    {
        if (deckManager.RerollDeck(handBlocksData))
        {
            GameUIManager.instance.DisplayRerollCount(blockGame.rerollCount);
            DrawBlocks();
        }
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
                GameUIManager.instance.PlayMatchAnimation(matches);
            }

            GameUIManager.instance.UpdateScore(blockGame.currentScore);
            if (stageManager.CheckStageClear(blockGame))
            {
                EndStage(true);
            }
        }

        // 손패 다 쓰면 드로우
        if (handBlocks.All(block => block == null)) {
            DrawBlocks();
        }
        return success;
    }
    

    // ------------------------------
    // BLOCKGAME LAYER - end
    // ------------------------------

}
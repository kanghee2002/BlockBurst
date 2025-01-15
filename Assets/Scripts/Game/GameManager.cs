using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public GameData gameData;
    public RunData currentRun;
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
    const int HAND_BLOCK_COUNT = 3;

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
        for (int i = 0; i < 10; i++)
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

    // ------------------------------
    // GAME LAYER - end
    // ------------------------------

    // ------------------------------
    // RUN LAYER - start
    // ------------------------------

    public void StartNewRun()
    {
        // 초기화
        currentRun = new RunData();
        currentRun.Initialize(gameData);

        EffectManager.instance.Initialize(ref currentRun);

        stageManager.Initialize(ref currentRun);

        shopManager.Initialize(ref currentRun, itemTemplates);

        GameUIManager.instance.Initialize(currentRun);
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
        GameUIManager.instance.OnStageStart(currentStageIndex, selectedStage);
    }

    public void StartStage(StageData stage)
    {
        blockGame = new BlockGameData();
        blockGame.Initialize(currentRun);

        board = new Board();
        board.Initialize(currentRun, blockGame);

        deckManager.Initialize(ref currentRun, ref blockGame);

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
            if (stageManager.currentStage.type == StageType.BOSS)
            {
                EndGame(true);
            }
            else 
            {
                currentStageIndex++;
                StartShop();
            }
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

    public bool OnItemPurchased(int index)
    {
        ItemData shopItem = shopItems[index];
        shopItems[index] = null;
        return shopManager.PurchaseItem(shopItem);
    }

    public void OnShopReroll()
    {
        shopManager.RerollShop(shopItems);
        StartShop();
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
        for (int i = 0; i < HAND_BLOCK_COUNT; i++)
        {
            handBlocksData.Add(deckManager.DrawBlock());
            handBlocks.Add(new Block());
            handBlocks[i].Initialize(handBlocksData[i], blockId++);
        }
        GameUIManager.instance.OnBlocksDrawn(handBlocks);
    }

    public void OnRerolled()
    {
        if (deckManager.RerollDeck(handBlocksData.ToArray()))
        {
            DrawBlocks();
        }
    }

    public bool TryPlaceBlock(int idx, Vector2Int pos) {
        Block block = handBlocks[idx];
        bool success = board.PlaceBlock(block, pos);
        if (success) {
            // 손에서 블록 제거
            handBlocks.RemoveAt(idx);
            handBlocksData.RemoveAt(idx);
            
            // UI 업데이트 트리거
            GameUIManager.instance.OnBlockPlaced(block, pos);
            /*
            // Match 처리된 결과 가져오기 및 애니메이션 실행
            List<Match> matches = board.GetLastMatches();
            if (matches.Count > 0) {
                GameUIManager.instance.PlayMatchAnimation(matches);
            }
            */

            GameUIManager.instance.UpdateScore(blockGame.currentScore);
        }
        return success;
    }
    

    // ------------------------------
    // BLOCKGAME LAYER - end
    // ------------------------------

}
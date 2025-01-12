using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    const int STAGE_CHOICE_COUNT = 2;
    public int currentStageIndex = 1;

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
        deckManager.Initialize(ref currentRun, ref blockGame);
        shopManager.Initialize(ref currentRun, itemTemplates);

        StartStageSelection(StageType.NORMAL);
    }

    public void StartStageSelection(StageType stageType)
    {
        // stage Template에서 stagetype이 맞는 것을 랜덤하게 추출
        var templates = stageTemplates.Where(stage => stage.type == stageType).ToArray();
        for (int i = 0; i < nextStageChoices.Length; i++)
        {
            nextStageChoices[i] = templates[Random.Range(0, templates.Length)];
        }

        // UI에 전달
        GameUIManager.instance.OnStageSelection(nextStageChoices);
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
        // 스테이지 시작
        stageManager.StartStage(stage);
        
        blockGame = new BlockGameData();
        blockGame.Initialize(currentRun);
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
            else if (currentStageIndex < 2)
            {
                currentStageIndex++;
                StartStageSelection(StageType.NORMAL);
            }
            else if (currentStageIndex == 2)
            {
                currentStageIndex++;
                StartStageSelection(StageType.BOSS);
            }
        } 
        else 
        {
            EndGame(false);
        }
    }

    // ------------------------------
    // RUN LAYER - end
    // ------------------------------

    // ------------------------------
    // BLOCKGAME LAYER - start
    // ------------------------------

    public void StartBlockGame()
    {
        // 블록 게임 시작
        Debug.Log("Block Game Start");
    }

    // ------------------------------
    // BLOCKGAME LAYER - end
    // ------------------------------

}
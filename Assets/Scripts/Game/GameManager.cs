using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public GameData gameData;
    public RunData currentRun;
    public BlockGameData blockGame;

    public DeckManager deckManager;
    public ShopManager shopManager;
    public EffectManager effectManager;
    public StageManager stageManager;

    public StageData[] stageTemplates;
    public ItemData[] itemTemplates;

    public void StartNewGame()
    {
        // 각종 초기화
        Debug.Log("Game Start");
        gameData = new GameData();
        gameData.Initialize();

        StartNewRun();
    }

    public void StartNewRun()
    {
        // 초기화
        currentRun = new RunData();
        currentRun.Initialize(gameData);

        stageManager.Initialize(ref currentRun);
        effectManager.Initialize(ref currentRun);
        deckManager.Initialize(ref currentRun, ref blockGame);
        shopManager.Initialize(ref currentRun, itemTemplates);

        StartStage((StageType)0);
    }

    public void StartStage(StageType stageType)
    {
        // stage Template에서 stagetype이 맞는 것을 랜덤하게 추출
        var stages = stageTemplates.Where(stage => stage.type == stageType).ToArray();
        StageData stage = stages[Random.Range(0, stages.Length)];

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
            else
            {
                StartStage(stageManager.currentStage.type + 1);
            }
        } 
        else 
        {
            EndGame(false);
        }
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    //private GameManager gameManager;
    //private StageManager stageManager;
    //private DeckManager deckManager;

    [SerializeField] private GameObject stageInfoUIInstance;
    [SerializeField] private GameObject scoreInfoUIInstance;
    [SerializeField] private GameObject actionInfoUIInstance;
    [SerializeField] private GameObject rerollInfoUIInstance;
    [SerializeField] private GameObject goldInfoUIInstance;

    private StageInfoUI stageInfoUI;
    private ScoreInfoUI scoreInfoUI;
    private ActionInfoUI actionInfoUI;
    private RerollInfoUI rerollInfoUI;
    private GoldInfoUI goldInfoUI;

    void Start()
    {
        //initializeManagerInstances();
        initializeUIInstances();
        fetchUIData();
    }

    void initializeManagerInstances()
    {
        //gameManager = GameObject.Find("GameManager").getComponent<GameManager>();
        //stageManager = GameObject.Find("StageManager").getComponent<StageManager>();
        //deckManager = GameObject.Find("DeckManager").getComponent<DeckManager>();
    }

    void initializeUIInstances()
    {
        stageInfoUI = stageInfoUIInstance.GetComponent<StageInfoUI>();
        scoreInfoUI = scoreInfoUIInstance.GetComponent<ScoreInfoUI>();
        actionInfoUI = actionInfoUIInstance.GetComponent<ActionInfoUI>();
        rerollInfoUI = rerollInfoUIInstance.GetComponent<RerollInfoUI>();
        goldInfoUI = goldInfoUIInstance.GetComponent<GoldInfoUI>();
    }

    void fetchUIData()
    {
        // update stage info ui
        stageInfoUI.UpdateAnte(0);
        stageInfoUI.UpdateRound(0);
        stageInfoUI.UpdateDebuffText("asdf");
        stageInfoUI.UpdateScoreAtLeast(0); 

        // update socre info ui
        scoreInfoUI.UpdateScore(0);

        //update reroll info ui
        rerollInfoUI.UpdateLeftRerollCount(0);
        rerollInfoUI.UpdateMaximumRerollCount(0);

        //update gold info ui
        goldInfoUI.UpdateGainGold(0);
        goldInfoUI.UpdateCurrentGold(0);
    }
}

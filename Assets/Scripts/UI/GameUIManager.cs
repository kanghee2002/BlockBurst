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
    [SerializeField] private GameObject runInfoButtonUIInstance;
    [SerializeField] private GameObject runInfoUIInstance;
    [SerializeField] private GameObject optionButtonUIInstance;
    [SerializeField] private GameObject optionUIInstance;
    [SerializeField] private GameObject boardUIInstance;
    [SerializeField] private GameObject rerollButtonUIInstance;
    [SerializeField] private GameObject handUIInstance;
    [SerializeField] private GameObject deckButtonUIInstance;
    [SerializeField] private GameObject deckInfoUIInstance;
    [SerializeField] private GameObject itemSetUIInstance;

    private StageInfoUI stageInfoUI;
    private ScoreInfoUI scoreInfoUI;
    private ActionInfoUI actionInfoUI;
    private RerollInfoUI rerollInfoUI;
    private GoldInfoUI goldInfoUI;
    private RunInfoButtonUI runInfoButtonUI;
    private RunInfoUI runInfoUI;
    private OptionButtonUI optionButtonUI;
    private OptionUI optionUI;
    private BoardUI boardUI;
    private RerollButtonUI rerollButtonUI;
    private HandUI handUI;
    private DeckButtonUI deckButtonUI;
    private DeckInfoUI deckInfoUI;
    private ItemSetUI itemSetUI;

    private enum UIstate
    {
        playing,
        runInfo,
        option,
        deckInfo
    }
    private UIstate state;

    private void Start()
    {
        state = UIstate.playing;
        initializeManagerInstances();
        initializeUIInstances();
        fetchUIData();
    }

    private void initializeManagerInstances()
    {
        //gameManager = GameObject.Find("GameManager").getComponent<GameManager>();
        //stageManager = GameObject.Find("StageManager").getComponent<StageManager>();
        //deckManager = GameObject.Find("DeckManager").getComponent<DeckManager>();
    }

    private void initializeUIInstances()
    {
        stageInfoUI = stageInfoUIInstance.GetComponent<StageInfoUI>();
        scoreInfoUI = scoreInfoUIInstance.GetComponent<ScoreInfoUI>();
        actionInfoUI = actionInfoUIInstance.GetComponent<ActionInfoUI>();
        rerollInfoUI = rerollInfoUIInstance.GetComponent<RerollInfoUI>();
        goldInfoUI = goldInfoUIInstance.GetComponent<GoldInfoUI>();
        runInfoButtonUI = runInfoButtonUIInstance.GetComponent<RunInfoButtonUI>();
        runInfoUI = runInfoUIInstance.GetComponent<RunInfoUI>();
        optionButtonUI = optionButtonUIInstance.GetComponent<OptionButtonUI>();
        optionUI = optionUIInstance.GetComponent<OptionUI>();
        boardUI = boardUIInstance.GetComponent<BoardUI>();
        rerollButtonUI = rerollButtonUIInstance.GetComponent<RerollButtonUI>();
        handUI = handUIInstance.GetComponent<HandUI>();
        deckButtonUI = deckButtonUIInstance.GetComponent<DeckButtonUI>();
        deckInfoUI = deckInfoUIInstance.GetComponent<DeckInfoUI>();
        itemSetUI = itemSetUIInstance.GetComponent<ItemSetUI>();


    }

    private void fetchUIData()
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

    // RunInfo methods
    public void runInfoButtonUIPressed()
    {
        if (state == UIstate.playing)
        {
            state = UIstate.runInfo;
            runInfoUI.openRunInfoUI();
        }
    }

    public void runInfoBackButtonUIPressed()
    {
        if (state == UIstate.runInfo)
        {
            state = UIstate.playing;
            runInfoUI.closeRunInfoUI();
        }
    }

    // Option methods
    public void optionButtonUIPressed()
    {
        if (state == UIstate.playing)
        {
            state = UIstate.option;
            optionUI.openOptionUI();
        }
    }

    public void optionBackButtonUIPressed()
    {
        if (state == UIstate.option)
        {
            state = UIstate.playing;
            optionUI.closeOptionUI();
        }
    }

    // DeckInfo methods
    public void deckButtonUIPressed()
    {
        if (state == UIstate.playing)
        {
            state = UIstate.deckInfo;
            deckInfoUI.openDeckInfoUI();
        }
    }

    public void deckInfoBackButtonUIPressed()
    {
        if (state == UIstate.option)
        {
            state = UIstate.deckInfo;
            deckInfoUI.closeDeckInfoUI();
        }
    }
}

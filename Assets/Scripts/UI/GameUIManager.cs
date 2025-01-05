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

    private enum SceneState
    {
        selecting,
        playing,
        buying
    }

    private enum PopupState
    {
        none,
        runInfo,
        option,
        deckInfo
    }
    private PopupState popupState;

    private void Start()
    {
        popupState = PopupState.none;
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
    public void RunInfoButtonUIPressed()
    {
        if (popupState == PopupState.none)
        {
            popupState = PopupState.runInfo;
            runInfoUI.OpenRunInfoUI();
        }
    }

    public void RunInfoBackButtonUIPressed()
    {
        if (popupState == PopupState.runInfo)
        {
            popupState = PopupState.none;
            runInfoUI.CloseRunInfoUI();
        }
    }

    // Option methods
    public void OptionButtonUIPressed()
    {
        if (popupState == PopupState.none)
        {
            popupState = PopupState.option;
            optionUI.OpenOptionUI();
        }
    }

    public void OptionBackButtonUIPressed()
    {
        if (popupState == PopupState.option)
        {
            popupState = PopupState.none;
            optionUI.CloseOptionUI();
        }
    }

    // DeckInfo methods
    public void DeckButtonUIPressed()
    {
        if (popupState == PopupState.none)
        {
            popupState = PopupState.deckInfo;
            deckInfoUI.OpenDeckInfoUI();
        }
    }

    public void DeckInfoBackButtonUIPressed()
    {
        if (popupState == PopupState.option)
        {
            popupState = PopupState.deckInfo;
            deckInfoUI.CloseDeckInfoUI();
        }
    }
}

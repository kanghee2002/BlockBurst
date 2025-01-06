using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    //private GameManager gameManager;
    //private StageManager stageManager;
    //private DeckManager deckManager;

    [SerializeField] private StageInfoUI stageInfoUI;
    [SerializeField] private ScoreInfoUI scoreInfoUI;
    [SerializeField] private ActionInfoUI actionInfoUI;
    [SerializeField] private RerollInfoUI rerollInfoUI;
    [SerializeField] private GoldInfoUI goldInfoUI;
    [SerializeField] private RunInfoButtonUI runInfoButtonUI;
    [SerializeField] private RunInfoUI runInfoUI;
    [SerializeField] private OptionButtonUI optionButtonUI;
    [SerializeField] private OptionUI optionUI;
    [SerializeField] private BoardUI boardUI;
    [SerializeField] private RerollButtonUI rerollButtonUI;
    [SerializeField] private HandUI handUI;
    [SerializeField] private DeckButtonUI deckButtonUI;
    [SerializeField] private DeckInfoUI deckInfoUI;
    [SerializeField] private ItemSetUI itemSetUI;

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
        fetchUIData();
    }

    private void initializeManagerInstances()
    {
        //gameManager = GameObject.Find("GameManager").getComponent<GameManager>();
        //stageManager = GameObject.Find("StageManager").getComponent<StageManager>();
        //deckManager = GameObject.Find("DeckManager").getComponent<DeckManager>();
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

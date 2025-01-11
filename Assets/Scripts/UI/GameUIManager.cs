using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    //private GameManager gameManager;

    [SerializeField] private StageInfoUI stageInfoUI;
    [SerializeField] private ScoreInfoUI scoreInfoUI;
    [SerializeField] private ActionInfoUI actionInfoUI;
    [SerializeField] private GoldInfoUI goldInfoUI;
    [SerializeField] private RunInfoButtonUI runInfoButtonUI;
    [SerializeField] private RunInfoUI runInfoUI;
    [SerializeField] private OptionButtonUI optionButtonUI;
    [SerializeField] private OptionUI optionUI;
    [SerializeField] private BoardUI boardUI;
    [SerializeField] private RerollButtonUI rerollButtonUI;
    [SerializeField] private RerollInfoUI rerollInfoUI;
    [SerializeField] private HandUI handUI;
    [SerializeField] private DeckButtonUI deckButtonUI;
    [SerializeField] private DeckInfoUI deckInfoUI;
    [SerializeField] private ItemSetUI itemSetUI;
    [SerializeField] private ShopSignboardUI shopSignboardUI;
    [SerializeField] private ItemBoardUI itemBoardUI;
    [SerializeField] private ItemShowcaseUI itemShowcaseUI;
    [SerializeField] private StageSelectionSignboardUI stageSelectionSignboardUI;
    [SerializeField] private StageSelectionBoardUI stageSelectionBoardUI;

    public enum SceneState
    {
        selecting,
        playing,
        shopping
    }
    private SceneState sceneState;

    private enum PopupState
    {
        none,
        runInfo,
        option,
        deckInfo
    }
    private PopupState popupState;

    private void Awake()
    {
        popupState = PopupState.none;
        initializeManagerInstances();
    }

    private void initializeManagerInstances()
    {
        //gameManager = GameObject.Find("GameManager").getComponent<GameManager>();
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

    // Reroll Button methods
    public void RerollButtonUIPressed()
    {
        if (sceneState == SceneState.playing && popupState == PopupState.none)
        {
            Debug.Log("���� ��ư ����");
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
        if (popupState == PopupState.deckInfo)
        {
            popupState = PopupState.none;
            deckInfoUI.CloseDeckInfoUI();
        }
    }

    // Item Board methods
    public void NextStageButtonUIPressed()
    {
        if (sceneState == SceneState.shopping && popupState == PopupState.none)
        {
            Debug.Log("���� ���������� ��ư ����");
        }
    }

    public void ItemRerollButtonUIPressed()
    {
        if (sceneState == SceneState.shopping && popupState == PopupState.none)
        {
            Debug.Log("������ ���� ��ư ����");
        }
    }

    // StageSelectionUi methods
    public void NextStageChoiceButtonUIPressed(int choiceIndex)
    {
        if (sceneState == SceneState.selecting && popupState == PopupState.none)
        {
            Debug.Log("���� �������� ������ ��ư ������."
                + "\n������ζ�� ���⿡�� ������ ���� �ְ� �޾ƾ߰���?"
                + "\n�� �׷� �� �𸣰ڰ� �ϴ� selecting -> playing���� sceneState �ٲ�.");
            ChangeSceneState(SceneState.playing);

        }
    }

    public void ChangeSceneState(SceneState stateToSet)
    {
        sceneState = stateToSet;
        if (sceneState == SceneState.playing)
        {
            sceneState = SceneState.playing;
            stageSelectionSignboardUI.CloseStageSelectionSignboardUI();
            stageSelectionBoardUI.CloseNextStageChoiceUI();
            stageInfoUI.OpenStageInfoUI();
            scoreInfoUI.OpenScoreInfoUI();
            boardUI.OpenBoardUI();
            rerollButtonUI.OpenRerollButtonUI();
            handUI.OpenHandUI();
        }
    }
}

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
            GetRunInfo();
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
            GetOption();
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
            OnRerolled();
        }
    }

    // DeckInfo methods
    public void DeckButtonUIPressed()
    {
        if (popupState == PopupState.none)
        {
            popupState = PopupState.deckInfo;
            deckInfoUI.OpenDeckInfoUI();
            GetDeckInfo();
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
            OnRerolled();
        }
    }

    // StageSelectionUI methods
    public void NextStageChoiceButtonUIPressed(int choiceIndex)
    {
        if (sceneState == SceneState.selecting && popupState == PopupState.none)
        {
            Debug.Log("���� �������� ������ ��ư ������. "
                + "������ζ�� ���⿡�� ������ ���� �ְ� �޾ƾ߰���? "
                + "�ϴ� selecting -> playing���� sceneState �ٲ�.");
            ChangeSceneState(SceneState.playing);

        }
    }

    public void ChangeSceneState(SceneState stateToSet)
    {
        sceneState = stateToSet;
        if (sceneState == SceneState.playing)
        {
            StartCoroutine(CoroutineSelectingToPlaying());
        }
        else if (sceneState == SceneState.shopping)
        {
            StartCoroutine(CoroutinePlayingToShopping());
        }
        else if (sceneState == SceneState.selecting)
        {
            StartCoroutine(CoroutineShoppingToSelecting());
        }
    }

    IEnumerator CoroutineSelectingToPlaying()
    {
        stageSelectionSignboardUI.CloseStageSelectionSignboardUI();
        stageSelectionBoardUI.CloseStageSelectionBoardUI();

        yield return new WaitForSeconds(0.5f);

        stageInfoUI.OpenStageInfoUI();
        scoreInfoUI.OpenScoreInfoUI();
        boardUI.OpenBoardUI();
        rerollButtonUI.OpenRerollButtonUI();
        handUI.OpenHandUI();
        
        yield return null;
    }
    IEnumerator CoroutinePlayingToShopping()
    {
        stageInfoUI.CloseStageInfoUI();
        scoreInfoUI.CloseScoreInfoUI();
        boardUI.CloseBoardUI();
        rerollButtonUI.CloseRerollButtonUI();
        handUI.CloseHandUI();

        yield return new WaitForSeconds(0.5f);

        shopSignboardUI.OpenShopSignboardUI();
        itemBoardUI.OpenItemBoardUI();
        
        yield return null;
    }
    IEnumerator CoroutineShoppingToSelecting()
    {
        shopSignboardUI.CloseShopSignboardUI();
        itemBoardUI.CloseItemBoardUI();

        yield return new WaitForSeconds(0.5f);

        stageSelectionSignboardUI.OpenStageSelectionSignboardUI();
        stageSelectionBoardUI.OpenStageSelectionBoardUI();

        yield return null;
    }

    // ���� model �κа��� �ǻ������ ���� â��... �� ���� �ִ�.
    // On: GameUIManager -(�̺�Ʈ)-> GameManager
    // Notify: GameManager -(�̺�Ʈ)-> GameUIManager
    // Get: GameUIManager�� GameManager�κ��� ������ ������
    public void NotifyChapterIsUpdated(int updatedChapter)
    {
        stageInfoUI.UpdateChapter(updatedChapter);
    }

    public void NotifyStageIsUpdated(int updatedStage)
    {
       
        stageInfoUI.UpdateStage(updatedStage);
    }
    public void NotifyDebuffTextIsUpdated(string updatedDebuffText)
    {
        stageInfoUI.UpdateDebuffText(updatedDebuffText);
    }

    public void NotifyScoreAtLeastIsUpdated(int updatedScoreAtLeast)
    {
        stageInfoUI.UpdateScoreAtLeast(updatedScoreAtLeast);
    }

    public void NotifyScoreIsUpdated(int updatedScore)
    {
        scoreInfoUI.UpdateScore(updatedScore);
    }

    public void NotifyGoldIsUpdated(int updatedGold)
    {
        goldInfoUI.UpdateGold(updatedGold);
    }
    public void GetRunInfo()
    {
        Debug.Log("���ӸŴ����� �÷��̾ �� ������ ������! �� ���� ��������!");
    }

    public void GetOption()
    {
        Debug.Log("���ӸŴ����� �÷��̾ �ɼ��� ������! ���� �����Ǿ��ִ� �ɼ� ������ ��������!");
    } // �ɼǿ� �� �׸� ����, �÷��̾ � �ɼ��� ������ �� ���� �װͿ� �ش��ϴ� �� ���� Set�� �� �ְ� �޼��� �ʿ���.

    public void OnRerolled()
    {
        Debug.Log("���ӸŴ����� ���ѹ�ư�� ���Ⱦ�!!!");
    }
    public void NotifyRerolled()
    {
        // ������ ����Ǿ��� �� ȣ��Ǵ� �޼���
        // �з����ͷ� ��ο� �� ��ϵ鿡 ���� ������ �޾ƿͼ�
        // ����� �ڵ忡 ������ ���� �޼�����...
        // �� ���� ���� ���� �׳� �� �Լ��� ���Ѱ�...
    }

    public void GetDeckInfo()
    {
        Debug.Log("���ӸŴ����� �÷��̾ ���� ������! �� ���� ��������!");
    }
}

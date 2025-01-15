using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;
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

    public void Initialize(RunData runData)
    {
        goldInfoUI.Initialize(runData.gold);
        actionInfoUI.Initialize(0, 0);
        runInfoUI.Initialize(runData);
        deckInfoUI.Initialize(runData);
        stageSelectionBoardUI.OpenStageSelectionBoardUI(); // 나중에 옮겨
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
            Debug.Log("다음 스테이지로 버튼 눌림");
            GameManager.instance.StartStageSelection();
            ChangeSceneState(SceneState.selecting);
        }
    }

    public void ItemRerollButtonUIPressed()
    {
        if (sceneState == SceneState.shopping && popupState == PopupState.none)
        {
            Debug.Log("아이템 리롤 버튼 눌림");
            GameManager.instance.OnShopReroll();
        }
    }

    // StageSelectionUI methods
    public void NextStageChoiceButtonUIPressed(int choiceIndex)
    {
        if (sceneState == SceneState.selecting && popupState == PopupState.none)
        {
            GameManager.instance.OnStageSelection(choiceIndex);   
            /*
            Debug.Log("다음 스테이지 선택지 버튼 눌렸음. "
                + "원래대로라면 여기에서 정보를 뭔가 주고 받아야겠지? "
                + "일단 selecting -> playing으로 sceneState 바꿈.");
                */
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

    public void GetRunInfo()
    {
        Debug.Log("게임매니저야 플레이어가 런 정보를 열었어! 런 정보 가져갈게!");
    }

    public void GetOption()
    {
        Debug.Log("게임매니저야 플레이어가 옵션을 열었어! 현재 설정되어있는 옵션 정보들 가져갈게!");
    } // 옵션에 들어갈 항목에 따라, 플레이어가 어떤 옵션을 조작할 때 마다 그것에 해당하는 것 마다 Set할 수 있게 메서드 필요함.

    public void OnRerolled()
    {
        // TEST
        GameManager.instance.EndStage(true);
        /*
        Debug.Log("게임매니저야 리롤버튼이 눌렸어!!!");
        GameManager.instance.OnRerolled();
        */
    }

    public void OnEndStage()
    {
        
    }

    public void NotifyRerolled()
    {
        // 리롤이 시행되었을 때 호출되는 메서드
        // 패러미터로 드로우 된 블록들에 대한 정보를 받아와서
        // 블록을 핸드에 새팅해 놓는 메서드임...
        // 난 뭐가 뭔지 몰라서 그냥 빈 함수로 냅둘게...
    }

    public void GetDeckInfo()
    {
        Debug.Log("게임매니저야 플레이어가 덱을 열었어! 덱 정보 가져갈게!");
    }
    
    public void OnStageSelection(StageData[] nextStageChoices, int currentChapterIndex, int currentStageIndex)
    {
        stageSelectionSignboardUI.Initialize(currentChapterIndex, currentStageIndex);
        // stageData들을 받아와서 UI에 뿌려주는 메서드
        if (nextStageChoices.Length == 2)
        {
            stageSelectionBoardUI.InitializeNextStageChoiceUI(nextStageChoices);
        }
        else
        {
            // 추후 지원 예정
            /*
            stageSelectionSignboardUI.InitializeNextStageChoiceUI(nextStageChoices);
            stageSelectionSignboardUI.OpenStageSelectionSignboardUI();
            */
        }
    }

    public void OnStageStart(int chapterIndex, int stageIndex, StageData stageData)
    {        
        boardUI.gameObject.SetActive(true);
        stageInfoUI.Initialize(chapterIndex, stageIndex, stageData);
        scoreInfoUI.UpdateScore(0);
        ChangeSceneState(SceneState.playing);
    }

    public void OnBlocksDrawn(List<Block> blocks)
    {

        handUI.Initialize(blocks);
    }

    public bool TryPlaceBlock(int idx, Vector2Int pos, GameObject blockObj)
    {
        return GameManager.instance.TryPlaceBlock(idx, pos, blockObj);
    }

    public void OnShopStart(List<ItemData> items)
    {
        ChangeSceneState(SceneState.shopping);
        itemShowcaseUI.Initialize(items);
    }

    public void OnItemShowcaseItemButtonPressed(int index)
    {
        int gold = GameManager.instance.OnItemPurchased(index);
        if (gold != -1)
        {
            goldInfoUI.UpdateGold(gold);
            itemShowcaseUI.PurchaseItem(index);
        }
    }
    public void OnBlockPlaced(GameObject blockObj, Block block, Vector2Int pos) {
        boardUI.OnBlockPlaced(blockObj, block, pos);
    }

    public void PlayMatchAnimation(List<Match> matches) {
        boardUI.ProcessMatchAnimation(matches);
    }

    public void UpdateScore(int score) {
        scoreInfoUI.UpdateScore(score);
    }

    public void UpdateGold(int gold) {
        goldInfoUI.UpdateGold(gold);
    }

    public void BackToMain()
    {
        GameManager.instance.BackToMain();
    }
}

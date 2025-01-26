using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;

    [SerializeField] private StageInfoUI stageInfoUI;
    [SerializeField] private ScoreInfoUI scoreInfoUI;
    [SerializeField] private ActionInfoUI actionInfoUI;
    [SerializeField] private GoldInfoUI goldInfoUI;
    [SerializeField] private RerollButtonUI rerollButtonUI;
    [SerializeField] private HandUI handUI;
    [SerializeField] private DeckButtonUI deckButtonUI;
    [SerializeField] private ItemSetUI itemSetUI;
    [SerializeField] private ShopSignboardUI shopSignboardUI;
    [SerializeField] private ItemBoardUI itemBoardUI;
    [SerializeField] private StageSelectionSignboardUI stageSelectionSignboardUI;
    [SerializeField] private StageSelectionBoardUI stageSelectionBoardUI;
    
    [SerializeField] private RunInfoUI runInfoUI;
    [SerializeField] private OptionUI optionUI;
    [SerializeField] private DeckInfoUI deckInfoUI;
    [SerializeField] private BoardUI boardUI;

    public enum SceneState
    {
        none,
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
        sceneState = SceneState.none;
        popupState = PopupState.none;
        
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
        actionInfoUI.Initialize(0, 0, 0);
        runInfoUI.Initialize(runData);
        deckInfoUI.Initialize(runData);
        sceneState = SceneState.selecting;
        OpenSceneState(sceneState);
    }

    // BUTTON METHODS

    // RunInfo methods
    public void OnRunInfoButtonUIPressed()
    {
        if (popupState == PopupState.none)
        {
            popupState = PopupState.runInfo;
            runInfoUI.OpenRunInfoUI();
        }
    }

    public void OnRunInfoBackButtonUIPressed()
    {
        if (popupState == PopupState.runInfo)
        {
            popupState = PopupState.none;
            runInfoUI.CloseRunInfoUI();
        }
    }

    // Option methods
    public void OnOptionButtonUIPressed()
    {
        if (popupState == PopupState.none)
        {
            popupState = PopupState.option;
            optionUI.OpenOptionUI();
        }
    }

    public void OnOptionBackButtonUIPressed()
    {
        if (popupState == PopupState.option)
        {
            popupState = PopupState.none;
            optionUI.CloseOptionUI();
        }
    }

    // Reroll Button methods
    public void OnRerollButtonUIPressed()
    {
        if (sceneState == SceneState.playing && popupState == PopupState.none)
        {
            GameManager.instance.OnRerolled();
        }
    }

    // DeckInfo methods
    public void OnDeckButtonUIPressed()
    {
        if (popupState == PopupState.none)
        {
            popupState = PopupState.deckInfo;
            deckInfoUI.OpenDeckInfoUI();
            deckInfoUI.Initialize(GameManager.instance.GetDeckInfo());
        }
    }

    public void OnDeckInfoBackButtonUIPressed()
    {
        if (popupState == PopupState.deckInfo)
        {
            popupState = PopupState.none;
            deckInfoUI.CloseDeckInfoUI();
        }
    }

    // Item Board methods
    public void OnNextStageButtonUIPressed()
    {
        if (sceneState == SceneState.shopping && popupState == PopupState.none)
        {
            //Debug.Log("다음 스테이지로 버튼 눌림");
            GameManager.instance.StartStageSelection();
            ChangeSceneState(SceneState.selecting);
        }
    }
    
    public void OnItemShowcaseItemButtonPressed(int index)
    {
        int gold = GameManager.instance.OnItemPurchased(index);
        if (gold != -1)
        {
            goldInfoUI.UpdateGold(gold);
            itemBoardUI.PurchaseItem(index);
        }
    }

    public void OnItemRerollButtonUIPressed()
    {
        if (sceneState == SceneState.shopping && popupState == PopupState.none)
        {
            //Debug.Log("아이템 리롤 버튼 눌림");
            GameManager.instance.OnShopReroll();
        }
    }

    // StageSelectionUI methods
    public void OnNextStageChoiceButtonUIPressed(int choiceIndex)
    {
        if (sceneState == SceneState.selecting && popupState == PopupState.none)
        {
            GameManager.instance.OnStageSelection(choiceIndex);
        }
    }

    public void OnStageSelection(StageData[] nextStageChoices, int currentChapterIndex, int currentStageIndex)
    {
        stageSelectionSignboardUI.Initialize(currentChapterIndex, currentStageIndex);
        // stageData들을 받아와서 UI에 뿌려주는 메서드
        if (nextStageChoices.Length == 2)
        {
            stageSelectionBoardUI.Initialize(nextStageChoices);
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

    public void ChangeSceneState(SceneState stateToSet)
    {
        SceneState prevState = sceneState;
        sceneState = stateToSet;

        StartCoroutine(ChangeSceneStateCoroutine(prevState, sceneState));
    }

    private IEnumerator ChangeSceneStateCoroutine(SceneState prevState, SceneState stateToSet)
    {
        CloseSceneState(prevState);
        yield return new WaitForSeconds(0.5f);
        OpenSceneState(stateToSet);
    }

    public void CloseSceneState(SceneState stateToClose)
    {
        switch (stateToClose)
        {
            case SceneState.selecting:
                stageSelectionSignboardUI.CloseStageSelectionSignboardUI();
                stageSelectionBoardUI.CloseStageSelectionBoardUI();
                break;
            case SceneState.playing:
                stageInfoUI.CloseStageInfoUI();
                scoreInfoUI.CloseScoreInfoUI();
                boardUI.CloseBoardUI();
                rerollButtonUI.CloseRerollButtonUI();
                handUI.CloseHandUI();
                break;
            case SceneState.shopping:
                shopSignboardUI.CloseShopSignboardUI();
                itemBoardUI.CloseItemBoardUI();
                break;
            default:
                break;
        }
    }
    
    public void OpenSceneState(SceneState stateToOpen)
    {
        switch (stateToOpen)
        {
            case SceneState.selecting:
                stageSelectionSignboardUI.OpenStageSelectionSignboardUI();
                stageSelectionBoardUI.OpenStageSelectionBoardUI();
                break;
            case SceneState.playing:
                stageInfoUI.OpenStageInfoUI();
                scoreInfoUI.OpenScoreInfoUI();
                boardUI.OpenBoardUI();
                rerollButtonUI.OpenRerollButtonUI();
                handUI.OpenHandUI();
                break;
            case SceneState.shopping:
                shopSignboardUI.OpenShopSignboardUI();
                itemBoardUI.OpenItemBoardUI();
                break;
            default:
                break;
        }
    }

    public void OnStageStart(int chapterIndex, int stageIndex, StageData stageData, BlockGameData blockGame)
    {        
        boardUI.gameObject.SetActive(true);
        stageInfoUI.Initialize(chapterIndex, stageIndex, stageData);
        scoreInfoUI.Initialize(0);
        rerollButtonUI.Initialize(blockGame.rerollCount);

        boardUI.Initialize(blockGame.boardRows, blockGame.boardColumns);
        
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

    public void OnShopStart(List<ItemData> items, bool isFirst = false)
    {
        if (isFirst)
        {
            ChangeSceneState(SceneState.shopping);
        }
        itemBoardUI.Initialize(items);
    }

    public void OnBlockPlaced(GameObject blockObj, Block block, Vector2Int pos) {
        boardUI.OnBlockPlaced(blockObj, block, pos);
    }

    public void PlayMatchAnimation(List<Match> matches, Dictionary<Match, List<int>> scores, float delay) {
        boardUI.ProcessMatchAnimation(matches, scores, delay);
        actionInfoUI.ProcessScoreUpdateAnimation(scores, delay);
    }

    public void UpdateChip(int chip)
    {
        actionInfoUI.UpdateChip(chip);
    }

    public void UpdateMultiplierByAdd(int addingValue) {
        actionInfoUI.AddMultiplier(addingValue);
    }

    public void UpdateMultiplier(int multiplier)
    {
        actionInfoUI.UpdateMuliplier(multiplier);
    }

    public void UpdateProduct(int product)
    {
        actionInfoUI.UpdateProduct(product);
    }

    public void UpdateScore(int score) {
        scoreInfoUI.UpdateScore(score);
    }

    public void UpdateGold(int gold) {
        goldInfoUI.UpdateGold(gold);
    }

    public void DisplayRerollCount(int rerollCount) {
        rerollButtonUI.DisplayRerollCount(rerollCount);
    }

    public void DisplayDeckCount(int deckCount, int maxDeckCount) {
        deckButtonUI.DisplayDeckCount(deckCount, maxDeckCount);
    }

    public void BackToMain()
    {
        GameManager.instance.BackToMain();
    }

    public void DisplayItemSet(List<ItemData> items)
    {
        itemSetUI.Initialize(items);
    }

    public void PlayItemEffectAnimation(string effectDescription, int index, float delay)
    {
        itemSetUI.PlayEffectAnimation(effectDescription, index, delay);
    }

    public void BlockCells(HashSet<Vector2Int> cells)
    {
        boardUI.BlockCells(cells);
    }
}

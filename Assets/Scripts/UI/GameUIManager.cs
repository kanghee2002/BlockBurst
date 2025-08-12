using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;

    [SerializeField] private Background background;

    [SerializeField] private DeckSelectionBoardUI deckSelectionBoardUI;
    [SerializeField] private PlayerDataBoardUI playerDataBoardUI;
    [SerializeField] private StatisticsContainerUI statisticsContainerUI;
    [SerializeField] private UnlockContainerUI unlockInfoContainerUI;
    [SerializeField] private BasicUI gameInfoUI;
    [SerializeField] private BasicUI lobbyUI;
    [SerializeField] private ButtonUI optionButtonUI;
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
    [SerializeField] private UnlockNotificationUI unlockNotificationUI;
    [SerializeField] private Toggle tutorialToggle;

    [SerializeField] private RunInfoUI runInfoUI;
    [SerializeField] private OptionUI optionUI;
    [SerializeField] private DeckInfoUI deckInfoUI;
    [SerializeField] private ItemDetailUI itemDetailUI;
    [SerializeField] private ClearInfoUI clearInfoUI;
    [SerializeField] private BoardUI boardUI;

    [Header("UI Colors")]
    [SerializeField] private List<Color> selectingBackgroundColors;
    [SerializeField] private List<Color> selectingBossBackgroundColors;
    [SerializeField] private List<Color> playingBackgroundColors;
    [SerializeField] private List<Color> playingBossBackgroundColors;
    [SerializeField] private List<Color> shoppingBackgroundColors;
    [SerializeField] private Color currentUIColor;


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
        deckInfo,
        itemDetail,
        deckSelection,
        playerData,
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
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(RunData runData)
    {
        deckSelectionBoardUI.CloseDeckSelectionBoardUI();

        gameInfoUI.OpenUI();
        itemSetUI.OpenItemSetUI();
        lobbyUI.CloseUI();
        DisplayItemSet(runData.activeItems, runData.maxItemCount);

        sceneState = SceneState.selecting;
        OpenSceneState(sceneState);

        goldInfoUI.Initialize(runData.gold);
        actionInfoUI.Initialize(_chip: 0, _multiplier: runData.baseMatchMultipliers[MatchType.ROW], _product: 0);
        actionInfoUI.SetChipLayoutColor(currentUIColor);
    }

    // BUTTON METHODS

    // RunInfo methods
    public void OnRunInfoButtonUIPressed()
    {
        if (popupState == PopupState.none)
        {
            popupState = PopupState.runInfo;
            GameManager.instance.OnRunInfoRequested();
            runInfoUI.OpenRunInfoUI();

            GameManager.instance.ProcessTutorialStep("Run");
        }
    }

    public void OnRunInfoCallback(RunData runData, float startTime, BlockType? mostPlacedBlockType)
    {
        runInfoUI.Initialize(runData, startTime, mostPlacedBlockType);
    }

    public void OnRunInfoBackButtonUIPressed()
    {
        if (popupState == PopupState.runInfo)
        {
            popupState = PopupState.none;
            runInfoUI.CloseRunInfoUI();

            GameManager.instance.ProcessTutorialStep("RunBack");
        }
    }

    // Option methods
    public void OnOptionButtonUIPressed()
    {
        if (popupState == PopupState.none)
        {
            popupState = PopupState.option;
            optionUI.SetLayoutsColor(currentUIColor);
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

            GameManager.instance.ProcessTutorialStep("Reroll");
        }
    }

    // DeckInfo methods
    public void OnDeckButtonUIPressed()
    {
        if (popupState == PopupState.none)
        {
            popupState = PopupState.deckInfo;
            GameManager.instance.OnDeckInfoRequested();
            deckInfoUI.SetLayoutsColor(currentUIColor);
            deckInfoUI.OpenDeckInfoUI();

            GameManager.instance.ProcessTutorialStep("Deck");
        }
    }

    public void OnDeckInfoCallback(RunData runData, BlockGameData blockGameData)
    {
        deckInfoUI.Initialize(runData, blockGameData, sceneState == SceneState.playing);
    }

    public void OnDeckInfoBoostUIPressed(int index)
    {
        // 팝업 블러로 닫기 -> DeckInfo.isShowingBoostDetail로 관리

        if (popupState == PopupState.deckInfo)
        {
            GameManager.instance.OnBoostInfoRequested(index);
            itemDetailUI.OpenItemDetailUI();
        }
    }

    public void OnBoostInfoCallback(ItemData boostData, int index)
    {
        itemDetailUI.Initialize(boostData, index, isBoost: true, isPurchase: false);
    }

    public void OnDeckInfoBackButtonUIPressed()
    {
        if (popupState == PopupState.deckInfo)
        {
            popupState = PopupState.none;
            deckInfoUI.CloseDeckInfoUI();

            GameManager.instance.ProcessTutorialStep("DeckBack");
        }
    }

    public void OnDeckSelectionButtonUIPressed()
    {
        if (popupState == PopupState.none)
        {
            popupState = PopupState.deckSelection;
            GameManager.instance.OnDeckLevelInfoRequested();
            DataManager.instance.OnDeckSelectionPlayerDataRequested();
            UnlockManager.instance.OnDeckUnlockInfoRequested();
            deckSelectionBoardUI.Initialize();
            deckSelectionBoardUI.OpenDeckSelectionBoardUI();
        }
    }

    public void OnStatisticsButtonUIPressed()
    {
        if (popupState == PopupState.none)
        {
            popupState = PopupState.playerData;
            playerDataBoardUI.OpenPlayerDataBoardUI();

            DataManager.instance.OnStatisticsPlayerDataRequested();
            UnlockManager.instance.OnUnlockInfoRequested();
        }
    }

    public void OnDeckUnlockInfoRequested(UnlockInfo[] deckUnlockInfoTemplates)
    {
        deckSelectionBoardUI.InitializeDeckUnlockInfo(deckUnlockInfoTemplates);
    }

    public void OnUnlockInfoCallback(UnlockInfo[] unlockInfoTemplates, List<string> unlockedItems)
    {
        unlockInfoContainerUI.Initialize(unlockInfoTemplates, unlockedItems);
    }

    public void OnStatisticsPlayerDataCallback(PlayerData playerData)
    {
        statisticsContainerUI.Initialize(playerData);
    }

    public void OnDeckSelectionPlayerDataCallback(PlayerData playerData)
    {
        deckSelectionBoardUI.InitializePlayerData(playerData);
    } 

    public void OnPlayButtonUIPressed(DeckType deckType, int level)
    {
        if (popupState == PopupState.deckSelection)
        {
            popupState = PopupState.none;
            GameManager.instance.StartNewGame(deckType, level);
        }
    }

    public void OnDeckLevelInfoCallback(DeckData[] deckTemplates, LevelData[] levelTemplates)
    {
        deckSelectionBoardUI.InitializeDeckLevelInfo(deckTemplates, levelTemplates);
    }

    // Popup Blur Method
    public void OnPopupBlurUIPressed()
    {
        // RunInfo
        if (popupState == PopupState.runInfo)
        {
            popupState = PopupState.none;
            runInfoUI.CloseRunInfoUI();

            GameManager.instance.ProcessTutorialStep("RunBack");
        }

        // Option
        else if (popupState == PopupState.option)
        {
            popupState = PopupState.none;
            optionUI.CloseOptionUI();
        }

        // DeckInfo
        else if (popupState == PopupState.deckInfo)
        {
            if (deckInfoUI.isShowingBoostDetail)
            {
                deckInfoUI.isShowingBoostDetail = false;

                itemDetailUI.CloseItemDetailUI(false);
            }
            else
            {
                popupState = PopupState.none;
                deckInfoUI.CloseDeckInfoUI();

                GameManager.instance.ProcessTutorialStep("DeckBack");
            }
        }

        // ItemDetail
        else if (popupState == PopupState.itemDetail)
        {
            popupState = PopupState.none;

            itemDetailUI.CloseItemDetailUI(true);
        }

        else if (popupState == PopupState.deckSelection)
        {
            popupState = PopupState.none;

            deckSelectionBoardUI.CloseDeckSelectionBoardUI();
        }

        else if (popupState == PopupState.playerData)
        {
            popupState = PopupState.none;

            playerDataBoardUI.ClosePlayerDataBoardUI();
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
    
    public void OnItemShowcaseItemButtonUIPressed(int index)
    {
        if (popupState == PopupState.none)
        {
            popupState = PopupState.itemDetail;

            GameManager.instance.OnShopItemInfoRequested(index);
            itemDetailUI.OpenItemDetailUI();

            GameManager.instance.ProcessTutorialStep("ItemClicked");
        }
    }

    public void OnShopItemInfoCallback(ItemData itemData, int index)
    {
        itemDetailUI.Initialize(itemData, index, isBoost: false, isPurchase: true);
    }

    public void OnItemDetailCancelButtonUIPressed()
    {
        if (popupState == PopupState.itemDetail)
        {
            popupState = PopupState.none;

            itemDetailUI.CloseItemDetailUI(true);
        }
        else if (popupState == PopupState.deckInfo)
        {
            deckInfoUI.isShowingBoostDetail = false;

            itemDetailUI.CloseItemDetailUI(false);
        }
    }

    public void OnItemShowcasePurchaseButtonUIPressed(int index)
    {
        int gold = GameManager.instance.OnItemPurchased(index);
        if (gold != -1)
        {
            OnItemDetailCancelButtonUIPressed();

            goldInfoUI.UpdateGold(gold);
            itemBoardUI.PurchaseItem(index);

            AudioManager.instance.SFXShopBuy();

            GameManager.instance.ProcessTutorialStep("Purchase");
        }
        else
        {
            AudioManager.instance.SFXShopFail();
        }
    }

    public void OnItemRerollButtonUIPressed()
    {
        if (sceneState == SceneState.shopping && popupState == PopupState.none)
        {
            //Debug.Log("아이템 리롤 버튼 눌림");
            GameManager.instance.OnShopReroll();

            GameManager.instance.ProcessTutorialStep("ShopReroll");
        }
    }

    public void OnItemSetUIPressed(int index)
    {
        if (popupState == PopupState.none)
        {
            popupState = PopupState.itemDetail;

            GameManager.instance.OnItemInfoRequested(index);
            itemDetailUI.OpenItemDetailUI();
        }
    }

    public void OnItemInfoCallback(ItemData itemData, int index)
    {
        itemDetailUI.Initialize(itemData, index, isBoost: false, isPurchase: false);
    }

    public void OnItemSetDiscardButtonUIPressed(int index)
    {
        itemSetUI.DiscardItem(index);
        OnItemDetailCancelButtonUIPressed();
    }

    // StageSelectionUI methods
    public void OnNextStageChoiceButtonUIPressed(int choiceIndex)
    {
        if (sceneState == SceneState.selecting && popupState == PopupState.none)
        {
            GameManager.instance.OnStageSelection(choiceIndex);
            GameManager.instance.ProcessTutorialStep("StageChoice");
        }
    }

    public void OnStageSelection(StageData[] nextStageChoices, int currentChapterIndex, int currentStageIndex)
    {
        stageSelectionSignboardUI.Initialize(currentChapterIndex, currentStageIndex);
        scoreInfoUI.InitializeSelecting();
        stageInfoUI.InitializeSelecting();

        // stageData들을 받아와서 UI에 뿌려주는 메서드
        if (nextStageChoices.Length == 2)
        {
            stageSelectionBoardUI.Initialize(nextStageChoices, currentStageIndex);
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

        Debug.Log($"SceneState changed from {prevState} to {sceneState}");

        currentUIColor = new Color(Random.value, Random.value, Random.value);

        background.SetColor(currentUIColor);

        StartCoroutine(ChangeSceneStateCoroutine(prevState, sceneState));
        if (prevState == SceneState.playing && sceneState == SceneState.shopping)
        {
            Debug.Log("상점으로 전환");
            AudioManager.instance.ShopTransitionIn();
        }
        else if (prevState == SceneState.shopping && sceneState == SceneState.selecting)
        {
            Debug.Log("상점에서 나감");
            AudioManager.instance.ShopTransitionOut();
        }
        else 
        {
            Debug.Log("일반 전환");
            AudioManager.instance.SFXtransition();
        }
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
                if (GameManager.instance.GetCurrentStageIndex() == 3)
                {
                    currentUIColor = selectingBossBackgroundColors[Random.Range(0, selectingBossBackgroundColors.Count)];
                }
                else
                {
                    currentUIColor = selectingBackgroundColors[Random.Range(0, selectingBackgroundColors.Count)];
                }
                optionButtonUI.SetUIColor(currentUIColor);
                actionInfoUI.SetChipLayoutColor(currentUIColor);
                stageInfoUI.SetUIColor(currentUIColor);
                scoreInfoUI.SetUIColor(currentUIColor);
                stageSelectionSignboardUI.OpenStageSelectionSignboardUI();
                stageSelectionBoardUI.OpenStageSelectionBoardUI(currentUIColor);
                break;
            case SceneState.playing:
                if (GameManager.instance.GetCurrentStageIndex() == 3)
                {
                    currentUIColor = playingBossBackgroundColors[Random.Range(0, playingBossBackgroundColors.Count)];
                }
                else
                {
                    currentUIColor = playingBackgroundColors[Random.Range(0, playingBackgroundColors.Count)];
                }
                stageInfoUI.OpenStageInfoUI();
                optionButtonUI.SetUIColor(currentUIColor);
                actionInfoUI.SetChipLayoutColor(currentUIColor);
                stageInfoUI.SetUIColor(currentUIColor);
                scoreInfoUI.SetUIColor(currentUIColor);
                //scoreInfoUI.OpenScoreInfoUI();
                boardUI.OpenBoardUI();
                rerollButtonUI.OpenRerollButtonUI(currentUIColor);
                handUI.OpenHandUI();
                break;
            case SceneState.shopping:
                currentUIColor = shoppingBackgroundColors[Random.Range(0, shoppingBackgroundColors.Count)];
                shopSignboardUI.OpenShopSignboardUI();
                itemBoardUI.OpenItemBoardUI();
                optionButtonUI.SetUIColor(currentUIColor);
                actionInfoUI.SetChipLayoutColor(currentUIColor);
                stageInfoUI.SetUIColor(currentUIColor);
                scoreInfoUI.SetUIColor(currentUIColor);
                break;
            default:
                break;
        }
        background.SetColor(currentUIColor);
    }

    public void OnStageStart(int chapterIndex, int stageIndex, StageData stageData, BlockGameData blockGame)
    {        
        boardUI.gameObject.SetActive(true);
        stageInfoUI.InitializePlaying(chapterIndex, stageIndex, stageData);
        scoreInfoUI.InitializePlaying(currentScore: 0, scoreAtLeast: stageData.clearRequirement);
        rerollButtonUI.Initialize(blockGame.rerollCount);

        boardUI.Initialize(blockGame.boardRows, blockGame.boardColumns);
        
        ChangeSceneState(SceneState.playing);

        AudioManager.instance.ChangeStage(stageIndex);
    }

    public void OnBlocksDrawn(List<Block> blocks)
    {
        handUI.Initialize(blocks);
    }

    public bool TryPlaceBlock(int idx, Vector2Int pos, GameObject blockObj)
    {
        return GameManager.instance.TryPlaceBlock(idx, pos, blockObj);
    }

    public void OnShopStart(List<ItemData> items, int rerollCost, int currentChapterIndex, int currentStageIndex, bool isFirst = false)
    {
        if (isFirst)
        {
            ChangeSceneState(SceneState.shopping);
        }
        stageInfoUI.InitializeShopping(currentChapterIndex, currentStageIndex);
        itemBoardUI.Initialize(items, rerollCost);
        shopSignboardUI.Initialize(currentChapterIndex, currentStageIndex);
        scoreInfoUI.InitializeShopping();
    }

    public void UpdateShopRerollCost(int rerollCost)
    {
        itemBoardUI.UpdateRerollCost(rerollCost);
    }
 
    public void OnRotateBlock(int idx)
    {
        GameManager.instance.OnRotateBlock(idx);
    }

    public void OnBlockRotateCallback(int idx, Block block)
    {
        handUI.RotateBlock(idx, block);
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

    public void UpdateScore(int score, bool isAdditive = false) {
        scoreInfoUI.UpdateScore(score, isAdditive);
    }

    public void UpdateGold(int gold, bool isAdditive = false) {
        goldInfoUI.UpdateGold(gold, isAdditive);
    }

    public void PlayStageEffectAnimation()
    {
        stageInfoUI.ProcessStageEffectAnimation();
    }

    public void DisplayRerollCount(int rerollCount, bool isAdditive = false) {
        rerollButtonUI.UpdateRerollCount(rerollCount, isAdditive);
    }

    public void DisplayDeckCount(int deckCount, int maxDeckCount) {
        deckButtonUI.DisplayDeckCount(deckCount, maxDeckCount);
    }

    public void BackToMain()
    {
        GameManager.instance.BackToMain();
    }

    public void MakeNewRun()
    {
        AudioManager.instance.RestartGame();
        GameManager.instance.MakeNewRun();
    }

    public void PlayItemFullAnimation()
    {
        itemSetUI.PlayItemFullAnimation();
    }

    public void PlayNotEnoughGoldAnimation()
    {
        itemDetailUI.PlayNotEnoughGoldAnimation();
    }

    public void DisplayItemSet(List<ItemData> items, int maxItemCount, int discardIndex = -1)
    {
        itemSetUI.Initialize(items, maxItemCount, discardIndex);
    }

    public void UpdateItemTriggerCount(int index, int count)
    {
        itemSetUI.UpdateTriggerCountDescription(index, count);
    }

    public void StartItemShakeAnimation(int index, bool isBlockRelated)
    {
        itemSetUI.StartShakeAnimation(index, isBlockRelated);
    }

    public void StopItemShakeAnimation(bool isBlockRelated)
    {
        itemSetUI.StopShakeAnimation(isBlockRelated);
    }

    public void StopAllItemShakeAnimation()
    {
        StopItemShakeAnimation(true);
        StopItemShakeAnimation(false);
    }

    public void StartWarningStageEffectAnimation(bool isBlockRelated)
    {
        stageInfoUI.StartWarningStageEffectAnimation(isBlockRelated);
    }

    public void StopWarningStageEffectAnimation(bool isBlockRelated)
    {
        stageInfoUI.StopWarningStageEffectAnimation(isBlockRelated);
    }

    public void PlayItemEffectAnimation(string effectDescription, int index, float delay)
    {
        itemSetUI.PlayEffectAnimation(effectDescription, index, delay);

        AudioManager.instance.SFXEffectON();
    }

    public void BlockCells(HashSet<Vector2Int> cells)
    {
        boardUI.BlockCells(cells);
    }

    public void PlayStageClearAnimation(float delay)
    {
        boardUI.ProcessStageClearAnimation(delay);
    }

    public void ENDSTAGE()
    {
        GameManager.instance.EndStage();
    }

    public void OnGameEnd(bool isCleared, int currentChapterIndex, int currentStageIndex, RunData.History history, BlockType mostPlacedBlockType, string loseReason)
    {
        clearInfoUI.Initialize(isCleared, currentChapterIndex, currentStageIndex, history, mostPlacedBlockType, loseReason: loseReason);
        clearInfoUI.SetLayoutsColor(playingBackgroundColors[2]);
        clearInfoUI.OpenClearInfoUI(isCleared);

        if (isCleared) AudioManager.instance.SFXGameWin();
        else AudioManager.instance.SFXGameOver();
    }

    public void InfiniteMode()
    {
        GameManager.instance.InfiniteMode();
        clearInfoUI.CloseClearInfoUI();
    }

    public void PlayUnlockAnimation(UnlockInfo unlockInfo)
    {
        unlockNotificationUI.PlayUnlockAnimation(unlockInfo, currentUIColor);
    }

    public void OnBGMVolumeChanged(float value)
    {
        AudioManager.instance.ChangeBGMVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        AudioManager.instance.ChangeSFXVolume(value);
    }

    public void SetTutorialToggleValue(bool value)
    {
        tutorialToggle.isOn = value;
    }

    public void OnToggleTutorial(bool value)
    {
        GameManager.instance.SetTutorialValue(value);
    }

    public void TEST_BUTTON()
    {
        GameManager.instance.TEST_BUTTON();
    }
}

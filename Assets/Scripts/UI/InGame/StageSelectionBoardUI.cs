using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectionBoardUI : MonoBehaviour
{
    [SerializeField] private GameObject stageSelectionBoardUI;
    [SerializeField] private RectTransform rectTransform;
    // inside anchored position = (182,-128)
    private const float insidePositionY = -128;
    private const float outsidePositionOffsetY = -1080;
    private const float duration = 0.2f;

    [SerializeField] private GameUIManager gameUIManager;
    [SerializeField] private NextStageChoiceUI[] nextStageChoiceUI;

    public void OnNextStageChoiceButtonUIPressed(int choiceIndex)
    {
        gameUIManager.NextStageChoiceButtonUIPressed(choiceIndex);
    }

    public void InitializeNextStageChoiceUI(StageData[] nextStageChoices)
    {
        if (nextStageChoices.Length == nextStageChoiceUI.Length)
        {
            for (int i = 0; i < nextStageChoices.Length; i++)
            {
                nextStageChoiceUI[i].Initialize(nextStageChoices[i]);
            }
        } 
        else
        {
            Debug.LogError("length of nextStageChoices and nextStageChoiceUI is different");
            return;
        }
    }

    public void OpenStageSelectionBoardUI()
    {
        stageSelectionBoardUI.SetActive(true);
        rectTransform.DOAnchorPosY(insidePositionY, duration)
            .SetEase(Ease.OutCubic);
    }

    public void CloseStageSelectionBoardUI()
    {
        rectTransform.DOAnchorPosY(insidePositionY + outsidePositionOffsetY, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                stageSelectionBoardUI.SetActive(false);
            });
    }
}

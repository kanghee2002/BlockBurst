using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectionSignboardUI : MonoBehaviour
{
    [SerializeField] private GameObject stageSelectionSignboardUI;
    [SerializeField] private RectTransform rectTransform;
    // inside anchored position = (300,256)
    private const float insidePositionY = 256;
    private const float outsidePositionOffsetY = 540;
    private const float duration = 0.2f;
    public void OpenStageSelectionSignboardUI()
    {
        stageSelectionSignboardUI.SetActive(true);
        rectTransform.DOAnchorPosY(insidePositionY, duration)
            .SetEase(Ease.OutCubic);
    }

    public void CloseStageSelectionSignboardUI()
    {
        rectTransform.DOAnchorPosY(insidePositionY + outsidePositionOffsetY, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                stageSelectionSignboardUI.SetActive(false);
            });
    }
}

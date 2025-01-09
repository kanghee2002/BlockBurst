using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RunInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject runInfoUI;
    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private GameUIManager gameUIManager;

    private const float insidePositionY = 0;
    private const float outsidePositionY = -1080;
    private const float duration = 0.2f;

    public void OpenRunInfoUI()
    {
        runInfoUI.SetActive(true);
        rectTransform.DOAnchorPosY(insidePositionY, duration)
            .SetEase(Ease.OutCubic);
    }

    public void CloseRunInfoUI()
    {
        rectTransform.DOAnchorPosY(outsidePositionY, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
             {
                 runInfoUI.SetActive(false);
             });
    }

    public void OnRunInfoBackButtonUIPressed()
    {
        gameUIManager.RunInfoBackButtonUIPressed();
    }
}

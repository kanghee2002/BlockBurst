using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private GameObject optionUI;
    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private GameUIManager gameUIManager;

    private const float centerPositionY = 0;
    private const float outsidePositionY = -1080;
    private const float duration = 0.2f;

    public void OpenOptionUI()
    {
        optionUI.SetActive(true);
        rectTransform.DOAnchorPosY(centerPositionY, duration)
            .SetEase(Ease.OutSine);
    }

    public void CloseOptionUI()
    {
        rectTransform.DOAnchorPosY(outsidePositionY, duration)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                optionUI.SetActive(false);
            });
    }

    public void OnOptionBackButtonUIPressed()
    {
        gameUIManager.OptionBackButtonUIPressed();
    }
}

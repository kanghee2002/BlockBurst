using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject deckInfoUI;
    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private GameUIManager gameUIManager;

    private const float insidePositionX = 0;
    private const float outsidePositionX = 1920;
    private const float duration = 0.2f;

    public void OpenDeckInfoUI()
    {
        deckInfoUI.SetActive(true);
        rectTransform.DOAnchorPosX(insidePositionX, duration)
            .SetEase(Ease.OutCubic);
    }

    public void CloseDeckInfoUI()
    {
        rectTransform.DOAnchorPosX(outsidePositionX, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                deckInfoUI.SetActive(false);
            });
    }

    public void OnDeckInfoBackButtonUIPressed()
    {
        gameUIManager.DeckInfoBackButtonUIPressed();
    }
}

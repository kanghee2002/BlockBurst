using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RerollButtonUI : MonoBehaviour
{
    [SerializeField] private GameObject rerollButtonUI;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI rerollCountText;
    // inside anchored position = (-188,-400)
    private const float insidePositionX = -188;
    private const float outsidePositionOffsetX = 480;
    private const float duration = 0.2f;

    [SerializeField] private GameUIManager gameUIManager;

    public void OnRerollButtonUIPressed()
    {
        gameUIManager.RerollButtonUIPressed();
    }
    public void OpenRerollButtonUI()
    {
        rerollButtonUI.SetActive(true);
        rectTransform.DOAnchorPosX(insidePositionX, duration)
            .SetEase(Ease.OutCubic);
    }

    public void CloseRerollButtonUI()
    {
        rectTransform.DOAnchorPosX(insidePositionX + outsidePositionOffsetX, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                rerollButtonUI.SetActive(false);
            });
    }

    public void DisplayRerollCount(int rerollCount)
    {
        rerollCountText.text = rerollCount.ToString();
    }
}

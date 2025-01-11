using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField] private GameObject handUI;
    [SerializeField] private RectTransform rectTransform;
    // inside anchored position = (-188,0)
    private const float insidePositionX = -188;
    private const float outsidePositionOffsetX = 480;
    private const float duration = 0.2f;
    public void OpenHandUI()
    {
        handUI.SetActive(true);
        rectTransform.DOAnchorPosX(insidePositionX, duration)
            .SetEase(Ease.OutCubic);
    }

    public void CloseHandUI()
    {
        rectTransform.DOAnchorPosX(insidePositionX + outsidePositionOffsetX, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                handUI.SetActive(false);
            });
    }
}

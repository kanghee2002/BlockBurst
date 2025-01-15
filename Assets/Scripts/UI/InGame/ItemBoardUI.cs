using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoardUI : MonoBehaviour
{
    [SerializeField] private GameUIManager gameUIManager;
    [SerializeField] private RectTransform rectTransform;

    private const float insidePositionY = -128; // 도착할 Y 위치
    private const float outsidePositionOffsetY = -800; // 숨겨질 Y 위치
    private const float duration = 0.2f; // 애니메이션 시간

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(192, insidePositionY + outsidePositionOffsetY);
    }

    public void OnNextStageButtonUIPressed()
    {
        gameUIManager.NextStageButtonUIPressed();
    }

    public void OnItemRerollButtonUIPressed()
    {
        gameUIManager.ItemRerollButtonUIPressed();
    }

    public void OpenItemBoardUI()
    {
        gameObject.SetActive(true);
        rectTransform.DOAnchorPosY(insidePositionY, duration)
            .SetEase(Ease.OutCubic);
    }

    public void CloseItemBoardUI()
    {
        rectTransform.DOAnchorPosY(insidePositionY + outsidePositionOffsetY, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }
}

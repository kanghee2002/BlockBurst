using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BoardCellUI : MonoBehaviour 
{
    private string blockId;
    private Vector2Int cellIndex;
    private Image cellImage;
    private Color originalColor;
    private Sprite originalSprite;
    private bool isShadow = false;
    private bool isAnimating = false;
    private bool hideShadowReserved = false; // HideShadow 예약 플래그
    private bool isBlocked = false;

    private void Awake()
    {
        cellImage = GetComponent<Image>();
        originalColor = cellImage.color;
        originalSprite = cellImage.sprite;
    }

    public void SetCellIndex(Vector2Int index) {
        cellIndex = index;
    }

    public Vector2Int GetCellIndex() {
        return cellIndex;
    }

    public void SetBlockInfo(string id) {
        blockId = id;
    }

    public void ClearCell()
    {
        if (isBlocked) return;
        blockId = "";
        cellImage.sprite = originalSprite;
        cellImage.color = originalColor;
    }

    public void CopyVisualFrom(Transform cellUI) {
        GetComponent<Image>().sprite = cellUI.GetComponent<Image>().sprite;
    }

    private void AnimateShadow(Color targetColor, bool setShadow) {
        isAnimating = true;
        hideShadowReserved = false; // 예약 초기화
        Tween tween = cellImage.DOColor(targetColor, 0.1f);
        tween.OnComplete(() => {
            isAnimating = false;
            isShadow = setShadow;
            
            // 예약된 HideShadow 실행
            if (hideShadowReserved && setShadow) {
                HideShadow();
            }
        });
    }

    public void ShowShadow() {
        if (isBlocked) return;
        if (!isAnimating && !isShadow) {
            Color shadowColor = string.IsNullOrEmpty(blockId) ? 
                new Color(.6f, .6f, .6f, 1f) : 
                new Color(.7f, 0, 0, 1f);
            AnimateShadow(shadowColor, true);
        }
    }

    public void HideShadow() {
        if (isBlocked) return;
        if (isAnimating) {
            hideShadowReserved = true; // 애니메이션 중일 경우 예약
        } else if (isShadow) {
            AnimateShadow(originalColor, false);
        }
    }

    public void BlockCell() {
        isBlocked = true;
        cellImage.color = new Color(0.5f, 0.5f, 0.5f);
    }

    public void PlayHighlightAnimation() {
        if (isBlocked) return;
        cellImage.DOColor(new Color(1f, 1f, 1f), 0.2f).SetLoops(2, LoopType.Yoyo);
    }

    public void PlayClearAnimation() {
        if (isBlocked) return;
        Vector3 originalScale = transform.localScale;

        DOTween.Kill(transform);
        DOTween.Kill(cellImage);

        Sequence sequence = DOTween.Sequence();
        cellImage.sprite = originalSprite;
        cellImage.color = originalColor;

        sequence.Append(transform.DOScale(0, 0.3f).SetEase(Ease.InBack));
        sequence.Join(cellImage.DOFade(0, 0.3f));

        sequence.OnComplete(() => {
            transform.localScale = originalScale;
            cellImage.color = originalColor;
        });
    }
}

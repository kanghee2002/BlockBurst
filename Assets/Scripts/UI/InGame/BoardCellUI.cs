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

    public void CopyVisualFrom(Transform cellUI) {
        GetComponent<Image>().sprite = cellUI.GetComponent<Image>().sprite;
    }

    public void ShowShadow() {
        cellImage.DOColor(new Color(0.5f, 0.5f, 1f, 0.5f), 0.1f);
    }

    public void HideShadow() {
        cellImage.DOColor(originalColor, 0.1f);
    }

    public void PlayHighlightAnimation() {
        cellImage.DOColor(Color.yellow, 0.2f).SetLoops(2, LoopType.Yoyo);
    }

    public void PlayClearAnimation() {
        // 원래 스케일 값을 저장
        Vector3 originalScale = transform.localScale;
        
        // 애니메이션 시퀀스 생성
        Sequence sequence = DOTween.Sequence();
        
        cellImage.sprite = originalSprite;
        cellImage.color = originalColor;
        
        // 사라지는 애니메이션
        sequence.Append(transform.DOScale(0, 0.3f).SetEase(Ease.InBack));
        sequence.Join(cellImage.DOFade(0, 0.3f));
        
        // 애니메이션이 끝난 후 원래 상태로 복원
        sequence.OnComplete(() => {
            transform.localScale = originalScale;
            cellImage.color = originalColor;
        });
    }
}
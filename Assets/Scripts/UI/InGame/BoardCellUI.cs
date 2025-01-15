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

    private void Awake()
    {
        cellImage = GetComponent<Image>();
        originalColor = cellImage.color;
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
        transform.DOScale(0, 0.3f).SetEase(Ease.InBack);
        cellImage.DOFade(0, 0.3f);
    }
}
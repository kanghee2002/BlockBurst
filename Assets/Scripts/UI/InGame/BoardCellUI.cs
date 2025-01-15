using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BoardCellUI : MonoBehaviour 
{
    private string blockId;
    private Vector2Int cellIndex;

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

    public void PlayHighlightAnimation() {
        GetComponent<Image>().DOColor(Color.yellow, 0.2f).SetLoops(2, LoopType.Yoyo);
    }

    public void PlayClearAnimation() {
        transform.DOScale(0, 0.3f).SetEase(Ease.InBack);
        GetComponent<Image>().DOFade(0, 0.3f);
    }
}
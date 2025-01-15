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

    public void CopyVisualFrom(BlockUI blockUI) {
        // blockUI의 시각적 요소를 복사
    }

    public void PlayHighlightAnimation() {
        GetComponent<Image>().DOColor(Color.yellow, 0.2f).SetLoops(2, LoopType.Yoyo);
    }

    public void PlayClearAnimation() {
        transform.DOScale(0, 0.3f).SetEase(Ease.InBack);
        GetComponent<Image>().DOFade(0, 0.3f);
    }
}
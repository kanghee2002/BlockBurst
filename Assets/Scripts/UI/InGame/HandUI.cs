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
    
    [SerializeField] private GameObject BoardUICanvas;
    [SerializeField] private GameObject blockPrefab;

    [SerializeField] private BoardUI boardUI;

    private void Awake()
    {
        BoardUICanvas = GameObject.Find("BoardUICanvas");
    }

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

    public void Initialize(List<Block> hand)
    {
        int idx = 0;
        foreach (Block block in hand)
        {
            GameObject blockObj = Instantiate(blockPrefab, BoardUICanvas.transform);
            blockObj.transform.localPosition = new Vector3(800, (idx - 1) * 200, 0);
            var blockUI = blockObj.GetComponent<BlockUI>();
            blockUI.Initialize(block, idx++);
            Debug.Log(block.Shape);
        }
    }
}

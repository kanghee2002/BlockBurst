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

    public void Initialize(List<BlockData> hand)
    {
        boardUI = GameObject.Find("BoardUI").GetComponent<BoardUI>();
        foreach (BlockData blockData in hand)
        {
            GameObject block = Instantiate(blockPrefab, BoardUICanvas.transform);
            var blockUI = block.GetComponent<BlockUI>();
            blockUI.Initialize(blockData);
            boardUI.activeBlocks[blockData.id] = block;
        }
    }
}

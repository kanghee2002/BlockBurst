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

    GameObject[] blockUIs;

    private void Awake()
    {
        BoardUICanvas = GameObject.Find("BoardUICanvas");
    }

    public void OpenHandUI()
    {
        handUI.SetActive(true);
        rectTransform.DOAnchorPosX(insidePositionX, duration)
            .SetEase(Ease.OutCubic);

        for (int i = 0; i < blockUIs.Length; i++)
        {
            var blockObj = blockUIs[i];
            blockObj.GetComponent<CanvasGroup>().DOFade(1f, 0.2f)
                .SetDelay((3 - i) * 0.2f); // 순차적으로 나타나도록 딜레이 추가
        }
    }

    public void CloseHandUI()
    {
        foreach (GameObject blockUI in blockUIs)
        {
            if (blockUI) Destroy(blockUI);
        }
        rectTransform.DOAnchorPosX(insidePositionX + outsidePositionOffsetX, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                handUI.SetActive(false);
            });
    }

    public void Initialize(List<Block> hand)
    {
        blockUIs = new GameObject[hand.Count];
        int idx = 0;
        foreach (Block block in hand)
        {
            GameObject blockObj = Instantiate(blockPrefab, BoardUICanvas.transform);
            blockObj.transform.localPosition = new Vector3(750, (idx - 1) * 200, 0); // 위치는 임시
            var blockUI = blockObj.GetComponent<BlockUI>();
            blockUI.Initialize(block, idx);
            
            // 투명하게 초기화
            blockObj.GetComponent<CanvasGroup>().alpha = 0f;
            
            blockUIs[idx] = blockObj;
            idx++;
        }
    }
}

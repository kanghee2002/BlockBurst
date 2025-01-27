using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    private RectTransform rectTransform;
    // inside anchored position = (-188,0)
    private const float insidePositionX = -188;
    private const float outsidePositionOffsetX = 480;
    private const float duration = 0.2f;
    
    [SerializeField] private GameObject blockPrefab;

    private bool isOpen = false;

    GameObject[] blockUIs;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(List<Block> hand)
    {
        gameObject.SetActive(true);
        ClearHandUI();
        blockUIs = new GameObject[hand.Count];
        int idx = 0;
        foreach (Block block in hand)
        {
            GameObject blockObj = Instantiate(blockPrefab, transform);
            blockObj.transform.localPosition = new Vector3(0, (1 - idx) * 200, 0); // 위치는 임시
            var blockUI = blockObj.GetComponent<BlockUI>();
            blockUI.Initialize(block, idx);
            
            // 투명하게 초기화
            blockObj.GetComponent<CanvasGroup>().alpha = 0f;
            
            blockUIs[idx] = blockObj;
            idx++;
        }
        if (isOpen)
        {
            FadeInBlocks();
        }
    }

    public void OpenHandUI()
    {
        if (!isOpen)
        {
            isOpen = true;
            UIUtils.OpenUI(rectTransform, "X", insidePositionX, duration);
            FadeInBlocks();
        }
    }

    public void CloseHandUI()
    {
        if (isOpen)
        {
            isOpen = false;
            ClearHandUI();
            UIUtils.CloseUI(rectTransform, "X", insidePositionX, outsidePositionOffsetX, duration);
        }
    }

    void FadeInBlocks()
    {
        AudioManager.instance.SFXRechargeBlock(blockUIs.Length);
        for (int i = 0; i < blockUIs.Length; i++)
        {
            var blockObj = blockUIs[i];
            blockObj.GetComponent<CanvasGroup>().DOFade(1f, 0.2f)
                .SetDelay((i + 1) * 0.2f); // 순차적으로 나타나도록 딜레이 추가
        }
    }

    public void ClearHandUI()
    {
        if (blockUIs != null) 
        {
            foreach (GameObject blockUI in blockUIs)
            {
                if (blockUI) Destroy(blockUI);
            }
        }
    }
}

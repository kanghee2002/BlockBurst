using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    private RectTransform rectTransform;
    // inside anchored position = (-188,0)
    private const float windowsInsidePositionX = -188;
    private const float mobileInsidePositionX = 0;
    private const float outsidePositionOffsetX = 680;
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
            GameObject blockObj = Instantiate(blockPrefab, transform.GetChild(1));
            if (GameManager.instance.applicationType == ApplicationType.Windows)
            {
                blockObj.transform.localPosition = new Vector3(0, (1 - idx) * 175 / transform.GetChild(1).GetComponent<RectTransform>().localScale.x, 0); // 위치는 임시
            }
            else if (GameManager.instance.applicationType == ApplicationType.Mobile)
            {
                blockObj.transform.localPosition = new Vector3(-50 + idx * 100, 0); // 위치는 임시
            }

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

    public void RotateBlock(int idx, Block block)
    {
        blockUIs[idx].GetComponent<BlockUI>().Initialize(block, idx);
    }    

    public void OpenHandUI()
    {
        if (!isOpen)
        {
            isOpen = true;
            if (GameManager.instance.applicationType == ApplicationType.Windows)
            {
                UIUtils.OpenUI(rectTransform, "X", windowsInsidePositionX, duration);
            }
            else
            {
                UIUtils.OpenUI(rectTransform, "X", mobileInsidePositionX, duration);
            }
            FadeInBlocks();
        }
    }

    public void CloseHandUI()
    {
        if (isOpen)
        {
            isOpen = false;
            ClearHandUI();
            if (GameManager.instance.applicationType == ApplicationType.Windows)
            {
                UIUtils.CloseUI(rectTransform, "X", windowsInsidePositionX, outsidePositionOffsetX, duration);
            }
            else
            {
                UIUtils.CloseUI(rectTransform, "X", mobileInsidePositionX, outsidePositionOffsetX, duration);
            }
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

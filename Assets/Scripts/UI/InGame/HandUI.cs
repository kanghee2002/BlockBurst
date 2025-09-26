using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandUI : MonoBehaviour
{
    private RectTransform rectTransform;
    // inside anchored position = (-188,0)
    private const float windowsInsidePositionX = -188;
    private const float mobileInsidePositionX = 112;
    private const float outsidePositionOffsetX = -400;
    private const float duration = 0.2f;

    public Image bg;
    public Image fg1;
    public Image fg2;
    
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private GameObject deckButtonUI;

    private bool isOpen = false;

    private int currentHandCount;

    private const float blockAnimInterval = 0.2f;
    private const float blockMoveDelay = 1f;

    private GameObject[] blockUIs;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(List<Block> hand)
    {
        gameObject.SetActive(true);
        ClearHandUI();
        blockUIs = new GameObject[hand.Count];
        currentHandCount = hand.Count;
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
                blockObj.transform.localPosition = blockObj.transform.parent.InverseTransformPoint(deckButtonUI.transform.position);
            }

            var blockUI = blockObj.GetComponent<BlockUI>();
            blockUI.Initialize(block, idx, GetBlockPosition(idx));
            
            // 투명하게 초기화
            blockObj.GetComponent<CanvasGroup>().alpha = 0f;
            blockObj.transform.localScale = Vector3.one * 0.01f;

            blockUIs[idx] = blockObj;
            idx++;
        }

        PlayDrawAnimation(blockUIs);

        if (isOpen)
        {
            FadeInBlocks();
        }
    }

    public void RotateBlock(int idx, Block block)
    {
        blockUIs[idx].GetComponent<BlockUI>().Initialize(block, idx, GetBlockPosition(idx));
    }

    public void SetColorOfUI(Color uiColor, Color textColor)
    {
        UIUtils.SetImageColorByScalar(bg, uiColor, 1f);
        UIUtils.SetImageColorByScalar(fg1, uiColor, 3f / 5f);
        UIUtils.SetImageColorByScalar(fg2, uiColor, 3f / 5f);

    }

    public void OpenHandUI()
    {
        if (!isOpen)
        {
            isOpen = true;
            if (GameManager.instance.applicationType == ApplicationType.Windows)
            {
                UIUtils.OpenUI(rectTransform, "Y", windowsInsidePositionX, duration);
            }
            else
            {
                UIUtils.OpenUI(rectTransform, "Y", mobileInsidePositionX, duration);
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
                UIUtils.CloseUI(rectTransform, "Y", windowsInsidePositionX, outsidePositionOffsetX, duration);
            }
            else
            {
                UIUtils.CloseUI(rectTransform, "Y", mobileInsidePositionX, outsidePositionOffsetX, duration);
            }
        }
    }

    void FadeInBlocks()
    {
        AudioManager.instance.SFXRechargeBlock(blockUIs.Length);
        for (int i = 0; i < blockUIs.Length; i++)
        {
            var blockObj = blockUIs[i];
            blockObj.GetComponent<CanvasGroup>().DOFade(1f, 0.05f)
               .SetDelay(i * blockAnimInterval); // 순차적으로 나타나도록 딜레이 추가
        }
    }

    public void ClearHandUI()
    {
        if (blockUIs != null) 
        {
            PlayDiscardAnimation(blockUIs);
        }
    }

    private Vector3 GetBlockPosition(int idx)
    {
        return new Vector3(-50 * (currentHandCount - 1) + idx * 100, 0);
    }

    private void PlayDrawAnimation(GameObject[] blocks)
    {
        int idx = 0;
        foreach (GameObject block in blocks)
        {
            Vector3 deckButtonPos = block.transform.parent.InverseTransformPoint(deckButtonUI.transform.position);
            Vector3 finalPos = GetBlockPosition(idx);

            block.transform.localPosition = deckButtonPos;

            Vector3[] path = new Vector3[] { new Vector3((deckButtonPos.x + finalPos.x) / 2f, 60f), finalPos };

            block.transform.DOLocalPath(path, blockMoveDelay, PathType.CatmullRom)
                .SetEase(Ease.OutExpo).SetDelay(blockAnimInterval * (idx + 1));

            block.transform.DOScale(Vector3.one * 0.25f, blockMoveDelay)
                .SetEase(Ease.OutExpo).SetDelay(blockAnimInterval * (idx + 1));

            idx++;
        }
    } 

    private void PlayDiscardAnimation(GameObject[] blocks)
    {
        foreach (GameObject block in blocks)
        {
            if (block)
            {
                block.transform.DOKill();

                Vector3 deckButtonPos = block.transform.parent.InverseTransformPoint(deckButtonUI.transform.position);

                Vector3[] path = new Vector3[] { new Vector3((block.transform.localPosition.x + deckButtonPos.x) / 2f, -60f), deckButtonPos };

                Sequence sequence = DOTween.Sequence();

                sequence.Append(block.transform.DOLocalPath(path, blockMoveDelay, PathType.CatmullRom)
                    .SetEase(Ease.OutExpo));

                sequence.Join(block.transform.DOScale(Vector3.one * 0.01f, blockMoveDelay)
                    .SetEase(Ease.OutExpo));

                sequence.OnComplete(() => Destroy(block));
            }
        }
    }
}

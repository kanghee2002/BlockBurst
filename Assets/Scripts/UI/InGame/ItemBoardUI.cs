using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class ItemBoardUI : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private Transform itemShowcaseTransform;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private TextMeshProUGUI rerollCostText;
    GameObject[] itemUIs;
    Vector3[] originalPositions;

    private const float insidePositionY = -128; // 도착할 Y 위치
    private const float outsidePositionOffsetY = -800; // 숨겨질 Y 위치
    private const float duration = 0.2f; // 애니메이션 시간

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(192, insidePositionY + outsidePositionOffsetY);
    }
    
    public void Initialize(List<ItemData> items, int rerollCost)
    {   
        if (gameObject.activeSelf)  // 이미 active 상태면 리롤 애니메이션 재생
        {
            PlayRerollAnimation(items, rerollCost);
            return;
        }

        // 처음 초기화할 때는 기존 로직대로 진행
        gameObject.SetActive(true);
        ClearItemShowcaseUI();
        CreateItems(items);
        rerollCostText.text = "$" + rerollCost;
    }

    public void OpenItemBoardUI()
    {
        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
    }

    public void CloseItemBoardUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionOffsetY, duration);
    }

    private void PlayRerollAnimation(List<ItemData> items, int rerollCost)
    {
        float jumpHeight = 10f;
        float rotateAmount = 10f;
        float duration = 0.3f;
        float staggerTime = 0.1f;

        for (int i = 0; i < itemUIs.Length; i++)
        {
            itemUIs[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        for (int i = 0; i < itemUIs.Length; i++)
        {
            int index = i;
            GameObject card = itemUIs[i];

            card.GetComponent<ItemDescriptionUI>().descriptionCanvasGroup.alpha = 0;

            DOTween.Kill(card.transform);
            DOTween.Kill(card.GetComponent<CanvasGroup>());
            card.transform.localPosition = originalPositions[i];

            card.transform.DOScale(1f, 0.2f);

            Vector3 originalPosition = card.transform.localPosition;

            // 간단한 시퀀스: 올라가면서 흔들거리다가 -> 데이터 변경 -> 내려오면서 멈춤
            Sequence cardSequence = DOTween.Sequence();
            
            // 좌우 흔들기
            cardSequence.Append(card.transform.DOLocalMoveY(originalPosition.y + jumpHeight, duration)
                .SetEase(Ease.OutQuad));
            cardSequence.Join(card.transform.DORotate(new Vector3(0, 0, rotateAmount), duration/4)
                .SetLoops(4, LoopType.Yoyo)
                .SetEase(Ease.Linear));

            // 데이터 변경
            cardSequence.AppendCallback(() => {
                if (index < items.Count)
                {
                    SetImage(card, items[index]);
                    card.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = items[index].itemName;
                    card.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = "<color=yellow>$" + items[index].cost.ToString() + "</color>";
                    card.GetComponent<ItemDescriptionUI>().Initialize(items[index]);
                }
            });

            // 내려오기
            cardSequence.Append(card.transform.DOLocalMove(originalPosition, duration));
            cardSequence.Join(card.transform.DORotate(Vector3.zero, duration));
            cardSequence.Join(card.GetComponent<CanvasGroup>().DOFade(1, duration));

            cardSequence.SetDelay(i * staggerTime);
            cardSequence.Play();

            // 끝날 때에 맞추어 raycast 활성
            if (i == itemUIs.Length - 1)
            {
                cardSequence.OnComplete(() => {
                    for (int j = 0; j < itemUIs.Length; j++)
                    {
                        itemUIs[j].GetComponent<CanvasGroup>().blocksRaycasts = true;
                    }
                });
            }
        }

        rerollCostText.text = "$" + rerollCost;
    }

    private void CreateItems(List<ItemData> items)
    {
        itemUIs = new GameObject[items.Count];
        originalPositions = new Vector3[itemUIs.Length];

        for (int i = 0; i < items.Count; i++)
        {
            int currentIndex = i;
            ItemData itemData = items[currentIndex];
            itemUIs[i] = Instantiate(itemPrefab, itemShowcaseTransform);
            GameObject itemUI = itemUIs[i];
            itemUI.transform.localPosition = new Vector3((currentIndex - 1) * 250, 0, 0);

            SetImage(itemUI, items[currentIndex]);

            itemUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = itemData.itemName;
            itemUI.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = "<color=yellow>$" + itemData.cost.ToString() + "</color>";
            itemUI.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => {
                GameUIManager.instance.OnItemShowcaseItemButtonPressed(currentIndex);
            });

            itemUI.GetComponent<ItemDescriptionUI>().Initialize(items[currentIndex]);
            itemUI.GetComponent<ItemDescriptionUI>().descriptionCanvasGroup.alpha = 0;

            originalPositions[i] = itemUI.transform.localPosition;
        }
    }

    public void PurchaseItem(int idx)
    {
        GameObject card = itemUIs[idx];
        CanvasGroup canvasGroup = card.GetComponent<CanvasGroup>();

        card.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() => canvasGroup.alpha = 0);

        canvasGroup.blocksRaycasts = false;
    }

    public void ClearItemShowcaseUI()
    {
        if (itemUIs != null)
        {
            foreach (GameObject itemUI in itemUIs)
            {
                Destroy(itemUI);
            }
        }
    }

    private void SetImage(GameObject itemUI, ItemData item)
    {
        string blockPresetPath = "Sprites/Block/Preset/";
        string upgradePath = "Sprites/Item/Upgrade/";
        string itemPath = "Sprites/Item/Item/";

        itemUI.transform.GetChild(1).gameObject.SetActive(false);

        if (item.type == ItemType.ADD_BLOCK)
        {
            string path = blockPresetPath + item.block.type.ToString();
            Sprite blockPreset = Resources.Load<Sprite>(path);
            itemUI.transform.GetChild(0).GetComponent<Image>().sprite = blockPreset;
        }
        else if (item.type == ItemType.ITEM || item.type == ItemType.BOOST || item.type == ItemType.CONVERT_BLOCK)
        {
            string path = itemPath + item.id;
            Sprite itemSprite = Resources.Load<Sprite>(path);
            itemUI.transform.GetChild(0).GetComponent<Image>().sprite = itemSprite;
        }
        else if (item.type == ItemType.UPGRADE)
        {
            // 블록 프리셋 배치
            string blockPath = blockPresetPath;
            foreach (EffectData effectData in item.effects)
            {
                foreach (BlockType blockType in effectData.blockTypes)
                {
                    blockPath += blockType.ToString();
                }
            }
            Sprite blockPreset = Resources.Load<Sprite>(blockPath);
            itemUI.transform.GetChild(0).GetComponent<Image>().sprite = blockPreset;

            // 아이콘 배치
            string IconPath = upgradePath;
            if (item.effects[0].type == EffectType.GOLD_MODIFIER)
            {
                IconPath += "GoldUpgrade";
            }
            else if (item.effects[0].type == EffectType.MULTIPLIER_MODIFIER)
            {
                IconPath += "MultiplierUpgrade";
            }
            else if (item.effects[0].type == EffectType.REROLL_MODIFIER)
            {
                IconPath += "RerollUpgrade";
            }
            else if (item.effects[0].type == EffectType.BLOCK_REUSE_MODIFIER)
            {
                IconPath += "ReuseUpgrade";
            }
            else if (item.effects[0].type == EffectType.SCORE_MODIFIER)
            {
                IconPath += "ScoreUpgrade";
            }

            Sprite IconSprite = Resources.Load<Sprite>(IconPath);
            itemUI.transform.GetChild(1).gameObject.SetActive(true);
            itemUI.transform.GetChild(1).GetComponent<Image>().sprite = IconSprite;
        }
    }
}

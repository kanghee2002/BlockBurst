using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class ItemBoardUI : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private Transform itemShowcaseTransform;
    [SerializeField] private Transform boostShowcaseTransform;
    [SerializeField] private Transform blockShowcaseTransform;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject mobileItemPrefab;
    [SerializeField] private TextMeshProUGUI rerollCostText;

    [Header("UI Color")]
    [SerializeField] private Color itemShowcaseColor;
    [SerializeField] private Color boostShowcaseColor;
    [SerializeField] private Color blockItemShowcaseColor;

    private GameObject[] currentItemUIs;

    private Vector3[] originalPositions;

    private const float windowsInsidePositionY = -128; // 도착할 Y 위치
    private const float mobileInsidePositionY = -175; // 도착할 Y 위치
    private const float outsidePositionOffsetY = -1000; // 숨겨질 Y 위치
    private const float duration = 0.2f; // 애니메이션 시간

    private Sequence slowShakeSequence;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
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
        UpdateRerollCost(rerollCost);
    }

    public void UpdateRerollCost(int rerollCost)
    {
        rerollCostText.text = "새로고침 • $" + rerollCost;
    }

    public void OpenItemBoardUI()
    {
        gameObject.SetActive(true);
        
        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            UIUtils.OpenUI(rectTransform, "Y", windowsInsidePositionY, duration);
        }
        else
        {
            UIUtils.OpenUI(rectTransform, "Y", mobileInsidePositionY, duration);
        }
    }

    public void CloseItemBoardUI()
    {
        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            UIUtils.CloseUI(rectTransform, "Y", windowsInsidePositionY, outsidePositionOffsetY, duration);
        }
        else
        {
            UIUtils.CloseUI(rectTransform, "Y", mobileInsidePositionY, outsidePositionOffsetY, duration);
        }
    }

    private void PlayRerollAnimation(List<ItemData> items, int rerollCost)
    {
        float jumpHeight = 10f;
        float rotateAmount = 10f;
        float duration = 0.3f;
        float staggerTime = 0.05f;

        for (int i = 0; i < currentItemUIs.Length; i++)
        {
            int index = i;
            GameObject card = currentItemUIs[index];

            if (items[index] == null)
            {
                card.transform.DOScale(0f, 0.2f).OnComplete(() => card.SetActive(false));
                continue;
            }

            if (GameManager.instance.applicationType == ApplicationType.Windows)
            {
                card.GetComponent<ItemDescriptionUI>().DescriptionFadeOut();
            }

            DOTween.Kill(card.GetComponent<CanvasGroup>());
            card.transform.localPosition = originalPositions[index];

            Vector3 originalPosition = card.transform.localPosition;

            card.transform.DOKill();
            card.transform.DOScale(1f, 0.2f);

            // 간단한 시퀀스: 올라가면서 흔들거리다가 -> 데이터 변경 -> 내려오면서 멈춤
            Sequence cardSequence = DOTween.Sequence();

            // 좌우 흔들기
            cardSequence.Append(card.transform.DOLocalMoveY(originalPosition.y + jumpHeight, duration)
                .SetEase(Ease.OutQuad));
            cardSequence.Join(card.transform.DORotate(new Vector3(0, 0, rotateAmount), duration / 4)
                .SetLoops(4, LoopType.Yoyo)
                .SetEase(Ease.Linear));

            // 데이터 변경
            cardSequence.AppendCallback(() => {
                if (index < items.Count)
                {
                    GameObject itemUI = currentItemUIs[index];
                    ItemData itemData = items[index];
                    itemUI.SetActive(true);

                    SetImage(card, items[index]);
                    itemUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = itemData.itemName;
                    itemUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "$" + itemData.cost;
                    itemUI.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    itemUI.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => {
                        GameUIManager.instance.OnItemShowcaseItemButtonUIPressed(index);
                    });
                }
            });
            
            // 내려오기
            cardSequence.Append(card.transform.DOLocalMove(originalPosition, duration));
            cardSequence.Join(card.transform.DORotate(Vector3.zero, duration));
            cardSequence.Join(card.GetComponent<CanvasGroup>().DOFade(1, duration));

            cardSequence.SetDelay(i * staggerTime);
            cardSequence.Play();

            // 끝날 때에 맞추어 raycast 활성
            if (i == currentItemUIs.Length - 1)
            {
                cardSequence.OnComplete(() => {
                    for (int j = 0; j < currentItemUIs.Length; j++)
                    {
                        currentItemUIs[j].GetComponent<CanvasGroup>().blocksRaycasts = true;
                    }
                });
            }
        }

        UpdateRerollCost(rerollCost);
    }

    private void CreateItems(List<ItemData> items)
    {
        //int itemCount = items.Count(item => item.type == ItemType.ITEM);
        //int boostCount = items.Count(item => item.type == ItemType.BOOST);
        //int blockItemCount = items.Count(item => item.type != ItemType.ITEM && item.type != ItemType.BOOST);

        if (GameManager.instance.applicationType == ApplicationType.Mobile)
        {
            int maxItemCount = 6, index = 0;
            currentItemUIs = new GameObject[maxItemCount];
            originalPositions = new Vector3[maxItemCount];

            CreateSpecificItems(items, ref index, itemShowcaseTransform, UIUtils.itemTypeColors[ItemType.ITEM]);
            CreateSpecificItems(items, ref index, boostShowcaseTransform, UIUtils.itemTypeColors[ItemType.BOOST]);
            CreateSpecificItems(items, ref index, blockShowcaseTransform, UIUtils.itemTypeColors[ItemType.ADD_BLOCK]);
        }
    }

    private void CreateSpecificItems(List<ItemData> items, ref int index, Transform parent, Color uiColor)
    {
        int maxItemCount = 2;
        float startingPosY = 110, itemInterval = 170;

        for (int i = 0; i < maxItemCount; i++)
        {
            int currentIndex = index;
            currentItemUIs[index] = Instantiate(mobileItemPrefab, parent);
            GameObject itemUI = currentItemUIs[index];

            itemUI.transform.localPosition = new Vector3(0, startingPosY - itemInterval * i, 0);

            originalPositions[index] = itemUI.transform.localPosition;

            if (items[index] == null)
            {
                itemUI.GetComponent<Image>().color = uiColor;
                currentItemUIs[index].SetActive(false);
                index++;
                break;
            }

            ItemData itemData = items[index];
            SetImage(itemUI, itemData);

            itemUI.GetComponent<Image>().color = uiColor;
            itemUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = itemData.itemName;
            itemUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "$" + itemData.cost;
            itemUI.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => {
                GameUIManager.instance.OnItemShowcaseItemButtonUIPressed(currentIndex);
            });

            UIUtils.PlaySlowShakeAnimation(itemUI.transform.GetChild(0), rotateAmount: 5f, duration: 3f, delay: Random.Range(0f, 3f));

            index++;
        }
    }

    public void PurchaseItem(int idx)
    {
        GameObject card = currentItemUIs[idx];
        CanvasGroup canvasGroup = card.GetComponent<CanvasGroup>();

        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            card.GetComponent<ItemDescriptionUI>().DescriptionFadeOut();
        }

        card.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() => canvasGroup.alpha = 0);

        canvasGroup.blocksRaycasts = false;
    }

    public void ClearItemShowcaseUI()
    {
        if (currentItemUIs != null)
        {
            foreach (GameObject itemUI in currentItemUIs)
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

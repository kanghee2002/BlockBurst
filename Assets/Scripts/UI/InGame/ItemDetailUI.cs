using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailUI : MonoBehaviour
{
    [SerializeField] private PopupBlurImage popupBlurImage;

    [Header("Item")]
    [SerializeField] private Image outer;
    [SerializeField] private Image inner;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image rarityLayout;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private TextMeshProUGUI itemRarityText;
    [SerializeField] private ButtonUI interactButtonUI;
    [SerializeField] private TextMeshProUGUI interactButtonText;
    [SerializeField] private Image showNotEnoughGoldImage;

    private RectTransform rectTransform;

    private string currentDescription;

    private const float insidePositionY = 0;
    private const float outsidePositionY = -1080;
    private const float duration = 0.2f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(ItemData itemData, int index, bool isBoost, bool isPurchase)
    {
        SetItemImage(itemData);
        SetLayoutColor(itemData);
        itemNameText.text = itemData.itemName;
        currentDescription = GetDescription(itemData, isPurchase);
        itemDescriptionText.text = currentDescription;
        itemPriceText.text = "$" + itemData.cost;
        itemRarityText.text = UIUtils.itemRarityNames[itemData.rarity];

        if (isBoost)                // 덱 창에서 부스트 누른 경우 
        {
            interactButtonUI.gameObject.SetActive(false);
        }
        else if (isPurchase)        // 상점의 아이템을 누른 경우
        {
            interactButtonUI.gameObject.SetActive(true);

            int itemIndex = index;
            interactButtonUI.SetOnClickMethod(() =>
            {
                GameUIManager.instance.OnItemShowcasePurchaseButtonUIPressed(itemIndex);
            });
            interactButtonText.text = "구매";
        }
        else                        // 소지 중인 아이템을 누른 경우
        {
            interactButtonUI.gameObject.SetActive(true);

            int itemIndex = index;
            interactButtonUI.SetOnClickMethod(() =>
            {
                GameUIManager.instance.OnItemSetDiscardButtonUIPressed(itemIndex);
            });
            interactButtonText.text = "버리기";
        }

        UIUtils.PlaySlowShakeAnimation(itemImage.transform, rotateAmount: 4f, duration: 2f);
    }

    private string GetDescription(ItemData item, bool isPurchase)
    {
        string description = "";

        for (int i = 0; i < item.effects.Count; i++)
        {
            string tmp = UIUtils.GetEffectValueText(item.effects[i].effectName.Replace("\n", " "), item.effects[i]);
            string tmp2 = UIUtils.GetTriggerValueText(tmp, item.effects[i]);
            description += UIUtils.SetBlockNameToIcon(tmp2);
            if (i != item.effects.Count - 1)
            {
                description += "\n";
            }
        }

        if (!isPurchase)
        {
            EffectData triggerEffect = item.effects.FirstOrDefault(effect => effect.triggerMode == TriggerMode.Interval);

            if (triggerEffect != null)
            {
                description += $" (현재 횟수: {triggerEffect.triggerCount})";
            }
        }

        currentDescription = description;

        description = UIUtils.SetBlockNameToIcon(currentDescription);

        return description;
    }

    private void SetItemImage(ItemData item)
    {
        string blockPresetPath = "Sprites/Block/Preset/";
        string upgradePath = "Sprites/Item/Upgrade/";
        string itemPath = "Sprites/Item/Item/";

        itemIcon.gameObject.SetActive(false);

        if (item.type == ItemType.ADD_BLOCK)
        {
            string path = blockPresetPath + item.block.type.ToString();
            Sprite blockPreset = Resources.Load<Sprite>(path);
            itemImage.sprite = blockPreset;
        }
        else if (item.type == ItemType.ITEM || item.type == ItemType.BOOST || item.type == ItemType.CONVERT_BLOCK)
        {
            string path = itemPath + item.id;
            Sprite itemSprite = Resources.Load<Sprite>(path);
            itemImage.sprite = itemSprite;
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
            itemImage.sprite = blockPreset;

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
            itemIcon.gameObject.SetActive(true);
            itemIcon.sprite = IconSprite;
        }
    }

    public void PlayNotEnoughGoldAnimation()
    {

        if (showNotEnoughGoldImage.gameObject.activeSelf)
        {
            return;
        }
        showNotEnoughGoldImage.gameObject.SetActive(true);
        showNotEnoughGoldImage.color = new Color(0f, 0f, 0f, 0.8f);

        float duration = 0.5f, endDelay = 0.2f;

        RectTransform textRect = showNotEnoughGoldImage.transform.GetChild(0).GetComponent<RectTransform>();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(textRect.DOPunchScale(Vector3.one * 1.2f, duration,
            vibrato: 7, elasticity: 0.3f)
            .SetEase(Ease.OutQuad));

        sequence.AppendInterval(endDelay);

        sequence.OnComplete(() =>
        {
            textRect.DOScale(Vector3.one, duration);
            showNotEnoughGoldImage.gameObject.SetActive(false);
        });
    }

    private void SetLayoutColor(ItemData itemData)
    {
        ItemType itemType = itemData.type;

        if (itemData.type == ItemType.ITEM)
        {
            itemType = ItemType.ITEM;
        }
        else if (itemData.type == ItemType.BOOST)
        {
            itemType = ItemType.BOOST;
        }
        else
        {
            itemType = ItemType.ADD_BLOCK;
        }
        
        UIUtils.SetImageColorByScalar(outer, UIUtils.itemTypeColors[itemType], 7f / 10f, duration: 0.1f);
        UIUtils.SetImageColorByScalar(inner, UIUtils.itemTypeColors[itemType], 9f / 10f, duration: 0.1f);

        rarityLayout.color = UIUtils.rarityColors[itemData.rarity];
    }

    public void OpenItemDetailUI()
    {
        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
        popupBlurImage.OpenPopupBlurImage(new Color(0.0f, 0.0f, 0.0f, 0.9f));
    }

    public void CloseItemDetailUI(bool isClosingPopupBlur)
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionY, duration);
        
        if (isClosingPopupBlur)
        {
            popupBlurImage.ClosePopupBlurImage();
        }
    }
}

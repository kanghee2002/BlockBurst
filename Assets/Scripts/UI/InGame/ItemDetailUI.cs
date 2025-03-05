using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemDetailUI : MonoBehaviour
{
    [SerializeField] private PopupBlurImage popupBlurImage;

    [Header("Item")]
    [SerializeField] private Image layout;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private ButtonUI interactButtonUI;
    [SerializeField] private TextMeshProUGUI interactButtonText;

    private RectTransform rectTransform;

    private string currentDescription;

    private const float insidePositionY = 0;
    private const float outsidePositionY = -1080;
    private const float duration = 0.2f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(ItemData itemData, int index, bool isPurchase)
    {
        SetItemImage(itemData);
        SetLayoutColor(itemData);
        itemNameText.text = itemData.itemName;
        currentDescription = GetDescription(itemData);
        itemDescriptionText.text = currentDescription;
        itemPriceText.text = "$" + itemData.cost;

        if (isPurchase)     // 상점의 아이템을 누른 경우
        {
            int itemIndex = index;
            interactButtonUI.SetOnClickMethod(() =>
            {
                GameUIManager.instance.OnItemShowcasePurchaseButtonUIPressed(itemIndex);
            });
            interactButtonText.text = "구매";
        }
        else                // 소지 중인 아이템을 누른 경우
        {
            int itemIndex = index;
            interactButtonUI.SetOnClickMethod(() =>
            {
                GameUIManager.instance.OnItemSetDiscardButtonUIPressed(itemIndex);
            });
            interactButtonText.text = "버리기";
        }
    }

    private string GetDescription(ItemData item)
    {
        string description = "";

        for (int i = 0; i < item.effects.Count; i++)
        {
            description += item.effects[i].effectName.Replace("\n", " ");
            if (i != item.effects.Count - 1)
            {
                description += "\n";
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

    private void SetLayoutColor(ItemData itemData)
    {
        float scalar = 9f / 10f;

        if (itemData.type == ItemType.ITEM)
        {
            layout.color = UIUtils.itemTypeColors[ItemType.ITEM] * scalar;
        }
        else if (itemData.type == ItemType.BOOST)
        {
            layout.color = UIUtils.itemTypeColors[ItemType.BOOST] * scalar;
        }
        else
        {
            layout.color = UIUtils.itemTypeColors[ItemType.ADD_BLOCK] * scalar;
        }
    }

    public void OpenItemDetailUI()
    {
        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
        popupBlurImage.OpenPopupBlurImage(new Color(0.0f, 0.0f, 0.0f, 0.9f));
    }

    public void CloseItemDetailUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionY, duration);
        popupBlurImage.ClosePopupBlurImage();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ItemDescriptionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject itemDescriptionUI;
    [SerializeField] private float fadeTime = 0.2f;  // fade in/out 시간

    private ItemData item;
    private string currentDescription;
    private CanvasGroup descriptionCanvasGroup;

    // 효과 타입별 색상 정의
    private readonly Dictionary<ItemEffectType, Color> effectColors = new Dictionary<ItemEffectType, Color>()
    {
        { ItemEffectType.SCORE, new Color(0x0b/255f, 0xa9/255f, 0x05/255f) }, // #0ba905
        { ItemEffectType.DECK, new Color(0x47/255f, 0x38/255f, 0xff/255f) },  // #4738ff
        { ItemEffectType.GOLD, new Color(0xd9/255f, 0xa7/255f, 0x38/255f) },  // #d9a738
        { ItemEffectType.OTHER, new Color(0xfc/255f, 0x8b/255f, 0x4d/255f) }  // #fc8b4d
    };

    // 레어도별 색상 정의
    private readonly Dictionary<ItemRarity, Color> rarityColors = new Dictionary<ItemRarity, Color>()
    {
        { ItemRarity.SILVER, new Color(0xb0/255f, 0xa7/255f, 0xb8/255f) },   // #b0a7b8
        { ItemRarity.GOLD, new Color(0xff/255f, 0xc8/255f, 0x57/255f) },     // #ffc857
        { ItemRarity.PLATINUM, new Color(0xc0/255f, 0xf9/255f, 0xff/255f) }  // #c0f9ff
    };

    private void Awake()
    {
        descriptionCanvasGroup = itemDescriptionUI.GetComponent<CanvasGroup>();
        if (descriptionCanvasGroup == null)
        {
            descriptionCanvasGroup = itemDescriptionUI.AddComponent<CanvasGroup>();
        }
        descriptionCanvasGroup.alpha = 0f;
    }

    public void Initialize(ItemData item)
    {
        this.item = item;
        currentDescription = GetDescription(item);
        itemDescriptionUI.transform.GetChild(1).GetComponent<Image>().color = effectColors[item.effectType]; // 효과
        itemDescriptionUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentDescription;
        transform.GetChild(6).GetComponent<Image>().color = rarityColors[item.rarity]; // 레어도
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(currentDescription))
        {
            return;
        }
        // Fade In
        DOTween.Kill(descriptionCanvasGroup);
        descriptionCanvasGroup.DOFade(1f, fadeTime);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Fade Out
        DOTween.Kill(descriptionCanvasGroup);
        descriptionCanvasGroup.DOFade(0f, fadeTime);
    }

    private string GetDescription(ItemData item)
    {
        string description = "<size=35>";
        
        if (item.type == ItemType.ITEM)
        {
            description += "[아이템]";
        }
        else if (item.type == ItemType.UPGRADE)
        {
            description += "[강화]";
        }
        else
        {
            description += "[덱]";
        }

        description += "</size>\n";

        for (int i = 0; i < item.effects.Count; i++)
        {
            description += item.effects[i].effectName;
            if (i != item.effects.Count - 1)
            {
                description += "\n";
            }
        }
        currentDescription = description;

        return description;
    }
}
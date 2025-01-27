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
        { ItemEffectType.SCORE, new Color(0x36/255f, 0xc5/255f, 0xf4/255f) }, // #36c5f4
        { ItemEffectType.DECK, new Color(0x9d/255f, 0xe6/255f, 0x4e/255f) },  // #9de64e
        { ItemEffectType.GOLD, new Color(0xf3/255f, 0xa8/255f, 0x33/255f) },  // #f3a833
        { ItemEffectType.OTHER, new Color(0xcc/255f, 0x99/255f, 0xff/255f) }  // #cc99ff
    };

    // 레어도별 색상 정의
    private readonly Dictionary<ItemRarity, Color> rarityColors = new Dictionary<ItemRarity, Color>()
    {
        { ItemRarity.SILVER, new Color(0xb0/255f, 0xa7/255f, 0xb8/255f) },   // #b0a7b8
        { ItemRarity.GOLD, new Color(0xf3/255f, 0xa8/255f, 0x33/255f) },     // #f3a833
        { ItemRarity.PLATINUM, new Color(0x36/255f, 0xc5/255f, 0xf4/255f) }  // #36c5f4
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
        string description = "";

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
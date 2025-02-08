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
    [SerializeField] private Image rarityLayout;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private Image outline;

    private ItemData item;
    private string currentDescription;
    private CanvasGroup descriptionCanvasGroup;

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
        itemDescriptionUI.transform.GetChild(1).GetComponent<Image>().color = UIUtils.effectColors[ItemEffectType.OTHER]; // 회색
        itemDescriptionUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentDescription;
        if (outline != null) outline.color = UIUtils.rarityColors[item.rarity]; // 레어도
        rarityLayout.color = UIUtils.rarityColors[item.rarity];
        rarityLayout.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = UIUtils.itemRarityNames[item.rarity];
        itemTypeText.text = UIUtils.itemTypeNames[item.type];
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
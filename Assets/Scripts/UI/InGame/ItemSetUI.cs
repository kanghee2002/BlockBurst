using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class ItemSetUI : MonoBehaviour
{
    [SerializeField] private GameObject itemIconPrefab;
    [SerializeField] private float iconOverlap = 30f;    
    [SerializeField] private float hoverScale = 1.2f;    
    [SerializeField] private float hoverDuration = 0.2f; 
    private float startPos = -380f;
    private List<GameObject> itemIcons = new List<GameObject>();

    public void Initialize(List<ItemData> items)
    {
        Clear();

        float iconWidth = itemIconPrefab.GetComponent<RectTransform>().rect.width;
        float spacing = iconWidth - iconOverlap;

        foreach (ItemData item in items)
        {
            GameObject itemIcon = Instantiate(itemIconPrefab, transform);
            
            RectTransform rectTransform = itemIcon.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(startPos + spacing * itemIcons.Count, 0);
            
            itemIcon.transform.GetChild(0).GetComponent<Image>().sprite = GetImage(item);
            SetUpgradeIcon(itemIcon, item);

            GameObject description = itemIcon.transform.GetChild(2).gameObject;

            description.GetComponent<TextMeshProUGUI>().text = GetDescription(item);

            EventTrigger trigger = itemIcon.AddComponent<EventTrigger>();
            
            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((data) => {
                OnItemEnter(description);
                itemIcon.transform.DOScale(hoverScale, hoverDuration).SetEase(Ease.OutQuad);
                itemIcon.transform.SetAsLastSibling();
            });
            trigger.triggers.Add(enterEntry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => {
                OnItemExit(description);
                itemIcon.transform.DOScale(1f, hoverDuration).SetEase(Ease.OutQuad);
                itemIcon.transform.SetSiblingIndex(itemIcons.IndexOf(itemIcon));
            });
            trigger.triggers.Add(exitEntry);

            EventTrigger.Entry clickEntry = new EventTrigger.Entry();
            clickEntry.eventID = EventTriggerType.PointerClick;
            clickEntry.callback.AddListener((data) => {  });
            trigger.triggers.Add(clickEntry);
            
            itemIcon.transform.SetSiblingIndex(itemIcons.Count);
            itemIcons.Add(itemIcon);
        }

        // 아이템 많을 때 임시 위치 조정
        if (itemIcons.Count >= 12 && itemIcons.Count <= 22)       // 두 줄일 때
        {
            for (int i = 0; i < itemIcons.Count; i++)
            {
                RectTransform rectTransform = itemIcons[i].GetComponent<RectTransform>();
                
                // 첫 번째 줄
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 40f);   
                
                if (i > 10) // 두 번째 줄
                {
                    rectTransform.anchoredPosition = new Vector2(startPos + spacing * (i - 11), -40f);

                }
            }
        }
        else if (itemIcons.Count > 22)       // 세 줄 이상일 때
        {
            for (int i = 0; i < itemIcons.Count; i++)
            {
                RectTransform rectTransform = itemIcons[i].GetComponent<RectTransform>();

                int line = i / 11;

                rectTransform.anchoredPosition = new Vector2(startPos + spacing * (i - 11 * line), 80f + (-80f) * line);
            }
        }
    }

    private Sprite GetImage(ItemData item)
    {
        string blockPresetPath = "Sprites/Block/Preset/";
        string itemPath = "Sprites/Item/Item/";
        
        if (item.type == ItemType.ADD_BLOCK)
        {
            return Resources.Load<Sprite>(blockPresetPath + item.block.type.ToString());
        }
        else if (item.type == ItemType.ITEM)
        {
            return Resources.Load<Sprite>(itemPath + item.id);
        }
        else if (item.type == ItemType.DELETE_BLOCK || item.type == ItemType.UPGRADE)
        {
            return Resources.Load<Sprite>(blockPresetPath + item.block.type.ToString());
        }
        return null;
    }

    private void SetUpgradeIcon(GameObject itemUI, ItemData item)
    {
        string upgradePath = "Sprites/Item/Upgrade/";
        itemUI.transform.GetChild(1).gameObject.SetActive(false);

        if (item.type != ItemType.DELETE_BLOCK && item.type != ItemType.UPGRADE)
            return;

        string IconPath = upgradePath;
        if (item.type == ItemType.DELETE_BLOCK)
        {
            IconPath += "DeleteUpgrade";
        }
        else if (item.effects[0].type == EffectType.GOLD_MODIFIER)
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

    public void Clear()
    {
        foreach (GameObject icon in itemIcons)
        {
            if (icon != null)
                Destroy(icon);
        }
        itemIcons.Clear();
    }

    private void OnItemEnter(GameObject description)
    {
        description.SetActive(true);
    }

    private void OnItemExit(GameObject description)
    {
        description.SetActive(false);
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

        return description;
    }

    public void PlayEffectAnimation(string effectDescription, int index, float delay)
    {
        GameObject currentItem = itemIcons[index];

        TextMeshProUGUI currentText = currentItem.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        currentText.gameObject.SetActive(true);
        currentText.text = effectDescription;

        Vector2 originalPosition = currentText.rectTransform.anchoredPosition;
        Color originalColor = currentText.color;
        float yOffset = 10f;

        currentItem.transform.DOPunchScale(Vector2.one * 1.2f, 0.3f, 10, 0.5f);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(currentText.rectTransform.DOAnchorPosY(originalPosition.y + yOffset, 0.3f)
            .SetEase(Ease.InOutQuad));
        sequence.Join(currentText.DOFade(0f, delay).SetEase(Ease.OutQuad));

        sequence.OnComplete(() =>
        {
            currentText.rectTransform.anchoredPosition = originalPosition;
            currentText.color = originalColor;
            currentText.gameObject.SetActive(false);
        });
    }
}
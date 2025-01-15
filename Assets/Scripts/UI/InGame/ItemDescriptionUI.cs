using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDescriptionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject itemDescriptionUI;

    private ItemData item;
    private string currentDescription;

    public void Initialize(ItemData item)
    {
        this.item = item;
        currentDescription = GetDescription(item);
        itemDescriptionUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentDescription;
        itemDescriptionUI.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentDescription == "")
        {
            return;
        }
        itemDescriptionUI.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemDescriptionUI.SetActive(false);
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

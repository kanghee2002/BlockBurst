using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemShowcaseUI : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;

    public void Initialize(List<ItemData> items)
    {   
        for (int i = 0; i < items.Count; i++)
        {
            int currentIndex = i;
            ItemData itemData = items[i];
            GameObject itemUI = Instantiate(itemPrefab, transform);
            itemUI.transform.localPosition = new Vector3((i - 1) * 250, 0, 0);

            itemUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = itemData.id;
            itemUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = itemData.cost.ToString();
            itemUI.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => {
                GameUIManager.instance.OnItemShowcaseItemButtonPressed(currentIndex);
            });
        }
    }

    public void OpenItemShowcaseUI()
    {

    }

    public void CloseItemShowcaseUI()
    {
        gameObject.SetActive(false);
    }
}

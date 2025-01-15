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
            ItemData itemData = items[currentIndex];
            GameObject itemUI = Instantiate(itemPrefab, transform);
            itemUI.transform.localPosition = new Vector3((currentIndex - 1) * 250, 0, 0);

            SetImage(itemUI, items[currentIndex]);

            itemUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = itemData.itemName;
            itemUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = itemData.cost.ToString();
            itemUI.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => {
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
        else if (item.type == ItemType.ITEM)
        {
            string path = itemPath + item.id;
            Sprite itemSprite = Resources.Load<Sprite>(path);
            itemUI.transform.GetChild(0).GetComponent<Image>().sprite = itemSprite;
        }
        else if (item.type == ItemType.DELETE_BLOCK || item.type == ItemType.UPGRADE)
        {
            // 블록 프리셋 배치
            string blockPath = blockPresetPath + item.block.type.ToString();
            Sprite blockPreset = Resources.Load<Sprite>(blockPath);
            itemUI.transform.GetChild(0).GetComponent<Image>().sprite = blockPreset;

            // 아이콘 배치
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
    }
}

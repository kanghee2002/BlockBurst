using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemBoardUI : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private Transform itemShowcaseTransform;
    [SerializeField] private GameObject itemPrefab;
    GameObject[] itemUIs;

    private const float insidePositionY = -128; // 도착할 Y 위치
    private const float outsidePositionOffsetY = -800; // 숨겨질 Y 위치
    private const float duration = 0.2f; // 애니메이션 시간

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(192, insidePositionY + outsidePositionOffsetY);
    }
    
    public void Initialize(List<ItemData> items)
    {   
        gameObject.SetActive(true);
        // 기존 itemUI 삭제
        ClearItemShowcaseUI();

        itemUIs = new GameObject[items.Count];
        for (int i = 0; i < items.Count; i++)
        {
            int currentIndex = i;
            ItemData itemData = items[currentIndex];
            itemUIs[i] = Instantiate(itemPrefab, itemShowcaseTransform);
            GameObject itemUI = itemUIs[i];
            itemUI.transform.localPosition = new Vector3((currentIndex - 1) * 250, 0, 0);

            SetImage(itemUI, items[currentIndex]);

            itemUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = itemData.itemName;
            itemUI.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "$" + itemData.cost.ToString();
            itemUI.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => {
                GameUIManager.instance.OnItemShowcaseItemButtonPressed(currentIndex);
            });

            itemUI.GetComponent<ItemDescriptionUI>().Initialize(items[currentIndex]);
        }
    }

    public void OpenItemBoardUI()
    {
        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
    }

    public void CloseItemBoardUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionOffsetY, duration);
    }

    public void PurchaseItem(int idx)
    {
        Destroy(itemUIs[idx]);
    }

    public void ClearItemShowcaseUI()
    {
        if (itemUIs != null)
        {
            foreach (GameObject itemUI in itemUIs)
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

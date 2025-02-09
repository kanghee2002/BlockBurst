using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("DEBUG")]
    // 첫 상점에서 나올 아이템 지정
    [SerializeField] private List<string> firstShopItemList;

    private RunData runData;
    [SerializeField] private List<ItemData> currentItems;

    private DeckManager deckManager;

    public int rerollCost = 2;

    private Dictionary<ItemType, int> itemWeights;

    public void Initialize(ref RunData data, ItemData[] items)
    {
        runData = data;
        currentItems = new List<ItemData>();
        foreach (ItemData item in items)
        {
            AddItem(item);
            // 특수 블록을 좀 복사
            if (item.type == ItemType.ADD_BLOCK && Enums.IsSpecialBlockType(item.block.type))
            {
                AddItem(item);
                AddItem(item);
            }
            // 기본 블록을 좀 복사
            if (item.type == ItemType.ADD_BLOCK && Enums.IsDefaultBlockType(item.block.type))
            {
                AddItem(item);
            }

            // 현재 트리거 값 초기화
            item.effects.ForEach(effect => effect.triggerCount = 0);
        }
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();

        itemWeights = new Dictionary<ItemType, int>()
        {
            { ItemType.ADD_BLOCK, 40 },
            { ItemType.CONVERT_BLOCK, 10 },
            { ItemType.ITEM, 32 },
            { ItemType.BOOST, 8 },
            { ItemType.UPGRADE, 20 },
        };
    }

    // 튜토리얼 용 함수
    public void AddFirstItem(List<string> items)
    {
        foreach (string item in items)
        {
            firstShopItemList.Add(item);
        }
    }

    public int PurchaseItem(ItemData item)
    {
        if (item == null) return -1;

        if (item.type == ItemType.ITEM && runData.activeItems.Count >= runData.maxItemCount)
        {
            GameManager.instance.PlayItemFullAnimation();
            return -1;
        }

        if (runData.gold < item.cost)
        {
            return -1;
        } 
        else
        {
            if (item.isPurchasableAgain)
            {
                AddItem(item);
            }

            GameManager.instance.UpdateGold(-item.cost);
            ApplyItem(item);
            return runData.gold;
        }
    }

    public ItemData PopItem()
    {
        if (currentItems.Count == 0)
        {
            return null;
        }

        // DEBUG or TUTORIAL /////////////////////////////////////////
        if (firstShopItemList.Count > 0)
        {
            int index = currentItems.FindIndex(x => x.id == firstShopItemList[0]);
            ItemData itemData = currentItems[index];
            currentItems.RemoveAt(index);
            firstShopItemList.RemoveAt(0);

            return itemData;
        }
        ///////////////////////////////////////////////////////////////

        List<ItemData> filteredItems = new List<ItemData>();
        for (int i = 0; i < 10000; i++)
        {
            ItemType selectedItemType = SelectByWeight(itemWeights);
            filteredItems = currentItems.Where(item => item.type == selectedItemType).ToList();
            if (filteredItems.Count > 0) break;
        }

        int idx = Random.Range(0, filteredItems.Count);
        ItemData item = filteredItems[idx];
        currentItems.Remove(item);

        return item;
    }

    private void ApplyItem(ItemData item)
    {
        if (item.type == ItemType.ADD_BLOCK)
        {
            int addCount = 0;
            if (Enums.IsDefaultBlockType(item.block.type))
            {
                addCount = 2;
            }
            else
            {
                addCount = 1;
            }

            for (int i = 0; i < addCount; i++)
            {
                deckManager.AddBlockToRunDeck(item.block);
            }

            // 특수 블록은 첫 구매에만 효과 등록
            if (Enums.IsSpecialBlockType(item.block.type))
            {
                foreach (EffectData effect in item.block.effects)
                {
                    if (EffectManager.instance.ContainsEffect(effect))
                    {
                        continue;
                    }
                    EffectManager.instance.AddEffect(effect);
                }
            }
        }
        else if (item.type == ItemType.CONVERT_BLOCK)
        {
            int convertCount = 2;
            for (int i = 0; i < convertCount; i++)
            {
                deckManager.RemoveRandomBlockFromRunDeck(item.block.type);
            }
            for (int i = 0; i < convertCount; i++)
            {
                deckManager.AddBlockToRunDeck(item.block);
            } 
        }
        else if (item.type == ItemType.ITEM)
        {
            runData.activeItems.Add(item);
            foreach (EffectData effect in item.effects)
            {
                EffectManager.instance.AddEffect(effect);
            }
        }
        else if (item.type == ItemType.BOOST)
        {
            // 진행정보 창에 추가 필요
            runData.activeBoosts.Add(item);
            foreach (EffectData effect in item.effects)
            {
                EffectManager.instance.AddEffect(effect);
            }
        }
        else if (item.type == ItemType.UPGRADE) 
        {
            foreach (EffectData effect in item.effects)
            {
                // 아이템처럼 취급
                // 구매 시 등록 (BlockData의 effects로 등록 X)
                EffectManager.instance.AddEffect(effect);
                CellEffectManager.instance.AddEffect(effect.blockTypes, effect.type.ToString());
            }
        }

        if (item.type == ItemType.ADD_BLOCK ||
            item.type == ItemType.CONVERT_BLOCK)
        {
            GameManager.instance.UpdateDeckCount(runData.availableBlocks.Count, runData.availableBlocks.Count);
        }
    }

    public void UpgradeBlock(BlockData block, EffectData effect)
    {
        block.effects.Add(effect);
    }

    public void RerollShop(List<ItemData> items)
    {
        foreach (ItemData item in items)
        {
            if (item != null)
            {
                AddItem(item);
            }
        }
    }

    public void AddItem(ItemData item)
    {
        // append
        currentItems.Add(item);
    }

    private int GetTotalWeights()
    {
        return itemWeights.Sum(x => x.Value);
    }

    private T SelectByWeight<T>(Dictionary<T, int> weights)
    {
        // 누적 가중치 만들기
        List<(T, int)> weightThresholds = new List<(T, int)>();

        int sum = 0;
        foreach (var itemWeight in weights)
        {
            sum += itemWeight.Value;
            weightThresholds.Add((itemWeight.Key, sum));
        }

        // 뽑기
        int randomWeight = Random.Range(0, GetTotalWeights()) + 1;

        for (int i = 0; i < weightThresholds.Count; i++)
        {
            if (weightThresholds[i].Item2 >= randomWeight)
            {
                return weightThresholds[i].Item1;
            }
        }

        Debug.Log("Error: ShopManager Weighted Random");
        return default(T);
    }
}
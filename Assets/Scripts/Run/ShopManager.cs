using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private RunData runData;
    private List<ItemData> currentItems;

    private DeckManager deckManager;

    public int rerollCost = 2;

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
        }
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
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
        else {
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
        int idx = Random.Range(0, currentItems.Count);
        ItemData item = currentItems[idx];
        currentItems.RemoveAt(idx);

        if (item.type != ItemType.ITEM)
        {
            AddItem(item);
        }

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
        else if (item.type == ItemType.UPGRADE) 
        {
            foreach (EffectData effect in item.effects)
            {
                // 아이템처럼 취급
                // 구매 시 등록 (BlockData의 effects로 등록 X)
                EffectManager.instance.AddEffect(effect);
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
}
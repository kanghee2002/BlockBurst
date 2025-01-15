using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private RunData runData;
    private List<ItemData> currentItems;

    public DeckManager deckManager;

    public void Initialize(ref RunData data, ItemData[] items)
    {
        runData = data;
        currentItems = new List<ItemData>(items);
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
    }

    public bool PurchaseItem(ItemData item)
    {
        if (item == null || runData.gold < item.cost)
        {
            return false;
        } 
        else {
            runData.gold -= item.cost;
            ApplyItem(item);
            return true;
        }
    }

    public ItemData PopItem()
    {
        if (currentItems.Count == 0)
        {
            return null;
        }
        ItemData item = currentItems[0];
        currentItems.RemoveAt(0);
        return item;
    }

    private void ApplyItem(ItemData item)
    {
        if (item.type == ItemType.ADD_BLOCK)
        {
            int addCount = 2;
            for (int i = 0; i < addCount; i++)
            {
                deckManager.AddBlock(item.block);
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
        else if (item.type == ItemType.DELETE_BLOCK)
        {
            int deleteCount = 2;
            for (int i = 0; i < deleteCount; i++)
            {
                deckManager.RemoveBlock(item.block);
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
    }

    public void UpgradeBlock(BlockData block, EffectData effect)
    {
        block.effects.Add(effect);
    }
}
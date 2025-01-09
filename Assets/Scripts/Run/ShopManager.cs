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
        if (item.type == ItemType.BLOCK)
        {
            deckManager.AddBlock(item.block);
        }
        else if (item.type == ItemType.ITEM)
        {
            runData.activeItems.Add(item);
            foreach (EffectData effect in item.effects)
            {
                EffectManager.instance.AddEffect(effect);
                if (effect.trigger == TriggerType.ON_ACQUIRE)
                {
                    EffectManager.instance.ApplyEffect(effect);
                }
            }
        } 
        else if (item.type == ItemType.UPGRADE) 
        {
            foreach (EffectData effect in item.effects)
            {
                UpgradeBlock(item.block, effect);
            }
        }
    }

    public void UpgradeBlock(BlockData block, EffectData effect)
    {
        block.effects.Add(effect);
    }
}
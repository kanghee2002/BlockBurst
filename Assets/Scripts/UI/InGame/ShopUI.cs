using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public void DisplayItems(List<ItemData> items);
    public void UpdateGold(int gold);
    public void ShowItemDetail(ItemData item);
    public void ShowUpgradeOptions(BlockData block);
    public void UpdatePurchaseState(bool canPurchase);
}

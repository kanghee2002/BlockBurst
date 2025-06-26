using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Linq;

public class UnlockManager : MonoBehaviour
{
    public static UnlockManager instance = null;

    public Action<int> onIOplaceCountUpdate;
    public Action<int> onZSplaceCountUpdate;
    public Action<int> onJLTplaceCountUpdate;
    public Action<int> onRerollCountUpdate;
    public Action<int> onItemPurchaseCountUpdate;
    public Action<int> onMaxScoreUpdate;
    public Action<bool> onHasWonUpdate;

    private PlayerData playerData;

    private const string wheel = "Wheel";

    // 모든 해금 아이템 리스트
    public readonly List<string> lockedItems = new()
    {
        wheel,
    };

    private void Awake()
    {
        // singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(PlayerData playerData)
    {
        this.playerData = playerData;

        if (!playerData.unlockedItems.Contains(wheel))
        {
            onRerollCountUpdate += CheckWheelCondtion;
        }
    }

    private void CheckWheelCondtion(int rerollCount)
    {
        int requirement = 5;

        if (rerollCount >= requirement)
        {
            playerData.AddUnlockedItem(wheel);
            
            GameManager.instance.AddUnlockedItem(wheel);

            GameManager.instance.PlayUnlockAnimation(null);

            onRerollCountUpdate -= CheckWheelCondtion;
        }
    }

    // 해금된 아이템만 반환
    public ItemData[] GetUnlockedItems(ItemData[] itemTemplats)
    {
        ItemData[] unlockedItems = itemTemplats.ToArray();

        List<string> lockedItemIDs = GetLockedItems();

        unlockedItems = unlockedItems.Where(x => !lockedItemIDs.Contains(x.id)).ToArray();

        return unlockedItems;
    }

    // 현재 해금되지 않은 아이템 리스트
    private List<string> GetLockedItems()
    {
        List<string> currentLockedItems = lockedItems.ToList();

        foreach (string itemID in playerData.unlockedItems)
        {
            currentLockedItems.Remove(itemID);
        }

        return currentLockedItems;
    }
}

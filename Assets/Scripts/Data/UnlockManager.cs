using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Linq;

public class UnlockManager : MonoBehaviour
{
    public static UnlockManager instance = null;

    public Action<int> onIplaceCountUpdate;
    public Action<int> onOplaceCountUpdate;
    public Action<int> onZplaceCountUpdate;
    public Action<int> onSplaceCountUpdate;
    public Action<int> onJplaceCountUpdate;
    public Action<int> onLplaceCountUpdate;
    public Action<int> onTplaceCountUpdate;
    public Action<int> onRerollCountUpdate;
    public Action<int> onItemPurchaseCountUpdate;
    public Action<int> onMaxScoreUpdate;
    public Action<int> onMaxChapterUpdate;
    public Action<bool> onHasWonUpdate;

    // 모든 해금 아이템 리스트
    private UnlockInfoList UnlockInfoList;

    private PlayerData playerData;

    private const string wheel = "Wheel";

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameManager.instance.PlayUnlockAnimation(null);
        }
    }

    public void Initialize(PlayerData playerData)
    {
        this.playerData = playerData;

        UnlockInfoList = new UnlockInfoList();
        UnlockInfoList.Initialize();

        foreach (UnlockInfo<int> unlockInfo in UnlockInfoList.intList)
        {
            SetSubscribe(unlockInfo, true);
        }

        foreach (UnlockInfo<bool> unlockInfo in UnlockInfoList.boolList)
        {

        }
    }

    public void SetSubscribe<T>(UnlockInfo<T> unlockInfo, bool isSubscribing)
    {
        switch (unlockInfo.trigger)
        {
            case UnlockTrigger.RerollCount:
                //if (isSubscribing) onRerollCountUpdate += unlockInfo.condition;
                break;
        }
    }

    public void TryUnlock(UnlockTarget unlockTarget, string targetName)
    {
        // Unlock
        if (unlockTarget == UnlockTarget.Item)
        {
            GameManager.instance.AddUnlockedItem(targetName);
        }
        else if (unlockTarget == UnlockTarget.Deck)
        {
            //
        }
        GameManager.instance.PlayUnlockAnimation(null);

        Debug.Log(targetName + " Unlokced!");
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
        List<string> currentLockedItems = UnlockInfoList.intList.Select(x => x.targetName).ToList();

        foreach (string itemID in playerData.unlockedItems)
        {
            currentLockedItems.Remove(itemID);
        }

        return currentLockedItems;
    }
}
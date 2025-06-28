using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Linq;

public class UnlockManager : MonoBehaviour
{
    public static UnlockManager instance = null;

    public Action<int> onPlaceCountIUpdate;
    public Action<int> onPlaceCountOUpdate;
    public Action<int> onPlaceCountZUpdate;
    public Action<int> onPlaceCountSUpdate;
    public Action<int> onPlaceCountJUpdate;
    public Action<int> onPlaceCountLUpdate;
    public Action<int> onPlaceCountTUpdate;
    public Action<int> onRerollCountUpdate;
    public Action<int> onItemPurchaseCountUpdate;
    public Action<int> onMaxScoreUpdate;
    public Action<int> onMaxChapterUpdate;
    public Action<int> onWinCountUpdate;
    public Action<int> onBoardHalfFullCountUpdate;
    public Action<int> onIOrerollCountUpdate;
    public Action<int> onZSrerollCountUpdate;
    public Action<int> onJLTrerollCountUpdate;
    public Action<int> onMaxBaseMultiplierUpdate;
    public Action<int> onMaxMultiplierUpdate;
    public Action<int> onMaxBaseRerollCountUpdate;
    public Action<int> onMaxGoldUpdate;
    public Action<int> onHasOnlyIOUpdate;
    public Action<int> onHasOnlyZSUpdate;
    public Action<int> onHasOnlyJLTUpdate;

    // 모든 해금 아이템 리스트
    private UnlockInfoList UnlockInfoList;

    private PlayerData playerData;

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

        UnlockInfoList = new UnlockInfoList();
        UnlockInfoList.Initialize();

        // 해금되지 않은 경우만 Action 등록
        foreach (UnlockInfo unlockInfo in UnlockInfoList.list)
        {
            if (unlockInfo.targetType == UnlockTarget.Item &&
                !playerData.IsItemUnlocked(unlockInfo.targetName))
            {
                SetSubscribe(unlockInfo, true);
            }
            
            if (unlockInfo.targetType == UnlockTarget.Deck &&
                !playerData.IsDeckUnlocked(unlockInfo.targetName))
            {
                SetSubscribe(unlockInfo, true);
            }
        }
    }

    public void SetSubscribe(UnlockInfo unlockInfo, bool isSubscribing)
    {
        switch (unlockInfo.trigger)
        {
            case UnlockTrigger.IplaceCount:
                if (isSubscribing) onPlaceCountIUpdate += unlockInfo.condition;
                else onPlaceCountIUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.OplaceCount:
                if (isSubscribing) onPlaceCountOUpdate += unlockInfo.condition;
                else onPlaceCountOUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.RerollCount:
                if (isSubscribing) onRerollCountUpdate += unlockInfo.condition;
                else onRerollCountUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.ItemPurchaseCount:
                if (isSubscribing) onItemPurchaseCountUpdate += unlockInfo.condition;
                else onItemPurchaseCountUpdate -= unlockInfo.condition;
                break;
        }
    }

    public void Unlock(UnlockInfo unlockInfo, string targetName)
    {
        // Unlock
        if (unlockInfo.targetType == UnlockTarget.Item)
        {
            GameManager.instance.AddUnlockedItem(targetName);
        }
        else if (unlockInfo.targetType == UnlockTarget.Deck)
        {
            //TODO
        }

        SetSubscribe(unlockInfo, false);
        
        GameManager.instance.PlayUnlockAnimation(unlockInfo);
    }

    // 해금된 아이템만 반환
    public ItemData[] GetUnlockedItems(ItemData[] itemTemplats)
    {
        ItemData[] unlockedItems = itemTemplats.ToArray();

        List<string> lockedItemIDs = GetLockedTargets();

        unlockedItems = unlockedItems.Where(x => !lockedItemIDs.Contains(x.id)).ToArray();

        return unlockedItems;
    }

    // 현재 해금되지 않은 목록 반환
    private List<string> GetLockedTargets()
    {
        List<string> currentLockedItems = UnlockInfoList.list.Select(x => x.targetName).ToList();

        foreach (string itemID in playerData.unlockedItems)
        {
            currentLockedItems.Remove(itemID);
        }

        foreach (PlayerData.DeckInfo deckInfo in playerData.unlockedDecks)
        {
            currentLockedItems.Remove(deckInfo.deckName);
        }

        return currentLockedItems;
    }
}
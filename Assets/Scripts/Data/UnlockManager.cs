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
    public Action<int> onShopRerollCountUpdate;
    public Action<int> onMaxScoreUpdate;
    public Action<int> onMaxChapterUpdate;
    public Action<int> onWinCountUpdate;
    public Action<int> onBoardHalfFullCountUpdate;
    public Action<int> onRerollCountIOUpdate;
    public Action<int> onRerollCountZSUpdate;
    public Action<int> onRerollCountJLTUpdate;
    public Action<int> onMaxBaseMultiplierUpdate;
    public Action<int> onMaxMultiplierUpdate;
    public Action<int> onMaxBaseRerollCountUpdate;
    public Action<int> onMaxGoldUpdate;
    public Action<int> onHasOnlyIOUpdate;
    public Action<int> onHasOnlyZSUpdate;
    public Action<int> onHasOnlyJLUpdate;
    public Action<int> onDefaultDeckWinCountUpdate;
    public Action<int> onYoYoDeckWinCountUpdate;
    public Action<int> onDiceDeckWinCountUpdate;
    public Action<int> onTelescopeDeckWinCountUpdate;
    public Action<int> onMirrorDeckWinCountUpdate;
    public Action<int> onBombDeckWinCountUpdate;
    public Action<int> onClearedMaxLevelUpdate;

    // 모든 해금 아이템 리스트
    private UnlockInfo[] unlockInfoTemplates;

    private PlayerData playerData;

    private bool isInitialized;

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

        unlockInfoTemplates = Resources.LoadAll<UnlockInfo>("ScriptableObjects/UnlockInfo");
        
        foreach (UnlockInfo info in unlockInfoTemplates)
        {
            info.Initialize();
        }

        isInitialized = false;
    }

    public void Initialize(PlayerData playerData)
    {
        if (isInitialized)
        {
            return;
        }
        isInitialized = true;

        this.playerData = playerData;

        // 해금되지 않은 경우만 Action 등록
        foreach (UnlockInfo unlockInfo in unlockInfoTemplates)
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

    public void Unlock(UnlockInfo unlockInfo, string targetName)
    {
        // Unlock
        if (unlockInfo.targetType == UnlockTarget.Item)
        {
            GameManager.instance.AddUnlockedItem(targetName);
        }
        else if (unlockInfo.targetType == UnlockTarget.Deck)
        {
            DataManager.instance.AddUnlockedDeck(targetName);
        }

        SetSubscribe(unlockInfo, false);
        
        GameManager.instance.PlayUnlockAnimation(unlockInfo);
    }

    public void OnDeckUnlockInfoRequested()
    {
        UnlockInfo[] deckUnlockInfoTemplates = unlockInfoTemplates.Where(info => info.targetType == UnlockTarget.Deck).ToArray();
        GameUIManager.instance.OnDeckUnlockInfoRequested(deckUnlockInfoTemplates);
    }

    public void OnUnlockInfoRequested()
    {
        UnlockInfo[] itemUnlockInfoTemplates = unlockInfoTemplates.Where(info => info.targetType == UnlockTarget.Item).ToArray();
        GameUIManager.instance.OnUnlockInfoCallback(itemUnlockInfoTemplates, playerData.GetUnlockedItems());
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
        List<string> currentLockedItems = unlockInfoTemplates.Select(x => x.targetName).ToList();

        foreach (string itemID in playerData.GetUnlockedItems())
        {
            currentLockedItems.Remove(itemID);
        }

        foreach (DeckInfo deckInfo in playerData.GetUnlockedDecks())
        {
            currentLockedItems.Remove(deckInfo.deckName);
        }

        return currentLockedItems;
    }

    private void SetSubscribe(UnlockInfo unlockInfo, bool isSubscribing)
    {
        switch (unlockInfo.trigger)
        {
            case UnlockTrigger.placeCountI:
                if (isSubscribing) onPlaceCountIUpdate += unlockInfo.condition;
                else onPlaceCountIUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.placeCountO:
                if (isSubscribing) onPlaceCountOUpdate += unlockInfo.condition;
                else onPlaceCountOUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.placeCountZ:
                if (isSubscribing) onPlaceCountZUpdate += unlockInfo.condition;
                else onPlaceCountZUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.placeCountS:
                if (isSubscribing) onPlaceCountSUpdate += unlockInfo.condition;
                else onPlaceCountSUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.placeCountJ:
                if (isSubscribing) onPlaceCountJUpdate += unlockInfo.condition;
                else onPlaceCountJUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.placeCountL:
                if (isSubscribing) onPlaceCountLUpdate += unlockInfo.condition;
                else onPlaceCountLUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.placeCountT:
                if (isSubscribing) onPlaceCountTUpdate += unlockInfo.condition;
                else onPlaceCountTUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.RerollCount:
                if (isSubscribing) onRerollCountUpdate += unlockInfo.condition;
                else onRerollCountUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.ItemPurchaseCount:
                if (isSubscribing) onItemPurchaseCountUpdate += unlockInfo.condition;
                else onItemPurchaseCountUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.ShopRerollCount:
                if (isSubscribing) onShopRerollCountUpdate += unlockInfo.condition;
                else onShopRerollCountUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.MaxScore:
                if (isSubscribing) onMaxScoreUpdate += unlockInfo.condition;
                else onMaxScoreUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.MaxChapter:
                if (isSubscribing) onItemPurchaseCountUpdate += unlockInfo.condition;
                else onItemPurchaseCountUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.WinCount:
                if (isSubscribing) onWinCountUpdate += unlockInfo.condition;
                else onWinCountUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.BoardHalfFullCount:
                if (isSubscribing) onBoardHalfFullCountUpdate += unlockInfo.condition;
                else onBoardHalfFullCountUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.RerollCountIO:
                if (isSubscribing) onRerollCountIOUpdate += unlockInfo.condition;
                else onRerollCountIOUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.RerollCountZS:
                if (isSubscribing) onRerollCountZSUpdate += unlockInfo.condition;
                else onRerollCountZSUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.RerollCountJLT:
                if (isSubscribing) onRerollCountJLTUpdate += unlockInfo.condition;
                else onRerollCountJLTUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.MaxBaseMultiplier:
                if (isSubscribing) onMaxBaseMultiplierUpdate += unlockInfo.condition;
                else onMaxBaseMultiplierUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.MaxMultiplier:
                if (isSubscribing) onMaxMultiplierUpdate += unlockInfo.condition;
                else onMaxMultiplierUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.MaxBaseRerollCount:
                if (isSubscribing) onMaxBaseRerollCountUpdate += unlockInfo.condition;
                else onMaxBaseRerollCountUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.MaxGold:
                if (isSubscribing) onMaxGoldUpdate += unlockInfo.condition;
                else onMaxGoldUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.HasOnlyIO:
                if (isSubscribing) onHasOnlyIOUpdate += unlockInfo.condition;
                else onHasOnlyIOUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.HasOnlyZS:
                if (isSubscribing) onHasOnlyZSUpdate += unlockInfo.condition;
                else onHasOnlyZSUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.HasOnlyJL:
                if (isSubscribing) onHasOnlyJLUpdate += unlockInfo.condition;
                else onHasOnlyJLUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.DefaultDeckWinCount:
                if (isSubscribing) onDefaultDeckWinCountUpdate += unlockInfo.condition;
                else onDefaultDeckWinCountUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.YoYoDeckWinCount:
                if (isSubscribing) onYoYoDeckWinCountUpdate += unlockInfo.condition;
                else onYoYoDeckWinCountUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.DiceDeckWinCount:
                if (isSubscribing) onDiceDeckWinCountUpdate += unlockInfo.condition;
                else onDiceDeckWinCountUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.TelescopeDeckWinCount:
                if (isSubscribing) onTelescopeDeckWinCountUpdate += unlockInfo.condition;
                else onTelescopeDeckWinCountUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.MirrorDeckWinCount:
                if (isSubscribing) onMirrorDeckWinCountUpdate += unlockInfo.condition;
                else onMirrorDeckWinCountUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.BombDeckWinCount:
                if (isSubscribing) onBombDeckWinCountUpdate += unlockInfo.condition;
                else onBombDeckWinCountUpdate -= unlockInfo.condition;
                break;
            case UnlockTrigger.ClearedMaxLevel:
                if (isSubscribing) onClearedMaxLevelUpdate += unlockInfo.condition;
                else onClearedMaxLevelUpdate -= unlockInfo.condition;
                break;
        }
    }
}
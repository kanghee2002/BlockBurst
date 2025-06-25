using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [Serializable]
    public struct DeckInfo
    {
        public DeckType deckType;
        public int level;

        public DeckInfo(DeckType deckType, int level)
        {
            this.deckType = deckType;
            this.level = level;
        }
    }

    // 저장되는 데이터
    public List<DeckInfo> unlockedDecks;
    public List<string> unlockedItems;
    public int IOplaceCount;
    public int ZSplaceCount;
    public int JLTplaceCount;
    public int rerollCount;
    public int itemPurchaseCount;
    public int maxScore;
    public bool hasWon;

    private const int MAX_LEVEL = 5;

    public PlayerData()
    {
        unlockedDecks = new List<DeckInfo>();
        unlockedItems = new List<string>();
        IOplaceCount = 0;
        ZSplaceCount = 0;
        JLTplaceCount = 0;
        rerollCount = 0;
        itemPurchaseCount = 0;
        maxScore = 0;
        hasWon = false;
    }

    // 덱 해금 추가
    public void AddUnlockedDeck(DeckType deckType)
    {
        int index = unlockedDecks.FindIndex(x => x.deckType == deckType);

        if (index != -1)
        {
            Debug.LogError("해금하려는 덱이 이미 존재함: " + deckType); ;
            return;
        }

        unlockedDecks.Add(new DeckInfo(deckType, 1));
    }

    // 덱 레벨 업데이트
    public void UpdateDeckLevel(DeckType deckType)
    {
        int index = unlockedDecks.FindIndex(x => x.deckType == deckType);

        if (index == -1)
        {
            Debug.LogError("레벨 업데이트하려는 덱이 존재하지 않음" + deckType);
            return;
        }

        DeckInfo deckinfo = unlockedDecks[index];
        
        if (deckinfo.level < MAX_LEVEL)
        {
            deckinfo.level++;
        }
    }

    // 아이템 해금 추가
    public void AddUnlockedItem(string itemID)
    {
        unlockedItems.Add(itemID);
    }

    // 데이터 업데이트
    public void UpdateBlockPlaceCount(Block block)
    {
        if (block.Type == BlockType.I || block.Type == BlockType.O)
        {
            IOplaceCount++;
            UnlockManager.instance.onIOplaceCountUpdate?.Invoke(IOplaceCount);
        }
        else if (block.Type == BlockType.Z || block.Type == BlockType.S)
        {
            ZSplaceCount++;
            UnlockManager.instance.onZSplaceCountUpdate?.Invoke(ZSplaceCount);
        }
        else if (block.Type == BlockType.J || block.Type == BlockType.L || block.Type == BlockType.T)
        {
            JLTplaceCount++;
            UnlockManager.instance.onJLTplaceCountUpdate?.Invoke(JLTplaceCount);
        }
    }

    public void UpdateRerollCount()
    {
        rerollCount++;
        UnlockManager.instance.onRerollCountUpdate?.Invoke(rerollCount);
    }
    public void UpdateItemPurchaseCount()
    {
        itemPurchaseCount++;
        UnlockManager.instance.onItemPurchaseCountUpdate?.Invoke(itemPurchaseCount);
    }
    public void UpdateMaxScore(int score)
    {
        maxScore = score;
        UnlockManager.instance.onMaxScoreUpdate?.Invoke(maxScore);
    }
    public void UpdateHasWon(bool value)
    {
        if (hasWon != value)
        {
            UnlockManager.instance.onHasWonUpdate?.Invoke(value);
        }
        hasWon = value;
    }
}

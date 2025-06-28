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
        public string deckName;
        public int level;

        public const int MAX_LEVEL = 5;

        public DeckInfo(DeckType deckType, int level)
        {
            this.deckType = deckType;
            this.deckName = deckType.ToString();
            this.level = level;
        }
    }

    // 저장되는 데이터
    public List<DeckInfo> unlockedDecks;
    public List<string> unlockedItems;
    public int IplaceCount;
    public int OplaceCount;
    public int ZplaceCount;
    public int SplaceCount;
    public int JplaceCount;
    public int LplaceCount;
    public int TplaceCount;
    public int rerollCount;
    public int itemPurchaseCount;
    public int maxScore;
    public int maxChapter;
    public int maxStage;
    public int boardHalfFullCount;
    public int IOrerollCount;
    public int ZSrerollCount;
    public int JLTrerollCount;
    public int maxBaseMultiplier;
    public int maxMultiplier;
    public int maxBaseRerollCount;
    public int maxGold;
    public bool hasWon;
    public bool hasOnlyIO;
    public bool hasOnlyZS;
    public bool hasOnlyJLT;

    public PlayerData()
    {
        unlockedDecks = new List<DeckInfo>();
        unlockedItems = new List<string>();
        IplaceCount = 0;
        OplaceCount = 0;
        ZplaceCount = 0;
        SplaceCount = 0;
        JplaceCount = 0;
        LplaceCount = 0;
        TplaceCount = 0;
        rerollCount = 0;
        itemPurchaseCount = 0;
        maxScore = 0;
        maxChapter = 0;
        maxStage = 0;
        boardHalfFullCount = 0;
        IOrerollCount = 0;
        ZSrerollCount = 0;
        JLTrerollCount = 0;
        maxBaseMultiplier = 0;
        maxMultiplier = 0;
        maxBaseRerollCount = 0;
        maxGold = 0;
        hasWon = false;
    }

    // 덱 해금 추가
    public void AddUnlockedDeck(string deckName)
    {
        int index = unlockedDecks.FindIndex(x => x.deckName == deckName);

        if (index != -1)
        {
            Debug.LogError("해금하려는 덱이 이미 존재함: " + deckName); ;
            return;
        }

        DeckType deckType = Enums.GetEnumByString<DeckType>(deckName);
        DeckInfo deckInfo = new DeckInfo(deckType, 1);
        unlockedDecks.Add(deckInfo);
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

        DeckInfo deckInfo = unlockedDecks[index];
        
        if (deckInfo.level < DeckInfo.MAX_LEVEL)
        {
            deckInfo.level++;
        }
    }

    public bool IsDeckUnlocked(string deckName)
    {
        if (unlockedDecks.FindIndex(deck => deck.deckName == deckName) == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool IsItemUnlocked(string itemName)
    {
        return unlockedItems.Contains(itemName);
    }

    // 아이템 해금 추가
    public void AddUnlockedItem(string itemID)
    {
        unlockedItems.Add(itemID);
    }

    // 데이터 업데이트
    public void UpdateBlockPlaceCount(Block block)
    {
        if (block.Type == BlockType.I)
        {
            IplaceCount++;
            UnlockManager.instance.onIplaceCountUpdate?.Invoke(IplaceCount);
        }
        else if (block.Type == BlockType.O)
        {
            OplaceCount++;
            UnlockManager.instance.onOplaceCountUpdate?.Invoke(OplaceCount);
        }
        else if (block.Type == BlockType.Z)
        {
            ZplaceCount++;
            UnlockManager.instance.onZplaceCountUpdate?.Invoke(ZplaceCount);
        }
        else if (block.Type == BlockType.S)
        {
            SplaceCount++;
            UnlockManager.instance.onSplaceCountUpdate?.Invoke(SplaceCount);
        }
        else if (block.Type == BlockType.J)
        {
            JplaceCount++;
            UnlockManager.instance.onJplaceCountUpdate?.Invoke(JplaceCount);
        }
        else if (block.Type == BlockType.L)
        {
            LplaceCount++;
            UnlockManager.instance.onLplaceCountUpdate?.Invoke(LplaceCount);
        }
        else if (block.Type == BlockType.T)
        {
            TplaceCount++;
            UnlockManager.instance.onTplaceCountUpdate?.Invoke(TplaceCount);
        }
    }

    public void UpdateRerollCount()
    {
        rerollCount++;

        if (UnlockManager.instance.onRerollCountUpdate == null)
        {
            Debug.Log("NULL!");
        }

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
    public void UpdateMaxChapterStage(int chapter, int stage)
    {
        maxChapter = chapter;
        maxStage = stage;
        UnlockManager.instance.onMaxChapterUpdate?.Invoke(chapter);
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

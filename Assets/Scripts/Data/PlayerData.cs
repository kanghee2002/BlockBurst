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
    public int placeCountI;
    public int placeCountO;
    public int placeCountZ;
    public int placeCountS;
    public int placeCountJ;
    public int placeCountL;
    public int placeCountT;
    public int rerollCount;
    public int itemPurchaseCount;
    public int maxScore;
    public int maxChapter;
    public int maxStage;
    public int winCount;
    public int boardHalfFullCount;
    public int rerollCountIO;
    public int rerollCountZS;
    public int rerollCountJLT;
    public int maxBaseMultiplier;
    public int maxMultiplier;
    public int maxBaseRerollCount;
    public int maxGold;
    public int hasOnlyIO;
    public int hasOnlyZS;
    public int hasOnlyJLT;

    public PlayerData()
    {
        unlockedDecks = new List<DeckInfo>();
        unlockedItems = new List<string>();
        placeCountI = 0;
        placeCountO = 0;
        placeCountZ = 0;
        placeCountS = 0;
        placeCountJ = 0;
        placeCountL = 0;
        placeCountT = 0;
        rerollCount = 0;
        itemPurchaseCount = 0;
        maxScore = 0;
        maxChapter = 0;
        maxStage = 0;
        winCount = 0;
        boardHalfFullCount = 0;
        rerollCountIO = 0;
        rerollCountZS = 0;
        rerollCountJLT = 0;
        maxBaseMultiplier = 0;
        maxMultiplier = 0;
        maxBaseRerollCount = 0;
        maxGold = 0;
        hasOnlyIO = 0;
        hasOnlyZS = 0;
        hasOnlyJLT = 0;
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

    // 아이템 해금 추가
    public void AddUnlockedItem(string itemID)
    {
        unlockedItems.Add(itemID);
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

    // 데이터 업데이트
    public void UpdateBlockPlaceCount(Block block)
    {
        if (block.Type == BlockType.I)
        {
            placeCountI++;
            UnlockManager.instance.onPlaceCountIUpdate?.Invoke(placeCountI);
        }
        else if (block.Type == BlockType.O)
        {
            placeCountO++;
            UnlockManager.instance.onPlaceCountOUpdate?.Invoke(placeCountO);
        }
        else if (block.Type == BlockType.Z)
        {
            placeCountZ++;
            UnlockManager.instance.onPlaceCountZUpdate?.Invoke(placeCountZ);
        }
        else if (block.Type == BlockType.S)
        {
            placeCountS++;
            UnlockManager.instance.onPlaceCountSUpdate?.Invoke(placeCountS);
        }
        else if (block.Type == BlockType.J)
        {
            placeCountJ++;
            UnlockManager.instance.onPlaceCountJUpdate?.Invoke(placeCountJ);
        }
        else if (block.Type == BlockType.L)
        {
            placeCountL++;
            UnlockManager.instance.onPlaceCountLUpdate?.Invoke(placeCountL);
        }
        else if (block.Type == BlockType.T)
        {
            placeCountT++;
            UnlockManager.instance.onPlaceCountTUpdate?.Invoke(placeCountT);
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
    public void UpdateMaxChapterStage(int chapter, int stage)
    {
        maxChapter = chapter;
        maxStage = stage;
        UnlockManager.instance.onMaxChapterUpdate?.Invoke(chapter);
    }
    public void UpdateWinCount(int value)
    {
        winCount++;
        UnlockManager.instance.onWinCountUpdate?.Invoke(winCount);
    }
    public void UpdateBoardHalfFullCount()
    {
        boardHalfFullCount++;
        UnlockManager.instance.onBoardHalfFullCountUpdate?.Invoke(boardHalfFullCount);
    }
}

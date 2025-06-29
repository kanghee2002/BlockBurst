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
    public int rerollCountIO;
    public int rerollCountZS;
    public int rerollCountJLT;
    public int maxBaseMultiplier;
    public int maxMultiplier;
    public int maxBaseRerollCount;
    public int maxGold;
    public int hasOnlyIO;
    public int hasOnlyZS;
    public int hasOnlyJL;

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
        rerollCountIO = 0;
        rerollCountZS = 0;
        rerollCountJLT = 0;
        maxBaseMultiplier = 0;
        maxMultiplier = 0;
        maxBaseRerollCount = 0;
        maxGold = 0;
        hasOnlyIO = 0;
        hasOnlyZS = 0;
        hasOnlyJL = 0;
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
        switch (block.Type)
        {
            case BlockType.I:
                placeCountI++;
                UnlockManager.instance.onPlaceCountIUpdate?.Invoke(placeCountI);
                break;
            case BlockType.O:
                placeCountO++;
                UnlockManager.instance.onPlaceCountOUpdate?.Invoke(placeCountO);
                break;
            case BlockType.Z:
                placeCountZ++;
                UnlockManager.instance.onPlaceCountZUpdate?.Invoke(placeCountZ);
                break;
            case BlockType.S:
                placeCountS++;
                UnlockManager.instance.onPlaceCountSUpdate?.Invoke(placeCountS);
                break;
            case BlockType.J:
                placeCountJ++;
                UnlockManager.instance.onPlaceCountJUpdate?.Invoke(placeCountJ);
                break;
            case BlockType.L:
                placeCountL++;
                UnlockManager.instance.onPlaceCountLUpdate?.Invoke(placeCountL);
                break;
            case BlockType.T:
                placeCountT++;
                UnlockManager.instance.onPlaceCountTUpdate?.Invoke(placeCountT);
                break;
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
    public void UpdateWinCount()
    {
        winCount++;
        UnlockManager.instance.onWinCountUpdate?.Invoke(winCount);
    }
    public void UpdateBlockRerollCount(BlockData block)
    {
        switch (block.type)
        {
            case BlockType.I:
            case BlockType.O:
                rerollCountIO++;
                UnlockManager.instance.onRerollCountIOUpdate?.Invoke(rerollCountIO);
                break;
            case BlockType.Z:
            case BlockType.S:
                rerollCountZS++;
                UnlockManager.instance.onRerollCountZSUpdate?.Invoke(rerollCountZS);
                break;
            case BlockType.J:
            case BlockType.L:
            case BlockType.T:
                rerollCountJLT++;
                UnlockManager.instance.onRerollCountJLTUpdate?.Invoke(rerollCountJLT);
                break;
        }
    }
    public void UpdateMaxBaseMultiplier(int value)
    {
        maxBaseMultiplier = value;
        UnlockManager.instance.onMaxBaseMultiplierUpdate?.Invoke(maxBaseMultiplier);
    }
    public void UpdateMaxMultiplier(int value)
    {
        maxMultiplier = value;
        UnlockManager.instance.onMaxMultiplierUpdate?.Invoke(maxMultiplier);
    }
    public void UpdateMaxBaseRerollCount(int value)
    {
        maxBaseRerollCount = value;
        UnlockManager.instance.onMaxBaseRerollCountUpdate?.Invoke(maxBaseRerollCount);
    }
    public void UpdateMaxGold(int value)
    {
        maxGold = value;
        UnlockManager.instance.onMaxGoldUpdate?.Invoke(maxGold);
    }
    public void UpdateHasOnlyIO()
    {
        hasOnlyIO++;
        UnlockManager.instance.onHasOnlyIOUpdate?.Invoke(hasOnlyIO);
    }
    public void UpdateHasOnlyZS()
    {
        hasOnlyZS++;
        UnlockManager.instance.onHasOnlyZSUpdate?.Invoke(hasOnlyZS);
    }
    public void UpdateHasOnlyJL()
    {
        hasOnlyJL++;
        UnlockManager.instance.onHasOnlyJLUpdate?.Invoke(hasOnlyJL);
    }
}

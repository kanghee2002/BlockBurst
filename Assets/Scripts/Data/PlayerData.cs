using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DeckInfo
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

[Serializable]
public class PlayerData
{

    // 저장되는 데이터
    public bool tutorialValue;
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
    public int shopRerollCount;
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
    public int defaultDeckWinCount;
    public int yoyoDeckWinCount;
    public int diceDeckWinCount;
    public int telescopeDeckWinCount;
    public int mirrorDeckWinCount;
    public int bombDeckWinCount;
    public int clearedMaxLevel;

    public PlayerData()
    {
        tutorialValue = true;
        DeckInfo defaultDeck = new DeckInfo(DeckType.Default, 0);
        unlockedDecks = new List<DeckInfo>() { defaultDeck };
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
        shopRerollCount = 0;
        maxScore = 0;
        maxChapter = 1;
        maxStage = 1;
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
        defaultDeckWinCount = 0;
        yoyoDeckWinCount = 0;
        diceDeckWinCount = 0;
        telescopeDeckWinCount = 0;
        mirrorDeckWinCount = 0;
        bombDeckWinCount = 0;
        clearedMaxLevel = -1;
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

    public List<DeckInfo> GetUnlockedDecks()
    {
        return unlockedDecks.ToList();
    }

    public List<string> GetUnlockedItems()
    {
        return unlockedItems.ToList();
    }
}

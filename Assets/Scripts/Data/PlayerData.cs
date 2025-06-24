using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    // 저장되는 데이터
    public List<DeckType> unlockedDecks;
    public List<string> unlockedItems;
    public int IOplaceCount;
    public int ZSplaceCount;
    public int JLplaceCount;
    public int rerollCount;
    public int itemPurchaseCount;
    public int maxScore;

    public PlayerData()
    {
        unlockedDecks = new List<DeckType>();
        unlockedItems = new List<string>();
        IOplaceCount = 0;
        ZSplaceCount = 0;
        JLplaceCount = 0;
        rerollCount = 0;
        itemPurchaseCount = 0;
        maxScore = 0;
    }

    // 데이터 업데이트
    public void UpdateBlockPlaceCount(Block block)
    {
        if (block.Type == BlockType.I || block.Type == BlockType.O)
        {
            IOplaceCount++;
        }
        else if (block.Type == BlockType.Z || block.Type == BlockType.S)
        {
            ZSplaceCount++;
        }
        else if (block.Type == BlockType.J || block.Type == BlockType.L)
        {
            JLplaceCount++;
        }
    }

    public void UpdateRerollCount() => rerollCount++;
    public void UpdateItemPurchaseCount() => itemPurchaseCount++;
    public void UpdateMaxScore(int score) => maxScore = score;

    // 해금 확인
    private void CheckUnlock()
    {

    }
}

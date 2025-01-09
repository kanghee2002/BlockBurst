using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    private List<BlockData> discardPile;
    private RunData runData;
    private BlockGameData blockGameData;

    public void Initialize(ref RunData data, ref BlockGameData blockData)
    {
        runData = data;
        blockGameData = blockData;

        discardPile = new List<BlockData>();
    }

    public BlockData DrawBlock()
    {
        BlockData block = null;
        if (blockGameData.deck.Count == 0)
        {
            Debug.Log("Deck is empty");
        }
        else
        {
            block = blockGameData.deck[0];
            blockGameData.deck.RemoveAt(0);
        }

        return block;
    }

    private void ShuffleDeck()
    {
        // deck을 랜덤 셔플
        for (int i = 0; i < blockGameData.deck.Count; i++)
        {
            int randomIndex = Random.Range(i, blockGameData.deck.Count);
            BlockData temp = blockGameData.deck[i];
            blockGameData.deck[i] = blockGameData.deck[randomIndex];
            blockGameData.deck[randomIndex] = temp;
        }

    }

    public bool RerollDeck(BlockData[] remains)
    {
        if (blockGameData.rerollCount <= 0)
        {
            return false;
        }
        else 
        {
            blockGameData.rerollCount--;
            foreach (BlockData block in blockGameData.deck)
            {
                AddBlock(block);
            }
            return true;
        }
    }

    public void ProcessBlockReuse(BlockData block)
    {
        if (block.reuseCount > 0)
        {
            block.reuseCount--;
            AddBlock(block);
        }
    }

    // 랜덤한 위치에 추가
    public void AddBlock(BlockData block)
    {
        blockGameData.deck.Insert(Random.Range(0, blockGameData.deck.Count), block);
    }

    public void RemoveBlock(BlockData block)
    {
        discardPile.Add(block);
        blockGameData.deck.Remove(block);
    }
}
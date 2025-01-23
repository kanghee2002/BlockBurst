using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    private List<BlockData> discardPile;
    private RunData runData;
    private BlockGameData blockGameData;

    public void Initialize(ref BlockGameData blockData, ref RunData runData)
    {
        this.runData = runData;
        blockGameData = blockData;

        foreach (BlockData block in runData.availableBlocks)
        {
            AddBlockToBlockGameDeck(block);
        }

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

    public bool RerollDeck(List<BlockData> remains)
    {
        if (blockGameData.rerollCount <= 0)
        {
            return false;
        }
        else 
        {
            blockGameData.rerollCount--;
            foreach (BlockData block in remains)
            {
                if (block) AddBlockToBlockGameDeck(block);
            }
            return true;
        }
    }

    public void ProcessBlockReuse(BlockData block)
    {
        if (block.reuseCount > 0)
        {
            block.reuseCount--;
            AddBlockToBlockGameDeck(block);
        }
    }

    // 랜덤한 위치에 추가
    public void AddBlockToBlockGameDeck(BlockData block)
    {
        blockGameData.deck.Insert(Random.Range(0, blockGameData.deck.Count), block);
    }

    public void AddBlockToRunDeck(BlockData block)
    {
        runData.availableBlocks.Add(block);
    }

    public void RemoveBlockFromGameDeck(BlockData block)
    {
        discardPile.Add(block);
        blockGameData.deck.Remove(block);
    }

    public void RemoveBlockFromRunDeck(BlockData block)
    {
        runData.availableBlocks.Remove(block);
    }

    // 덱 셔플
    private void ShuffleDeck()
    {
        for (int i = 0; i < blockGameData.deck.Count; i++)
        {
            int j = Random.Range(i, blockGameData.deck.Count);
            BlockData tmp = blockGameData.deck[i];
            blockGameData.deck[i] = blockGameData.deck[j];
            blockGameData.deck[j] = tmp;
        }
    }
}
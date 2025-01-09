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
    /*
    private List<Block> InstantiateBlocks(List<BlockData> blockData)
    {
        List<Block> blocks = new List<Block>();

        foreach (BlockData data in blockData)
        {
            GameObject blockObject = Instantiate(data.prefab);
            Block block = blockObject.GetComponent<Block>();
            BlockType blockType = data.type;
            block.Initialize(data, idCount, runData.blockReuses[blockType]);
            idCount++;
            blocks.Add(block);

            // TEST
            blockObject.transform.position = new Vector3(10, 10);
        }

        return blocks;
    }
    */

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
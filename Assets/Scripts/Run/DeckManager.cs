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
            int idx = Random.Range(0, blockGameData.deck.Count);
            block = blockGameData.deck[idx];
            blockGameData.deck.RemoveAt(idx);

            if (blockGameData.deck.Count == 0 && !blockGameData.isDeckEmpty)
            {
                EffectManager.instance.TriggerEffects(TriggerType.ON_DECK_EMPTY);
                blockGameData.isDeckEmpty = true;
            }
        }

        EffectManager.instance.EndTriggerEffect();

        return block;
    }

    public bool RerollDeck(List<BlockData> remains)
    {
        if (blockGameData.rerollCount <= 0)
        {
            AnimationData animationData = new AnimationData();
            animationData.animationType = AnimationType.UpdateReroll;
            animationData.value = 0;
            animationData.isValueAdditive = true;

            AnimationManager.instance.ProcessAnimation(animationData);

            return false;
        }
        else
        {
            blockGameData.rerollCount--;

            AnimationData animationData = new AnimationData();
            animationData.animationType = AnimationType.UpdateReroll;
            animationData.value = -1;
            animationData.isValueAdditive = true;

            AnimationManager.instance.ProcessAnimation(animationData);

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

    // append
    public void AddBlockToBlockGameDeck(BlockData block)
    {
        blockGameData.deck.Add(block);
    }

    public void AddBlockToRunDeck(BlockData block)
    {
        runData.availableBlocks.Add(block);
    }

    // BlockGame
    public void RemoveBlockFromGameDeck(BlockData block)
    {
        discardPile.Add(block);
        blockGameData.deck.Remove(block);
    }

    public void RemoveBlockFromGameDeck(BlockType blockType)
    {
        blockGameData.deck.Remove(blockGameData.deck.Find(block => block.type == blockType));
    }

    // Run
    public void RemoveBlockFromRunDeck(BlockData block)
    {
        runData.availableBlocks.Remove(block);

        DataManager.instance.UpdateHasOnlySpecificBlock(runData.availableBlocks);
    }

    public void RemoveBlockFromRunDeck(BlockType blockType)
    {
        runData.availableBlocks.Remove(runData.availableBlocks.Find(block => block.type == blockType));

        DataManager.instance.UpdateHasOnlySpecificBlock(runData.availableBlocks);
    }

    public bool RemoveRandomBlockFromRunDeck(List<BlockType> exceptionType, bool excludeSpecialBlocks)
    {
        List<BlockData> blocks = runData.availableBlocks.ToList();

        if (exceptionType != null)
        {
            blocks.RemoveAll(x => exceptionType.Contains(x.type));
        }

        if (excludeSpecialBlocks)
        {
            blocks.RemoveAll(x => Enums.IsSpecialBlockType(x.type));
        }

        if (blocks.Count == 0 ) return false;

        int randomIndex = Random.Range(0, blocks.Count);
        BlockType randomType = blocks[randomIndex].type;
        int removingIndex = runData.availableBlocks.FindIndex(x => x.type == randomType);

        if (removingIndex == -1) return false;

        runData.availableBlocks.RemoveAt(removingIndex);

        DataManager.instance.UpdateHasOnlySpecificBlock(runData.availableBlocks);

        return true;
    }

    public void ConvertBlock(int count)
    {
        List<BlockType> highestScoreBlocks = runData.baseBlockScores
                        .Where(kv => kv.Value == runData.baseBlockScores.Values.Max())
                        .Select(kv => kv.Key)
                        .ToList();

        int removeCount = 0;
        for (int i = 0; i < count; i++)
        {
            if (RemoveRandomBlockFromRunDeck(highestScoreBlocks, true))
            {
                removeCount++;
            }

        }

        BlockType convertBlockType = highestScoreBlocks[Random.Range(0, highestScoreBlocks.Count)];
        BlockData convertBlock = Resources.Load<BlockData>("ScriptableObjects/Block/Block" + convertBlockType + "Data");

        for (int i = 0; i < removeCount; i++)
        {
            AddBlockToRunDeck(convertBlock);
        }
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
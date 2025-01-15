using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance = null;
    private RunData runData;
    private BlockGameData blockGameData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(ref RunData data)
    {
        runData = data;
    }

    public void InitializeBlockGameData(ref BlockGameData data)
    {
        blockGameData = data;
    }

    public void AddEffect(EffectData effect)
    {
        runData.activeEffects.Add(effect);
        if (effect.trigger == TriggerType.ON_ACQUIRE)
        {
            ApplyEffect(effect);
        }
    }

    public bool RemoveEffect(EffectData effect)
    {
        return runData.activeEffects.Remove(effect);
    }

    public bool ContainsEffect(EffectData effect)
    {
        return runData.activeEffects.Contains(effect);
    }

    public void TriggerEffects(TriggerType trigger, int triggerValue = 0, BlockType[] blockTypes = null, int blockId = -1) 
    {
        foreach (EffectData effect in runData.activeEffects)
        {
            if (effect.trigger == trigger && IsIncluded(effect.blockTypes, blockTypes) &&
                effect.blockId == blockId && effect.triggerValue == triggerValue)
            {
                ApplyEffect(effect, blockId);
            }
        }
    }

    public void ApplyEffect(EffectData effect, int blockId = -1)
    {
        MatchType matchType = MatchType.ROW;

        // 확률 적용
        if (effect.probability != 1)
        {
            if (!CheckProbability(effect.probability))
            {
                return;
            }
        }

        // 효과 적용
        switch (effect.type)
        {
            case EffectType.SCORE_MODIFIER:
                foreach (BlockType blockType in effect.blockTypes)
                {
                    runData.baseBlockScores[blockType] += effect.effectValue;
                    blockGameData.blockScores[blockType] += effect.effectValue;
                }
                break;
            case EffectType.SCORE_MULTIPLIER:
                foreach (BlockType blockType in effect.blockTypes)
                {
                    //runData.baseBlockScores[blockType] += effect.effectValue;
                    blockGameData.blockScores[blockType] *= effect.effectValue;
                }
                break;
            case EffectType.MULTIPLIER_MODIFIER:
                blockGameData.matchMultipliers[matchType] += effect.effectValue;
                break;
            case EffectType.BASEMULTIPLIER_MODIFIER:
                runData.baseMatchMultipliers[matchType] += effect.effectValue;
                blockGameData.matchMultipliers[matchType] += effect.effectValue;
                break;
            case EffectType.BASEMULTIPLIER_MULTIPLIER:
                runData.baseMatchMultipliers[matchType] *= effect.effectValue;
                blockGameData.matchMultipliers[matchType] *= effect.effectValue;
                break;
            case EffectType.REROLL_MODIFIER:
                blockGameData.rerollCount += effect.effectValue;
                if (blockGameData.rerollCount < 0) blockGameData.rerollCount = 0;
                break;
            case EffectType.BASEREROLL_MODIFIER:            
                runData.baseRerollCount += effect.effectValue;
                blockGameData.rerollCount += effect.effectValue;
                if (runData.baseRerollCount < 0) runData.baseRerollCount = 0;
                if (blockGameData.rerollCount < 0) blockGameData.rerollCount = 0;
                break;
            case EffectType.BASEREROLL_MULTIPLIER:
                runData.baseRerollCount *= effect.effectValue;
                blockGameData.rerollCount *= effect.effectValue;
                break;
            case EffectType.GOLD_MODIFIER:
                runData.gold += effect.effectValue;
                if (runData.gold < 0) runData.gold = 0;
                break;
            case EffectType.GOLD_MULTIPLIER:
                runData.gold *= effect.effectValue;
                break;
            case EffectType.BOARD_SIZE_MODIFIER:
                blockGameData.boardRows += effect.effectValue;
                blockGameData.boardColumns += effect.effectValue;
                break;
            case EffectType.BOARD_CORNER_BLOCK:
                blockGameData.isCornerBlocked = true;
                break;
            case EffectType.BOARD_RANDOM_BLOCK:
                blockGameData.inactiveBlockCells = GetRandomBlockCells(blockGameData.boardRows, blockGameData.boardColumns, effect.effectValue);
                break;
            case EffectType.DECK_MODIFIER:
                // TODO
                break;
            case EffectType.BLOCK_REUSE_MODIFIER:
                // TODO
                break;
            case EffectType.SQUARE_CLEAR:
                // TODO
                break; 
            case EffectType.BLOCK_MULTIPLIER:
                foreach (BlockType blockType in effect.blockTypes)
                {
                    MultiplyBlock(blockType, effect.effectValue);
                }
                break;
            case EffectType.BLOCK_DELETE:
                foreach (BlockType blockType in effect.blockTypes)
                {
                    runData.availableBlocks.RemoveAll(data => data.type == blockType);
                }
                break;
            case EffectType.BLOCK_DELETE_WITH_COUNT:
                break;
            case EffectType.ROW_LINE_CLEAR:
                // TODO
                break;
            case EffectType.COLUMN_LINE_CLEAR:
                // TODO
                break;
            case EffectType.DRAW_BLOCK_COUNT_MODIFIER:
                blockGameData.drawBlockCount = effect.effectValue;
                break;
            default:
                break;
        }
    }

    private bool IsIncluded(BlockType[] arr1, BlockType[] arr2)
    {
        return arr1.All(x => arr2.Contains(x));
    }

    private bool CheckProbability(float probability)
    {
        return Random.Range(0, 1f) <= probability;
    }

    private HashSet<Vector2Int> GetRandomBlockCells(int boardRows, int boardColumns, int count)
    {
        HashSet<Vector2Int> result = new HashSet<Vector2Int>();

        for (int i = 0; i < 10000; i++)
        {
            if (result.Count == count)
            {
                break;
            }
            int x = Random.Range(0, boardRows);
            int y = Random.Range(0, boardColumns);
            result.Add(new Vector2Int(x, y));
        }
        return result;
    }

    private void MultiplyBlock(BlockType blockType, int multiplier)
    {
        int count = runData.availableBlocks.Count(blockData => blockData.type == blockType);

        BlockData block = runData.availableBlocks.Find(data => data.type == blockType);

        for (int i = 0; i <  count * (multiplier - 1); i++)
        {
            runData.availableBlocks.Add(block.Clone());
        }
    }
}
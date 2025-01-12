using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    }

    public bool RemoveEffect(EffectData effect)
    {
        return runData.activeEffects.Remove(effect);
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

        if (effect.probability != 1)
        {
            // 랜덤 적용
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
            case EffectType.MULTIPLIER_MODIFIER:
                blockGameData.matchMultipliers[matchType] += effect.effectValue;
                break;
            case EffectType.BASEMULTIPLIER_MODIFIER:
                runData.baseMatchMultipliers[matchType] += effect.effectValue;
                blockGameData.baseMatchMultiplier[matchType] += effect.effectValue;
                blockGameData.matchMultipliers[matchType] += effect.effectValue;
                break;
            case EffectType.BASEMULTIPLIER_MULTIPLIER:
                runData.baseMatchMultipliers[matchType] *= effect.effectValue;
                blockGameData.baseMatchMultiplier[matchType] *= effect.effectValue;
                blockGameData.matchMultipliers[matchType] *= effect.effectValue;
                break;
            case EffectType.REROLL_MODIFIER:
                blockGameData.rerollCount += effect.effectValue;
                break;
            case EffectType.BASEREROLL_MODIFIER:            
                runData.currentRerollCount += effect.effectValue;
                blockGameData.rerollCount += effect.effectValue;
                break;
            case EffectType.BASEREROLL_MULTIPLIER:
                runData.currentRerollCount *= effect.effectValue;
                blockGameData.rerollCount *= effect.effectValue;
                break;
            case EffectType.GOLD_MODIFIER:
                runData.gold += effect.effectValue;
                break;
            case EffectType.GOLD_MULTIPLIER:
                runData.gold *= effect.effectValue;
                break;
            case EffectType.BOARD_SIZE_MODIFIER:
                runData.boardSize += effect.effectValue;
                break;
            case EffectType.BOARD_CORNER:
                // TODO
                break;
            case EffectType.BOARD_RANDOM_BLOCK:
                // TODO
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
                // TODO
                break;
            case EffectType.BLOCK_DELETE:
                // TODO
                break;
            case EffectType.ROW_LINE_CLEAR:
                // TODO
                break;
            case EffectType.COLUMN_LINE_CLEAR:
                // TODO
                break;
            case EffectType.DRAW_BLOCK_COUNT_MODIFIER:
                // TODO
                break;
            default:
                break;
        }
    }

    private bool IsIncluded(BlockType[] arr1, BlockType[] arr2)
    {
        return arr1.All(x => arr2.Contains(x));
    }
}
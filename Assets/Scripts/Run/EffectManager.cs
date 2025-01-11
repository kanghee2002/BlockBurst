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

    public void AddEffect(EffectData effect)
    {
        runData.activeEffects.Add(effect);
    }

    public bool RemoveEffect(EffectData effect)
    {
        return runData.activeEffects.Remove(effect);
    }

    public void TriggerEffects(TriggerType trigger, BlockType[] blockTypes = null, int blockId = -1, int triggerValue = 0) 
    {
        foreach (EffectData effect in runData.activeEffects)
        {
            if (effect.trigger == trigger)
            {
                ApplyEffect(effect);
            }
        }
    }

    public void ApplyEffect(EffectData effect)
    {
        // 임시 변수 선언
        BlockType blockType;
        MatchType matchType;
        BlockData blockData;
        /*
        // 효과 적용
        switch (effect.type)
        {
            case EffectType.SCORE_MODIFIER:
                blockType = (BlockType)effect.parameters[0];
                blockGameData.blockScores[blockType] += effect.value;
                break;
            case EffectType.MULTIPLIER_MODIFIER:
                matchType = (MatchType)effect.parameters[0];
                blockGameData.matchMultipliers[matchType] += effect.value;
                break;
            case EffectType.BASEMULTIPLIER_MODIFIER:
                matchType = (MatchType)effect.parameters[0];
                runData.baseMatchMultipliers[matchType] += effect.value;
                break;
            case EffectType.BASEMULTIPLIER_MULTIPLIER:
                matchType = (MatchType)effect.parameters[0];
                runData.baseMatchMultipliers[matchType] *= effect.value;
                break;
            case EffectType.REROLL_MODIFIER:
                blockGameData.rerollCount += effect.value;
                break;
            case EffectType.BASEREROLL_MODIFIER:            
                runData.currentRerollCount += effect.value;
                break;
            case EffectType.BASEREROLL_MULTIPLIER:
                runData.currentRerollCount *= effect.value;
                break;
            case EffectType.GOLD_MODIFIER:
                runData.gold += effect.value;
                break;
            case EffectType.GOLD_MULTIPLIER:
                runData.gold *= effect.value;
                break;
            case EffectType.BOARD_SIZE_MODIFIER:
                runData.boardSize += effect.value;
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
                blockData = (BlockData)effect.parameters[0];
                blockData.reuseCount += effect.value;
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
        */
    }
}
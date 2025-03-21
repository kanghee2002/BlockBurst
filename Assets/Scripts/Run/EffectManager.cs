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

    private List<string> lastTriggeredEffects = new List<string>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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

    // 모든 트리거 호출 이후, 시각 효과를 위해 호출
    public void EndTriggerEffect()
    {
        if (IsStageEffectTriggered(lastTriggeredEffects))
        {
            GameManager.instance.PlayStageEffectAnimation();
        }
        GameManager.instance.PlayItemEffectAnimation(lastTriggeredEffects.ToList());
        lastTriggeredEffects.Clear();
    }

    // 게임 중 블록을 놓았을 때, 시각 효과를 위해 호출
    public void EndTriggerEffectOnPlace(List<Match> matches)
    {
        if (IsStageEffectTriggered(lastTriggeredEffects))
        {
            GameManager.instance.PlayStageEffectAnimation();
        }
        float matchAnimationTime = GameManager.instance.GetMatchAnimationTime(matches);
        GameManager.instance.PlayItemEffectAnimation(lastTriggeredEffects.ToList(), matchAnimationTime);
        lastTriggeredEffects.Clear();
    }

    public void TriggerEffects(TriggerType trigger, int triggerValue = 0, BlockType[] blockTypes = null) 
    {
        foreach (EffectData effect in runData.activeEffects)
        {
            if (effect.trigger == trigger && IsIncluded(blockTypes, effect.blockTypes))
            {
                if (!CheckTriggerCount(effect, triggerValue))
                {
                    continue;
                }

                ApplyEffect(effect);
            }
        }
    }

    private bool CheckTriggerCount(EffectData effect, int triggerValue)
    {
        if (effect.triggerMode == TriggerMode.None)
        {
            return true;
        }

        if (effect.triggerMode == TriggerMode.Exact)
        {
            if (effect.triggerValue == triggerValue) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        effect.triggerCount++;

        GameManager.instance.UpdateItemTriggerCount(effect);

        if (effect.triggerMode == TriggerMode.Interval)
        {
            if (effect.triggerValue == effect.triggerCount)
            {
                effect.triggerCount = 0;
                
                GameManager.instance.UpdateItemTriggerCount(effect);
                
                return true;
            }
        }

        return false;
    }

    public void ApplyEffect(EffectData effect)
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

        lastTriggeredEffects.Add(effect.id);

        // 효과 적용
        switch (effect.type)
        {
            case EffectType.SCORE_MODIFIER:
                foreach (BlockType blockType in effect.blockTypes)
                {
                    if (effect.scope == EffectScope.Run)
                    {
                        runData.baseBlockScores[blockType] += effect.effectValue;
                    }
                    blockGameData.blockScores[blockType] += effect.effectValue;
                }
                break;
            case EffectType.SCORE_MULTIPLIER:
                foreach (BlockType blockType in effect.blockTypes)
                {
                    if (effect.scope == EffectScope.Run)
                    {
                        runData.baseBlockScores[blockType] *= effect.effectValue;
                    }
                    blockGameData.blockScores[blockType] *= effect.effectValue;
                }
                break;
            case EffectType.MULTIPLIER_MODIFIER:
                blockGameData.matchMultipliers[matchType] += effect.effectValue;
                break;
            case EffectType.BASEMULTIPLIER_MODIFIER:
                if (effect.scope == EffectScope.Run)
                {
                    runData.baseMatchMultipliers[matchType] += effect.effectValue;
                }
                blockGameData.matchMultipliers[matchType] += effect.effectValue;
                break;
            case EffectType.BASEMULTIPLIER_MULTIPLIER:
                if (effect.scope == EffectScope.Run)
                {
                    runData.baseMatchMultipliers[matchType] *= effect.effectValue;
                }
                blockGameData.matchMultipliers[matchType] *= effect.effectValue;
                break;
            case EffectType.REROLL_MODIFIER:
                GameManager.instance.UpdateRerollCount(effect.effectValue);
                break;
            case EffectType.REROLL_MULTIPLIER:
                GameManager.instance.UpdateRerollCount(effect.effectValue, isMultiplying: true);
                break;
            case EffectType.BASEREROLL_MODIFIER:           
                runData.baseRerollCount += effect.effectValue;
                if (runData.baseRerollCount < 0) runData.baseRerollCount = 0;
                GameManager.instance.UpdateRerollCount(effect.effectValue);
                break;
            case EffectType.BASEREROLL_MULTIPLIER:
                runData.baseRerollCount *= effect.effectValue;
                GameManager.instance.UpdateRerollCount(effect.effectValue, isMultiplying: true);
                break;
            case EffectType.GOLD_MODIFIER:
                GameManager.instance.UpdateGold(effect.effectValue);
                break;
            case EffectType.GOLD_MULTIPLIER:
                GameManager.instance.UpdateGold(effect.effectValue, isMultiplying: true);
                break;
            case EffectType.BOARD_SIZE_MODIFIER:
                if (effect.scope == EffectScope.Run)
                {
                    runData.baseBoardRows += effect.effectValue;
                    runData.baseBoardColumns += effect.effectValue;
                }
                blockGameData.boardRows += effect.effectValue;
                blockGameData.boardColumns += effect.effectValue;
                break;
            case EffectType.BOARD_CORNER_BLOCK:
                blockGameData.isCornerBlocked = true;
                break;
            case EffectType.BOARD_RANDOM_BLOCK:
                blockGameData.inactiveCellCount += effect.effectValue;
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
                    if (effect.scope == EffectScope.Run)
                    {
                        runData.availableBlocks.RemoveAll(data => data.type == blockType);
                    }
                    blockGameData.deck.RemoveAll(data => data.type == blockType);
                }
                break;
            case EffectType.BLOCK_DELETE_WITH_COUNT:
                // TODO
                break;
            case EffectType.RANDOM_ROW_LINE_CLEAR:
                List<int> rowIndices = GetRandomIndices(blockGameData.boardRows, 1);
                GameManager.instance.ForceLineClearBoard(MatchType.ROW, rowIndices);
                break;
            case EffectType.RANDOM_COLUMN_LINE_CLEAR:
                List<int> columnIndices = GetRandomIndices(blockGameData.boardColumns, 1);
                GameManager.instance.ForceLineClearBoard(MatchType.COLUMN, columnIndices);
                break;
            case EffectType.RANDOM_LINE_CLEAR:
                ProcessRandomLineClear();
                break;
            case EffectType.BOARD_CLEAR:
                List<int> indices = Enumerable.Range(0, blockGameData.boardColumns).ToList();
                GameManager.instance.ForceLineClearBoard(MatchType.ROW, indices);
                break;
            case EffectType.DRAW_BLOCK_COUNT_MODIFIER:
                blockGameData.drawBlockCount = effect.effectValue;
                break;
            case EffectType.ROW_LINE_CLEAR:
                rowIndices = Enumerable.Range(0, Mathf.Min(effect.effectValue, runData.baseBoardRows)).ToList();
                GameManager.instance.ForceLineClearBoard(MatchType.ROW, rowIndices);
                break;
            case EffectType.COLUMN_LINE_CLEAR:
                columnIndices = Enumerable.Range(0, Mathf.Min(effect.effectValue, runData.baseBoardColumns)).ToList();
                GameManager.instance.ForceLineClearBoard(MatchType.COLUMN, columnIndices);
                break;
            case EffectType.EFFECT_VALUE_MODIFIER:
                effect.modifyingEffect.effectValue += effect.effectValue;
                break;
            case EffectType.SHOP_REROLL_COST_MODIFIER:
                runData.shopBaseRerollCost += effect.effectValue;
                GameManager.instance.UpdateShopRerollCost(effect.effectValue);
                break;
            case EffectType.SHOP_REROLL_COST_GROWTH_MODIFIER:
                runData.shopRerollCostGrowth = effect.effectValue;
                break;
            case EffectType.SHOP_ITEM_COUNT_MODIFIER:
                runData.shopItemCounts[ItemType.ITEM] = effect.effectValue;
                break;
            case EffectType.SHOP_BOOST_COUNT_MODIFIER:
                runData.shopItemCounts[ItemType.BOOST] = effect.effectValue;
                break;
            case EffectType.SHOP_BLOCK_COUNT_MODIFIER:
                runData.shopItemCounts[ItemType.ADD_BLOCK] = effect.effectValue;
                break;
            case EffectType.COMMON_WEIGHTS_MULTIPLIER:
                runData.itemRarityWeights[ItemRarity.COMMON] *= effect.effectValue;
                break;
            case EffectType.RARE_WEIGHTS_MULTIPLIER:
                runData.itemRarityWeights[ItemRarity.RARE] *= effect.effectValue;
                break;
            case EffectType.EPIC_WEIGHTS_MULTIPLIER:
                runData.itemRarityWeights[ItemRarity.EPIC] *= effect.effectValue;
                break;
            case EffectType.LEGENDARY_WEIGHTS_MULTIPLIER:
                runData.itemRarityWeights[ItemRarity.LEGENDARY] *= effect.effectValue;
                break;
            case EffectType.MULTIPLIER_MULTIPLER:
                blockGameData.matchMultipliers[matchType] *= effect.effectValue;
                break;
            default:
                break;
        }
    }

    private bool IsIncluded(BlockType[] arr1, BlockType[] arr2)
    {
        if (arr2 != null)
        {
            if (arr2.Length == 0) return true;
        }
        if (arr1 == null || arr2 == null) return true;
        return arr1.All(x => arr2.Contains(x));
    }

    private bool CheckProbability(float probability)
    {
        return Random.Range(0f, 1f) <= probability;
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

    private List<int> GetRandomIndices(int boardSize, int count)
    {
        HashSet<int> result = new HashSet<int>();

        for (int i = 0; i < 10000; i++)
        {
            if (result.Count == count)
            {
                break;
            }
            result.Add(Random.Range(0, boardSize));
        }

        return result.ToList();
    }

    private void ProcessRandomLineClear()
    {
        if (CheckProbability(0.5f))
        {
            List<int> rowIndices = GetRandomIndices(blockGameData.boardRows, 1);
            GameManager.instance.ForceLineClearBoard(MatchType.ROW, rowIndices);
        } 
        else
        {
            List<int> columnIndices = GetRandomIndices(blockGameData.boardColumns, 1);
            GameManager.instance.ForceLineClearBoard(MatchType.COLUMN, columnIndices);
        }
    }

    private bool IsStageEffectTriggered(List<string> effectIdList)
    {
        foreach (string effectId in effectIdList)
        {
            EffectData effect = runData.activeEffects.FirstOrDefault(x => x.id == effectId);

            if (effect != null && effect.scope == EffectScope.Stage)
            {
                return true;
            }
        }
        return false;
    }
}
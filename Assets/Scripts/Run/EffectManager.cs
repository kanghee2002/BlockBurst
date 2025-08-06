using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance = null;
    private RunData runData;
    private BlockGameData blockGameData;

    private List<AnimationData> triggeredAnimations = new();

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
        /*if (IsStageEffectTriggered(triggeredAnimations))
        {
            GameManager.instance.PlayStageEffectAnimation();
        }*/
        GameManager.instance.PlayItemEffectAnimation(triggeredAnimations.ToList());
        triggeredAnimations.Clear();
    }

    // 게임 중 블록을 놓았을 때, 시각 효과를 위해 호출
    public void EndTriggerEffectOnPlace(List<Match> matches)
    {
        /*if (IsStageEffectTriggered(triggeredAnimations))
        {
            GameManager.instance.PlayStageEffectAnimation();
        }*/
        //float matchAnimationTime = GameManager.instance.GetMatchAnimationTime(matches);
        GameManager.instance.PlayItemEffectAnimation(triggeredAnimations.ToList(), isMatching: matches.Count > 0);
        triggeredAnimations.Clear();
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

        int finalValue = GetFinalValue(effect);

        int additiveValue = finalValue;     // 애니메이션 처리를 위한 효과 증가값

        // 효과 적용
        switch (effect.type)
        {
            case EffectType.SCORE_MODIFIER:
                foreach (BlockType blockType in effect.blockTypes)
                {
                    if (effect.scope == EffectScope.Run)
                    {
                        runData.baseBlockScores[blockType] += finalValue;
                    }
                    blockGameData.blockScores[blockType] += finalValue;
                }
                break;
            case EffectType.SCORE_MULTIPLIER:
                foreach (BlockType blockType in effect.blockTypes)
                {
                    if (effect.scope == EffectScope.Run)
                    {
                        runData.baseBlockScores[blockType] *= finalValue;
                    }
                    blockGameData.blockScores[blockType] *= finalValue;
                }
                break;
            case EffectType.MULTIPLIER_MODIFIER:
                blockGameData.matchMultipliers[matchType] += finalValue;
                break;
            case EffectType.BASEMULTIPLIER_MODIFIER:
                if (effect.scope == EffectScope.Run)
                {
                    runData.baseMatchMultipliers[matchType] += finalValue;
                }
                blockGameData.matchMultipliers[matchType] += finalValue;
                break;
            case EffectType.BASEMULTIPLIER_MULTIPLIER:
                if (effect.scope == EffectScope.Run)
                {
                    int originalMultiplier = runData.baseMatchMultipliers[matchType];
                    additiveValue = runData.baseMatchMultipliers[matchType];
                    runData.baseMatchMultipliers[matchType] *= finalValue;
                    if (effect.maxValue != -1)
                    {
                        runData.baseMatchMultipliers[matchType] = Math.Min(originalMultiplier + effect.maxValue, runData.baseMatchMultipliers[matchType]);
                        additiveValue = Math.Min(additiveValue, effect.maxValue);
                    }
                }
                blockGameData.matchMultipliers[matchType] *= finalValue;
                break;
            case EffectType.REROLL_MODIFIER:
                blockGameData.rerollCount += finalValue;
                if (blockGameData.rerollCount < 0) blockGameData.rerollCount = 0;
                break;
            case EffectType.REROLL_MULTIPLIER:
                additiveValue = blockGameData.rerollCount * (finalValue - 1);
                blockGameData.rerollCount *= finalValue;
                break;
            case EffectType.BASEREROLL_MODIFIER:           
                runData.baseRerollCount += finalValue;
                if (runData.baseRerollCount < 0) runData.baseRerollCount = 0;
                break;
            case EffectType.BASEREROLL_MULTIPLIER:
                int originalRerollCount = runData.baseRerollCount;
                additiveValue = runData.baseRerollCount;
                runData.baseRerollCount *= finalValue;
                if (effect.maxValue != -1)
                {
                    runData.baseRerollCount = Math.Min(originalRerollCount + effect.maxValue, runData.baseRerollCount);
                    additiveValue = Math.Min(additiveValue, effect.maxValue);
                }
                break;
            case EffectType.GOLD_MODIFIER:
                runData.gold += finalValue;
                break;
            case EffectType.GOLD_MULTIPLIER:
                int addingValue = runData.gold * (finalValue - 1);
                additiveValue = addingValue;
                if (effect.maxValue != -1)
                {
                    addingValue = Math.Min(effect.maxValue, addingValue);
                    runData.gold += addingValue;
                }
                runData.gold += addingValue;
                break;
            case EffectType.BOARD_SIZE_MODIFIER:
                if (effect.scope == EffectScope.Run)
                {
                    runData.baseBoardRows += finalValue;
                    runData.baseBoardColumns += finalValue;
                }
                blockGameData.boardRows += finalValue;
                blockGameData.boardColumns += finalValue;
                break;
            case EffectType.BOARD_CORNER_BLOCK:
                blockGameData.isCornerBlocked = true;
                break;
            case EffectType.BOARD_RANDOM_BLOCK:
                blockGameData.inactiveCellCount += finalValue;
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
                    MultiplyBlock(blockType, finalValue);
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
                for (int i = 0; i < finalValue; i++)
                {
                    foreach (BlockType blockType in effect.blockTypes)
                    {
                        GameManager.instance.RemoveBlockFromRunDeck(blockType);
                    }
                }
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
                blockGameData.drawBlockCount = finalValue;
                break;
            case EffectType.ROW_LINE_CLEAR:
                rowIndices = Enumerable.Range(0, Mathf.Min(finalValue, runData.baseBoardRows)).ToList();
                GameManager.instance.ForceLineClearBoard(MatchType.ROW, rowIndices);
                break;
            case EffectType.COLUMN_LINE_CLEAR:
                columnIndices = Enumerable.Range(0, Mathf.Min(finalValue, runData.baseBoardColumns)).ToList();
                GameManager.instance.ForceLineClearBoard(MatchType.COLUMN, columnIndices);
                break;
            case EffectType.EFFECT_VALUE_MODIFIER:
                effect.modifyingEffect.effectValue += finalValue;
                break;
            case EffectType.SHOP_REROLL_COST_MODIFIER:
                runData.shopBaseRerollCost += finalValue;
                GameManager.instance.UpdateShopRerollCost(finalValue);
                break;
            case EffectType.SHOP_REROLL_COST_GROWTH_MODIFIER:
                runData.shopRerollCostGrowth = finalValue;
                break;
            case EffectType.SHOP_ITEM_COUNT_MODIFIER:
                runData.shopItemCounts[ItemType.ITEM] = finalValue;
                break;
            case EffectType.SHOP_BOOST_COUNT_MODIFIER:
                runData.shopItemCounts[ItemType.BOOST] = finalValue;
                break;
            case EffectType.SHOP_BLOCK_COUNT_MODIFIER:
                runData.shopItemCounts[ItemType.ADD_BLOCK] = finalValue;
                break;
            case EffectType.COMMON_WEIGHTS_MULTIPLIER:
                runData.itemRarityWeights[ItemRarity.COMMON] *= finalValue;
                break;
            case EffectType.RARE_WEIGHTS_MULTIPLIER:
                runData.itemRarityWeights[ItemRarity.RARE] *= finalValue;
                break;
            case EffectType.EPIC_WEIGHTS_MULTIPLIER:
                runData.itemRarityWeights[ItemRarity.EPIC] *= finalValue;
                break;
            case EffectType.LEGENDARY_WEIGHTS_MULTIPLIER:
                runData.itemRarityWeights[ItemRarity.LEGENDARY] *= finalValue;
                break;
            case EffectType.MULTIPLIER_MULTIPLER:
                additiveValue = blockGameData.matchMultipliers[matchType];
                blockGameData.matchMultipliers[matchType] *= finalValue;
                break;
            default:
                break;
        }

        AnimationData animationData = AnimationManager.instance.GetAnimationData(effect, additiveValue);
        triggeredAnimations.Add(animationData);

        DataManager.instance.UpdateMaxBaseMultiplier(runData.baseMatchMultipliers[matchType]);
        DataManager.instance.UpdateMaxMultiplier(blockGameData.matchMultipliers[matchType]);
        DataManager.instance.UpdateMaxBaseRerollCount(runData.baseRerollCount);
        DataManager.instance.UpdateMaxGold(runData.gold);
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

    public int GetFinalValue(EffectData effect)
    {
        int value = effect.effectValue;
        int scalingValue = 1;

        // 계수 적용
        switch (effect.scalingFactor)
        {
            case ScalingFactor.None:
                break;
            case ScalingFactor.RemainingBlockCount:
                scalingValue = blockGameData.deck.Count;
                break;
            case ScalingFactor.BoostCount:
                scalingValue = runData.activeBoosts.Count;
                break;
            case ScalingFactor.CurrentGold:
                scalingValue = runData.gold;
                break;
            case ScalingFactor.RerollCount:
                scalingValue = blockGameData.rerollCount;
                break;
            default:
                break;
        }

        scalingValue = Mathf.FloorToInt(scalingValue * effect.scalingMultiplier);

        return value * scalingValue;
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
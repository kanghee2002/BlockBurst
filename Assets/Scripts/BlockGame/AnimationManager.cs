using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AnimationData
{
    public AnimationType animationType;
    public int value;
    public int index;

    [Header("Optional")]
    public float delayMultiplier;
    public bool isValueAdditive;
    public List<Match> matches;
    public EffectData effect;
    public List<AnimationData> subAnimations;

    [Header("Animation")]
    public bool resetSpeed;
}

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance = null;

    // Data
    private RunData runData;
    private BlockGameData blockGameData;

    // Delay
    private float defaultDelay;                 // 기본 딜레이
    private float scoreAnimationDelay;          // 점수 증가 딜레이
    private float scoreToItemEffectDelay;       // 점수 증가 이후 아이템 효과 발동 이전 딜레이
    private float itemEffectDelay;              // 아이템 효과 간 딜레이

    // Animation
    private Queue<AnimationData> animationQueue;
    private bool isPlaying;
    private float speed;

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

        defaultDelay = 0.01f;
        scoreAnimationDelay = 0.01f;
        scoreToItemEffectDelay = 0.3f;
        itemEffectDelay = 0.25f;

        animationQueue = new Queue<AnimationData>();
        isPlaying = false;
        speed = 1f;
    }

    public void InitializeRunData(ref RunData runData)
    {
        this.runData = runData;
    }

    public void InitializeBlockGameData(ref BlockGameData blockGameData)
    {
        this.blockGameData = blockGameData;
    }

    public void EnqueueAnimation(AnimationData animationData)
    {
        animationQueue.Enqueue(animationData);

        if (!isPlaying)
        {
            isPlaying = true;

            StartCoroutine(ProcessAnimationCoroutine());
        }
    }

    private IEnumerator ProcessAnimationCoroutine()
    {
        while (animationQueue.Count > 0)
        {
            AnimationData animationData = animationQueue.Dequeue();
            float delay = defaultDelay;

            if (animationData.delayMultiplier != 0f)
            {
                delay *= animationData.delayMultiplier;
            }

            delay = ProcessAnimation(animationData, delay);

            if (animationData.resetSpeed)
            {
                speed = 1f;
            }
            else
            {
                speed *= 1.05f;
            }

            delay /= speed;

            yield return new WaitForSeconds(delay);
        }

        isPlaying = false;
    }

    public float ProcessAnimation(AnimationData animationData, float delay = 0f)
    {
        switch (animationData.animationType)
        {
            case AnimationType.Delay:
                break;
            case AnimationType.UpdateChip:
                GameUIManager.instance.UpdateChip(animationData.value);
                break;
            case AnimationType.UpdateMultiplier:
                if (animationData.isValueAdditive)
                {
                    GameUIManager.instance.UpdateMultiplierByAdd(animationData.value);
                }
                else
                {
                    GameUIManager.instance.UpdateMultiplier(animationData.value);
                }
                break;
            case AnimationType.UpdateProduct:
                GameUIManager.instance.UpdateProduct(animationData.value);
                break;
            case AnimationType.UpdateScore:
                GameUIManager.instance.UpdateScore(animationData.value, isAdditive: true);
                break;
            case AnimationType.UpdateReroll:
                GameUIManager.instance.DisplayRerollCount(animationData.value, animationData.isValueAdditive);
                break;
            case AnimationType.UpdateGold:
                GameUIManager.instance.UpdateGold(animationData.value, animationData.isValueAdditive);
                break;
            case AnimationType.LineClear:
                GameUIManager.instance.PlayMatchAnimation(animationData.matches, GetScoreDictionary(animationData.matches), scoreAnimationDelay);
                delay = GetMatchTime(animationData.matches);
                break;
            case AnimationType.ItemEffect:
                string effectDescription = GetItemEffectDescription(animationData.effect, animationData.value);
                GameUIManager.instance.PlayItemEffectAnimation(effectDescription, animationData.index, itemEffectDelay);
                delay = itemEffectDelay;
                foreach (AnimationData subAnimation in animationData.subAnimations)
                {
                    ProcessAnimation(subAnimation);
                }
                break;
            case AnimationType.StageClear:
                break;
            case AnimationType.ItemActivated:
                break;
            case AnimationType.ItemDeactivated:
                break;
        }

        return delay;
    }

    public AnimationData GetAnimationData(EffectData effect, int additiveValue)
    {
        AnimationData animationData = new AnimationData();

        animationData.value = additiveValue;
        animationData.effect = effect;
        animationData.isValueAdditive = true;

        switch (effect.type)
        {
            case EffectType.SCORE_MODIFIER:
            case EffectType.SCORE_MULTIPLIER:
                animationData.animationType = AnimationType.UpdateBlockScore;
                break;
            case EffectType.MULTIPLIER_MODIFIER:
            case EffectType.BASEMULTIPLIER_MODIFIER:
            case EffectType.MULTIPLIER_MULTIPLER:
            case EffectType.BASEMULTIPLIER_MULTIPLIER:
                animationData.animationType = AnimationType.UpdateMultiplier;
                break;
            case EffectType.REROLL_MODIFIER:
            case EffectType.REROLL_MULTIPLIER:
            case EffectType.BASEREROLL_MODIFIER:
            case EffectType.BASEREROLL_MULTIPLIER:
                animationData.animationType = AnimationType.UpdateReroll;
                break;
            case EffectType.GOLD_MODIFIER:
            case EffectType.GOLD_MULTIPLIER:
                animationData.animationType = AnimationType.UpdateGold;
                break;
            case EffectType.EFFECT_VALUE_MODIFIER:
                animationData.animationType = AnimationType.None;
                break;
            default:
                animationData.animationType = AnimationType.None;
                break;
        }
        return animationData;
    }

    // Match에 해당하는 점수 리스트 만들기
    private Dictionary<Match, List<int>> GetScoreDictionary(List<Match> matches)
    {
        Dictionary<Match, List<int>> dictionary = new Dictionary<Match, List<int>>();

        foreach (Match match in matches)
        {
            List<int> scores = new List<int>();

            int blockIndex = 0;
            if (match.matchType == MatchType.ROW)
            {
                for (int x = 0; x < blockGameData.boardColumns; x++)
                {
                    if (match.validIndices.Contains(x))
                    {
                        BlockType blockType = match.blocks[blockIndex].Item1;
                        int score = blockGameData.blockScores[blockType];
                        scores.Add(score);
                        blockIndex++;
                    }
                    else
                    {
                        scores.Add(0);
                    }
                }
            }
            else if (match.matchType == MatchType.COLUMN)
            {
                for (int y = 0; y < blockGameData.boardRows; y++)
                {
                    if (match.validIndices.Contains(y))
                    {
                        BlockType blockType = match.blocks[blockIndex].Item1;
                        int score = blockGameData.blockScores[blockType];
                        scores.Add(score);
                        blockIndex++;
                    }
                    else
                    {
                        scores.Add(0);
                    }
                }
            }
            dictionary.Add(match, scores);
        }
        return dictionary;
    }

    private string GetItemEffectDescription(EffectData effect, int effectValue)
    {
        string description = "";

        string value = effectValue > 0 ? "+" + effectValue : effectValue.ToString();

        switch (effect.type)
        {
            case EffectType.SCORE_MODIFIER:
                description = "<color=#0088FF>" + value + "</color>";
                break;
            case EffectType.SCORE_MULTIPLIER:
                description = "<color=#0088FF>X" + effectValue.ToString() + "</color>";
                break;
            case EffectType.MULTIPLIER_MODIFIER:
                description = "<color=red>" + value + "</color>";
                break;
            case EffectType.BASEMULTIPLIER_MODIFIER:
                description = "<color=red>" + value + "</color>";
                break;
            case EffectType.BASEMULTIPLIER_MULTIPLIER:
                description = "<color=red>X" + effectValue.ToString() + "</color>";
                break;
            case EffectType.REROLL_MODIFIER:
            case EffectType.BASEREROLL_MODIFIER:
                description = "<color=white>" + value + "</color>";
                break;
            case EffectType.REROLL_MULTIPLIER:
            case EffectType.BASEREROLL_MULTIPLIER:
                description = "<color=white>X" + effectValue.ToString() + "</color>";
                break;
            case EffectType.GOLD_MODIFIER:
                description = "<color=yellow>" + value + "</color>";
                break;
            case EffectType.GOLD_MULTIPLIER:
                description = "<color=yellow>X" + effectValue.ToString() + "</color>";
                break;
            case EffectType.BOARD_SIZE_MODIFIER:
                description = "<color=white>보드 크기" + value + "</color>";
                break;
            case EffectType.BOARD_CORNER_BLOCK:
                description = "<color=white>보드 가장자리\n막힘!</color>";
                break;
            case EffectType.BOARD_RANDOM_BLOCK:
                description = "<color=white>보드 무작위\n" + effectValue.ToString() + "칸 막힘!</color>";
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
                description = "<color=white>블록 " + effectValue.ToString() + "배!</color>";
                break;
            case EffectType.BLOCK_DELETE:
                description = "<color=white>블록 모두\n삭제!</color>";
                break;
            case EffectType.BLOCK_DELETE_WITH_COUNT:
                // TODO
                break;
            case EffectType.ROW_LINE_CLEAR:
            case EffectType.RANDOM_ROW_LINE_CLEAR:
                description = "<color=white>가로\n지움!</color>";
                break;
            case EffectType.COLUMN_LINE_CLEAR:
            case EffectType.RANDOM_COLUMN_LINE_CLEAR:
                description = "<color=white>세로\n지움!</color>";
                break;
            case EffectType.BOARD_CLEAR:
                description = "<color=white>보드\n지움!</color>";
                break;
            case EffectType.DRAW_BLOCK_COUNT_MODIFIER:
                description = "<color=white>선택지가\n" + effectValue.ToString() + "개로!</color>";
                break;
            case EffectType.RANDOM_LINE_CLEAR:
                description = "<color=white>무작위\n한 줄 지움!</color>";
                break;
            case EffectType.EFFECT_VALUE_MODIFIER:
                if (effect.modifyingEffect.type == EffectType.GOLD_MODIFIER)
                {
                    description = "<color=yellow>" + value + "</color>";
                }
                else if (effect.modifyingEffect.type == EffectType.SCORE_MODIFIER)
                {
                    description = "<color=#0088FF>" + value + "</color>";
                }
                else
                {
                    description = "<color=red>" + value + "</color>";
                }
                break;
            case EffectType.MULTIPLIER_MULTIPLER:
                description = "<color=red>X" + effectValue.ToString() + "</color>";
                break;
        }
        return description;
    }

    private float GetMatchTime(List<Match> matches)
    {
        float totalTime = 0f;
        int width = blockGameData.boardColumns, height = blockGameData.boardRows;

        // 총 걸리는 시간 계산
        foreach (Match match in matches)
        {
            if (match.matchType == MatchType.ROW)
            {
                totalTime += scoreAnimationDelay * width;
            }
            else if (match.matchType == MatchType.COLUMN)
            {
                totalTime += scoreAnimationDelay * height;
            }
        }

        totalTime += scoreToItemEffectDelay;

        return totalTime;
    }
}

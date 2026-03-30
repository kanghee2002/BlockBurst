using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 런 저장 전용 DTO. <see cref="RunSaveMapper"/>로 <see cref="RunData"/>와 변환한다.
/// </summary>
[Serializable]
public class RunSaveData
{
    public const int CurrentSaveVersion = 1;

    public int saveVersion = CurrentSaveVersion;

    public int currentChapterIndex;
    public int currentStageIndex;
    public string currentDeckId;
    public string currentLevelId;
    /// <summary>이어하기 시 <see cref="GameManager.StartStage"/>에 바로 진입할 스테이지 SO id.</summary>
    public string currentStageId;

    public RunData.History history;

    public BlockTypeIntDictionary baseBlockScores;
    public MatchTypeIntDictionary baseMatchMultipliers;

    public List<string> availableBlockIds;
    public List<string> activeItemIds;
    public List<string> activeBoostIds;
    public List<EffectState> activeEffects;

    public int gold;
    public int baseRerollCount;
    public int maxItemCount;
    public int baseBoardRows;
    public int baseBoardColumns;
    public int baseDrawBlockCount;
    public int shopBaseRerollCost;
    public int shopRerollCostIncreasePercentage;

    public ItemTypeIntDictionary shopItemCounts;
    public ItemRarityIntDictionary itemRarityWeights;
}

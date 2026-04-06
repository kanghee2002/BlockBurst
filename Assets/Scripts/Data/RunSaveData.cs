using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 런 저장 전용 DTO. <see cref="RunSaveMapper"/>로 <see cref="RunData"/>와 변환한다.
/// </summary>
[Serializable]
public class RunSaveData
{
    /// <summary>이 버전 미만 저장은 로드하지 않는다.</summary>
    public const int MinSupportedSaveVersion = 1;

    /// <summary>새로 저장할 때 쓰는 버전. 올리면 <see cref="TryMigrate"/>에 분기를 추가한다.</summary>
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
    public List<string> activeUpgradeIds;
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

    /// <summary>
    /// 디스크에서 읽은 DTO를 현재 <see cref="CurrentSaveVersion"/>까지 올린다. 실패 시 false.
    /// </summary>
    public static bool TryMigrate(ref RunSaveData save)
    {
        if (save == null)
            return false;

        if (save.saveVersion < MinSupportedSaveVersion)
        {
            Debug.LogWarning($"RunSaveData.TryMigrate: unsupported version {save.saveVersion} (min {MinSupportedSaveVersion}).");
            return false;
        }

        if (save.saveVersion > CurrentSaveVersion)
        {
            Debug.LogWarning($"RunSaveData.TryMigrate: save is newer than this build ({save.saveVersion} > {CurrentSaveVersion}).");
            return false;
        }

        while (save.saveVersion < CurrentSaveVersion)
        {
            switch (save.saveVersion)
            {
                // When bumping CurrentSaveVersion, add a case for the previous version:
                // case 1:
                //     save.newField = defaultValue;
                //     save.saveVersion = 2;
                //     break;
                default:
                    Debug.LogWarning($"RunSaveData.TryMigrate: no migration path from version {save.saveVersion}.");
                    return false;
            }
        }

        return true;
    }
}

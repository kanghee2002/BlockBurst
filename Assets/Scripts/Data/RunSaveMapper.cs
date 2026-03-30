using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <see cref="RunData"/>와 <see cref="RunSaveData"/> 간 변환. SO 복원은 <see cref="ScriptableDataManager"/>가 <see cref="ScriptableDataManager.Awake"/>로 준비된 뒤 수행한다.
/// </summary>
public static class RunSaveMapper
{
    /// <summary>누락된 블록/아이템 id는 건너뛰고 경고만 남긴다. 덱/레벨 id가 비어 있으면 해당 필드는 null이다.</summary>
    public static RunSaveData ToSaveData(RunData runData)
    {
        // RunSaveData를 새로 만들고 스칼라·히스토리·덱/레벨 id·딕셔너리(래퍼 변환)를 명시적으로 매핑한다. 리스트 필드는 빈 컬렉션으로 둔 뒤 아래에서 채운다.
        var saveData = new RunSaveData
        {
            saveVersion = RunSaveData.CurrentSaveVersion,
            currentChapterIndex = runData.currentChapterIndex,
            currentStageIndex = runData.currentStageIndex,
            currentDeckId = runData.currentDeck != null ? runData.currentDeck.id : null,
            currentLevelId = runData.currentLevel != null ? runData.currentLevel.id : null,
            currentStageId = runData.currentStageId,
            history = runData.history,
            baseBlockScores = new BlockTypeIntDictionary(runData.baseBlockScores),
            baseMatchMultipliers = new MatchTypeIntDictionary(runData.baseMatchMultipliers),
            availableBlockIds = new List<string>(),
            activeItemIds = new List<string>(),
            activeBoostIds = new List<string>(),
            activeEffects = new List<EffectState>(),
            gold = runData.gold,
            baseRerollCount = runData.baseRerollCount,
            maxItemCount = runData.maxItemCount,
            baseBoardRows = runData.baseBoardRows,
            baseBoardColumns = runData.baseBoardColumns,
            baseDrawBlockCount = runData.baseDrawBlockCount,
            shopBaseRerollCost = runData.shopBaseRerollCost,
            shopRerollCostIncreasePercentage = runData.shopRerollCostIncreasePercentage,
            shopItemCounts = new ItemTypeIntDictionary(runData.shopItemCounts),
            itemRarityWeights = new ItemRarityIntDictionary(runData.itemRarityWeights)
        };

        // 사용 가능한 블록 SO를 순서 유지한 채 BaseData.id 문자열 리스트로 옮긴다. id 없는 에셋은 건너뛴다.
        if (runData.availableBlocks != null)
        {
            foreach (BlockData block in runData.availableBlocks)
            {
                if (block == null)
                    continue;
                if (string.IsNullOrEmpty(block.id))
                {
                    Debug.LogWarning("RunSaveMapper: BlockData without id skipped in availableBlocks.");
                    continue;
                }

                saveData.availableBlockIds.Add(block.id);
            }
        }

        // 활성 아이템 SO를 id 리스트로 옮긴다.
        if (runData.activeItems != null)
        {
            foreach (ItemData item in runData.activeItems)
            {
                if (item == null)
                    continue;
                if (string.IsNullOrEmpty(item.id))
                {
                    Debug.LogWarning("RunSaveMapper: ItemData without id skipped in activeItems.");
                    continue;
                }

                saveData.activeItemIds.Add(item.id);
            }
        }

        // 활성 부스트(아이템) SO를 id 리스트로 옮긴다.
        if (runData.activeBoosts != null)
        {
            foreach (ItemData item in runData.activeBoosts)
            {
                if (item == null)
                    continue;
                if (string.IsNullOrEmpty(item.id))
                {
                    Debug.LogWarning("RunSaveMapper: ItemData without id skipped in activeBoosts.");
                    continue;
                }

                saveData.activeBoostIds.Add(item.id);
            }
        }

        // 활성 효과는 JSON 직렬화를 위해 인스턴스를 복제해 리스트에 담는다(원본 RunData와 참조를 분리).
        if (runData.activeEffects != null)
        {
            foreach (EffectState state in runData.activeEffects)
            {
                if (state == null)
                    continue;
                saveData.activeEffects.Add(state.Clone());
            }
        }

        // 직렬화 대상 DTO를 반환한다.
        return saveData;
    }

    /// <summary>
    /// DTO와 레지스트리로 새 <see cref="RunData"/>를 만든다. 인자가 없으면 null.
    /// </summary>
    public static RunData FromSaveData(RunSaveData saveData)
    {
        if (saveData == null)
            return null;
        if (ScriptableDataManager.instance == null)
        {
            Debug.LogError("RunSaveMapper.FromSaveData: ScriptableDataManager is null.");
            return null;
        }

        ScriptableDataManager sdManager = ScriptableDataManager.instance;

        RunData runData = new RunData();

        // ToSaveData의 객체 초기화 블록·이후 리스트 채우기와 같은 순서로 역매핑한다(의존성 때문의 재배치는 아님).
        runData.currentChapterIndex = saveData.currentChapterIndex;
        runData.currentStageIndex = saveData.currentStageIndex;
        runData.currentStageId = saveData.currentStageId;

        runData.currentDeck = string.IsNullOrEmpty(saveData.currentDeckId) ? null : sdManager.GetDeck(saveData.currentDeckId);
        runData.currentLevel = string.IsNullOrEmpty(saveData.currentLevelId) ? null : sdManager.GetLevel(saveData.currentLevelId);

        runData.history = saveData.history;

        runData.baseBlockScores = saveData.baseBlockScores != null
            ? saveData.baseBlockScores.ToDictionary()
            : new Dictionary<BlockType, int>();
        runData.baseMatchMultipliers = saveData.baseMatchMultipliers != null
            ? saveData.baseMatchMultipliers.ToDictionary()
            : new Dictionary<MatchType, int>();

        runData.gold = saveData.gold;
        runData.baseRerollCount = saveData.baseRerollCount;
        runData.maxItemCount = saveData.maxItemCount;
        runData.baseBoardRows = saveData.baseBoardRows;
        runData.baseBoardColumns = saveData.baseBoardColumns;
        runData.baseDrawBlockCount = saveData.baseDrawBlockCount;
        runData.shopBaseRerollCost = saveData.shopBaseRerollCost;
        runData.shopRerollCostIncreasePercentage = saveData.shopRerollCostIncreasePercentage;

        runData.shopItemCounts = saveData.shopItemCounts != null
            ? saveData.shopItemCounts.ToDictionary()
            : new Dictionary<ItemType, int>();
        runData.itemRarityWeights = saveData.itemRarityWeights != null
            ? saveData.itemRarityWeights.ToDictionary()
            : new Dictionary<ItemRarity, int>();

        // ToSaveData의 availableBlockIds 채우기에 대응: id → BlockData.
        runData.availableBlocks = new List<BlockData>();
        if (saveData.availableBlockIds != null)
        {
            foreach (string id in saveData.availableBlockIds)
            {
                if (string.IsNullOrEmpty(id))
                    continue;
                if (sdManager.TryGetBlock(id, out BlockData block))
                    runData.availableBlocks.Add(block);
                else
                    Debug.LogWarning($"RunSaveMapper: skipped unknown block id '{id}'.");
            }
        }

        // ToSaveData의 activeItemIds 채우기에 대응.
        runData.activeItems = new List<ItemData>();
        if (saveData.activeItemIds != null)
        {
            foreach (string id in saveData.activeItemIds)
            {
                if (string.IsNullOrEmpty(id))
                    continue;
                if (sdManager.TryGetItem(id, out ItemData item))
                    runData.activeItems.Add(item);
                else
                    Debug.LogWarning($"RunSaveMapper: skipped unknown item id '{id}'.");
            }
        }

        // ToSaveData의 activeBoostIds 채우기에 대응.
        runData.activeBoosts = new List<ItemData>();
        if (saveData.activeBoostIds != null)
        {
            foreach (string id in saveData.activeBoostIds)
            {
                if (string.IsNullOrEmpty(id))
                    continue;
                if (sdManager.TryGetItem(id, out ItemData item))
                    runData.activeBoosts.Add(item);
                else
                    Debug.LogWarning($"RunSaveMapper: skipped unknown boost id '{id}'.");
            }
        }

        // ToSaveData의 activeEffects 채우기에 대응(복제).
        runData.activeEffects = new List<EffectState>();
        if (saveData.activeEffects != null)
        {
            foreach (EffectState state in saveData.activeEffects)
            {
                if (state == null)
                    continue;
                runData.activeEffects.Add(state.Clone());
            }
        }

        return runData;
    }
}

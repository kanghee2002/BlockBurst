using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunData
{
    public struct History
    {
        public float startTime;
        public int[] blockHistory;
        public int rerollCount;
        public int itemPurchaseCount;
        public int maxScore;
    }

    // 스테이지 진행 데이터
    public int currentChapterIndex;
    public int currentStageIndex;
    public DeckData currentDeck;
    public LevelData currentLevel;
    public History history;
    public Dictionary<BlockType, int> baseBlockScores;          // 기본 블록 점수
    public Dictionary<MatchType, int> baseMatchMultipliers;     // 기본 배수
    public List<BlockData> availableBlocks;                     // 사용 가능한 블록들
    public List<ItemData> activeItems;                          // 활성화된 아이템들
    public List<ItemData> activeBoosts;                         // 활성화된 부스트들
    public List<EffectData> activeEffects;                      // 활성화된 효과들
    public int gold;                                            // 현재 보유 골드
    public int baseRerollCount;                                 // 기본 리롤 횟수
    public int maxItemCount;                                    // 소지 가능 아이템 수
    public int baseBoardRows;                                   // 보드 크기
    public int baseBoardColumns;                                // 보드 크기
    public int baseDrawBlockCount;                              // 기본 드로우 블록 수
    public int shopBaseRerollCost;                              // 상점 리롤 비용
    public int shopRerollCostIncreasePercentage;                // 상점 리롤 비용 확률
    public Dictionary<ItemType, int> shopItemCounts;            // 상점에 등장하는 아이템 가짓수
    public Dictionary<ItemRarity, int> itemRarityWeights;       // 상점 희귀도 등급

    public void Initialize(GameData gameData)
    {
        currentChapterIndex = 1;
        currentStageIndex = 1;
        currentDeck = gameData.currentDeck;
        currentLevel = gameData.currentLevel;
        history = new History();
        baseBlockScores = new Dictionary<BlockType, int>(gameData.defaultBlockScores);
        baseMatchMultipliers = new Dictionary<MatchType, int>(gameData.defaultMatchMultipliers);
        availableBlocks = new List<BlockData>(gameData.defaultBlocks);
        activeItems = new List<ItemData>();
        activeBoosts = new List<ItemData>();
        activeEffects = new List<EffectData>(gameData.defaultEffects);
        gold = gameData.startingGold;
        baseRerollCount = gameData.defaultRerollCount;
        maxItemCount = gameData.maxItemCount;
        baseBoardRows = gameData.baseBoardRows;
        baseBoardColumns = gameData.baseBoardColumns;
        baseDrawBlockCount = 3;
        shopBaseRerollCost = 2;
        shopRerollCostIncreasePercentage = 100;
        shopItemCounts = new Dictionary<ItemType, int>()
        {
            { ItemType.ITEM, 2 },
            { ItemType.BOOST, 1 },
            { ItemType.ADD_BLOCK, 2 },
        };
        itemRarityWeights = new Dictionary<ItemRarity, int>()
        {
            { ItemRarity.COMMON, 40 },
            { ItemRarity.RARE, 30},
            { ItemRarity.EPIC, 20 },
            { ItemRarity.LEGENDARY, 2 },
        };
    }
}


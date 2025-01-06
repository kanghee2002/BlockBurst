using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunData
{
    // 스테이지 진행 데이터
    public Dictionary<BlockType, int> baseBlockScores;          // 기본 블록 점수
    public Dictionary<MatchType, float> baseMatchMultipliers;   // 기본 매치 배율
    public List<BlockData> availableBlocks;                     // 사용 가능한 블록들
    public List<ItemData> activeItems;                          // 활성화된 아이템들
    public List<EffectData> activeEffects;                      // 활성화된 효과들
    public StageData currentStage;                              // 현재 스테이지
    public Dictionary<string, int> blockReuses;                 // 블록별 재사용 횟수
    public int gold;                                            // 현재 보유 골드
    public int baseRerollCount;                                 // 기본 리롤 횟수
    public int currentRerollCount;                              // 현재 리롤 횟수
    public float baseMultiplier;                                // 기본 배율

    public void Initialize(GameData gameData)
    {
        baseBlockScores = new Dictionary<BlockType, int>(gameData.defaultBlockScores);
        baseMatchMultipliers = new Dictionary<MatchType, float>(gameData.defaultMatchMultipliers);
        availableBlocks = new List<BlockData>(gameData.defaultBlocks);
        activeEffects = new List<EffectData>();
        blockReuses = new Dictionary<string, int>();
        gold = gameData.startingGold;
        baseRerollCount = gameData.defaultRerollCount;
        currentRerollCount = baseRerollCount;
        baseMultiplier = 1.0f;
    }
}

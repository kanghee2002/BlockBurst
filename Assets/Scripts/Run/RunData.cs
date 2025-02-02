using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunData
{
    // 스테이지 진행 데이터
    public Dictionary<BlockType, int> baseBlockScores;          // 기본 블록 점수
    public Dictionary<MatchType, int> baseMatchMultipliers;     // 기본 배수
    public List<BlockData> availableBlocks;                     // 사용 가능한 블록들
    public List<ItemData> activeItems;                          // 활성화된 아이템들
    public List<EffectData> activeEffects;                      // 활성화된 효과들
    public Dictionary<BlockType, int> blockReuses;              // 블록별 재사용 횟수
    public int gold;                                            // 현재 보유 골드
    public int baseRerollCount;                                 // 기본 리롤 횟수
    public int maxItemCount;                                       // 소지 가능 아이템 수
    public int baseBoardRows;                                   // 보드 크기
    public int baseBoardColumns;                                // 보드 크기
    public int baseDrawBlockCount;                              // 기본 드로우 블록 수

    public void Initialize(GameData gameData)
    {
        baseBlockScores = new Dictionary<BlockType, int>(gameData.defaultBlockScores);
        baseMatchMultipliers = new Dictionary<MatchType, int>(gameData.defaultMatchMultipliers);
        availableBlocks = new List<BlockData>(gameData.defaultBlocks);
        activeItems = new List<ItemData>();
        activeEffects = new List<EffectData>();
        blockReuses = new Dictionary<BlockType, int>();
        gold = gameData.startingGold;
        baseRerollCount = gameData.defaultRerollCount;
        maxItemCount = gameData.maxItemCount;
        baseBoardRows = 8;
        baseBoardColumns = 8;
        baseDrawBlockCount = 3;
    }
}

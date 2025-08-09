using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ApplicationType
{
    Windows, Mobile
}

public enum BlockType
{
    I,
    O,
    Z,
    S,
    J,
    L,
    T,
    SOLO,
    DUO,
    TRIO,
    X,
    CROSS,
    CORNER,
    M,
}

public enum MatchType
{
    ROW, COLUMN    // 행 매치
}

public enum StageType
{
    NORMAL, BOSS
}

public enum EffectType
{
    SCORE_MODIFIER,                     // 점수 수정
    SCORE_MULTIPLIER,                   // 점수 N배
    MULTIPLIER_MODIFIER,                // 배수 수정
    BASEMULTIPLIER_MODIFIER,            // 기본 배수 수정
    BASEMULTIPLIER_MULTIPLIER,          // 기본 배수 N배
    REROLL_MODIFIER,                    // 리롤 횟수 수정
    REROLL_MULTIPLIER,                  // 리롤 횟수 N배
    BASEREROLL_MODIFIER,                // 기본 리롤 횟수 수정
    BASEREROLL_MULTIPLIER,              // 기본 리롤 횟수 N배
    GOLD_MODIFIER,                      // 골드 수정
    GOLD_MULTIPLIER,                    // 골드 N배
    BOARD_SIZE_MODIFIER,                // 보드 크기 수정 (아이템은 X)
    BOARD_CORNER_BLOCK,                 // 보드 가장자리 제한
    BOARD_RANDOM_BLOCK,                 // 보드 무작위 N칸 제한
    DECK_MODIFIER,                      // x 덱 수정
    BLOCK_REUSE_MODIFIER,               // x 블록 재사용
    SQUARE_CLEAR,                       // x 사각형 클리어
    BLOCK_MULTIPLIER,                   // 덱의 특정 블록 N배
    BLOCK_DELETE,                       // 덱의 특정 블록 모두 삭제
    BLOCK_DELETE_WITH_COUNT,            // 덱의 특정 블록 N개 삭제
    RANDOM_ROW_LINE_CLEAR,              // 무작위 가로 1줄 지움
    RANDOM_COLUMN_LINE_CLEAR,           // 무작위 세로 1줄 지움
    BOARD_CLEAR,                        // 보드의 모든 블록 지움
    DRAW_BLOCK_COUNT_MODIFIER,          // 블록 선택지 수정
    RANDOM_LINE_CLEAR,                  // 무작위 1줄 지움
    ROW_LINE_CLEAR,                     // 가로 N줄 지움 (위쪽에서부터)
    COLUMN_LINE_CLEAR,                  // 세로 N줄 지움 (왼쪽에서부터)
    EFFECT_VALUE_MODIFIER,              // 다른 효과 수치 수정
    SHOP_REROLL_COST_MODIFIER,          // 상점 리롤 비용 수정
    SHOP_REROLL_PERCENTAGE_MODIFIER,    // 상점 리롤 비용 증가 확률 수정
    SHOP_ITEM_COUNT_MODIFIER,           // 상점 아이템 개수 수정
    SHOP_BOOST_COUNT_MODIFIER,          // 상점 부스트 개수 수정
    SHOP_BLOCK_COUNT_MODIFIER,          // 상점 블록 개수 수정
    COMMON_WEIGHTS_MULTIPLIER,          // 상점 일반 등장 확률 N배
    RARE_WEIGHTS_MULTIPLIER,            // 상점 희귀 등장 확률 N배
    EPIC_WEIGHTS_MULTIPLIER,            // 상점 특급 등장 확률 N배
    LEGENDARY_WEIGHTS_MULTIPLIER,       // 상점 전설 등장 확률 N배
    MULTIPLIER_MULTIPLER,               // 배수 N배
}

public enum TriggerType
{
    ON_ACQUIRE,                             // 획득할 때 (즉시)
    ON_BLOCK_PLACE,                         // 블록을 배치할 때 (특정 가능)
    ON_BLOCK_PLACE_WITHOUT_LINE_CLEAR,      // 블록을 배치할 때 줄이 지워지지 않으면
    ON_BLOCK_PLACE_WITH_LINE_CLEAR,         // 특정 블록을 배치하여 줄을 지울 때
    ON_BLOCK_CLEAR,                         // 블록이 모두 지워질 때
    ON_ROW_LINE_CLEAR,                      // 가로로 줄을 지울 때
    ON_COLUMN_LINE_CLEAR,                   // 세로로 줄을 지울 때
    ON_CROSS_LINE_CLEAR,                    // 가로, 세로를 한 번에 지울 때
    ON_MULTIPLE_LINE_CLEAR,                 // 여러 줄을 한 번에 지울 때
    ON_LINE_CLEAR,                          // 줄을 지울 때
    ON_LINE_CLEAR_WITH_COUNT,               // 줄을 N번째 지울 때
    ON_LINE_CLEAR_WITH_SPECIFIC_BLOCKS,     // 줄을 지울 때 특정 블록이 포함돼있으면
    ON_LINE_CLEAR_WITH_SAME_BLOCK,          // 줄을 지울 때 같은 종류의 블록이 있으면
    ON_LINE_CLEAR_WITH_DISTINCT_BLOCKS,     // 줄을 지울 때 모두 다른 종류의 블록이면
    ON_LINE_CLEAR_CONSECUTIVELY,            // 연속으로 줄을 지울 때 
    ON_FIRST_LINE_CLEAR,                    // 처음 줄이 지워질 때
    ON_DECK_EMPTY,                          // 덱이 비었을 때
    ON_REROLL,                              // 리롤할 때
    ON_REROLL_WITHOUT_PLACE,                // 블록을 배치하지 않고 리롤할 때
    ON_BOARD_HALF_FULL,                     // 보드가 반 이상 찼을 때
    ON_BOARD_NOT_HALF_FULL,                 // 보드가 반 이상 차지 않았을 때
    ON_GOLD_CHANGE,                         // x 골드가 바뀔 때
    ON_ENTER_STAGE,                         // 스테이지 진입할 때
    ON_BLOCK_PLACE_WITH_COUNT,              // 특정 블록을 N번 배치할 때
    ON_END_STAGE,                           // 스테이지 끝날 때
    ON_REROLL_SPECIFIC_BLOCK,               // 특정 블록을 리롤할 때
    ON_ROTATE_BLOCK,                        // 특정 블록을 회전할 때
    ON_ADD_BLOCK,                           // 특정 블록을 덱에 추가할 때
    ON_SHOP_REROLL,                         // 상점에서 리롤할 때
    ON_ITEM_PURCHASE,                       // 아이템을 살 때
}

public enum TriggerMode
{
    None, Exact, Interval,
}

public enum EffectScope
{
    Run, Stage
}

public enum ScalingFactor
{
    None, RemainingBlockCount, BoostCount, CurrentGold, RerollCount
}

public enum ItemType
{
    ADD_BLOCK, CONVERT_BLOCK, ITEM, BOOST, UPGRADE
}

public enum ItemEffectType
{
    SCORE, DECK, GOLD, OTHER
}

public enum ItemRarity
{
    COMMON, RARE, EPIC, LEGENDARY
}

public enum DeckType
{
    Default, YoYo, Dice, Telescope, Mirror, Bomb
}

// 해금 관련 -----------------------------------------------------------
public enum UnlockTarget 
{
    Item, Deck 
};

public enum UnlockTrigger
{
    placeCountI,
    placeCountO,
    placeCountZ,
    placeCountS,
    placeCountJ,
    placeCountL,
    placeCountT,
    RerollCount,
    ItemPurchaseCount,
    ShopRerollCount,
    MaxScore,
    MaxChapter,
    WinCount,
    BoardHalfFullCount,
    RerollCountIO,
    RerollCountZS,
    RerollCountJLT,
    MaxBaseMultiplier,
    MaxMultiplier,
    MaxBaseRerollCount,
    MaxGold,
    HasOnlyIO,
    HasOnlyZS,
    HasOnlyJL,
    DefaultDeckWinCount,
    YoYoDeckWinCount,
    DiceDeckWinCount,
    TelescopeDeckWinCount,
    MirrorDeckWinCount,
    BombDeckWinCount,
    ClearedMaxLevel,
    placeCountIO,
    placeCountZS,
    placeCountJL,
}
// -----------------------------------------------------------------------

public enum AnimationType
{
    None,
    Delay,
    UpdateChip,
    UpdateBlockScore,
    UpdateMultiplier,
    UpdateProduct,
    UpdateScore,
    UpdateReroll,
    UpdateGold,
    LineClear,
    ItemEffect,
    StageClear,
    ItemActivated,
    ItemDeactivated,
}

public class Enums
{
    public static bool IsDefaultBlockType(BlockType blockType)
    {
        switch (blockType)
        {
            case BlockType.I:
            case BlockType.O:
            case BlockType.Z:
            case BlockType.S:
            case BlockType.J:
            case BlockType.L:
            case BlockType.T:
                return true;
            default:
                return false;
        }
    }

    public static bool IsSpecialBlockType(BlockType blockType)
    {
        return !IsDefaultBlockType(blockType);
    }

    public static T GetEnumByString<T>(string enumName)
    {
        return (T)Enum.Parse(typeof(T), enumName);
    }

    public static Array GetEnumArray<T>()
    {
        return Enum.GetValues(typeof(T));
    }

    public static T GetRandomEnum<T>()
    {
        Array array = GetEnumArray<T>();
        return (T)array.GetValue(Random.Range(0, array.Length));
    }
}
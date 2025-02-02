using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    MEGASQUARE,
    ULTRASQUARE,
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
    SCORE_MODIFIER,             // * 점수 수정
    SCORE_MULTIPLIER,           // 점수 N배
    MULTIPLIER_MODIFIER,        // * 배수 수정
    BASEMULTIPLIER_MODIFIER,    // * 기본 배수 수정
    BASEMULTIPLIER_MULTIPLIER,  // * 기본 배수 N배
    REROLL_MODIFIER,            // * 리롤 횟수 수정
    REROLL_MULTIPLIER,          // * 리롤 횟수 N배
    BASEREROLL_MODIFIER,        // * 기본 리롤 횟수 수정
    BASEREROLL_MULTIPLIER,      // * 기본 리롤 횟수 N배
    GOLD_MODIFIER,              // * 골드 수정
    GOLD_MULTIPLIER,            // * 골드 N배
    BOARD_SIZE_MODIFIER,        // * 보드 크기 수정 (아이템은 X)
    BOARD_CORNER_BLOCK,         // * 보드 가장자리 제한
    BOARD_RANDOM_BLOCK,         // * 보드 무작위 N칸 제한
    DECK_MODIFIER,              // x 덱 수정
    BLOCK_REUSE_MODIFIER,       // x 블록 재사용
    SQUARE_CLEAR,               // x 사각형 클리어
    BLOCK_MULTIPLIER,           // * 덱의 특정 블록 N배
    BLOCK_DELETE,               // * 덱의 특정 블록 모두 삭제
    BLOCK_DELETE_WITH_COUNT,    // x 덱의 특정 블록 N개 삭제
    ROW_LINE_CLEAR,             // * 무작위 가로 1줄 지움
    COLUMN_LINE_CLEAR,          // * 무작위 세로 1줄 지움
    BOARD_CLEAR,                // x 보드의 모든 블록 지움
    DRAW_BLOCK_COUNT_MODIFIER,  // * 블록 선택지 수정
    RANDOM_LINE_CLEAR,          // * 무작위 1줄 지움
}

public enum TriggerType
{
    ON_ACQUIRE,                             // * 획득할 때 (즉시)
    ON_BLOCK_PLACE,                         // * 블록을 배치할 때 (특정 가능)
    ON_BLOCK_PLACE_WITHOUT_LINE_CLEAR,      // * 블록을 배치할 때 줄이 지워지지 않으면
    ON_BLOCK_PLACE_WITH_LINE_CLEAR,         // * 특정 블록을 배치하여 줄을 지울 때
    ON_BLOCK_CLEAR,                         // * 블록이 모두 지워질 때
    ON_ROW_LINE_CLEAR,                      // * 가로로 줄을 지울 때
    ON_COLUMN_LINE_CLEAR,                   // * 세로로 줄을 지울 때
    ON_CROSS_LINE_CLEAR,                    // * 가로, 세로를 한 번에 지울 때
    ON_MULTIPLE_LINE_CLEAR,                 // * 여러 줄을 한 번에 지울 때
    ON_LINE_CLEAR,                          // * 줄을 지울 때
    ON_LINE_CLEAR_WITH_COUNT,               // * 줄을 N번째 지울 때
    ON_LINE_CLEAR_WITH_SPECIFIC_BLOCKS,     // * 줄을 지울 때 특정 블록이 포함돼있으면
    ON_LINE_CLEAR_WITH_SAME_BLOCK,          // * 줄을 지울 때 같은 종류의 블록이 있으면
    ON_LINE_CLEAR_WITH_DISTINCT_BLOCKS,     // * 줄을 지울 때 모두 다른 종류의 블록이면
    ON_LINE_CLEAR_CONSECUTIVELY,            // * 연속으로 줄을 지울 때 
    ON_FIRST_LINE_CLEAR,                    // * 처음 줄이 지워질 때
    ON_DECK_EMPTY,                          // * 덱이 비었을 때
    ON_REROLL,                              // * 리롤할 때
    ON_REROLL_WITHOUT_PLACE,                // * 블록을 배치하지 않고 리롤할 때
    ON_BOARD_HALF_FULL,                     // * 보드가 반 이상 찼을 때
    ON_BOARD_NOT_HALF_FULL,                 // * 보드가 반 이상 차지 않았을 때
    ON_GOLD_CHANGE,                         // x 골드가 바뀔 때
    ON_ENTER_STAGE,                         // x 스테이지 진입할 때
    ON_BLOCK_PLACE_WITH_COUNT,              // * 특정 블록을 N번 배치할 때마다
}

public enum EffectScope
{
    Run, Stage
}

public enum ItemType
{
    ADD_BLOCK, DELETE_BLOCK, ITEM, UPGRADE
}

public enum ItemEffectType
{
    SCORE, DECK, GOLD, OTHER
}

public enum ItemRarity
{
    SILVER, GOLD, PLATINUM
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "BlockBurst/Effect")]
public class EffectData : ScriptableObject
{
    public string id;                                // 효과 ID
    public EffectType type;                         // 효과 타입
    public TriggerType trigger;                     // 발동 조건
    public Dictionary<string, float> modifiers;      // 데이터 수정자
}

public enum StageType
{
    NORMAL1, NORMAL2, BOSS
}

public enum EffectType
{
    SCORE_MODIFIER,      // 점수 수정
    MULTIPLIER_MODIFIER, // 배율 수정
    REROLL_MODIFIER,     // 리롤 수정
    GOLD_MODIFIER,       // 골드 수정
    BOARD_MODIFIER,      // 보드 수정
    DECK_MODIFIER,       // 덱 수정
    BLOCK_REUSE,        // 블록 재사용
    SPECIAL_CLEAR       // 특수 클리어
}

public enum TriggerType
{
    ON_BLOCK_PLACE,          // 블록 배치 시
    ON_BLOCK_CLEAR,          // 블록 제거 시
    ON_LINE_CLEAR,           // 라인 클리어 시
    ON_LINE_CLEAR_WITH_BLOCK, // 특정 블록으로 라인 클리어 시
    ON_DECK_EMPTY,           // 덱이 비었을 때
    ON_REROLL,              // 리롤 시
    ON_BOARD_STATE,         // 보드 상태 조건 충족 시
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "BlockBurst/Effect")]
public class EffectData : ScriptableObject
{
    public string id;                                // ȿ�� ID
    public EffectType type;                         // ȿ�� Ÿ��
    public TriggerType trigger;                     // �ߵ� ����
    public Dictionary<string, float> modifiers;      // ������ ������
}

public enum StageType
{
    NORMAL1, NORMAL2, BOSS
}

public enum EffectType
{
    SCORE_MODIFIER,      // ���� ����
    MULTIPLIER_MODIFIER, // ���� ����
    REROLL_MODIFIER,     // ���� ����
    GOLD_MODIFIER,       // ��� ����
    BOARD_MODIFIER,      // ���� ����
    DECK_MODIFIER,       // �� ����
    BLOCK_REUSE,        // ��� ����
    SPECIAL_CLEAR       // Ư�� Ŭ����
}

public enum TriggerType
{
    ON_BLOCK_PLACE,          // ��� ��ġ ��
    ON_BLOCK_CLEAR,          // ��� ���� ��
    ON_LINE_CLEAR,           // ���� Ŭ���� ��
    ON_LINE_CLEAR_WITH_BLOCK, // Ư�� ������� ���� Ŭ���� ��
    ON_DECK_EMPTY,           // ���� ����� ��
    ON_REROLL,              // ���� ��
    ON_BOARD_STATE,         // ���� ���� ���� ���� ��
}
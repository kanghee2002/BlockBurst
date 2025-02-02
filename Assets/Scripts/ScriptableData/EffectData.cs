using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "BlockBurst/Effect")]
public class EffectData : ScriptableObject
{
    public string id;                               // 효과 ID
    [TextArea] public string effectName;            // 효과 이름
    public EffectType type;                         // 효과 타입
    public TriggerType trigger;                     // 발동 조건
    public EffectScope scope;                       // 효과 범위
    public int effectValue;                         // 데이터 수정치

    [Header("Additional")]
    public BlockType[] blockTypes = null;
    public int blockId = -1;
    public float probability = 1;                   // 발동 확률
    public int triggerValue = 0;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "BlockBurst/Effect")]
public class EffectData : ScriptableObject
{
    public string id;                               // 효과 ID
    public EffectType type;                         // 효과 타입
    public TriggerType trigger;                     // 발동 조건
    public Dictionary<string, float> modifiers;     // 데이터 수정자
}
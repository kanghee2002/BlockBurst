using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "BlockBurst/Effect")]
public class EffectData : ScriptableObject
{
    public string id;                               // 효과 ID
    [TextArea] public string effectName;            // 효과 이름
    public EffectType type;                         // 효과 타입
    public EffectScope scope;                       // 효과 범위
    public int baseEffectValue;                     // 기본 데이터 수정치
    [HideInInspector] public int effectValue;       // 데이터 수정치
    
    [Header("Trigger Setting")]
    public TriggerType trigger;                     // 발동 조건
    public TriggerMode triggerMode = TriggerMode.None;
    public int triggerValue = 0;                    // 발동하는 트리거 값
    [HideInInspector] public int triggerCount = 0;  // 현재 트리거 값

    [Header("Additional")]
    public BlockType[] blockTypes = null;
    public float probability = 1;                   // 발동 확률
    public EffectData modifyingEffect;              // 수정할 데이터
    public int maxValue = -1;                       // 최대 수치

    [Header("Scaling Factor")]
    public ScalingFactor scalingFactor;             // 효과 계수 타입
    public float scalingMultiplier = 1f;            // 효과 계수 조정치
}
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 런타임 효과 인스턴스. SO 메타는 <see cref="effectDataId"/>로 외부(<see cref="ScriptableDataManager"/> 등)에서 조회하고, 여기서는 id와 런타임 필드만 둔다.
/// </summary>
[Serializable]
public class EffectState
{
    public string id;
    public string resourceKey;
    public string effectDataId;
    public int effectValue;
    public int triggerCount;
    /// <summary><see cref="EffectType.EFFECT_VALUE_MODIFIER"/>가 수정할 대상 state. 등록 시 해석.</summary>
    public string modifyingTargetStateId;

    public static EffectState CreateFromTemplate(EffectData effect, IReadOnlyList<EffectState> existingActiveEffects = null)
    {
        if (effect == null)
            throw new ArgumentNullException(nameof(effect));

        var state = new EffectState
        {
            id = Guid.NewGuid().ToString(),
            resourceKey = effect.resourceKey.ToString(),
            effectDataId = effect.id,
            effectValue = effect.baseEffectValue,
            triggerCount = 0,
            modifyingTargetStateId = null
        };

        if (effect.type == EffectType.EFFECT_VALUE_MODIFIER)
            state.TryResolveModifyingTarget(effect.modifyingEffect, existingActiveEffects);

        return state;
    }

    /// <summary>
    /// <paramref name="existingActiveEffects"/>에 아직 이 인스턴스는 포함되지 않은 것으로 보고,
    /// <paramref name="targetEffect"/>의 id와 같은 <see cref="effectDataId"/>를 가진 선행 state를 끝에서부터 찾아 <see cref="modifyingTargetStateId"/>에 넣는다.
    /// </summary>
    /// <param name="targetEffect"><see cref="EffectData.modifyingEffect"/>에 해당하는 SO. null이면 아무 것도 하지 않는다.</param>
    public void TryResolveModifyingTarget(EffectData targetEffect, IReadOnlyList<EffectState> existingActiveEffects)
    {
        if (targetEffect == null)
            return;

        string targetEffectId = targetEffect.id;
        if (string.IsNullOrEmpty(targetEffectId))
            return;

        if (existingActiveEffects == null)
        {
            modifyingTargetStateId = null;
            return;
        }

        for (int i = existingActiveEffects.Count - 1; i >= 0; i--)
        {
            EffectState candidate = existingActiveEffects[i];
            if (candidate.effectDataId == targetEffectId)
            {
                modifyingTargetStateId = candidate.id;
                return;
            }
        }

        modifyingTargetStateId = null;
        Debug.LogWarning(
            $"EFFECT_VALUE_MODIFIER: 등록 시점에 대상 효과 '{targetEffect.name}'에 해당하는 선행 EffectState가 없습니다. 발동 시 템플릿 매칭으로 재시도합니다.");
    }
    
    /// <summary>
    /// 저장·역직렬화 시 <see cref="RunData"/>와 DTO 간 참조를 나누기 위한 얕은 복제.
    /// JsonUtility는 빈 문자열을 쓰는 경우가 있어, 빈 문자열은 null로 맞춘다.
    /// </summary>
    public EffectState Clone()
    {
        return new EffectState
        {
            id = NormalizePersistString(id),
            resourceKey = NormalizePersistString(resourceKey),
            effectDataId = NormalizePersistString(effectDataId),
            effectValue = effectValue,
            triggerCount = triggerCount,
            modifyingTargetStateId = NormalizePersistString(modifyingTargetStateId)
        };
    }

    static string NormalizePersistString(string value)
    {
        if (string.IsNullOrEmpty(value))
            return null;
        return value;
    }
}

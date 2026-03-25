using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 런타임 효과 인스턴스. 메타는 <see cref="template"/>에서 읽고, effectValue/triggerCount만 여기서 갱신한다.
/// </summary>
[Serializable]
public class EffectState
{
    public Guid id;
    public EffectData template;
    public int effectValue;
    public int triggerCount;
    /// <summary><see cref="EffectType.EFFECT_VALUE_MODIFIER"/>가 수정할 대상 state. 등록 시 해석.</summary>
    public Guid? modifyingTargetStateId;

    public static EffectState CreateFromTemplate(EffectData effect, IReadOnlyList<EffectState> existingActiveEffects = null)
    {
        if (effect == null)
            throw new ArgumentNullException(nameof(effect));

        var state = new EffectState
        {
            id = Guid.NewGuid(),
            template = effect,
            effectValue = effect.baseEffectValue,
            triggerCount = 0,
            modifyingTargetStateId = null
        };
        state.TryResolveModifyingTarget(existingActiveEffects);
        return state;
    }
    
    /// <summary>
    /// <paramref name="existingActiveEffects"/>에 아직 이 인스턴스는 포함되지 않은 것으로 보고,
    /// <see cref="EffectType.EFFECT_VALUE_MODIFIER"/>인 경우 끝에서 가까운 쪽부터
    /// <see cref="EffectData.modifyingEffect"/>와 같은 템플릿 참조를 가진 state를 대상으로 지정한다.
    /// </summary>
    public void TryResolveModifyingTarget(IReadOnlyList<EffectState> existingActiveEffects)
    {
        if (template.type != EffectType.EFFECT_VALUE_MODIFIER)
            return;

        EffectData targetTemplate = template.modifyingEffect;
        if (targetTemplate == null)
            return;

        if (existingActiveEffects == null)
        {
            modifyingTargetStateId = null;
            return;
        }

        for (int i = existingActiveEffects.Count - 1; i >= 0; i--)
        {
            EffectState candidate = existingActiveEffects[i];
            if (candidate.template == targetTemplate)
            {
                modifyingTargetStateId = candidate.id;
                return;
            }
        }

        modifyingTargetStateId = null;
        Debug.LogWarning(
            $"EFFECT_VALUE_MODIFIER '{template.name}': 등록 시점에 modifyingEffect '{targetTemplate.name}'에 해당하는 선행 EffectState가 없습니다. 발동 시 템플릿 매칭으로 재시도합니다.");
    }
}

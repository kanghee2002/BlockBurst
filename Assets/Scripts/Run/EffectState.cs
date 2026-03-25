using System;
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

    public static EffectState CreateFromTemplate(EffectData effect)
    {
        if (effect == null)
            throw new ArgumentNullException(nameof(effect));

        return new EffectState
        {
            id = Guid.NewGuid(),
            template = effect,
            effectValue = effect.baseEffectValue,
            triggerCount = 0
        };
    }
}

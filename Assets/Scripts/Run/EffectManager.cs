using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private RunData runData;

    public void Initialize(ref RunData data)
    {
        runData = data;
    }

    public void AddEffect(EffectData effect)
    {
        runData.activeEffects.Add(effect);
    }

    public bool RemoveEffect(EffectData effect)
    {
        return runData.activeEffects.Remove(effect);
    }

    public void TriggerEffects(TriggerType trigger)
    {
        foreach (EffectData effect in runData.activeEffects)
        {
            if (effect.trigger == trigger)
            {
                ApplyEffect(effect);
            }
        }
    }

    private void ApplyEffect(EffectData effect)
    {
        // 효과 적용
        // 흠흐밍...
    }
}
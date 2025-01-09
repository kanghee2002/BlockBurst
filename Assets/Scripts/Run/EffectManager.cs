using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EffectManager : MonoBehaviour
{
    private RunData runData;
    private BlockGameData gameData;
    private Dictionary<TriggerType, List<EffectData>> triggerEffects;

    public void Initialize(RunData runData, BlockGameData blockGameData)
    {
        this.runData = runData;
        gameData = blockGameData;
        triggerEffects = new Dictionary<TriggerType, List<EffectData>>();

        TriggerType[] allTriggers = Enum.GetValues(typeof(TriggerType)).Cast<TriggerType>().ToArray();

        foreach (TriggerType trigger in allTriggers)
        {
            triggerEffects.Add(trigger, new List<EffectData>());
        }

    }

    // 효과 추가
    public void AddEffect(EffectData effect)
    {
        if (effect.trigger == TriggerType.ON_ACQUIRE)
        {
            //효과 발동
            ApplyEffect(effect);
        }
        
        triggerEffects[effect.trigger].Add(effect);
    }

    // 효과 제거
    public void RemoveEffect(EffectData effect)
    {
        for (int i = 0; i < triggerEffects[effect.trigger].Count; i++)
        {
            if (triggerEffects[effect.trigger][i].id == effect.id)
            {
                triggerEffects[effect.trigger].RemoveAt(i);
            }
        }
    }

    // 효과 트리거
    public void TriggerEffects(TriggerType trigger, List<BlockType> blockType = null)
    {

    }

    // 효과 적용
    private void ApplyEffect(EffectData effect)
    {

    }
}
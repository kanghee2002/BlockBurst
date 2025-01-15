using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private RunData runData;    // GM에서 받아온 RunData
    public StageData currentStage;

    public void Initialize(ref RunData data)
    {
        runData = data;
    }

    public void StartStage(StageData stage)
    {
        currentStage = stage;
        ApplyConstraints();

        EffectManager.instance.TriggerEffects(TriggerType.ON_ENTER_STAGE);
    }

    public bool CheckStageClear(BlockGameData blockGameData)
    {
        // 스테이지 클리어 조건 확인
        return (blockGameData.currentScore >= currentStage.clearRequirement);
    }

    private void ApplyConstraints()
    {
        // Effect들을 추가
        currentStage.constraints.ForEach(constraint =>
        {
            EffectManager.instance.AddEffect(constraint);
        });
    }

    public void GrantReward()
    {
        // 보상 지급
        runData.gold += currentStage.goldReward;
    }

    // 스테이지 클리어 시 적용된 제한 제거
    private void RemoveConstraints()
    {
        currentStage.constraints.ForEach(constraint =>
        {
            EffectManager.instance.RemoveEffect(constraint);
        });
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("DEBUG")]
    // 첫 스테이지 선택에 나올 스테이지 지정
    public List<string> firstStageList;

    private RunData runData;    // GM에서 받아온 RunData
    public StageData currentStage;

    public void Initialize(ref RunData data)
    {
        runData = data;
    }

    public void StartStage(StageData stage)
    {
        EffectManager.instance.TriggerEffects(TriggerType.ON_ENTER_STAGE);

        currentStage = stage;
        ApplyConstraints();

        EffectManager.instance.TriggerEffects(TriggerType.ON_BOARD_NOT_HALF_FULL);

        EffectManager.instance.EndTriggerEffect();
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
        GameManager.instance.UpdateGold(currentStage.goldReward, isStageReward: true);
        RemoveConstraints();
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
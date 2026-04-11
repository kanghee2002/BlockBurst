using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("DEBUG")]
    // 첫 스테이지 선택에 나올 스테이지 지정 (GameManager에서 resourceKey와 대소문자 무시 매칭)
    public List<string> firstStageList;

    private readonly List<StageData> currentStages = new List<StageData>();

    /// <summary>현재 적용 중인 제한(StageData) 목록.</summary>
    public IReadOnlyList<StageData> CurrentStages => currentStages;

    private readonly List<EffectState> activeConstraintStates = new List<EffectState>();

    public void StartStage(IReadOnlyList<StageData> stages)
    {
        if (stages == null || stages.Count == 0)
        {
            Debug.LogError("StageManager.StartStage: stages가 비어 있습니다.");
            return;
        }

        EffectManager.instance.TriggerEffects(TriggerType.ON_ENTER_STAGE);

        currentStages.Clear();
        foreach (StageData stage in stages)
        {
            currentStages.Add(stage);
        }

        ApplyConstraints();

        EffectManager.instance.TriggerEffects(TriggerType.ON_BOARD_NOT_HALF_FULL);

        EffectManager.instance.EndTriggerEffect();
    }

    public bool CheckStageClear(BlockGameData blockGameData)
    {
        return blockGameData.currentScore >= blockGameData.clearRequirement;
    }

    private void ApplyConstraints()
    {
        activeConstraintStates.Clear();
        foreach (StageData stage in currentStages)
        {
            foreach (EffectData constraint in stage.constraints)
            {
                EffectState state = EffectManager.instance.AddEffect(constraint);
                if (state != null)
                {
                    activeConstraintStates.Add(state);
                }
            }
        }
    }

    public void GrantReward()
    {
        GameManager.instance.UpdateGold(GameManager.instance.blockGame.goldReward, isStageReward: true);
        RemoveConstraints();
    }

    private void RemoveConstraints()
    {
        foreach (EffectState state in activeConstraintStates)
        {
            EffectManager.instance.RemoveEffect(state);
        }

        activeConstraintStates.Clear();
    }

    public void AddFirstStage(List<string> stages)
    {
        foreach (string stage in stages)
        {
            firstStageList.Add(stage);
        }
    }
}

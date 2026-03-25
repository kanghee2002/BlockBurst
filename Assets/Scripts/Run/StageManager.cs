using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("DEBUG")]
    // 첫 스테이지 선택에 나올 스테이지 지정
    public List<string> firstStageList;

    public StageData currentStage;
    private BlockGameData blockGameData;

    private readonly List<EffectState> activeConstraintStates = new();

    public void StartStage(StageData stage, BlockGameData blockGame)
    {
        blockGameData = blockGame;

        EffectManager.instance.TriggerEffects(TriggerType.ON_ENTER_STAGE);

        currentStage = stage;
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
        foreach (EffectData constraint in currentStage.constraints)
        {
            EffectState state = EffectManager.instance.AddEffect(constraint);
            if (state != null)
                activeConstraintStates.Add(state);
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

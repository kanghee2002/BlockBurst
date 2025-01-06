using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private RunData runData;
    private StageData currentStage;
    private BlockGameData gameData;

    // 스테이지 시작
    public void StartStage(StageData stage)
    {

    }

    // 스테이지 클리어 체크
    public bool CheckStageClear()
    {
        return true;
    }

    // 스테이지 제한사항 적용
    private void ApplyConstraints()
    {

    }

    // 클리어 보상 지급
    private void GrantReward()
    {

    }
}
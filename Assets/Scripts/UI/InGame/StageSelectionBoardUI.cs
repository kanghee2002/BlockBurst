using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectionBoardUI : MonoBehaviour
{
    [SerializeField] private GameUIManager gameUIManager;
    [SerializeField] private NextStageChoiceUI[] nextStageChoiceUI;

    public void OnNextStageChoiceButtonUIPressed(int choiceIndex)
    {
        gameUIManager.NextStageChoiceButtonUIPressed(choiceIndex);
    }
    public void OpenNextStageChoiceUI()
    {

    }

    public void CloseNextStageChoiceUI()
    {

    }
}

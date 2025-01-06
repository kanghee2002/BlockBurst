using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextStageChoiceButtonUI : MonoBehaviour
{
    [SerializeField] private GameUIManager gameUIManager;

    [SerializeField] private int choiceIndex;

    public void OnNextStageChoiceButtonUIPressed()
    {
        gameUIManager.NextStageChoiceButtonUIPressed(choiceIndex);
    }
}

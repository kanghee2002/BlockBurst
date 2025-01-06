using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextStageButtonUI : MonoBehaviour
{
    [SerializeField] private GameUIManager gameUIManager;

    public void OpenNextStageButtonUI()
    {

    }

    public void CloseNextStageButtonUI()
    {

    }

    public void OnNextStageButtonUIPressed()
    {
        gameUIManager.NextStageButtonUIPressed();
    }
}

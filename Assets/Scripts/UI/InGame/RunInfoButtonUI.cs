using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RunInfoButtonUI : MonoBehaviour
{

    [SerializeField] private GameUIManager gameUIManager;

    public void OnRunInfoButtonUIPressed()
    {
        gameUIManager.RunInfoButtonUIPressed();
    }
}

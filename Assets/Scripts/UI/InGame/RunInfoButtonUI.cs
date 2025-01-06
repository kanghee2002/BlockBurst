using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunInfoButtonUI : MonoBehaviour
{

    [SerializeField] private GameUIManager gameUIManager;

    public void OnRunInfoButtonUIPressed()
    {
        gameUIManager.RunInfoButtonUIPressed();
    }
}

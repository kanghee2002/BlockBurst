using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RunInfoButtonUI : ButtonUI
{

    [SerializeField] private GameUIManager gameUIManager;

    public override void OnClick()
    {
        gameUIManager.RunInfoButtonUIPressed();
    }
}

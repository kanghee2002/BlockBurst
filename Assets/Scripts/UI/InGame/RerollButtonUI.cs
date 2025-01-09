using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RerollButtonUI : MonoBehaviour
{
    [SerializeField] private GameUIManager gameUIManager;

    public void OnRerollButtonUIPressed()
    {
        gameUIManager.RerollButtonUIPressed();
    }
    public void OpenRerollButtonUI()
    {

    }

    public void CloseRerollButtonUI()
    {

    }
}

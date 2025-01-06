using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButtonUI : MonoBehaviour
{

    [SerializeField] private GameUIManager gameUIManager;

    public void OnOptionButtonUIPressed()
    {
        gameUIManager.OptionButtonUIPressed();
    }
}

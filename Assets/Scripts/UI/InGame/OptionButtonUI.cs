using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButtonUI : MonoBehaviour
{

    [SerializeField] private GameObject gameUIManagerInstance;
    private GameUIManager gameUIManager;

    private void Start()
    {
        initializeInstances();
    }

    private void initializeInstances()
    {
        gameUIManager = gameUIManagerInstance.GetComponent<GameUIManager>();
    }

    public void OnOptionButtonUIPressed()
    {
        gameUIManager.optionButtonUIPressed();
    }
}

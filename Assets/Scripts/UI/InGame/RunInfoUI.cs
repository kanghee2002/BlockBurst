using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunInfoUI : MonoBehaviour
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

    public void openRunInfoUI()
    {
        Debug.Log("RunInfoUI가 화면 밑에서 위로 올라옴");
    }

    public void closeRunInfoUI()
    {
        Debug.Log("RunInfoUI가 화면 중앙에서 아래로 내려감");
    }

    public void OnRunInfoBackButtonUIPressed()
    {
        gameUIManager.runInfoBackButtonUIPressed();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionUI : MonoBehaviour
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

    public void OpenOptionUI()
    {
        Debug.Log("OptionInfoUI가 화면 밑에서 위로 올라옴");
    }

    public void CloseOptionUI()
    {
        Debug.Log("OptionUI가 화면 중앙에서 아래로 내려감");
    }

    public void OnOptionBackButtonUIPressed()
    {
        gameUIManager.OptionBackButtonUIPressed();
    }
}

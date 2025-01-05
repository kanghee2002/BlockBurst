using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckInfoUI : MonoBehaviour
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

    public void OpenDeckInfoUI()
    {
        Debug.Log("DeckInfoUI가 화면 위에서 아래로 내려옴");
    }

    public void CloseDeckInfoUI()
    {
        Debug.Log("DeckInfoUI가 화면 중앙에서 위로 올라감");
    }

    public void OnDeckInfoBackButtonUIPressed()
    {
        gameUIManager.DeckInfoBackButtonUIPressed();
    }
}

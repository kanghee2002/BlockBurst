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
        Debug.Log("DeckInfoUI�� ȭ�� ������ �Ʒ��� ������");
    }

    public void CloseDeckInfoUI()
    {
        Debug.Log("DeckInfoUI�� ȭ�� �߾ӿ��� ���� �ö�");
    }

    public void OnDeckInfoBackButtonUIPressed()
    {
        gameUIManager.DeckInfoBackButtonUIPressed();
    }
}

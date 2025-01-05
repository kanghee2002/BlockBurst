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
        Debug.Log("RunInfoUI�� ȭ�� �ؿ��� ���� �ö��");
    }

    public void closeRunInfoUI()
    {
        Debug.Log("RunInfoUI�� ȭ�� �߾ӿ��� �Ʒ��� ������");
    }

    public void OnRunInfoBackButtonUIPressed()
    {
        gameUIManager.runInfoBackButtonUIPressed();
    }
}

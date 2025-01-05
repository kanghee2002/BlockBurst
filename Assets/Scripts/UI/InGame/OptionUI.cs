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
        Debug.Log("OptionInfoUI�� ȭ�� �ؿ��� ���� �ö��");
    }

    public void CloseOptionUI()
    {
        Debug.Log("OptionUI�� ȭ�� �߾ӿ��� �Ʒ��� ������");
    }

    public void OnOptionBackButtonUIPressed()
    {
        gameUIManager.OptionBackButtonUIPressed();
    }
}

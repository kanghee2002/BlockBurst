using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunInfoUI : MonoBehaviour
{

    [SerializeField] private GameUIManager gameUIManager;

    public void OpenRunInfoUI()
    {
        Debug.Log("RunInfoUI�� ȭ�� �ؿ��� ���� �ö��");
    }

    public void CloseRunInfoUI()
    {
        Debug.Log("RunInfoUI�� ȭ�� �߾ӿ��� �Ʒ��� ������");
    }

    public void OnRunInfoBackButtonUIPressed()
    {
        gameUIManager.RunInfoBackButtonUIPressed();
    }
}

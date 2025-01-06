using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionUI : MonoBehaviour
{

    [SerializeField] private GameUIManager gameUIManager;

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

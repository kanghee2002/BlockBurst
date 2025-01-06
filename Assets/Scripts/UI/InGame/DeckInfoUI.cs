using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckInfoUI : MonoBehaviour
{

    [SerializeField] private GameUIManager gameUIManager;

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

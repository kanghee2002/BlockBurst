using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoardUI : MonoBehaviour
{
    [SerializeField] private GameUIManager gameUIManager;
    public void OnNextStageButtonUIPressed()
    {
        gameUIManager.NextStageButtonUIPressed();
    }
    public void OnItemRerollButtonUIPressed()
    {
        gameUIManager.ItemRerollButtonUIPressed();
    }

    public void OpenItemBoardUI()
    {
        gameObject.SetActive(true);
    }

    public void CloseItemBoardUI()
    {
        gameObject.SetActive(false);
    }
}

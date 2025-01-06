using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRerollButtonUI : MonoBehaviour
{
    [SerializeField] private GameUIManager gameUIManager;

    public void OpenRerollButtonUI()
    {

    }

    public void CloseRerollButtonUI()
    {

    }

    public void OnItemRerollButtonUIPressed()
    {
        gameUIManager.ItemRerollButtonUIPressed();
    }
}

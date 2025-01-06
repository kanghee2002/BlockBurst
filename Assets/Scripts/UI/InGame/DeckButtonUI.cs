using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckButtonUI : MonoBehaviour
{

    [SerializeField] private GameUIManager gameUIManager;

    public void OnDeckButtonUIPressed()
    {
        gameUIManager.DeckButtonUIPressed();
    }
}

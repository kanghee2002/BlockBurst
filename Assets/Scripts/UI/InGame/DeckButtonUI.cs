using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckButtonUI : MonoBehaviour
{

    [SerializeField] private GameUIManager gameUIManager;
    [SerializeField] private TextMeshProUGUI deckCountText;

    public void OnDeckButtonUIPressed()
    {
        gameUIManager.DeckButtonUIPressed();
    }

    public void DisplayDeckCount(int deckCount, int maxDeckCount)
    {
        deckCountText.text = deckCount.ToString() + "/" + maxDeckCount.ToString();
    }
}

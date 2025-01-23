using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deckCountText;

    public void DisplayDeckCount(int deckCount, int maxDeckCount)
    {
        deckCountText.text = deckCount.ToString() + "/" + maxDeckCount.ToString();
        UIUtils.BounceText(deckCountText.transform);
    }
}

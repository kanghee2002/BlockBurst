using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeckButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deckCountText;
    public TextMeshProUGUI deckTitleText;
    public Image bg;

    public void DisplayDeckCount(int deckCount, int maxDeckCount)
    {
        // deckCountText.text = deckCount.ToString() + "/" + maxDeckCount.ToString();
        deckCountText.text = deckCount.ToString();
        UIUtils.BounceText(deckCountText.transform);
    }

    public void SetColorOfUI(Color uiColor, Color textColor)
    {
        Color uiDim = new Color(uiColor.r * 1.3f, uiColor.g * 1.3f, uiColor.b * 1.3f);
        UIUtils.SetImageColorByScalar(bg, uiColor, 1.3f);
        UIUtils.SetTextColorByScalar(deckCountText, textColor, 1f);
        UIUtils.SetTextColorByScalar(deckTitleText, textColor, 1f);
        GetComponent<ButtonUI>().SetUIColor(uiDim);

        
    }
}

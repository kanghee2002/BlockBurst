using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

        if (deckCount <= 10)
        {
            int scalar = 10 - deckCount;
            Color color = new Color(200f / 255f, 40f/ 255f, 40f / 255f);
            Color scaledColor = new Color(color.r + (1f - color.r) * scalar / 10f, color.g * deckCount / 10f, color.b * deckCount / 10f);
            Color resultColor = new Color(scaledColor.r, scaledColor.g, scaledColor.b, 1f);
            deckCountText.DOColor(resultColor, 0.5f)
                .SetEase(Ease.OutQuad);
        }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopRerollButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deckCountText;
    public TextMeshProUGUI deckTitleText;
    public Image bg;


    public void SetColorOfUI(Color uiColor, Color textColor)
    {
        UIUtils.SetImageColorByScalar(bg, uiColor, 1f);
        UIUtils.SetTextColorByScalar(deckCountText, textColor, 1f);
        UIUtils.SetTextColorByScalar(deckTitleText, textColor, 1f);
        GetComponent<ButtonUI>().SetUIColor(uiColor);

        
    }
}

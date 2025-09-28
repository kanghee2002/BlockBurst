using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private PopupBlurImage popupBlurImage;

    [SerializeField] private Image outerLayout;
    [SerializeField] private Image innerLayout;

    private RectTransform rectTransform;

    private const float insidePositionY = -365;
    private const float outsidePositionY = -1080;
    private const float duration = 0.2f;

    public List<Image> bgs;
    public List<ButtonUI> buttons;
    public List<Image> fgs;
    public List<TMPro.TextMeshProUGUI> texts;



    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OpenOptionUI()
    {
        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
        popupBlurImage.OpenPopupBlurImage(new Color(0.0f, 0.0f, 0.0f, 0.9f));
    }

    public void CloseOptionUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionY, duration);
        popupBlurImage.ClosePopupBlurImage();
    }

    public void SetLayoutsColor(Color uiColor, Color uiBrightColor, Color textColor)
    {
        UIUtils.SetImageColorByScalar(outerLayout, uiColor, 0.8f, duration: 0.05f);
        UIUtils.SetImageColorByScalar(innerLayout, uiColor, 3f / 5f, duration: 0.05f);
        foreach (Image bg in bgs)
        {
            UIUtils.SetImageColorByScalar(bg, uiColor, 1f, duration: 0.05f);
        }
        foreach (Image fg in fgs)
        {
            UIUtils.SetImageColorByScalar(fg, uiBrightColor, 1f, duration: 0.05f);
        }
        foreach (ButtonUI button in buttons)
        {
            button.SetUIColor(uiBrightColor);
        }
        foreach (TMPro.TextMeshProUGUI text in texts)
        {
            UIUtils.SetTextColorByScalar(text, uiColor, 0.5f);
        }
    }
}

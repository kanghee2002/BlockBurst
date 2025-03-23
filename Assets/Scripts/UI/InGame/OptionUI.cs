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

    private const float insidePositionY = 0;
    private const float outsidePositionY = -1080;
    private const float duration = 0.2f;

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

    public void SetLayoutsColor(Color uiColor)
    {
        UIUtils.SetImageColorByScalar(outerLayout, uiColor, 2f / 5f, duration: 0.05f);
        UIUtils.SetImageColorByScalar(innerLayout, uiColor, 3f / 5f, duration: 0.05f);
    }
}

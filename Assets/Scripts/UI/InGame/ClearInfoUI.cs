using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearInfoUI : MonoBehaviour
{
    [SerializeField] private PopupBlurImage popupBlurImage;

    private RectTransform rectTransform;

    private const float insidePositionY = 0;
    private const float outsidePositionY = 1536;
    private const float duration = 0.4f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OpenClearInfoUI()
    {
        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
        popupBlurImage.OpenPopupBlurImage(new Color(0.0f, 0.6f, 0.0f, 0.9f));
    }

    public void CloseClearInfoUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionY, duration);
        popupBlurImage.ClosePopupBlurImage();
    }
}

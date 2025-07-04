using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataBoardUI : MonoBehaviour
{
    [SerializeField] private PopupBlurImage popupBlurImage;

    [SerializeField] private StatisticsContainerUI statisticsContainerUI;
    [SerializeField] private UnlockContainerUI unlockContainerUI;

    private RectTransform rectTransform;

    private const float insidePositionX = 0;
    private const float outsidePositionOffsetX = 1000;
    private const float duration = 0.2f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize()
    {

    }

    public void OnStatisticsSwtichButtonUIClick()
    {
        statisticsContainerUI.gameObject.SetActive(true);
        unlockContainerUI.gameObject.SetActive(false);
    }

    public void OnUnlockSwtichButtonUIClick()
    {
        statisticsContainerUI.gameObject.SetActive(false);
        unlockContainerUI.gameObject.SetActive(true);
    }

    public void OpenPlayerDataBoardUI()
    {
        gameObject.SetActive(true);
        popupBlurImage.OpenPopupBlurImage(new Color(0.0f, 0.0f, 0.0f, 0.9f));
        UIUtils.OpenUI(rectTransform, "Y", insidePositionX, duration);
    }

    public void ClosePlayerDataBoardUI()
    {
        popupBlurImage.ClosePopupBlurImage();
        UIUtils.CloseUI(rectTransform, "Y", insidePositionX, outsidePositionOffsetX, duration);
    }
}

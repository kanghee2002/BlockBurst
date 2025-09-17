using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataBoardUI : MonoBehaviour
{
    [SerializeField] private PopupBlurImage popupBlurImage;

    [SerializeField] private StatisticsContainerUI statisticsContainerUI;
    [SerializeField] private UnlockContainerUI unlockContainerUI;

    [SerializeField] private GameObject UnlockAllLayout;

    [SerializeField] private ButtonUI statisticsSwitchButtonUI;
    [SerializeField] private ButtonUI unlockSwitchButtonUI;

    private RectTransform rectTransform;

    private const float insidePositionX = 310;
    private const float outsidePositionOffsetX = -1700;
    private const float duration = 0.4f;

    private Color activeWindowColor;
    private Color inactiveWindowColor;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        activeWindowColor = statisticsSwitchButtonUI.transform.GetChild(0).GetComponent<Image>().color;
        inactiveWindowColor = unlockSwitchButtonUI.transform.GetChild(0).GetComponent<Image>().color;
    }

    public void OnStatisticsSwtichButtonUIClick()
    {
        statisticsContainerUI.gameObject.SetActive(true);
        unlockContainerUI.gameObject.SetActive(false);

        statisticsSwitchButtonUI.SetUIColor(activeWindowColor);
        unlockSwitchButtonUI.SetUIColor(inactiveWindowColor);
    }

    public void OnUnlockSwtichButtonUIClick()
    {
        statisticsContainerUI.gameObject.SetActive(false);
        unlockContainerUI.gameObject.SetActive(true);

        statisticsSwitchButtonUI.SetUIColor(inactiveWindowColor);
        unlockSwitchButtonUI.SetUIColor(activeWindowColor);
    }

    public void OpenPlayerDataBoardUI()
    {
        gameObject.SetActive(true);
        popupBlurImage.OpenPopupBlurImage(new Color(0.0f, 0.0f, 0.0f, 0.9f));
        UIUtils.OpenUI(rectTransform, "Y", insidePositionX, duration);

        // UnlockAllLayout.SetActive(true);
    }

    public void ClosePlayerDataBoardUI()
    {
        popupBlurImage.ClosePopupBlurImage();
        UIUtils.CloseUI(rectTransform, "Y", insidePositionX, outsidePositionOffsetX, duration);

        UnlockAllLayout.SetActive(false);
    }


    // 평가용
    public void OnUnlockAllButtonUIPressed()
    {
        UnlockInfo[] unlockInfoTemplates = Resources.LoadAll<UnlockInfo>("ScriptableObjects/UnlockInfo");

        foreach (UnlockInfo unlockInfo in unlockInfoTemplates)
        {
            UnlockManager.instance.Unlock(unlockInfo, unlockInfo.targetName);
        }

        foreach (DeckType deckType in Enums.GetEnumList<DeckType>())
        {
            for (int i = 0; i < DeckInfo.MAX_LEVEL + 1; i++)
            {
                DataManager.instance.UpdateDeckLevel(deckType, i);
            }
        }

        UnlockAllLayout.SetActive(false);

        GameObject.Find("UnlockNotificationUI").SetActive(false);

        GameUIManager.instance.OnPopupBlurUIPressed();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfoUI : MonoBehaviour
{
    private RectTransform rectTransform;

    private const float insidePositionY = 0;
    private const float outsidePositionOffsetY = 1000;
    private const float duration = 0.2f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OpenStageSelectionSignboardUI()
    {
        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
    }

    public void CloseStageSelectionSignboardUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionOffsetY, duration);
    }
}

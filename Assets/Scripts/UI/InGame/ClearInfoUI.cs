using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearInfoUI : MonoBehaviour
{
    private RectTransform rectTransform;

    private const float insidePositionY = 0;
    private const float outsidePositionY = 1536;
    private const float duration = 0.4f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(bool isCleared)
    {
        gameObject.SetActive(true);
    }

    public void OpenClearInfoUI()
    {
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
    }

    public void CloseClearInfoUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionY, duration);
    }
}

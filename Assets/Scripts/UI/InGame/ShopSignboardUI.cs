using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSignboardUI : MonoBehaviour
{
    private RectTransform rectTransform;
    // inside anchored position = (300,256)
    private const float insidePositionY = 256;
    private const float outsidePositionOffsetY = 540;
    private const float duration = 0.2f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OpenShopSignboardUI()
    {
        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
    }

    public void CloseShopSignboardUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionOffsetY, duration);
    }
}

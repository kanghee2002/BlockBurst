using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RerollButtonUI : MonoBehaviour
{
    [SerializeField] private Image rerollButtonImage;
    [SerializeField] private TextMeshProUGUI rerollCountText;
    private RectTransform rectTransform;

    // inside anchored position = (-188,-400)
    private const float insidePositionX = -188;
    private const float outsidePositionOffsetX = 480;
    private const float duration = 0.2f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnRerollButtonUIPressed()
    {
        GameUIManager.instance.OnRerollButtonUIPressed();
    }

    public void OpenRerollButtonUI(Color uiColor)
    {
        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            UIUtils.OpenUI(rectTransform, "X", insidePositionX, duration);
        }

        rerollButtonImage.color = uiColor;
        GetComponent<ButtonUI>().Initialize();
    }

    public void CloseRerollButtonUI()
    {
        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            UIUtils.CloseUI(rectTransform, "X", insidePositionX, outsidePositionOffsetX, duration);
        }
    }

    public void Initialize(int rerollCount)
    {
        gameObject.SetActive(true);
        DisplayRerollCount(rerollCount);
    }

    public void DisplayRerollCount(int rerollCount)
    {
        rerollCountText.text = rerollCount.ToString();
        UIUtils.BounceText(rerollCountText.transform);
    }
}

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
    public TextMeshProUGUI rerollTitleText;
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
        UpdateRerollCount(rerollCount);
    }

    public void SetColorOfUI(Color uiColor, Color textColor)
    {
        UIUtils.SetImageColorByScalar(rerollButtonImage, uiColor, 1f);
        UIUtils.SetTextColorByScalar(rerollCountText, textColor, 1f);
        UIUtils.SetTextColorByScalar(rerollTitleText, textColor, 1f);

        
    }

    public void UpdateRerollCount(int rerollCount, bool isAdditive = false)
    {
        if (isAdditive)
        {
            rerollCountText.text = (GetCurrentRerollCount() + rerollCount).ToString();
        }
        else
        {
            rerollCountText.text = rerollCount.ToString();
        }
        UIUtils.BounceText(rerollCountText.transform);
    }

    private int GetCurrentRerollCount()
    {
        int rerollCount = 0;
        string text = rerollCountText.text;
        if (int.TryParse(text, out rerollCount))
        {
            return rerollCount;
        }
        else
        {
            Debug.Log("Error: multiplierText is not int form");
        }
        return rerollCount;
    }
}

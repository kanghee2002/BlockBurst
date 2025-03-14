using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectionSignboardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageSelectionSignboardText;
    [SerializeField] private Image innerLayout;
    [SerializeField] private Image outerLayout;
    private RectTransform rectTransform;
    // inside anchored position = (300,280)
    private const float insidePositionY = 280;
    private const float outsidePositionOffsetY = 540;
    private const float duration = 0.2f;

    private const string GREEN_COLOR = "10D275";
    private const string RED_COLOR = "D2101B";

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OpenStageSelectionSignboardUI()
    {
        if (GameManager.instance.applicationType == ApplicationType.Mobile) return;

        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
    }

    public void CloseStageSelectionSignboardUI()
    {
        if (GameManager.instance.applicationType == ApplicationType.Mobile) return;

        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionOffsetY, duration);
    }
    
    public void Initialize(int currentChapterIndex, int currentStageIndex)
    {
        string text = currentChapterIndex + " - " + currentStageIndex + "\n" + "스테이지를 선택하세요";
        stageSelectionSignboardText.text = text;

        if (currentStageIndex == 3)
        {
            outerLayout.color = UIUtils.HexToColor(RED_COLOR);
        }
        else
        {
            outerLayout.color = UIUtils.HexToColor(GREEN_COLOR);
        }
    }
}

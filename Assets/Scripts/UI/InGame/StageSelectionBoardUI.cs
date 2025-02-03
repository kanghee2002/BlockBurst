using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectionBoardUI : MonoBehaviour
{
    private RectTransform rectTransform;
    // inside anchored position = (182,-128)
    private const float insidePositionY = -128;
    private const float outsidePositionOffsetY = -1080;
    private const float duration = 0.2f;

    private const string GREEN_COLOR = "10D275";
    private const string RED_COLOR = "D2101B";

    [SerializeField] private NextStageChoiceUI[] nextStageChoiceUI;
    [SerializeField] private Image outerLayout;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(StageData[] nextStageChoices, int currentStageIndex)
    {
        gameObject.SetActive(true);
        if (nextStageChoices.Length == nextStageChoiceUI.Length)
        {
            for (int i = 0; i < nextStageChoices.Length; i++)
            {
                nextStageChoiceUI[i].Initialize(nextStageChoices[i]);
            }
        } 
        else
        {
            Debug.LogError("length of nextStageChoices and nextStageChoiceUI is different");
            return;
        }

        if (currentStageIndex == 3)
        {
            outerLayout.color = UIUtils.HexToColor(RED_COLOR);
        }
        else
        {
            outerLayout.color = UIUtils.HexToColor(GREEN_COLOR);
        }
    }

    public void OpenStageSelectionBoardUI()
    {
        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
    }

    public void CloseStageSelectionBoardUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionOffsetY, duration);
    }
}

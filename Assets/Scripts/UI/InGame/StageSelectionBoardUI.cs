using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectionBoardUI : MonoBehaviour
{
    private RectTransform rectTransform;
    // inside anchored position = (182,-128)
    private const float insidePositionY = -128;
    private const float outsidePositionOffsetY = -1080;
    private const float duration = 0.2f;

    [SerializeField] private NextStageChoiceUI[] nextStageChoiceUI;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(StageData[] nextStageChoices)
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

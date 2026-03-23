using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectionBoardUI : MonoBehaviour
{
    private RectTransform rectTransform;
    // inside anchored position = (182,-128)
    private const float windowsInsidePositionY = -128;
    private const float mobileInsidePositionY = 700;
    private const float outsidePositionOffsetY = -1600;
    private const float duration = 0.2f;

    private const string GREEN_COLOR = "10D275";
    private const string RED_COLOR = "D2101B";

    [SerializeField] private NextStageChoiceUI[] nextStageChoiceUI;
    [SerializeField] private Image outerLayout;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(
        int currentStageIndex,
        string[] debuffEffectNames0,
        int clearRequirement0,
        int goldReward0,
        string[] debuffEffectNames1,
        int clearRequirement1,
        int goldReward1)
    {
        gameObject.SetActive(true);
        if (nextStageChoiceUI.Length != 2)
        {
            Debug.LogError("nextStageChoiceUI must have length 2");
            return;
        }

        nextStageChoiceUI[0].Initialize(debuffEffectNames0, clearRequirement0, goldReward0);
        nextStageChoiceUI[1].Initialize(debuffEffectNames1, clearRequirement1, goldReward1);
        UIUtils.PlaySlowShakeAnimation(nextStageChoiceUI[0].transform, delay: 0f);
        UIUtils.PlaySlowShakeAnimation(nextStageChoiceUI[1].transform, delay: Random.Range(1f, 3f));

        if (currentStageIndex == 3)
        {
            outerLayout.color = UIUtils.HexToColor(RED_COLOR);
        }
        else
        {
            outerLayout.color = UIUtils.HexToColor(GREEN_COLOR);
        }
    }

    public void OpenStageSelectionBoardUI(Color uiColor)
    {
        gameObject.SetActive(true);

        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            UIUtils.OpenUI(rectTransform, "Y", windowsInsidePositionY, duration);
        }
        else
        {
            UIUtils.OpenUI(rectTransform, "Y", mobileInsidePositionY, duration);
        }

        for (int i = 0; i < nextStageChoiceUI.Length; i++)
        {
            nextStageChoiceUI[i].SetLayoutsColor(uiColor);
        }
    }

    public void CloseStageSelectionBoardUI()
    {
        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            UIUtils.CloseUI(rectTransform, "Y", windowsInsidePositionY, outsidePositionOffsetY, duration);
        }
        else
        {
            UIUtils.CloseUI(rectTransform, "Y", mobileInsidePositionY, outsidePositionOffsetY, duration);
        }
    }
}

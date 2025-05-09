using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class NextStageChoiceUI : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] private Image debuffTextLayout;
    [SerializeField] private Image scoreAtLeastTextLayout;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI debuffText;
    [SerializeField] private TextMeshProUGUI scoreAtLeastText;
    [SerializeField] private TextMeshProUGUI rewardGoldText;

    public void Initialize(StageData stageData)
    {
        UpdateDebuffText(stageData.constraints.Select(x => x.effectName).ToArray());
        UpdateScoreAtLeast(stageData.clearRequirement);
        UpdateRewardGold(stageData.goldReward);
    }

    public void SetLayoutsColor(Color uiColor)
    {
        UIUtils.SetImageColorByScalar(debuffTextLayout, uiColor, 1f);
        UIUtils.SetImageColorByScalar(scoreAtLeastTextLayout, uiColor, 5f / 4f);
    }

    public void UpdateDebuffText(string[] debuffTexts)
    {
        string debuffText = string.Join("\n", debuffTexts);
        debuffText = UIUtils.SetBlockNameToIcon(debuffText);
        debuffText = debuffText.Replace("\\n", " ").Replace(",", "");

        this.debuffText.text = debuffText;
    }

    public void UpdateScoreAtLeast(int scoreAtLeast)
    {
        int textSize = 45;
        
        if (scoreAtLeast >= 10000000)
        {
            textSize = 30;
        }
        else if (scoreAtLeast >= 1000000)
        {
            textSize = 35;
        }
        else if (scoreAtLeast >= 100000)
        {
            textSize = 40;
        }
        scoreAtLeastText.text = $"목표 점수\n<size={textSize}><color=#94EEFF>{scoreAtLeast}</color></size>";
    }

    public void UpdateRewardGold(int rewardGold)
    {
        rewardGoldText.text = "보상 <color=yellow>$" + rewardGold.ToString() + "</color>";
    }
}

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
        debuffText = debuffText.Replace("\\n", "\n").Replace(",", "");

        this.debuffText.text = debuffText;
    }

    public void UpdateScoreAtLeast(int scoreAtLeast)
    {
        scoreAtLeastText.text = "목표 점수\n<size=45><color=#94EEFF>" + scoreAtLeast.ToString() + "</color></size>";

        if (scoreAtLeast >= 10000000)
        {
            scoreAtLeastText.text = "목표 점수\n<size=40><color=#94EEFF>" + scoreAtLeast.ToString() + "</color></size>";
        }
    }

    public void UpdateRewardGold(int rewardGold)
    {
        rewardGoldText.text = "보상 <color=yellow>$" + rewardGold.ToString() + "</color>";
    }
}

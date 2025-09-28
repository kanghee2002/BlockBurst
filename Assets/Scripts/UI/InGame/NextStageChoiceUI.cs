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
    // public Image scoreAtLeastTextLayout;
    

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
        UIUtils.SetImageColorByScalar(gameObject.GetComponent<Image>(), uiColor, 5f / 4f);
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
        scoreAtLeastText.text = $"<color=#94EEFF>목표 {scoreAtLeast}점</color>";
    }

    public void UpdateRewardGold(int rewardGold)
    {
        rewardGoldText.text = "보상 " + rewardGold.ToString() + "$";
    }
}

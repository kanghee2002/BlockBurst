using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class NextStageChoiceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debuffText;
    [SerializeField] private TextMeshProUGUI scoreAtLeastText;
    [SerializeField] private TextMeshProUGUI rewardGoldText;

    public void Initialize(StageData stageData)
    {
        UpdateDebuffText(stageData.constraints.Select(x => x.effectName).ToArray());
        UpdateScoreAtLeast(stageData.clearRequirement);
        UpdateRewardGold(stageData.goldReward);
    }

    public void UpdateDebuffText(string[] debuffTexts)
    {
        string debuffText = string.Join("\n", debuffTexts);
        
        this.debuffText.text = debuffText;
    }

    public void UpdateScoreAtLeast(int scoreAtLeast)
    {
        scoreAtLeastText.text = "점수 요구치 " + scoreAtLeast.ToString();
    }

    public void UpdateRewardGold(int rewardGold)
    {
        rewardGoldText.text = "보상 $" + rewardGold.ToString();
    }
}

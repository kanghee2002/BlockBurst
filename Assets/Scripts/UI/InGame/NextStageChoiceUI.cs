using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class NextStageChoiceUI : MonoBehaviour
{
    TextMeshProUGUI debuffText;
    TextMeshProUGUI scoreAtLeastText;
    TextMeshProUGUI rewardGoldText;

    void Awake()
    {
        debuffText = transform.Find("DebuffText").GetComponent<TextMeshProUGUI>();
        scoreAtLeastText = transform.Find("ScoreAtLeastText").GetComponent<TextMeshProUGUI>();
        rewardGoldText = transform.Find("RewardGoldText").GetComponent<TextMeshProUGUI>();
    }

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
        scoreAtLeastText.text = scoreAtLeast.ToString();
    }

    public void UpdateRewardGold(int rewardGold)
    {
        rewardGoldText.text = rewardGold.ToString();
    }
}

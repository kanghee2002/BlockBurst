using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Linq;

public class StageInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI debuffText;
    [SerializeField] private TextMeshProUGUI scoreAtLeastText;

    [SerializeField] private GameObject stageInfoUI;
    [SerializeField] private RectTransform rectTransform;
    // inside anchored position = (300,320)
    private const float insidePositionY = 320;
    private const float outsidePositionOffsetY = 540;
    private const float duration = 0.2f;

    void Awake()
    {
        stageText = transform.Find("StageText").GetComponent<TextMeshProUGUI>();
        debuffText = transform.Find("DebuffText").GetComponent<TextMeshProUGUI>();
        scoreAtLeastText = transform.Find("ScoreAtLeastText").GetComponent<TextMeshProUGUI>();
    }

    public void OpenStageInfoUI()
    {
        rectTransform.DOAnchorPosY(insidePositionY, duration)
            .SetEase(Ease.OutCubic);
    }

    public void CloseStageInfoUI()
    {
        rectTransform.DOAnchorPosY(insidePositionY + outsidePositionOffsetY, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                stageInfoUI.SetActive(false);
            });
    }

    public void Initialize(int stageIndex, StageData stageData)
    {
        stageInfoUI.SetActive(true);
        //UpdateChapter(1);
        UpdateStage(stageIndex);
        UpdateDebuffText(stageData.constraints.Select(x => x.effectName).ToArray());
        UpdateScoreAtLeast(stageData.clearRequirement);
    }

    public void UpdateChapter(int chapter)
    {
        //Debug.Log("Chapter has been updated.");
    }

    public void UpdateStage(int stage)
    {
        //Debug.Log("Stage has been updated.");
        stageText.text = stage.ToString();
    }

    public void UpdateDebuffText(string[] debuffTexts)
    {
        //Debug.Log("DebuffText has been updated.");
        string text = string.Join("\n", debuffTexts);
        debuffText.text = text;
    }

    public void UpdateScoreAtLeast(int scoreAtLeast)
    {
        //Debug.Log("ScoreAtLeast has been updated.");
        scoreAtLeastText.text = scoreAtLeast.ToString();
    }
}

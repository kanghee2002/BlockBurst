using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Linq;

public class StageInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI debuffText;
    [SerializeField] private TextMeshProUGUI scoreAtLeastText;

    [SerializeField] private GameObject stageInfoUI;
    [SerializeField] private RectTransform rectTransform;
    // inside anchored position = (300,320)
    private const float insidePositionY = 320;
    private const float outsidePositionOffsetY = 540;
    private const float duration = 0.2f;

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

    public void Initialize(int chapterIndex, int stageIndex, StageData stageData)
    {
        stageInfoUI.SetActive(true);
        UpdateChapter(chapterIndex);
        UpdateStage(stageIndex);
        UpdateDebuffText(stageData.constraints.Select(x => x.effectName).ToArray());
        UpdateScoreAtLeast(stageData.clearRequirement);
    }

    public void UpdateChapter(int chapter)
    {
        chapterText.text = chapter.ToString();
    }

    public void UpdateStage(int stage)
    {
        stageText.text = stage.ToString();
    }

    public void UpdateDebuffText(string[] debuffTexts)
    {
        string text = string.Join("\n", debuffTexts);
        debuffText.text = text;
    }

    public void UpdateScoreAtLeast(int scoreAtLeast)
    {
        scoreAtLeastText.text = scoreAtLeast.ToString();
    }
}

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

    private RectTransform rectTransform;
    // inside anchored position = (300,320)
    private const float insidePositionY = 320;
    private const float outsidePositionOffsetY = 540;
    private const float duration = 0.2f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(int chapterIndex, int stageIndex, StageData stageData)
    {
        gameObject.SetActive(true);
        UpdateChapter(chapterIndex);
        UpdateStage(stageIndex);
        UpdateDebuffText(stageData.constraints.Select(x => x.effectName).ToArray());
        UpdateScoreAtLeast(stageData.clearRequirement);
    }

    public void OpenStageInfoUI()
    {
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
    }

    public void CloseStageInfoUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionOffsetY, duration);
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

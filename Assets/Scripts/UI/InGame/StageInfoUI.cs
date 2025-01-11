using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StageInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject stageInfoUI;
    [SerializeField] private RectTransform rectTransform;
    // inside anchored position = (300,320)
    private const float insidePositionY = 320;
    private const float outsidePositionOffsetY = 540;
    private const float duration = 0.2f;
    public void OpenStageInfoUI()
    {
        stageInfoUI.SetActive(true);
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
    public void UpdateChapter(int _chapter)
    {
        //Debug.Log("Chapter has been updated.");
    }

    public void UpdateStage(int _stage)
    {
        //Debug.Log("Stage has been updated.");
    }

    public void UpdateDebuffText(string _debuffText)
    {
        //Debug.Log("DebuffText has been updated.");
    }

    public void UpdateScoreAtLeast(int _scoreAtLeast)
    {
        //Debug.Log("ScoreAtLeast has been updated.");
    }
}

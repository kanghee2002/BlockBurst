using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private GameObject scoreInfoUI;
    [SerializeField] private RectTransform rectTransform;
    // inside anchored position = (300,80)
    private const float insidePositionY = 80;
    private const float outsidePositionOffsetY = 540;
    private const float duration = 0.2f;
    public void OpenScoreInfoUI()
    {
        scoreInfoUI.SetActive(true);
        rectTransform.DOAnchorPosY(insidePositionY, duration)
            .SetEase(Ease.OutCubic);
    }

    public void CloseScoreInfoUI()
    {
        rectTransform.DOAnchorPosY(insidePositionY + outsidePositionOffsetY, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                scoreInfoUI.SetActive(false);
            });
    }
    public void Initialize(int _score)
    {
        UpdateScore(_score);
    }
    public void UpdateScore(int _score)
    {
        scoreText.text = _score.ToString();
    }
}

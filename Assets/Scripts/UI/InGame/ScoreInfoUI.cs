using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private RectTransform rectTransform;
    // inside anchored position = (300,80)
    private const float insidePositionY = 80;
    private const float outsidePositionOffsetY = 540;
    private const float duration = 0.2f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    public void Initialize(int _score)
    {
        gameObject.SetActive(true);
        UpdateScore(_score);
    }

    public void OpenScoreInfoUI()
    {
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
    }

    public void CloseScoreInfoUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionOffsetY, duration);
    }

    public void UpdateScore(int _score)
    {
        scoreText.text = _score.ToString();
        UIUtils.BounceText(scoreText.transform);
    }
}

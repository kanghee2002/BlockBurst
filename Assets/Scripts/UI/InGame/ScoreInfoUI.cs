using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreInfoUI : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI scoreAtLeastText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Score Slider")]
    [SerializeField] private SlicedFilledImage scoreSlider;

    private RectTransform rectTransform;
    // inside anchored position = (300,80)
    private const float insidePositionY = 80;
    private const float outsidePositionOffsetY = 540;
    private const float duration = 0.2f;

    private int currentScore;
    private int scoreAtLeast;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    public void InitializePlaying(int currentScore, int scoreAtLeast)
    {
        currentScoreText.gameObject.SetActive(true);
        scoreAtLeastText.gameObject.SetActive(true);
        descriptionText.gameObject.SetActive(false);

        UpdateScore(currentScore);
        UpdateScoreAtLeast(scoreAtLeast);
    }

    public void InitializeShopping()
    {
        currentScoreText.gameObject.SetActive(false);
        scoreAtLeastText.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(true);
        descriptionText.text = "구매하세요!";
        UIUtils.BounceText(descriptionText.transform);
    }

    public void InitializeSelecting()
    {
        currentScoreText.gameObject.SetActive(false);
        scoreAtLeastText.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(true);
        descriptionText.text = "선택하세요!";
        UIUtils.BounceText(descriptionText.transform);
    }

    public void SetUIColor(Color uiColor)
    {
        UIUtils.SetImageColorByScalar(scoreSlider, uiColor, 1f);
        UIUtils.SetTextColorByScalar(currentScoreText, uiColor, 1f / 10f);
        UIUtils.SetTextColorByScalar(descriptionText, uiColor, 1f / 30f);
    }

    public void UpdateScoreAtLeast(int scoreAtLeast)
    {
        this.scoreAtLeast = scoreAtLeast;
        scoreAtLeastText.text = scoreAtLeast.ToString();
    }

    public void UpdateScore(int score)
    {
        currentScoreText.text = score.ToString();
        UIUtils.BounceText(currentScoreText.transform);

        if (scoreAtLeast <= 0) return;

        float scoreRatio = (float)score / scoreAtLeast;
        float duration = 0.3f;

        DOTween.To(
            () => scoreSlider.fillAmount,       // 현재 fillAmount를 얻는 람다
            x => scoreSlider.fillAmount = x,    // fillAmount 값을 설정하는 람다
            scoreRatio,
            duration
        ).SetEase(Ease.InOutQuad);
    }

    public void OpenScoreInfoUI()
    {
        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
        }
    }

    public void CloseScoreInfoUI()
    {
        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionOffsetY, duration);
        }
    }
}

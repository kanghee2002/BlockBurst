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

        this.currentScore = currentScore;

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
    }

    public void UpdateScoreAtLeast(int scoreAtLeast)
    {
        this.scoreAtLeast = scoreAtLeast;
        scoreAtLeastText.color = Color.white;

        scoreAtLeastText.text = scoreAtLeast.ToString();
    }

    public void UpdateScore(int score, bool isAdditive = false)
    {
        int value = score;
        if (isAdditive)
        {
            currentScore += score;
            value = currentScore;
        }

        currentScoreText.text = value.ToString();

        UIUtils.BounceText(currentScoreText.transform);

        float scoreRatio = scoreAtLeast <= 0 ? 0 : (float)value / scoreAtLeast;
        float duration = 0.3f;

        DOTween.To(
            () => scoreSlider.fillAmount,       // 현재 fillAmount를 얻는 람다
            x => scoreSlider.fillAmount = x,    // fillAmount 값을 설정하는 람다
            scoreRatio,
            duration
        ).SetEase(Ease.InOutQuad);

        UIUtils.SetTextColorByScalar(scoreAtLeastText, Color.white, 1f - scoreRatio);
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

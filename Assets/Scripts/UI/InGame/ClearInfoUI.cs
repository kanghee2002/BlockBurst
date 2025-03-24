using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClearInfoUI : MonoBehaviour
{
    [SerializeField] private PopupBlurImage popupBlurImage;
    [SerializeField] private TextMeshProUGUI seungRi;

    [SerializeField] private TextMeshProUGUI elapsedTimeText;
    [SerializeField] private TextMeshProUGUI maximumScoreText;
    [SerializeField] private Image mostPlacedBlockImage;
    [SerializeField] private TextMeshProUGUI chapterStageText;
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private TextMeshProUGUI placedBlockCountText;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI rerolledCountText;
    [SerializeField] private TextMeshProUGUI buyedCountText;

    [SerializeField] private GameObject infiniteButton;

    [Header("Layout")]
    [SerializeField] private Image outer;
    [SerializeField] private Image inner;

    private RectTransform rectTransform;

    private const float insidePositionY = 0;
    private const float outsidePositionY = 1536;
    private const float duration = 0.4f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(bool isCleared, int currentChapterIndex, int currentStageIndex, GameManager.History history, BlockType mostPlacedBlockType)
    {
        gameObject.SetActive(true);
        float elapsedTime = Time.time - history.startTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        int blockCount = 0;
        foreach (int count in history.blockHistory)
        {
            blockCount += count;
        }

        seungRi.text = isCleared ? "승리!" : "패배";
        
        infiniteButton.SetActive(isCleared);

        elapsedTimeText.text = $"{minutes:00}:{seconds:00}";
        maximumScoreText.text = history.maxScore.ToString();
        mostPlacedBlockImage.sprite = Resources.Load<Sprite>($"Sprites/Block/Preset/{mostPlacedBlockType.ToString()}");
        chapterStageText.text = currentChapterIndex + " - " + currentStageIndex;
        chapterText.text = currentChapterIndex.ToString();
        placedBlockCountText.text = blockCount.ToString();
        stageText.text = currentStageIndex.ToString();
        rerolledCountText.text = history.rerollCount.ToString();
        buyedCountText.text = history.itemPurchaseCount.ToString();
    }

    public void SetLayoutsColor(Color uiColor)
    {
        UIUtils.SetImageColorByScalar(outer, uiColor, 1f / 3f, duration: 0.05f);
        UIUtils.SetImageColorByScalar(inner, uiColor, 2f / 3f, duration: 0.05f);
    }

    public void OpenClearInfoUI(bool isCleared)
    {
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
        Color popupBlurColor = isCleared ? new Color(0.0f, 0.6f, 0.0f, 0.9f) : new Color(0.6f, 0.0f, 0.0f, 0.9f);
        popupBlurImage.OpenPopupBlurImage(popupBlurColor);
    }

    public void CloseClearInfoUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionY, duration);
        popupBlurImage.ClosePopupBlurImage();
    }
}

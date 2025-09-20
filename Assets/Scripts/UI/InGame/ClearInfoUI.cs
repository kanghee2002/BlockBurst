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
    [SerializeField] private TextMeshProUGUI loseReasonText;

    [SerializeField] private GameObject infiniteButton;
    [SerializeField] private GameObject loseReasonContainer;

    [Header("Layout")]
    [SerializeField] private Image outer;
    [SerializeField] private Image inner;

    public List<Image> bgs;
    public List<TextMeshProUGUI> texts;
    public List<ButtonUI> buttons;

    private RectTransform rectTransform;

    private const float insidePositionY = -170;
    private const float outsidePositionY = -1121.1f;
    private const float duration = 0.4f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(bool isCleared, int currentChapterIndex, int currentStageIndex, RunData.History history, BlockType mostPlacedBlockType, string loseReason = "")
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
        // loseReasonContainer.SetActive(!isCleared);
        loseReasonText.gameObject.SetActive(!isCleared);

        elapsedTimeText.text = $"{minutes:00}:{seconds:00}";
        maximumScoreText.text = history.maxScore.ToString();
        mostPlacedBlockImage.sprite = Resources.Load<Sprite>($"Sprites/Block/Preset/{mostPlacedBlockType.ToString()}");
        chapterStageText.text = currentChapterIndex + " - " + currentStageIndex;
        chapterText.text = currentChapterIndex.ToString();
        placedBlockCountText.text = blockCount.ToString();
        stageText.text = currentStageIndex.ToString();
        rerolledCountText.text = history.rerollCount.ToString();
        buyedCountText.text = history.itemPurchaseCount.ToString();
        loseReasonText.text = loseReason;
    }

    public void SetLayoutsColor(Color uiColor, Color textColor)
    {
        UIUtils.SetImageColorByScalar(outer, uiColor, 1, duration: 0.05f);
        UIUtils.SetImageColorByScalar(inner, uiColor, 1, duration: 0.05f);

        Color darkerColor = new Color(uiColor.r * .9f, uiColor.g * .9f, uiColor.b * .9f, uiColor.a);

        foreach (Image bg in bgs)
        {
            UIUtils.SetImageColorByScalar(bg, uiColor, .9f, duration: 0.05f);
        }
        foreach (TextMeshProUGUI text in texts)
        {
            UIUtils.SetTextColorByScalar(text, textColor, 1f);
        }
        foreach (ButtonUI button in buttons)
        {
            button.SetUIColor(darkerColor);
        }
    }

    public void OpenClearInfoUI(bool isCleared)
    {
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
        Color popupBlurColor = isCleared ? new Color(29 / 255f, 89 / 255f, 59 / 255f, 0.8f) : new Color(1f, 55 / 255f, 93 / 255f, 0.8f);
        popupBlurImage.OpenPopupBlurImage(popupBlurColor);
    }

    public void CloseClearInfoUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionY, duration);
        popupBlurImage.ClosePopupBlurImage();
    }
}

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
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private TextMeshProUGUI placedBlockCountText;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI rerolledCountText;
    [SerializeField] private TextMeshProUGUI buyedCountText;


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

        seungRi.text = isCleared ? "승리!" : "패배";

        elapsedTimeText.text = $"{minutes:00}:{seconds:00}";
        maximumScoreText.text = history.maxScore.ToString();
        mostPlacedBlockImage.sprite = Resources.Load<Sprite>($"Sprites/Block/Preset/{mostPlacedBlockType.ToString()}");
        chapterText.text = currentChapterIndex.ToString();
        placedBlockCountText.text = history.blockHistory.Length.ToString();
        stageText.text = currentStageIndex.ToString();
        rerolledCountText.text = history.rerollCount.ToString();
        buyedCountText.text = history.itemPurchaseCount.ToString();
    }

    public void OpenClearInfoUI()
    {
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
        popupBlurImage.OpenPopupBlurImage(new Color(0.0f, 0.6f, 0.0f, 0.9f));
    }

    public void CloseClearInfoUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionY, duration);
        popupBlurImage.ClosePopupBlurImage();
    }
}

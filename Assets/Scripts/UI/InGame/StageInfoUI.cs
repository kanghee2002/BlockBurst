using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Linq;

public class StageInfoUI : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] private Image chapterStageTextLayout;
    [SerializeField] private Image stageDescriptionTextLayout;
    [SerializeField] private GameObject scrollingTextMask;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI chapterStageText;
    [SerializeField] private TextMeshProUGUI stageDescriptionText;

    private Color defaultTextColor = Color.white;

    public List<TextMeshProUGUI> scrollTexts;

    private RectTransform rectTransform;
    // inside anchored position = (300,320)
    private const float insidePositionY = 271;
    private const float outsidePositionOffsetY = 540;
    private const float duration = 0.2f;

    private Coroutine scrollingTextAnimationCoroutine;

    private Sequence currentWarningSequence;

    private bool isBlockWarning;
    private bool isWarning;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void InitializePlaying(int chapterIndex, int stageIndex, StageData stageData)
    {
        gameObject.SetActive(true);
        scrollingTextMask.SetActive(false);

        if (scrollingTextAnimationCoroutine != null) StopCoroutine(scrollingTextAnimationCoroutine);

        UpdateChapter(chapterIndex);
        UpdateStage(stageIndex);
        UpdateStageDescriptionText(stageData.constraints.Select(x => x.effectName).ToArray());

        currentWarningSequence = null;
        isBlockWarning = false;
        isWarning = false;
    }

    public void InitializeShopping(int chapterIndex, int stageIndex)
    {
        UpdateChapterStage(chapterIndex, stageIndex);

        if (scrollingTextAnimationCoroutine != null) StopCoroutine(scrollingTextAnimationCoroutine);

        PlayScrollingTextAnimation("SHOP · SHOP ·", 35);
    }

    public void InitializeSelecting()
    {
        if (scrollingTextAnimationCoroutine != null) StopCoroutine(scrollingTextAnimationCoroutine);

        PlayScrollingTextAnimation("SELECT STAGE ·", 33);
    }

    public void SetUIColor(Color uiColor)
    {
        UIUtils.SetImageColorByScalar(chapterStageTextLayout, uiColor, 1);
        UIUtils.SetImageColorByScalar(stageDescriptionTextLayout, uiColor, 1);
    }

    public void SetTextColor(Color textColor)
    {
        UIUtils.SetTextColorByScalar(chapterStageText, textColor, 1);
        UIUtils.SetTextColorByScalar(chapterText, textColor, 1);
        UIUtils.SetTextColorByScalar(stageDescriptionText, textColor, 1);
        UIUtils.SetTextColorByScalar(stageText, textColor, 1);
        defaultTextColor = textColor;
        foreach (TextMeshProUGUI scrollingText in scrollTexts)
        {
            UIUtils.SetTextColorByScalar(scrollingText, textColor, 1);
        }
    }

    public void OpenStageInfoUI()
    {
        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
        }
    }

    public void CloseStageInfoUI()
    {
        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionOffsetY, duration);
        }
    }

    public void UpdateChapter(int chapter)
    {
        chapterText.text = chapter.ToString();
    }

    public void UpdateStage(int stage)
    {
        stageText.text = stage.ToString();
    }

    public void UpdateChapterStage(int chapter, int stage)
    {
        chapterStageText.text = chapter + "-" + stage;
    }

    public void UpdateStageDescriptionText(string[] debuffTexts)
    {
        stageDescriptionText.gameObject.SetActive(true);
        string debuffText = string.Join("\n", debuffTexts);
        debuffText = UIUtils.SetBlockNameToIcon(debuffText);
        debuffText = debuffText.Replace("\\n", " ").Replace(",", "\n");

        this.stageDescriptionText.text = debuffText;
    }

    public void PlayScrollingTextAnimation(string text, int fontSize)
    {
        scrollingTextMask.SetActive(true);
        stageDescriptionText.gameObject.SetActive(false);

        RectTransform[] scrollingTexts = scrollingTextMask.transform.Cast<Transform>()
                                    .OfType<RectTransform>()
                                    .ToArray();

        foreach (RectTransform scrollingTextRect in scrollingTexts)
        {
            TextMeshProUGUI scrollingText = scrollingTextRect.GetComponent<TextMeshProUGUI>();
            scrollingText.text = text;
            scrollingText.fontSize = fontSize;
        }

        UIUtils.BounceText(scrollingTextMask.transform);

        scrollingTextAnimationCoroutine = StartCoroutine(ScrollingTextAnimationCoroutine(scrollingTexts));
    }

    private IEnumerator ScrollingTextAnimationCoroutine(RectTransform[] scrollingTexts)
    {
        float speed = 0.3f;

        while (true)
        {
            foreach (RectTransform scrollingText in scrollingTexts)
            {
                scrollingText.position += Vector3.left * speed * Time.deltaTime;
                
                if (scrollingText.anchoredPosition.x < -360f)
                {
                    scrollingText.anchoredPosition = new Vector2(360f, scrollingText.anchoredPosition.y);
                }
            }

            yield return null;
        }
    }

    public void ProcessStageEffectAnimation()
    {
        float punchScale = 1.2f;
        float animationDuration = 0.2f;

        // 텍스트 점점 빨갛게, 커졌다가 원래 색으로, 작게 돌아가는 시퀀스
        Sequence sequence = DOTween.Sequence();

        sequence.Append(stageDescriptionText.transform.DOScale(punchScale, animationDuration)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad));

        sequence.Join(stageDescriptionText.DOColor(Color.red, animationDuration)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad));

        sequence.OnComplete(() => {
            stageDescriptionText.transform.DOScale(Vector3.one, animationDuration);
            stageDescriptionText.DOColor(Color.white, 0.1f);
        }
        );
    }

    public void StartWarningStageEffectAnimation(bool isBlockRelated)
    {
        if (isBlockRelated) isBlockWarning = true;
        else isWarning = true;

        RectTransform stageDescriptionTextRect = stageDescriptionText.GetComponent<RectTransform>();

        if (currentWarningSequence != null) currentWarningSequence.Kill();

        currentWarningSequence = DOTween.Sequence();

        currentWarningSequence.Append(
            stageDescriptionText.transform.DOPunchPosition(Vector3.one * 5f, 0.6f,
            vibrato: 10, elasticity: 0.5f)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.InOutQuad));

        currentWarningSequence.Join(stageDescriptionText.DOColor(Color.red, 0.2f).SetEase(Ease.OutQuad));

        currentWarningSequence.OnKill(() =>
        {
            stageDescriptionTextRect.DOAnchorPos(new Vector3(118.5f, 0, 0), 0.1f);
            stageDescriptionText.DOColor(defaultTextColor, 0.1f);
        }
        );
    }

    public void StopWarningStageEffectAnimation(bool isBlockRelated)
    {
        if (isBlockRelated) isBlockWarning = false;
        else isWarning = false;

        if (currentWarningSequence != null)
        {
            if (!isBlockWarning && !isWarning)
            {
                currentWarningSequence.Kill();
            }
        }
    }
}

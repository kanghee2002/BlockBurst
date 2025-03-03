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
    [SerializeField] private Image debuffTextLayout;
    [SerializeField] private Image scoreSliderFill;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI debuffText;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI scoreAtLeastText;

    private RectTransform rectTransform;
    // inside anchored position = (300,320)
    private const float insidePositionY = 271;
    private const float outsidePositionOffsetY = 540;
    private const float duration = 0.2f;

    private Sequence currentWarningSequence;

    private bool isBlockWarning;
    private bool isWarning;

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

        currentWarningSequence = null;
        isBlockWarning = false;
        isWarning = false;
    }

    public void OpenStageInfoUI(Color uiColor)
    {
        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
        }

        float colorScalar = 3f / 4f;
        UIUtils.SetImageColorByScalar(chapterStageTextLayout, uiColor, colorScalar);
        UIUtils.SetImageColorByScalar(debuffTextLayout, uiColor, colorScalar);
        UIUtils.SetImageColorByScalar(scoreSliderFill, uiColor, 1f);
        UIUtils.SetTextColorByScalar(currentScoreText, uiColor, 1f / 10f);
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

    public void UpdateDebuffText(string[] debuffTexts)
    {
        string debuffText = string.Join("\n", debuffTexts);
        debuffText = UIUtils.SetBlockNameToIcon(debuffText);
        debuffText = debuffText.Replace("\\n", " ").Replace(",", "\n");

        this.debuffText.text = debuffText;
    }

    public void UpdateScoreAtLeast(int scoreAtLeast)
    {
        scoreAtLeastText.text = scoreAtLeast.ToString();
    }

    public void ProcessStageEffectAnimation()
    {
        float punchScale = 1.2f;
        float animationDuration = 0.2f;

        // 텍스트 점점 빨갛게, 커졌다가 원래 색으로, 작게 돌아가는 시퀀스
        Sequence sequence = DOTween.Sequence();

        sequence.Append(debuffText.transform.DOScale(punchScale, animationDuration)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad));

        sequence.Join(debuffText.DOColor(Color.red, animationDuration)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad));

        sequence.OnComplete(() => {
            debuffText.transform.DOScale(Vector3.one, animationDuration);
            debuffText.DOColor(Color.white, 0.1f);
        }
        );
    }

    public void StartWarningStageEffectAnimation(bool isBlockRelated)
    {
        if (isBlockRelated) isBlockWarning = true;
        else isWarning = true;

        RectTransform debuffRect = debuffText.GetComponent<RectTransform>();

        if (currentWarningSequence != null) currentWarningSequence.Kill();

        currentWarningSequence = DOTween.Sequence();

        currentWarningSequence.Append(
            debuffText.transform.DOPunchPosition(Vector3.one * 7f, 0.3f,
            vibrato: 15, elasticity: 0.5f)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.InOutQuad));

        currentWarningSequence.Join(debuffText.DOColor(Color.red, 0.2f).SetEase(Ease.OutQuad));

        currentWarningSequence.OnKill(() =>
        {
            debuffRect.DOAnchorPos(Vector3.zero, 0.1f);
            debuffText.DOColor(Color.white, 0.1f);
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

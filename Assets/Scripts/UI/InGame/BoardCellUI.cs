using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BoardCellUI : MonoBehaviour 
{
    private string blockId;
    private int score;
    private Vector2Int cellIndex;
    private RectTransform textTransform;
    private TextMeshProUGUI scoreText;
    private Image cellImage;
    private Vector3 originalPosition;
    private Color originalColor;
    private Sprite originalSprite;
    private Material originalMaterial;
    private bool isShadow = false;
    private bool isAnimating = false;
    private bool hideShadowReserved = false; // HideShadow 예약 플래그
    private bool isBlocked = false;
    private Tween shadowTween;

    private Sequence currentSequence;

    private void Awake() {
        cellImage = GetComponent<Image>();
        originalColor = cellImage.color;
        originalSprite = cellImage.sprite;
        originalMaterial = new Material(cellImage.material);
        textTransform = transform.GetChild(0).GetComponent<RectTransform>();
        scoreText = textTransform.GetComponent<TextMeshProUGUI>();
    }

    public void Initialize(Vector2Int index, Color color) {
        blockId = "";
        cellIndex = index;
        cellImage.color = color;
        originalColor = color;
        originalPosition = transform.position;

        currentSequence = null;
    }

    public string GetBlockId() {
        return blockId;
    }

    public void SetCellIndex(Vector2Int index) {
        cellIndex = index;
    }

    public Vector2Int GetCellIndex() {
        return cellIndex;
    }

    public void SetBlockInfo(string id, int score) {
        blockId = id;
        this.score = score;
    }

    public void ClearCell()
    {
        if (isBlocked) return;
        ClearShadow();
        blockId = "";
        cellImage.sprite = originalSprite;
        cellImage.material = new Material(originalMaterial);
        cellImage.color = originalColor;
    }

    public void CopyVisualFrom(Transform cellUI)
    {
        ClearShadow();
        cellImage.sprite = cellUI.GetComponent<Image>().sprite;
    }

    public void StopClearAnimation()
    {
        transform.DOKill();
        cellImage.DOKill();
        transform.localScale = Vector3.one;

        currentSequence.Kill();
    }

    private void AnimateShadow(Color targetColor, bool setShadow) {
        isAnimating = true;
        hideShadowReserved = false; // 예약 초기화
        shadowTween = cellImage.DOColor(targetColor, 0.1f);
        shadowTween.OnComplete(() => {
            isAnimating = false;
            isShadow = setShadow;
            
            // 예약된 HideShadow 실행
            if (hideShadowReserved && setShadow) {
                HideShadow();
            }
        });
    }

    public void ShowShadow() {
        if (isBlocked) return;
        if (!isAnimating && !isShadow) {
            Color shadowColor = string.IsNullOrEmpty(blockId) ? 
                new Color(.6f, .6f, .6f, 1f) : 
                new Color(.7f, 0, 0, 1f);
            AnimateShadow(shadowColor, true);
        }
    }

    public void HideShadow() {
        if (isBlocked) return;
        if (isAnimating) {
            hideShadowReserved = true; // 애니메이션 중일 경우 예약
        } else if (isShadow) {
            Color returnColor = string.IsNullOrEmpty(blockId) ? 
                originalColor : 
                new Color(1f, 1f, 1f, 1f);
            AnimateShadow(returnColor, false);
        }
    }

    public void ClearShadow() {
        if (isBlocked) return;
        shadowTween.Kill();
        hideShadowReserved = false;
        isShadow = false;
        isAnimating = false;
        Color returnColor = string.IsNullOrEmpty(blockId) ? 
            originalColor : 
            new Color(1f, 1f, 1f, 1f);
        cellImage.color = returnColor;
    }

    public void BlockCell() {
        isBlocked = true;
        cellImage.color = new Color(0.5f, 0.5f, 0.5f);
    }

    public void PlayClearAnimation(float preDelay, float postDelay, bool isForceMatch)
    {
        currentSequence = DOTween.Sequence();

        currentSequence
            .AppendInterval(preDelay)
            .AppendCallback(() => PlayHighlightAnimation(0f, isForceMatch))
            .AppendInterval(postDelay)
            .AppendCallback(() => PlayClearAnimation());
    }

    public void PlayHighlightAnimation(float preDelay, bool isForceMatch) {
        if (isBlocked) return;

        cellImage.sprite = originalSprite;
        cellImage.color = originalColor;

        Color highlightColor = isForceMatch ? new Color(.2f, .5f, .7f) : new Color(1f, 1f, 1f);

        cellImage.DOColor(highlightColor, 0.2f)
            .SetLoops(1, LoopType.Yoyo)
            .SetDelay(preDelay);
    }

    public void PlayClearAnimation() {
        if (isBlocked) return;

        Vector3 originalScale = transform.localScale;

        transform.DOScale(0, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() => 
            {
                transform.localScale = originalScale;
                ClearCell();
            });
    }

    // 점수 표시 애니메이션
    public void PlayScoreAnimation(int score, float preDelay)
    {
        if (isBlocked) return;

        
        scoreText.text = score.ToString();

        Vector2 originalPosition = textTransform.anchoredPosition;
        Color originalColor = scoreText.color;
        float yOffset = 10f;

        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(preDelay)
            .AppendCallback(() => textTransform.gameObject.SetActive(true))
            .Join(textTransform.DOAnchorPosY(originalPosition.y + yOffset, 0.3f)
            .SetEase(Ease.InOutQuad));
        sequence.Join(scoreText.DOFade(0f, 0.8f));


        sequence.OnComplete(() =>
            {
                textTransform.anchoredPosition = originalPosition;
                scoreText.color = originalColor;
                textTransform.gameObject.SetActive(false);
            });
    }

    public void PlayStageClearAnimation()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float distance = Random.Range(1500f, 2000f);

        RectTransform rectTransform = GetComponent<RectTransform>();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DOJumpAnchorPos(randomDirection * distance, 500f, 1, 0.6f)
            .SetEase(Ease.OutQuad));
        sequence.AppendInterval(0.5f)
            .OnComplete(() => transform.position = originalPosition);

    }
}

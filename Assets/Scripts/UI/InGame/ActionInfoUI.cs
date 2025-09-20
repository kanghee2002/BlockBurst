using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionInfoUI : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] private Image chipTextLayout;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI chipText;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI productText;

    private float chipFontSize;

    public void Initialize(int _chip, int _multiplier, int _product)
    {
        chipFontSize = chipText.fontSize;

        UpdateChip(_chip);
        UpdateMuliplier(_multiplier);
        UpdateProduct(_product);
    }

    public void SetChipLayoutColor(Color uiColor)
    {
        UIUtils.SetImageColorByScalar(chipTextLayout, uiColor, 1);
    }

    public void SetTextColor(Color uiTextColor)
    {
        UIUtils.SetTextColorByScalar(chipText, uiTextColor, 1);
        UIUtils.SetTextColorByScalar(multiplierText, uiTextColor, 1);
        UIUtils.SetTextColorByScalar(productText, uiTextColor, 1);
    }

    public void UpdateChip(int _chip)
    {
        chipText.text = _chip.ToString();
        BounceTextSize(chipText);
    }

    public void UpdateMuliplier(int _multiplier)
    {
        multiplierText.text = "X" + _multiplier.ToString();
        BounceTextSize(multiplierText);
    }

    public void AddMultiplier(int addingMultiplier)
    {
        int multiplier = GetCurrentMultiplier() + addingMultiplier;
        multiplierText.text = "X" + multiplier.ToString();

        BounceTextSize(multiplierText);
    }

    public void UpdateProduct(int product)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(DOTween.To(() => 0, x =>
        {
            productText.text = x.ToString();
        }, product, 0.5f).SetEase(Ease.InSine));
        sequence.Join(productText.transform.DOPunchScale(Vector3.one * 1f, 
            duration: 0.5f, vibrato: 10, elasticity: 1f));

        sequence.OnComplete(() => productText.transform.DOScale(Vector3.one, 0.3f));
    }

    public void ProcessScoreUpdateAnimation(Dictionary<Match, List<int>> scores, float delay)
    {
        StartCoroutine(ScoreUpdateAnimationCoroutine(scores, delay));
    }

    private IEnumerator ScoreUpdateAnimationCoroutine(Dictionary<Match, List<int>> scores, float delay)
    {
        UpdateChip(0);
        foreach ((Match match, List<int> scoreList) in scores)
        {
            foreach (int score in scoreList)
            {
                UpdateChip(GetCurrentChip() + score);

                yield return new WaitForSeconds(delay);

                chipText.transform.DOKill();
            }
        }
        chipText.transform.DOKill();
        chipText.transform.DOScale(Vector3.one, 1f);
    }
    
    private void BounceTextSize(TextMeshProUGUI text, float duration = 0.15f, float punchSize = 10f)
    {
        float originalSize = text.fontSize;

        if (originalSize >= 100f) return;

        DOTween.To(() => text.fontSize, x => text.fontSize = x, originalSize + punchSize, duration)
        .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad)
            .OnKill(() => DOTween.To(() => text.fontSize, x => text.fontSize = x, chipFontSize, 0.2f));
    }

    private int GetCurrentChip()
    {
        int chip = 0;
        if (int.TryParse(chipText.text, out chip))
        {
            return chip;
        }
        else
        {
            Debug.Log("Error: chipText is not int form");
        }
        return chip;
    }

    private int GetCurrentMultiplier()
    {
        int multiplier = 0;
        string text = multiplierText.text.Substring(1);
        if (int.TryParse(text, out multiplier))
        {
            return multiplier;
        }
        else
        {
            Debug.Log("Error: multiplierText is not int form");
        }
        return multiplier;
    }
}

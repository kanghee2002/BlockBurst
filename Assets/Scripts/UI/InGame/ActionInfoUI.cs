using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI chipText;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI productText;

    public void Initialize(int _chip, int _multiplier, int _product)
    {
        UpdateChip(_chip);
        UpdateMuliplier(_multiplier);
        UpdateProduct(_product);
    }
    public void UpdateChip(int _chip)
    {
        chipText.text = _chip.ToString();
        UIUtils.BounceText(chipText.transform, duration: 0.15f, strength: 0.5f);
    }
    public void UpdateMuliplier(int _multiplier)
    {
        multiplierText.text = _multiplier.ToString();
        UIUtils.BounceText(multiplierText.transform, duration: 0.15f, strength: 0.5f);
    }

    public void AddMultiplier(int addingMultiplier)
    {
        int multiplier = GetCurrentMultiplier() + addingMultiplier;
        multiplierText.text = multiplier.ToString();

        UIUtils.BounceText(multiplierText.transform, duration: 0.15f, strength: 0.5f);
    }

    public void UpdateProduct(int product)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(DOTween.To(() => 0, x =>
        {
            productText.text = x.ToString();
        }, product, 1f).SetEase(Ease.InSine));
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
        if (int.TryParse(multiplierText.text, out multiplier))
        {
            return multiplier;
        }
        else
        {
            Debug.Log("Error: chipText is not int form");
        }
        return multiplier;
    }
}

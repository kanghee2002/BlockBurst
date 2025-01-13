using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI chipText;
    [SerializeField] private TextMeshProUGUI multiplierText;
    public void Initialize(int _chip, int _multiplier)
    {
        UpdateChip(_chip);
        UpdateMuliplier(_multiplier);
    }
    public void UpdateChip(int _chip)
    {
        chipText.text = _chip.ToString();
    }
    public void UpdateMuliplier(int _multiplier)
    {
        multiplierText.text = _multiplier.ToString();
    }
}

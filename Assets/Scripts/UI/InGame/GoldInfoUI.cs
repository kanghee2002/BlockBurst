using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;

    public void Initialize(int _gold)
    {
        UpdateGold(_gold);
    }
    public void UpdateGold(int _gold)
    {
        goldText.text = _gold.ToString();
    }
}

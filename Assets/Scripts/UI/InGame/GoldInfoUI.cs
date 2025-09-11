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

    public void UpdateGold(int _gold, bool isAdditive = false)
    {
        if (isAdditive)
        {
            int currentGold = GetCurrentGold() + _gold;
            if (currentGold < 0) currentGold = 0;
            goldText.text = "$" + currentGold.ToString();
        }
        else
        {
            goldText.text = "$" + _gold.ToString();
        }
        UIUtils.BounceText(goldText.transform);
    }

    public void SetUIColor(Color textColor)
    {
        UIUtils.SetTextColorByScalar(goldText, textColor, 1);
    }

    private int GetCurrentGold()
    {
        int gold = 0;
        string text = goldText.text.Substring(1);
        if (int.TryParse(text, out gold))
        {
            return gold;
        }
        else
        {
            Debug.Log("Error: goldText is not int form");
        }
        return gold;
    }
}

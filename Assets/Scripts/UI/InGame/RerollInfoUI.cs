using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RerollInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rerollCountText;

    public void Initialize(int _rerollCount)
    {
        UpdateRerollCount(_rerollCount);
    }
    public void UpdateRerollCount(int _rerollCount)
    {
        rerollCountText.text = _rerollCount.ToString();
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageSelectionSignboardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageSelectionSignboardText;
    private RectTransform rectTransform;
    // inside anchored position = (300,256)
    private const float insidePositionY = 256;
    private const float outsidePositionOffsetY = 540;
    private const float duration = 0.2f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OpenStageSelectionSignboardUI()
    {
        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
    }

    public void CloseStageSelectionSignboardUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionOffsetY, duration);
    }
    
    public void Initialize(int currentChapterIndex, int currentStageIndex)
    {
        string text = currentChapterIndex + " - " + currentStageIndex + "\n" + "스테이지를 선택하세요";
        stageSelectionSignboardText.text = text;
    }
}

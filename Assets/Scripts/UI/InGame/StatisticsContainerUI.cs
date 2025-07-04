using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatisticsContainerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI maxScoreText;
    [SerializeField] private TextMeshProUGUI maxChapterStageText;
    [SerializeField] private TextMeshProUGUI maxPlaceCountText;
    [SerializeField] private TextMeshProUGUI maxMultiplierText;
    [SerializeField] private TextMeshProUGUI maxRerollCountText;
    [SerializeField] private TextMeshProUGUI maxGoldText;
    
    public void Initialize(int maxScore, int maxChapter, int maxStage,
        BlockType mostPlacedBlockType, int maxPlaceCount,
        int maxMultiplier, int maxRerollCount, int maxGold)
    {
        maxScoreText.text = maxScore.ToString();

        maxChapterStageText.text = maxChapter + " - " + maxStage;

        maxPlaceCountText.text = string.Format("<sprite={0}> ({1})", (int)mostPlacedBlockType, maxPlaceCount);

        maxMultiplierText.text = maxMultiplier.ToString();

        maxRerollCountText.text = maxRerollCount.ToString();

        maxGoldText.text = maxGold.ToString();
    }
}

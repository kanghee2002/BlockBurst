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
    
    public void Initialize(PlayerData playerData)
    {
        maxScoreText.text = playerData.maxScore.ToString();

        maxChapterStageText.text = playerData.maxChapter + " - " + playerData.maxStage;

        (BlockType mostPlacedBlockType, int placeCount) = GetMostPlacedBlock(playerData);
        maxPlaceCountText.text = string.Format("<sprite={0}> ({1})", (int)mostPlacedBlockType, placeCount);

        maxMultiplierText.text = playerData.maxMultiplier.ToString();

        maxRerollCountText.text = playerData.maxBaseRerollCount.ToString();

        maxGoldText.text = playerData.maxGold.ToString();
    }

    private (BlockType, int) GetMostPlacedBlock(PlayerData playerData)
    {
        BlockType blockType = BlockType.I;
        int count = playerData.placeCountI;

        if (playerData.placeCountO > count)
        {
            blockType = BlockType.O;
            count = playerData.placeCountO;
        }
        if (playerData.placeCountZ > count)
        {
            blockType = BlockType.Z;
            count = playerData.placeCountZ;
        }
        if (playerData.placeCountS > count)
        {
            blockType = BlockType.S;
            count = playerData.placeCountS;
        }
        if (playerData.placeCountJ > count)
        {
            blockType = BlockType.J;
            count = playerData.placeCountJ;
        }
        if (playerData.placeCountL > count)
        {
            blockType = BlockType.L;
            count = playerData.placeCountL;
        }
        if (playerData.placeCountT > count)
        {
            blockType = BlockType.T;
            count = playerData.placeCountT;
        }

        return (blockType, count);
    }
}

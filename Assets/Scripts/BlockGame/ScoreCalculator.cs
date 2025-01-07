using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    public static ScoreCalculator Instance { get; private set; }

    public int Calculate(Match match, BlockGameData data)
    {
        switch (match.matchType)
        {
            case MatchType.ROW:
                break;
            case MatchType.COLUMN:
                break;
            case MatchType.SQUARE:
                break;
            default:
                Debug.Log("Error");
                break;
        }

        int totalScore = 0, totalMultiplier = 1;
        foreach (BlockType blockType in match.blockTypes)
        {
            totalScore += data.blockScores[blockType];
        }

        return totalScore * totalMultiplier;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    public static ScoreCalculator Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 파괴
        }
    }

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

        // 아이템 효과 발동

        int totalScore = 0, totalMultiplier = 1;
        foreach (BlockType blockType in match.blockTypes)
        {
            totalScore += data.blockScores[blockType];
        }

        return totalScore * totalMultiplier;
    }
}

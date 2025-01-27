using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    public static ScoreCalculator instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 파괴
        }
    }

    private int lastScore;

    public int GetLastScore()
    {
        int tmp = lastScore;
        lastScore = 0;
        return tmp;
    }

    public int Calculate(Match match, BlockGameData data)
    {
        switch (match.matchType)
        {
            case MatchType.ROW:
                break;
            case MatchType.COLUMN:
                break;
            default:
                Debug.Log("Error");
                break;
        }

        int totalScore = 0;
        foreach ((BlockType blockType, string id) in match.blocks)
        {
            totalScore += data.blockScores[blockType];
        }

        Debug.Log("블록 점수: " + totalScore);
        Debug.Log("배수 : " + data.matchMultipliers[MatchType.ROW]);

        lastScore += totalScore * data.matchMultipliers[MatchType.ROW];

        return totalScore * data.matchMultipliers[MatchType.ROW];
    }
}

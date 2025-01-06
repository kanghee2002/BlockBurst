using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfoUI : MonoBehaviour
{
    public void UpdateAnte(int _ante)
    {
        Debug.Log("Ante has been updated.");
    }

    public void UpdateRound(int _round)
    {
        Debug.Log("Round has been updated.");
    }

    public void UpdateDebuffText(string _debuffText)
    {
        Debug.Log("DebuffText has been updated.");
    }

    public void UpdateScoreAtLeast(int _scoreAtLeast)
    {
        Debug.Log("ScoreAtLeast has been updated.");
    }
}

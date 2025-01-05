using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoUI : MonoBehaviour
{
    public void UpdateScore(int currentScore, int targetScore);
    public void UpdateMoves(int remainingMoves, int totalMoves);
    public void UpdateMultiplier(float multiplier);
    public void ShowBlockScores(Dictionary<BlockType, int> scores);
    public void ShowStageProgress(StageRequirement requirement, StageProgress progress);
    public void ShowScorePopup(int amount, Vector2 position);
    public void UpdateLineClears(int current, int required);
}

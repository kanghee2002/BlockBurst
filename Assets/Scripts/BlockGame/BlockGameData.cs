using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGameData
{
    // 현재 게임 상태
    public Dictionary<BlockType, int> blockScores;      // 블록별 점수
    public Dictionary<MatchType, float> matchMultipliers;  // 매치 타입별 배율
    public int currentScore;                            // 현재 점수
    public int moveCount;                               // 이동 횟수
    public Dictionary<string, HashSet<Vector2Int>> activeBlockCells;  // 현재 활성화된 블록 cell 위치

    public void Initialize(RunData runData)
    {
        blockScores = new Dictionary<BlockType, int>(runData.baseBlockScores);
        matchMultipliers = new Dictionary<MatchType, float>(runData.baseMatchMultipliers);
        currentScore = 0;
        moveCount = 0;
        activeBlockCells = new Dictionary<string, HashSet<Vector2Int>>();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGameData
{
    // 블럭 게임 데이터
    public Dictionary<BlockType, int> blockScores;      // 블럭 점수
    public Dictionary<MatchType, float> matchMultipliers;  // 매치 배율
    public int currentScore;                            // 현재 점수
    public int moveCount;                               // 이동 횟수
    public Dictionary<string, HashSet<Vector2Int>> activeBlockCells;  // 활성 블럭 셀
    public List<BlockData> deck;
    public int rerollCount;

    public void Initialize(RunData runData)
    {
        blockScores = new Dictionary<BlockType, int>(runData.baseBlockScores);
        matchMultipliers = new Dictionary<MatchType, float>(runData.baseMatchMultipliers);
        currentScore = 0;
        moveCount = 0;
        activeBlockCells = new Dictionary<string, HashSet<Vector2Int>>();
        deck = new List<BlockData>(runData.availableBlocks);
        rerollCount = runData.currentRerollCount;
    }
}
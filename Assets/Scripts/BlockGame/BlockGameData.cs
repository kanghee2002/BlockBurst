using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGameData
{
    // ���� ���� ����
    public Dictionary<BlockType, int> blockScores;      // ���Ϻ� ����
    public Dictionary<MatchType, float> matchMultipliers;  // ��ġ Ÿ�Ժ� ����
    public int currentScore;                            // ���� ����
    public int moveCount;                               // �̵� Ƚ��
    public Dictionary<string, HashSet<Vector2Int>> activeBlockCells;  // ���� Ȱ��ȭ�� ���� cell ��ġ
    public List<BlockData> deck;
    public int rerollCount;
    public int score;

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
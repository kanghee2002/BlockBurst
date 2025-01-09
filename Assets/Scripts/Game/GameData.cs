using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    // �⺻ ���� ������
    public Dictionary<BlockType, int> defaultBlockScores;     // �⺻ ���� ����
    public Dictionary<MatchType, float> defaultMatchMultipliers;  // �⺻ ��ġ ����
    public List<BlockData> defaultBlocks;                     // �⺻ ���� ���
    public List<StageData> stagePool;                        // �������� Ǯ
    public int startingGold;                                 // ���� ���
    public int defaultRerollCount;                           // �⺻ ���� Ƚ��

    public void Initialize()
    {
        // �⺻�� ����
        defaultBlockScores = new Dictionary<BlockType, int>();
        defaultMatchMultipliers = new Dictionary<MatchType, float>();
        defaultBlocks = new List<BlockData>();
        stagePool = new List<StageData>();
        startingGold = 10;
        defaultRerollCount = 3;

        // �⺻ ���� ���� ����
        defaultBlockScores[BlockType.I] = 100;
        defaultBlockScores[BlockType.O] = 100;
        defaultBlockScores[BlockType.T] = 150;
        defaultBlockScores[BlockType.L] = 120;
        defaultBlockScores[BlockType.J] = 120;
        defaultBlockScores[BlockType.S] = 130;
        defaultBlockScores[BlockType.Z] = 130;

    }
}
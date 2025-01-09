using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunData
{
    // �������� ���� ������
    public Dictionary<BlockType, int> baseBlockScores;     // �⺻ ���� ����
    public Dictionary<MatchType, float> baseMatchMultipliers;  // �⺻ ��ġ ����
    public List<BlockData> availableBlocks;               // ��� ������ ���ϵ�
    public List<ItemData> activeItems;                    // Ȱ��ȭ�� �����۵�
    public List<EffectData> activeEffects;                // Ȱ��ȭ�� ȿ����
    public StageData currentStage;                        // ���� ��������
    public Dictionary<string, int> blockReuses;           // ���Ϻ� ���� Ƚ��
    public int gold;                                      // ���� ���� ���
    public int baseRerollCount;                           // �⺻ ���� Ƚ��
    public int currentRerollCount;                        // ���� ���� Ƚ��
    public float baseMultiplier;                          // �⺻ ����

    public void Initialize(GameData gameData)
    {
        baseBlockScores = new Dictionary<BlockType, int>(gameData.defaultBlockScores);
        baseMatchMultipliers = new Dictionary<MatchType, float>(gameData.defaultMatchMultipliers);
        availableBlocks = new List<BlockData>(gameData.defaultBlocks);
        activeEffects = new List<EffectData>();
        blockReuses = new Dictionary<string, int>();
        gold = gameData.startingGold;
        baseRerollCount = gameData.defaultRerollCount;
        currentRerollCount = baseRerollCount;
        baseMultiplier = 1.0f;
    }
}

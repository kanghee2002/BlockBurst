using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "BlockBurst/Stage")]
public class StageData : ScriptableObject
{
    public string id;                        // �������� ID
    public StageType type;                   // Normal1, Normal2, Boss
    public Vector2Int boardSize;             // ���� ũ��
    public List<Vector2Int> blockedCells;    // ��� �Ұ����� ��
    public List<EffectData> constraints;     // �������� ���ѻ���
    public StageRequirement clearRequirement; // Ŭ���� ����
    public int goldReward;                   // Ŭ���� ����
}

public class StageRequirement
{
    public int targetScore;      // ��ǥ ����
    public int maxMoves;         // �ִ� �̵� ��
    public int minLineClears;    // �ּ� ���� Ŭ���� ��
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "BlockBurst/Stage")]
public class StageData : ScriptableObject
{
    public string id;                        // 스테이지 ID
    public StageType type;                   // Normal1, Normal2, Boss
    public Vector2Int boardSize;             // 보드 크기
    public List<Vector2Int> blockedCells;    // 사용 불가능한 셀
    public List<EffectData> constraints;     // 스테이지 제한사항
    public StageRequirement clearRequirement; // 클리어 조건
    public int goldReward;                   // 클리어 보상
}

public class StageRequirement
{
    public int targetScore;      // 목표 점수
    public int maxMoves;         // 최대 이동 수
    public int minLineClears;    // 최소 라인 클리어 수
}
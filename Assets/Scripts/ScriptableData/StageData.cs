using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "BlockBurst/Stage")]
public class StageData : ScriptableObject
{
    public string id;                                       // 스테이지 ID
    public StageType type;                                  // Normal, Boss
    public Vector2Int boardSize;                            // 보드 사이즈
    public List<Vector2Int> blockedCells;                   // 막힌 셀
    public List<EffectData> constraints;                    // 제약 조건
    public float baseScoreMultiplier;                       // 기본 목표 점수 배수
    [HideInInspector] public int clearRequirement;          // 클리어 조건 (임시로 int)
    [HideInInspector] public int goldReward;                // 보상 골드
}

// 일단 안 씀
public struct StageRequirement
{
    public int targetScore;      // 목표 점수
    public int maxMoves;         // 최대 이동수
    public int minLineClears;    // 최소 라인 클리어 수
}
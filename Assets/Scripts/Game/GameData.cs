using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    // 기본 게임 데이터
    public Dictionary<BlockType, int> defaultBlockScores;           // 기본 블록 점수
    public Dictionary<MatchType, int> defaultMatchMultipliers;      // 기본 배수
    public List<BlockData> defaultBlocks;                           // 기본 블록 몰록
    public List<StageData> stagePool;                               // 스테이지 풀
    public int startingGold;                                        // 시작 골드
    public int defaultRerollCount;                                  // 기본 리롤 횟수
    public int defaultBlockCount;                                   // 기본 블록 수
    public float[] stageBaseScoreMultipliers;                       // 스테이지 목표 점수 배수
    public int stageBaseScores;                                   // 챕터 당 기본 목표 점수
    public int stageBaseReward;                                      // 스테이지 클리어 골드
    public float difficulty;

    public void Initialize()
    {
        // 기본값 설정
        defaultBlockScores = new Dictionary<BlockType, int>();
        defaultMatchMultipliers = new Dictionary<MatchType, int>();
        defaultBlocks = new List<BlockData>();
        stagePool = new List<StageData>();
        startingGold = 0;
        defaultRerollCount = 3;
        defaultBlockCount = 3;
        
        // 기본 블록 점수 설정
        defaultBlockScores[BlockType.I] = 10;
        defaultBlockScores[BlockType.O] = 10;
        defaultBlockScores[BlockType.Z] = 10;
        defaultBlockScores[BlockType.S] = 10;
        defaultBlockScores[BlockType.J] = 10;
        defaultBlockScores[BlockType.L] = 10;
        defaultBlockScores[BlockType.T] = 10;
        defaultBlockScores[BlockType.SOLO] = 10;
        defaultBlockScores[BlockType.DUO] = 10;
        defaultBlockScores[BlockType.TRIO] = 10;
        defaultBlockScores[BlockType.X] = 10;
        defaultBlockScores[BlockType.CROSS] = 10;
        defaultBlockScores[BlockType.CORNER] = 10;
        defaultBlockScores[BlockType.M] = 10;
        defaultBlockScores[BlockType.MEGASQUARE] = 0;
        defaultBlockScores[BlockType.ULTRASQUARE] = 0;

        // 기본 배수 설정
        defaultMatchMultipliers[MatchType.ROW] = 1;

        stageBaseScoreMultipliers = new float[3] { 1.3f, 1.5f, 2f };

        stageBaseScores = 100;
        stageBaseReward = 5;
        difficulty = 1.0f;
    }
}
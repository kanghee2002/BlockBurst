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

    public void Initialize()
    {
        // 기본값 설정
        defaultBlockScores = new Dictionary<BlockType, int>();
        defaultMatchMultipliers = new Dictionary<MatchType, int>();
        defaultBlocks = new List<BlockData>();
        stagePool = new List<StageData>();
        startingGold = 10;
        defaultRerollCount = 3;

        // 기본 블록 점수 설정
        defaultBlockScores[BlockType.I] = 100;
        defaultBlockScores[BlockType.O] = 100;
        defaultBlockScores[BlockType.T] = 150;
        defaultBlockScores[BlockType.L] = 120;
        defaultBlockScores[BlockType.J] = 120;
        defaultBlockScores[BlockType.S] = 130;
        defaultBlockScores[BlockType.Z] = 130;

    }
}
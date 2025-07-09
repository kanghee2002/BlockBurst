using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameData
{
    // 기본 게임 데이터
    public DeckData currentDeck;
    public LevelData currentLevel;
    public Dictionary<BlockType, int> defaultBlockScores;           // 기본 블록 점수
    public Dictionary<MatchType, int> defaultMatchMultipliers;      // 기본 배수
    public List<BlockData> defaultBlocks;                           // 기본 블록 몰록
    public List<EffectData> defaultEffects;                         // 기본 효과 목록
    public List<StageData> stagePool;                               // 스테이지 풀
    public int startingGold;                                        // 시작 골드
    public int defaultRerollCount;                                  // 기본 리롤 횟수
    public int maxItemCount;                                        // 소지 가능 아이템 아이템 수
    public int baseBoardRows;                                       // 보드 크기
    public int baseBoardColumns;                                    // 보드 크기 수
    public int stageBaseReward;                                     // 스테이지 클리어 골드
    public List<int> stageBaseScoreList;
    public List<float> stageScoreMultiplier;

    public void Initialize(BlockData[] blockTemplates, DeckData deckData, LevelData levelData)
    {
        // 기본값 설정
        currentDeck = deckData;
        currentLevel = levelData;
        defaultBlockScores = new Dictionary<BlockType, int>();
        defaultMatchMultipliers = new Dictionary<MatchType, int>();
        defaultBlocks = new List<BlockData>();
        defaultEffects = new List<EffectData>(deckData.effects);
        stagePool = new List<StageData>();
        startingGold = 5;
        defaultRerollCount = deckData.defaultRerlollCount;
        maxItemCount = deckData.maxItemCount;
        baseBoardRows = deckData.baseBoardRows;
        baseBoardColumns = deckData.baseBoardColumns;

        // 기본 블록 설정
        foreach (BlockType blockType in deckData.defaultBlocks)
        {
            foreach (BlockData block in blockTemplates)
            { 
                if (block.type == blockType)
                {
                    defaultBlocks.Add(block);
                    break;
                }
            }
        }

        // 기본 블록 점수 설정
        defaultBlockScores[BlockType.I] = 10;
        defaultBlockScores[BlockType.O] = 10;
        defaultBlockScores[BlockType.Z] = 20;
        defaultBlockScores[BlockType.S] = 20;
        defaultBlockScores[BlockType.J] = 15;
        defaultBlockScores[BlockType.L] = 15;
        defaultBlockScores[BlockType.T] = 15;
        defaultBlockScores[BlockType.SOLO] = 10;
        defaultBlockScores[BlockType.DUO] = 10;
        defaultBlockScores[BlockType.TRIO] = 10;
        defaultBlockScores[BlockType.X] = 10;
        defaultBlockScores[BlockType.CROSS] = 10;
        defaultBlockScores[BlockType.CORNER] = 10;
        defaultBlockScores[BlockType.M] = 10;

        // 기본 배수 설정
        defaultMatchMultipliers[MatchType.ROW] = 1;

        stageBaseReward = 5;

        stageBaseScoreList = new List<int>()
        {
            120,
            1000,
            4000,
            15000,
            50000,
            150000,
            500000,
            1600000,
        };

        stageScoreMultiplier = new List<float>()
        {
            1f, 2f, 3f
        };

        // 레벨 설정

        for (int i = 0; i < levelData.additionalStageScores.Count; i++)
        {
            stageBaseScoreList[i] += levelData.additionalStageScores[i];
        }

        stageBaseReward += levelData.additionalStageReward;

        defaultRerollCount += levelData.additionalBaseRerollCount;

        foreach (BlockType blockType in defaultBlockScores.Keys.ToList())
        {
            defaultBlockScores[blockType] += levelData.additionalBlockScore;
        }

        maxItemCount += levelData.additionalMaxItemCount;
    }
}
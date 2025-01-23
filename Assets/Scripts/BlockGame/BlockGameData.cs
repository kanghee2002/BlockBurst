using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGameData
{
    // 블럭 게임 데이터
    public Dictionary<BlockType, int> blockScores;                      // 블록 점수
    public Dictionary<MatchType, int> matchMultipliers;                 // 배수
    public int currentScore;                                            // 현재 점수
    public int moveCount;                                               // 이동 횟수
    public bool isCornerBlocked;                                        // 가장자리 막혔는지
    public HashSet<Vector2Int> inactiveBlockCells;                      // 활성 블럭 셀
    public List<BlockData> deck;                                        // 덱
    public int rerollCount;                                             // 리롤 횟수
    public int drawBlockCount;                                          // 드로우 블록 수
    public int boardRows;                                               // 보드 행
    public int boardColumns;                                            // 보드 열
    public bool isDeckEmpty;                                            // 

    public void Initialize(RunData runData)
    {
        blockScores = new Dictionary<BlockType, int>(runData.baseBlockScores);
        matchMultipliers = new Dictionary<MatchType, int>(runData.baseMatchMultipliers);
        currentScore = 0;
        moveCount = 0;
        isCornerBlocked = false;
        inactiveBlockCells = new HashSet<Vector2Int>();
        deck = new List<BlockData>();
        rerollCount = runData.baseRerollCount;
        drawBlockCount = runData.baseDrawBlockCount;
        boardRows = runData.baseBoardRows;
        boardColumns = runData.baseBoardColumns;
        isDeckEmpty = false;
    }
}
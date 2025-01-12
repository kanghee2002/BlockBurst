using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Cell[,] cells { get; private set; }

    private RunData runData;
    private BlockGameData gameData;

    private List<int> onClearEffectBlocks;      // 모두 지워질 때 효과 발동하는 블록 ID 저장
    private int matchCount;
    private bool hasMatched;
    private bool isHalfFull;

    public void Initialize(RunData runData, BlockGameData blockGameData)
    {
        this.runData = runData;
        gameData = blockGameData;
        EffectManager.instance.TriggerEffects(TriggerType.ON_BOARD_NOT_HALF_FULL);


        onClearEffectBlocks = new List<int>();
        matchCount = 0;
        hasMatched = false;
        isHalfFull = false;
    }

    // 블록 배치 처리
    public bool PlaceBlock(Block block, Vector2Int pos)
    {
        bool isPlaced = false;

        if (CanPlace(block, pos))
        {
            isPlaced = true;

            int width = block.Shape.GetLength(0);
            int height = block.Shape.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (block.Shape[x, y])
                    {
                        Vector2Int curPos = pos + new Vector2Int(x, y);

                        cells[curPos.x, curPos.y].SetBlock(block.Type, block.Id);
                    }
                }
            }
            EffectManager.instance.TriggerEffects(TriggerType.ON_BLOCK_PLACE);

            if (IsHalfFull())
            {
                // 이전에 반 이상 차 있지 않았다면
                if (!isHalfFull)
                {
                    EffectManager.instance.TriggerEffects(TriggerType.ON_BOARD_HALF_FULL);
                }
                isHalfFull = true;
            }
            else
            {
                // 이전에 반 이상 차 있었으면
                if (isHalfFull)
                {
                    EffectManager.instance.TriggerEffects(TriggerType.ON_BOARD_NOT_HALF_FULL);
                }
                isHalfFull = false;
            }

            ProcessMatches(block, pos);
        }

        return isPlaced;
    }

    // 강제 행 매치 처리 (무작위로 줄 지우는 효과)
    public void ForceRowMatches(List<int> rows)
    {
        foreach (int row in rows)
        {
            //List<Match> rowMatches = CheckRowMatch(row, 1);
        }
    }

    // 강제 열 매치 처리 (무작위로 줄 지우는 효과)
    public void ForceColumnMatches(List<int> columns)
    {

    }


    // 매치 처리
    private void ProcessMatches(Block block, Vector2Int pos)
    {
        List<Match> matches = CheckMatches(block, pos);

        // 효과 발동
        // 줄이 지워지지 않았을 때
        if (matches.Count == 0)
        {
            hasMatched = false;
            EffectManager.instance.TriggerEffects(TriggerType.ON_BLOCK_PLACE_WITHOUT_LINE_CLEAR);
        }
        // 줄이 지워졌을 때
        else
        {
            EffectManager.instance.TriggerEffects(TriggerType.ON_BLOCK_PLACE_WITH_LINE_CLEAR, blockTypes: new BlockType[] { block.Type });
            if (hasMatched)
            {
                EffectManager.instance.TriggerEffects(TriggerType.ON_LINE_CLEAR_CONSECUTIVELY);
            }
            hasMatched = true;
            if (matchCount == 0)
            {
                EffectManager.instance.TriggerEffects(TriggerType.ON_FIRST_LINE_CLEAR);
            }
            matchCount++;
            EffectManager.instance.TriggerEffects(TriggerType.ON_BLOCK_PLACE_WITH_LINE_CLEAR, blockTypes: new BlockType[] { block.Type });
            EffectManager.instance.TriggerEffects(TriggerType.ON_LINE_CLEAR);
            EffectManager.instance.TriggerEffects(TriggerType.ON_LINE_CLEAR_WITH_COUNT, triggerValue: matchCount);
            EffectManager.instance.TriggerEffects(TriggerType.ON_MULTIPLE_LINE_CLEAR, triggerValue: matches.Count);

            HashSet<(BlockType, int)> clearedBlocks = new HashSet<(BlockType, int)>();
            foreach (Match match in matches)
            {
                clearedBlocks.UnionWith(match.blocks);
            }

            // 특정 블록이 포함돼있을 때
            HashSet<BlockType> hashedBlockTypes = new HashSet<BlockType>(clearedBlocks.Select(x => x.Item1));
            foreach (BlockType blockType in hashedBlockTypes)
            {
                EffectManager.instance.TriggerEffects(TriggerType.ON_LINE_CLEAR_WITH_SPECIFIC_BLOCKS, blockTypes: new BlockType[] { blockType });  
            }
            // 같은 종류의 블록이 있으면
            if (clearedBlocks.Count > hashedBlockTypes.Count)
            {
                EffectManager.instance.TriggerEffects(TriggerType.ON_LINE_CLEAR_WITH_SAME_BLOCK);
            }
            // 모두 다른 종류의 블록이면
            if (clearedBlocks.Count == hashedBlockTypes.Count)
            {
                EffectManager.instance.TriggerEffects(TriggerType.ON_LINE_CLEAR_WITH_DISTINCT_BLOCKS);
            }
        }

        // 가로, 세로 줄 지우기 관련
        int rowClearCount = 0, columnClearCount = 0;
        foreach (Match match in matches)
        {
            if (match.matchType == MatchType.ROW) rowClearCount++;
            if (match.matchType == MatchType.COLUMN) columnClearCount++;
        }

        if (rowClearCount > 0)
        {
            EffectManager.instance.TriggerEffects(TriggerType.ON_ROW_LINE_CLEAR);
        }
        if (columnClearCount > 0)
        {
            EffectManager.instance.TriggerEffects(TriggerType.ON_COLUMN_LINE_CLEAR);
        }

        if (rowClearCount > 0 && columnClearCount > 0)
        {
            EffectManager.instance.TriggerEffects(TriggerType.ON_CROSS_LINE_CLEAR);
        }


        // 점수 계산
        int totalScore = 0;
        foreach (Match match in matches)
        {
            totalScore += ScoreCalculator.Instance.Calculate(match, gameData);
        }

        gameData.currentScore += totalScore;

        Debug.Log("현재 점수: " + gameData.currentScore);
        Debug.Log("현재 배수: " + gameData.matchMultipliers[MatchType.ROW]);

        // 배수 초기화
        if (matches.Count > 0)
        {
            Debug.Log("계산된 점수: " + totalScore);
            gameData.matchMultipliers = new(runData.baseMatchMultipliers);
        }
    }

    // 매치 확인
    private List<Match> CheckMatches(Block block, Vector2Int pos)
    {
        List<Match> matches = new List<Match>();

        List<int> rows = Enumerable.Range(pos.y, block.Shape.GetLength(1)).ToList();
        List<int> columns = Enumerable.Range(pos.x, block.Shape.GetLength(0)).ToList();

        List<Match> rowMatches = CheckRowMatch(rows);
        List<Match> columnMatches = CheckColumnMatch(columns);

        matches.AddRange(rowMatches);
        matches.AddRange(columnMatches);

        return matches;
    }

    // 행 매치 확인
    private List<Match> CheckRowMatch(List<int> rows)
    {
        List<Match> matches = new List<Match>();

        foreach (int y in rows)
        {
            // 한 줄 완성 확인
            bool isMatched = true;
            // TEST: 8 -> Board Size로 변경 필요
            for (int x = 0; x < 8; x++)
            {
                if (!cells[x, y].IsBlocked)
                {
                    isMatched = false;
                    break;
                }
            }

            // 한 줄 완성 시
            if (isMatched)
            {
                Match match = new Match()
                {
                    matchType = MatchType.ROW,
                    blocks = new List<(BlockType, int)>()
                };
                // TEST: 8 -> Board Size로 변경 필요
                for (int x = 0; x < 8; x++)
                {
                    Cell currentCell = cells[x, y];
                    match.blocks.Add(((BlockType)currentCell.Type, currentCell.BlockID));
                    currentCell.ClearBlock();
                }
                matches.Add(match);
            }
        }
        return matches;
    }

    // 열 매치 확인
    private List<Match> CheckColumnMatch(List<int> columns)
    {
        List<Match> matches = new List<Match>();

        foreach (int x in columns)
        {
            // 한 줄 완성 확인
            bool isMatched = true;
            // TEST: 8 -> Board Size로 변경 필요
            for (int y = 0; y < 8; y++)
            {
                if (!cells[x, y].IsBlocked)
                {
                    isMatched = false;
                    break;
                }
            }

            // 한 줄 완성 시
            if (isMatched)
            {
                Match match = new Match()
                {
                    matchType = MatchType.COLUMN,
                    blocks = new List<(BlockType, int)>()
                };
                // TEST: 8 -> Board Size로 변경 필요
                for (int y = 0; y < 8; y++)
                {
                    Cell currentCell = cells[x, y];
                    match.blocks.Add(((BlockType)currentCell.Type, currentCell.BlockID));
                    currentCell.ClearBlock();
                }
                matches.Add(match);
            }
        }
        return matches;
    }

    // 정사각형 매치 확인
    private Match CheckSquareMatch(Block block, Vector2Int pos)
    {
        return null;
    }

    // 블록 배치 가능 여부 확인
    private bool CanPlace(Block block, Vector2Int pos)
    {
        int width = block.Shape.GetLength(0);
        int height = block.Shape.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (block.Shape[x, y])
                {
                    Vector2Int curPos = pos + new Vector2Int(x, y);

                    if (IsOutOfBoard(curPos)) return false;

                    if (IsBlocked(curPos)) return false;
                }
            }
        }
        return true;
    }

    private bool IsOutOfBoard(Vector2Int pos)
    {
        // Need to change 8 to board size
        if (pos.x < 0 || pos.x >= 8 || pos.y < 0 || pos.y >= 8)
            return true;
        else
            return false;
    }

    private bool IsBlocked(Vector2Int pos)
    {
        if (cells[pos.x, pos.y].IsBlocked) return true;
        else return false;
    }

    private bool IsHalfFull()
    {
        int row = cells.GetLength(0), col = cells.GetLength(1);
        int blockCount = 0, cellCount = row * col;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (cells[i, j].IsBlocked && cells[i, j].BlockID != -1)
                {
                    blockCount++;
                }
            }
        }

        if (blockCount * 2 >= cellCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Test Code  ////////////////////////////////////////////////////////////
    [SerializeField] private Cell[] tmpCells;

    private void Start()
    {
        cells = new Cell[8, 8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                tmpCells[i * 8 + j].transform.position = new Vector3(j, -i);

                /*
                if ((i + j) % 2 == 0)
                {
                    tmpCells[i * 8 + j].GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
                }*/

                tmpCells[i * 8 + j].cellPosition = new Vector2Int(j, i);
                cells[j, i] = tmpCells[i * 8 + j];
                cells[j, i].Initialize();
            }
        }
    }
    //////////////////////////////////////////////////////////////////////
}

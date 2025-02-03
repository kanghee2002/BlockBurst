using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class Board
{
    public Cell[,] cells { get; private set; }

    private BlockGameData gameData;

    private List<(BlockType, string)> onClearEffectBlocks;      // 모두 지워질 때 효과 발동하는 블록 ID 저장
    private int matchCount;
    private bool hasMatched;
    private bool isHalfFull;

    private List<Match> lastMatches = new List<Match>();

    private Dictionary<BlockType, int> placedBlockCounts;

    public void Initialize(BlockGameData blockGameData)
    {
        gameData = blockGameData;
        onClearEffectBlocks = new List<(BlockType, string)>();
        matchCount = 0;
        hasMatched = false;
        isHalfFull = false;

        cells = new Cell[gameData.boardRows, gameData.boardColumns];
        for (int i = 0; i < gameData.boardRows; i++)
        {
            for (int j = 0; j < gameData.boardColumns; j++)
            {
                cells[i, j] = new Cell();
                cells[i, j].Initialize();
            }
        }
        lastMatches = new List<Match>();
        placedBlockCounts = new Dictionary<BlockType, int>()
        {
            { BlockType.I, 0 },
            { BlockType.O, 0 },
            { BlockType.Z, 0 },
            { BlockType.S, 0 },
            { BlockType.J, 0 },
            { BlockType.L, 0 },
            { BlockType.T, 0 },
        };

        GameManager.instance.PlayBoardRelatedAnimation(matchCount);
    }

    public void BlockCells(HashSet<Vector2Int> blockedCells)
    {
        foreach (Vector2Int pos in blockedCells)
        {
            cells[pos.y, pos.x].BlockCell();
        }
    }

    public List<Match> GetLastMatches()
    {
        var tmp = lastMatches.ToList();
        lastMatches.Clear();
        return tmp;
    }

    // 블록 배치 처리
    public bool PlaceBlock(Block block, Vector2Int pos)
    {
        bool isPlaced = false;

        if (CanPlace(block, pos))
        {
            isPlaced = true;

            // 블록의 OnClearEffects 트리거를 위해 추가
            onClearEffectBlocks.Add((block.Type, block.Id));

            // 블록을 몇 번 배치했는지 추가 및 트리거
            if (Enums.IsDefaultBlockType(block.Type))
            {
                placedBlockCounts[block.Type]++;
                EffectManager.instance.TriggerEffects(TriggerType.ON_BLOCK_PLACE_WITH_COUNT, blockTypes: new BlockType[] { block.Type }, triggerValue: placedBlockCounts[block.Type]);
            }

            foreach (Vector2Int shapePos in block.Shape)
            {
                Vector2Int worldPos = pos + shapePos;
                cells[worldPos.y, worldPos.x].SetBlock(block.Type, block.Id);
            }

            // 블록 배치 이펙트 트리거
            EffectManager.instance.TriggerEffects(TriggerType.ON_BLOCK_PLACE, blockTypes: new BlockType[] { block.Type });

            // 보드 상태 이펙트 체크
            if (IsHalfFull())
            {
                if (!isHalfFull)
                {
                    EffectManager.instance.TriggerEffects(TriggerType.ON_BOARD_HALF_FULL);
                }
                isHalfFull = true;
            }
            else
            {
                if (isHalfFull)
                {
                    EffectManager.instance.TriggerEffects(TriggerType.ON_BOARD_NOT_HALF_FULL);
                }
                isHalfFull = false;
            }

            ProcessMatches(block, pos);
        }

        EffectManager.instance.EndTriggerEffect();

        return isPlaced;
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

            HashSet<(BlockType, string)> clearedBlocks = new HashSet<(BlockType, string)>();
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

        GameManager.instance.PlayBoardRelatedAnimation(matchCount);

        // 지운 블록들 비우기
        ClearCells(matches);

        // 모두 지워질 때 효과를 가진 블록 중 지워진 것 있는지 확인
        CheckOnClearEffectBlocks();

        // 매치된 결과 저장
        lastMatches.AddRange(matches);

        // 아이템 시각 효과 실행
        EffectManager.instance.EndTriggerEffectOnPlace(lastMatches);

        CalculateScore(matches);

        if (matches.Count > 0)
        {
            ResetMultiplier();
        }

        TriggerHalfFullEffect(matches);
    }

    // 매치 확인
    private List<Match> CheckMatches(Block block, Vector2Int pos)
    {
        // Shape에서 실제 차지하는 범위 계산
        int minX = block.Shape.Min(p => p.x);
        int maxX = block.Shape.Max(p => p.x);
        int minY = block.Shape.Min(p => p.y);
        int maxY = block.Shape.Max(p => p.y);

        // 실제 영향 받는 행/열 계산
        List<int> rows = Enumerable.Range(
            pos.y + minY,
            maxY - minY + 1
        ).ToList();

        List<int> columns = Enumerable.Range(
            pos.x + minX,
            maxX - minX + 1
        ).ToList();

        List<Match> rowMatches = CheckRowMatch(rows);
        List<Match> columnMatches = CheckColumnMatch(columns);

        var matches = new List<Match>();
        matches.AddRange(rowMatches);
        matches.AddRange(columnMatches);

        return matches;
    }

    // 행 매치 확인
    private List<Match> CheckRowMatch(List<int> rows, bool isForce = false)
    {
        List<Match> matches = new List<Match>();
        int column = cells.GetLength(1);

        foreach (int y in rows)
        {
            // 한 줄 완성 확인
            bool isMatched = true;
            if (!isForce)
            {
                for (int x = 0; x < column; x++)
                {
                    if (!cells[y, x].IsBlocked)
                    {
                        isMatched = false;
                        break;
                    }
                }
            }

            // 한 줄 완성 시
            if (isMatched)
            {
                Match match = new Match()
                {
                    index = y,
                    validIndices = new List<int>(),
                    matchType = MatchType.ROW,
                    blocks = new List<(BlockType, string)>()
                };
                for (int x = 0; x < column; x++)
                {
                    Cell currentCell = cells[y, x];

                    if (currentCell.IsBlocked && currentCell.BlockID != "")
                    {
                        match.validIndices.Add(x);
                        match.blocks.Add(((BlockType)currentCell.Type, currentCell.BlockID));
                    }
                }
                matches.Add(match);
            }
        }

        return matches;
    }

    // 열 매치 확인
    private List<Match> CheckColumnMatch(List<int> columns, bool isForce = false)
    {
        List<Match> matches = new List<Match>();
        int row = cells.GetLength(0);

        foreach (int x in columns)
        {
            // 한 줄 완성 확인
            bool isMatched = true;
            if (!isForce)
            {
                for (int y = 0; y < row; y++)
                {
                    if (!cells[y, x].IsBlocked)
                    {
                        isMatched = false;
                        break;
                    }
                }
            }

            // 한 줄 완성 시
            if (isMatched)
            {
                Match match = new Match()
                {
                    index = x,
                    validIndices = new List<int>(),
                    matchType = MatchType.COLUMN,
                    blocks = new List<(BlockType, string)>()
                };
                for (int y = 0; y < row; y++)
                {
                    Cell currentCell = cells[y, x];

                    if (currentCell.IsBlocked && currentCell.BlockID != "")
                    {
                        match.validIndices.Add(y);
                        match.blocks.Add(((BlockType)currentCell.Type, currentCell.BlockID));
                    }
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

    // 강제 매치 처리 (무작위로 줄 지우는 효과)
    public List<Match> ForceMatches(MatchType matchType, List<int> indices)
    {
        List<Match> matches = new List<Match>();

        if (matchType == MatchType.ROW)
        {
            matches.AddRange(CheckRowMatch(indices, true));
        }
        else if (matchType == MatchType.COLUMN)
        {
            matches.AddRange(CheckColumnMatch(indices, true));
        }

        // 지운 블록들 비우기
        ClearCells(matches);

        // 모두 지워질 때 효과를 가진 블록 중 지워진 것 있는지 확인
        CheckOnClearEffectBlocks();

        // 매치된 결과 저장
        lastMatches.AddRange(matches);

        CalculateScore(matches);

        TriggerHalfFullEffect(matches);
        
        return matches;
    }

    // 강제 사각형 매치 처리 (무작위로 사각형 지우는 효과)
    public void ForceSquareMatches(List<Vector2Int> positions)
    {

    }


    private void CalculateScore(List<Match> matches)
    {
        // 점수 계산
        int totalScore = 0;
        foreach (Match match in matches)
        {
            totalScore += ScoreCalculator.instance.Calculate(match, gameData);
        }

        gameData.currentScore += totalScore;

        Debug.Log("현재 배수: " + gameData.matchMultipliers[MatchType.ROW]);

        if (totalScore > 0)
        {
            Debug.Log("계산된 점수: " + totalScore);
        }
    }

    private void ResetMultiplier()
    {
        gameData.matchMultipliers = new(GameManager.instance.runData.baseMatchMultipliers);
    }

    private void TriggerHalfFullEffect(List<Match> matches)
    {
        // 한 줄이 지워졌다면 보드 상태 이펙트 체크
        if (matches.Count > 0)
        {
            if (!IsHalfFull())
            {
                EffectManager.instance.TriggerEffects(TriggerType.ON_BOARD_NOT_HALF_FULL);
            }
            else
            {
                if (!isHalfFull)
                {
                    EffectManager.instance.TriggerEffects(TriggerType.ON_BOARD_HALF_FULL);
                }
            }
            isHalfFull = IsHalfFull();
        }
    }

    // 블록 배치 가능 여부 확인
    private bool CanPlace(Block block, Vector2Int pos)
    {
        foreach (Vector2Int shapePos in block.Shape)
        {
            Vector2Int worldPos = pos + shapePos;
            if (IsOutOfBoard(worldPos) || IsBlocked(worldPos)) 
                return false;
        }
        return true;
    }

    // 회전 고려
    public bool CanPlaceSome(BlockData blockData)
    {
        Block blockCopy = new Block();
        blockCopy.Initialize(blockData, 0);
        int row = cells.GetLength(0);
        int column = cells.GetLength(1);

        for (int i = 0; i < 4; i++)
        {
            for (int y = 0; y < row; y++)
            {
                for (int x = 0; x < column; x++)
                {
                    if (CanPlace(blockCopy, new Vector2Int(x, y)))
                    {
                        return true;
                    }
                }
            }
            blockCopy.RotateShape();
        }
        return false;
    }

    private bool IsOutOfBoard(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= cells.GetLength(1) || pos.y < 0 || pos.y >= cells.GetLength(0))
            return true;
        else
            return false;
    }

    private bool IsBlocked(Vector2Int pos)
    {
        if (cells[pos.y, pos.x].IsBlocked) return true;
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
                if (cells[i, j].IsBlocked && cells[i, j].BlockID != "")
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

    private void ClearCells(List<Match> matches)
    {
        int row = cells.GetLength(0);
        int column = cells.GetLength(1);

        foreach (Match match in matches)
        {
            if (match.matchType == MatchType.ROW)
            {
                for (int x = 0; x < column; x++)
                {
                    cells[match.index, x].ClearBlock();
                }
            }
            else if (match.matchType == MatchType.COLUMN)
            {
                for (int y = 0; y < row; y++)
                {
                    cells[y, match.index].ClearBlock();
                }
            }
        }
    }

    private void CheckOnClearEffectBlocks()
    {
        int row = cells.GetLength(0);
        int column = cells.GetLength(1);

        HashSet<string> blocks = new HashSet<string>();

        for (int y = 0; y < row; y++)
        {
            for (int x = 0; x < column; x++)
            {
                blocks.Add(cells[y, x].BlockID);
            }
        }

        for (int i = onClearEffectBlocks.Count - 1; i >= 0; i--)
        {
            (BlockType blockType, string id) = onClearEffectBlocks[i];

            if (blocks.Contains(id))
            {
                continue;
            }
            EffectManager.instance.TriggerEffects(TriggerType.ON_BLOCK_CLEAR, blockTypes: new BlockType[] { blockType });
            onClearEffectBlocks.RemoveAt(i);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Cell[,] cells { get; private set; }

    // TEST
    private RunData runData;
    private BlockGameData gameData;
    private List<int> onClearEffectBlocks;      // 모두 지워질 때 효과 발동하는 블록 ID 저장

    public void Initialize(RunData runData, BlockGameData blockGameData)
    {
        // TEST
        this.runData = runData;
        gameData = blockGameData;
        onClearEffectBlocks = new List<int>();
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
        if (matches.Count == 0)
        {
            EffectManager.instance.TriggerEffects(TriggerType.ON_BLOCK_PLACE_WITHOUT_LINE_CLEAR);
        }
        else
        {
            EffectManager.instance.TriggerEffects(TriggerType.ON_BLOCK_PLACE_WITH_LINE_CLEAR, blockTypes: new BlockType[] { block.Type });
            EffectManager.instance.TriggerEffects(TriggerType.ON_LINE_CLEAR);
        }

        int rowClearCount = 0, columnClearCount = 0;
        foreach (Match match in matches)
        {
            if (match.matchType == MatchType.ROW)
            {
                rowClearCount++;
                EffectManager.instance.TriggerEffects(TriggerType.ON_ROW_LINE_CLEAR);
            }
            if (match.matchType == MatchType.COLUMN)
            {
                columnClearCount++;
                EffectManager.instance.TriggerEffects(TriggerType.ON_COLUMN_LINE_CLEAR);
            }
        }

        if (rowClearCount > 0 && columnClearCount > 0)
        {
            EffectManager.instance.TriggerEffects(TriggerType.ON_CROSS_LINE_CLEAR);
        }
        if (rowClearCount > 1)
        {
            EffectManager.instance.TriggerEffects(TriggerType.ON_MULTIPLE_LINE_CLEAR, triggerValue: rowClearCount);
        }
        if (columnClearCount > 1)
        {
            EffectManager.instance.TriggerEffects(TriggerType.ON_MULTIPLE_LINE_CLEAR, triggerValue: columnClearCount);
        }


        // 점수 계산
        int totalScore = 0;
        foreach (Match match in matches)
        {
            totalScore += ScoreCalculator.Instance.Calculate(match, gameData);
        }

        gameData.currentScore += totalScore;

        // 배수 초기화
        if (matches.Count > 0)
        {
            gameData.matchMultipliers = new(runData.baseMatchMultipliers);
        }

        Debug.Log("현재 점수: " + gameData.currentScore);
    }
    
    // 매치 확인
    private List<Match> CheckMatches(Block block, Vector2Int pos)
    {
        List<Match> matches = new List<Match>();

        List<int> rows = Enumerable.Range(pos.y, block.Shape.GetLength(1)).ToList();
        List<int> columns = Enumerable.Range(pos.x, block.Shape.GetLength(0)).ToList();

        List<Match> rowMatches = CheckRowMatch(rows);
        List<Match> columnMatches = CheckColumnMatch(columns);

        foreach (Match match in rowMatches)
        {
            matches.Add(match);
        }

        foreach (Match match in columnMatches)
        {
            matches.Add(match);
        }

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
                    blockTypes = new List<BlockType>()
                };
                // TEST: 8 -> Board Size로 변경 필요
                for (int x = 0; x < 8; x++)
                {
                    Cell currentCell = cells[x, y];
                    match.blockTypes.Add((BlockType)currentCell.Type);
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
                    blockTypes = new List<BlockType>()
                };
                // TEST: 8 -> Board Size로 변경 필요
                for (int y = 0; y < 8; y++)
                {
                    Cell currentCell = cells[x, y];
                    match.blockTypes.Add((BlockType)currentCell.Type);
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

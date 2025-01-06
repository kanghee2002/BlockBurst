using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Cell[,] cells { get; private set; }

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

                        Debug.Log(block.Id);
                    }
                }
            }
        }

        ProcessMatches();

        return isPlaced;
    }

    public void ProcessMatches()
    {
        // Temporary Implement
        for (int i = 0; i < 8; i++)
        {
            // Check Row
            bool rowLine = true;
            for (int j = 0; j < 8; j++)
            {
                if (!cells[i, j].IsBlocked)
                {
                    rowLine = false;
                    break;
                }
            }
            if (rowLine)
            {
                matcedLineCount++;
                for (int j = 0; j < 8; j++)
                {
                    cells[i, j].ClearBlock();
                }
            }

            // Check Column
            bool colLine = true;
            for (int j = 0; j < 8; j++)
            {
                if (!cells[j, i].IsBlocked)
                {
                    colLine = false;
                    break;
                }
            }
            if (colLine)
            {
                matcedLineCount++;
                for (int j = 0; j < 8; j++)
                {
                    cells[j, i].ClearBlock();
                }
            }
        }

        Debug.Log("현재 지운 줄 수: " + matcedLineCount);
    }

    private List<Match> CheckMatches(Block block, Vector2Int pos)
    {
        List<Match> matches = new List<Match>();

        List<Match> rowMatches = CheckRowMatch(pos.y, block.Shape.GetLength(1));
        List<Match> colMatches = CheckColumnMatch(pos.x, block.Shape.GetLength(0));

        return null;
    }

    private List<Match> CheckRowMatch(int start, int length)
    {
        List<Match> matches = new List<Match>();

        for (int y = start; y < start + length; y++)
        {
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

            if (isMatched)
            {
                Match match = new Match()
                {
                    matchType = MatchType.ROW,
                    blockTypes = new List<BlockType>()
                };

                for (int x = 0; x < 8; x++)
                {
                    Cell currentCell = cells[x, y];
                    match.blockTypes.Add((BlockType)currentCell.Type);
                }
            }
        }

        return null;
    }

    private List<Match> CheckColumnMatch(int start, int length)
    {
        return null;
    }

    private Match CheckSquareMatch(Block block, Vector2Int pos)
    {
        return null;
    }

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

    private int matcedLineCount = 0;

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
            }
        }
    }
    //////////////////////////////////////////////////////////////////////
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class BoardUI : MonoBehaviour
{
    private RectTransform rectTransform;

    private const float block_size = 96f;
    private int width = 8;
    private int height = 8;
    [SerializeField] private GameObject prefabBoardCellUI;
    public BoardCellUI[,] boardCellsUI;

    private const float insidePositionY = -96;
    private const float outsidePositionOffsetY = -1080;
    private const float duration = 0.2f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(int rows, int columns)
    {
        gameObject.SetActive(true);
        height = rows;
        width = columns;

        // 기존 boardCell 비우기
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        boardCellsUI = new BoardCellUI[height, width];

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                GameObject newObject = Instantiate(prefabBoardCellUI);
                newObject.transform.SetParent(transform, false);
                newObject.GetComponent<RectTransform>().anchoredPosition
                    = new Vector2(col - (width - 1f) / 2, -(row - (height - 1f) / 2)) * block_size;
                
                var boardCellUI = newObject.GetComponent<BoardCellUI>();
                Color color = (row + col) % 2 == 0 ? new Color(0.2f, 0.2f, 0.2f, 1f) : new Color(.25f, .25f, .25f, 1f);
                boardCellUI.Initialize(new Vector2Int(col, row), color);
                boardCellsUI[row, col] = boardCellUI;
            }
        }
    }

    public void OpenBoardUI()
    {
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
    }

    public void CloseBoardUI()
    {
        // boardCell 초기화
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                boardCellsUI[row, col].ClearCell();
            }
        }
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionOffsetY, duration);
    }

    public void BlockCells(HashSet<Vector2Int> cells)
    {
        foreach (Vector2Int cell in cells)
        {
            boardCellsUI[cell.y, cell.x].BlockCell();
        }
    }
        
    public void OnBlockPlaced(GameObject blockObj, Block block, Vector2Int pos)
    {
        DecomposeBlockToBoard(blockObj, block, pos);
        Destroy(blockObj);
    }

    private void DecomposeBlockToBoard(GameObject blockObj, Block block, Vector2Int pos)
    {
        Transform visualObj = blockObj.transform.GetChild(1);
        foreach (Vector2Int shapePos in block.Shape)
        {
            Vector2Int cellPos = pos + shapePos;
            boardCellsUI[cellPos.y, cellPos.x].StopClearAnimation();
            boardCellsUI[cellPos.y, cellPos.x].SetBlockInfo(block.Id);
            boardCellsUI[cellPos.y, cellPos.x].CopyVisualFrom(visualObj);
        }
    }

    public void ProcessMatchAnimation(List<Match> matches, Dictionary<Match, List<int>> scores, float delay)
    {
        float totalTime = 0f, delayedTime = 0f;

        // 총 걸리는 시간 계산
        foreach (Match match in matches)
        {
            if (match.matchType == MatchType.ROW)
            {
                totalTime += delay * width;
            }
            else if (match.matchType == MatchType.COLUMN)
            {
                totalTime += delay * height;
            }
        }

        // 줄 지우는 효과
        foreach (Match match in matches)
        {
            if (match.matchType == MatchType.ROW)
            {
                for (int x = 0; x < width; x++)
                {
                    boardCellsUI[match.index, x].PlayClearAnimation(delayedTime, totalTime - delayedTime + 0.3f);

                    int score = scores[match][x];
                    boardCellsUI[match.index, x].PlayScoreAnimation(score, delayedTime);
                    delayedTime += delay;
                }
            }
            else if (match.matchType == MatchType.COLUMN)
            {
                for (int y = 0; y < height; y++)
                {
                    boardCellsUI[y, match.index].PlayClearAnimation(delayedTime, totalTime - delayedTime + 0.3f);

                    int score = scores[match][y];
                    boardCellsUI[y, match.index].PlayScoreAnimation(score, delayedTime);
                    delayedTime += delay;
                }
            }
        }

        // matches의 blocks의 개수를 모두 더함
        int totalBlocks = 0;
        foreach (Match match in matches)
        {
            totalBlocks += match.blocks.Count;
        }
        AudioManager.instance.SFXMatch(totalBlocks);
    }
}
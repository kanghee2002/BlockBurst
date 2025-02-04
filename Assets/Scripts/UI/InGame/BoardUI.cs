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

    [SerializeField] private RectTransform itemSetRect; // Up
    [SerializeField] private RectTransform deskMatRect; // Left
    [SerializeField] private RectTransform handUIRect; // Right

    private const float insidePositionY = -96;
    private const float outsidePositionOffsetY = -1080;
    private const float duration = 0.2f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void AutoSizing(int height, int width)
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        Camera cam = canvas.worldCamera ?? Camera.main;

        // UI 요소들의 화면상 위치와 크기를 정확하게 계산
        Vector2 itemSetScreen = cam.WorldToScreenPoint(itemSetRect.position);
        Vector2 deskMatScreen = cam.WorldToScreenPoint(deskMatRect.position);
        Vector2 handScreen = cam.WorldToScreenPoint(handUIRect.transform.GetChild(0).GetComponent<RectTransform>().position);

        // 각 UI의 실제 경계 위치 계산 (화면 좌표계)
        float itemSetBottom = itemSetScreen.y - ((itemSetRect.rect.height / 2) * canvas.scaleFactor);
        float deskMatRight = deskMatScreen.x + ((deskMatRect.rect.width / 2) * canvas.scaleFactor);
        float handLeft = handScreen.x - ((handUIRect.rect.width / 2) * canvas.scaleFactor);

        // UI 단위로 변환
        float availableWidth = (handLeft - deskMatRight) / canvas.scaleFactor;
        float availableHeight = itemSetBottom / canvas.scaleFactor;

        // 실제 보드의 필요 크기
        float boardWidth = block_size * width;
        float boardHeight = block_size * height;
        
        // 여유 공간을 약간 둬서 경계에 딱 붙지 않도록 함
        float scale = Mathf.Min(availableWidth / boardWidth, availableHeight / boardHeight) * 0.9f;

        // 최소 크기 제한
        scale = Mathf.Max(scale, 0.1f);
        
        // 크기 적용
        rectTransform.localScale = Vector3.one * scale;
        handUIRect.transform.GetChild(1).GetComponent<RectTransform>().localScale = Vector3.one * scale;

        Debug.Log($"Final calculation - Available space: {availableWidth}x{availableHeight}, Board size: {boardWidth}x{boardHeight}, Scale: {scale}");
    }

    public void Initialize(int rows, int columns)
    {
        gameObject.SetActive(true);
        height = rows;
        width = columns;

        AutoSizing(height, width);

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
        foreach (Vector2Int shapePos in block.Shape)
        {
            Vector2Int cellPos = pos + shapePos;
            boardCellsUI[cellPos.y, cellPos.x].StopClearAnimation();
            boardCellsUI[cellPos.y, cellPos.x].SetBlockInfo(block.Id, block.Score);
            Image image = boardCellsUI[cellPos.y, cellPos.x].GetComponent<Image>();
            CellEffectManager.instance.ApplyEffect(ref image, block.Type);
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

        int blockCount = 0;

        // 줄 지우는 효과
        foreach (Match match in matches)
        {
            if (match.matchType == MatchType.ROW)
            {
                for (int x = 0; x < width; x++)
                {
                    boardCellsUI[match.index, x].PlayClearAnimation(delayedTime, totalTime - delayedTime + 0.3f, match.isForceMatch);

                    int score = scores[match][x];
                    boardCellsUI[match.index, x].PlayScoreAnimation(score, delayedTime);
                    delayedTime += delay;
                }
                blockCount += width;
            }
            else if (match.matchType == MatchType.COLUMN)
            {
                for (int y = 0; y < height; y++)
                {
                    boardCellsUI[y, match.index].PlayClearAnimation(delayedTime, totalTime - delayedTime + 0.3f, match.isForceMatch);

                    int score = scores[match][y];
                    boardCellsUI[y, match.index].PlayScoreAnimation(score, delayedTime);
                    delayedTime += delay;
                }
                blockCount += height;
            }
        }

        AudioManager.instance.SFXMatch(blockCount);
    }
}
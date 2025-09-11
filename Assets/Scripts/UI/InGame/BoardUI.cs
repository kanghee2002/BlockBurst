using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
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

    [Header("StageClearAnimation")]
    [SerializeField] private RectTransform stageClearText;

    private const float windowsInsidePositionY = -96;
    private const float mobileInsidePositionY = -105;
    private const float outsidePositionOffsetY = -1080;
    private const float duration = 0.2f;

    const float MIN_SIZE = 0.5f;
    const float MAX_SIZE = 1.0f;

    private ColorSet currentColorSet;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        currentColorSet = new ColorSet();
    }
    
    private void AutoSizingWindows(int height, int width)
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        Camera cam = canvas.worldCamera ?? Camera.main;

        // World 좌표를 Viewport 좌표로 변환 (0~1 범위)
        Vector3 itemSetViewport = cam.WorldToViewportPoint(itemSetRect.position);
        Vector3 deskMatViewport = cam.WorldToViewportPoint(deskMatRect.position);
        Vector3 handViewport = cam.WorldToViewportPoint(handUIRect.transform.GetChild(0).GetComponent<RectTransform>().position);

        // Viewport 좌표를 현재 화면 해상도에 맞게 변환
        float screenHeight = Screen.height * cam.rect.height;
        float screenWidth = Screen.width * cam.rect.width;
        
        // UI 요소들의 실제 화면상 위치 계산
        Vector2 itemSetScreen = new Vector2(
            itemSetViewport.x * screenWidth,
            itemSetViewport.y * screenHeight
        );
        Vector2 deskMatScreen = new Vector2(
            deskMatViewport.x * screenWidth,
            deskMatViewport.y * screenHeight
        );
        Vector2 handScreen = new Vector2(
            handViewport.x * screenWidth,
            handViewport.y * screenHeight
        );

        // 각 UI의 실제 경계 위치 계산 (조정된 화면 좌표계)
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

        // 최대, 최소 크기 제한
        scale = Mathf.Max(scale, MIN_SIZE);
        scale = Mathf.Min(scale, MAX_SIZE);
        
        // 크기 적용
        rectTransform.localScale = Vector3.one * scale;
        handUIRect.transform.GetChild(1).GetComponent<RectTransform>().localScale = Vector3.one * scale;

        Debug.Log($"Final calculation - Available space: {availableWidth}x{availableHeight}, Board size: {boardWidth}x{boardHeight}, Scale: {scale}, Viewport: {cam.rect}");
    }

    private void AutoSizingMobile(int height, int width)
    {
        float scale = 0.44f;

        if (height > 9)
        {
            scale = (0.5f - (0.05f * (height - 9)));
        }
        rectTransform.localScale = Vector3.one * scale;
    }

    public void Initialize(int rows, int columns)
    {
        gameObject.SetActive(true);
        height = rows;
        width = columns;

        if (GameManager.instance.applicationType == ApplicationType.Mobile)
            AutoSizingMobile(height, width);
        else
            AutoSizingWindows(height, width);

        // 기존 boardCell 비우기
        foreach (Transform child in transform)
            {
                if (child.GetComponent<BoardCellUI>() != null)
                {
                    Destroy(child.gameObject);
                }
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
                Color color = (row + col) % 2 == 0 ? currentColorSet.backGroundLighterColor : currentColorSet.backGroundLighterColor;
                boardCellUI.Initialize(new Vector2Int(col, row), color);
                boardCellsUI[row, col] = boardCellUI;
            }
        }
        stageClearText.SetAsLastSibling();
        stageClearText.gameObject.SetActive(false);
    }

    public void OpenBoardUI()
    {
        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            UIUtils.OpenUI(rectTransform, "Y", windowsInsidePositionY, duration);
        }
        else
        {
            UIUtils.OpenUI(rectTransform, "Y", mobileInsidePositionY, duration);
        }
    }

    public void GetColorSet(ColorSet colorSet)
    {
        currentColorSet = colorSet;
    }

    public void GetTextColorSet(Color uiTextColor)
    {
        stageClearText.GetComponent<TextMeshProUGUI>().color = uiTextColor;
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
        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            UIUtils.CloseUI(rectTransform, "Y", windowsInsidePositionY, outsidePositionOffsetY, duration);
        }
        else
        {
            UIUtils.CloseUI(rectTransform, "Y", mobileInsidePositionY, outsidePositionOffsetY, duration);
        }
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

    public void ProcessStageClearAnimation(float delay)
    {
        // 총 걸리는 시간 계산
        float totalTime = height * width * delay;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                boardCellsUI[y, x].PlayStageClearAnimation();
            }
        }

        stageClearText.localScale = Vector3.one * 0.1f;

        Sequence sequence = DOTween.Sequence();

        sequence.SetDelay(0.7f);
        sequence.AppendCallback(() => stageClearText.gameObject.SetActive(true));
        sequence.Append(stageClearText.DOScale(Vector3.one * 1f, duration: 0.7f).SetEase(Ease.OutExpo));
    }
}
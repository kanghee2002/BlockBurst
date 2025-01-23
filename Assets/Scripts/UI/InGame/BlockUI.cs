using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

public class BlockUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject prefabBlockCellUI;
    private GameObject[,] blockCellsUI;
    private int blockCellsUIColumnCount;
    private int blockCellsUIRowCount;
    private const float block_size = 96f;

    private BoardUI boardUI;
    private BoardCellUI lastHighlightedCell;
    private List<BoardCellUI> currentShadowCells = new List<BoardCellUI>();

    [SerializeField] private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 dragOffset;
    private Vector3 originalPosition;
    private int idx;

    private int minX;
    private int minY;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        boardUI = GameObject.Find("BoardUI").GetComponent<BoardUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(Block block, int index)
    {
        originalPosition = transform.localPosition;
        idx = index;

        // Calculate dimensions based on shape data
        minX = block.Shape.Min(pos => pos.x);
        minY = block.Shape.Min(pos => pos.y);

        int maxX = block.Shape.Max(pos => pos.x);
        int maxY = block.Shape.Max(pos => pos.y);

        blockCellsUIRowCount = maxY - minY + 1;
        blockCellsUIColumnCount = maxX - minX + 1;
        
        blockCellsUI = new GameObject[blockCellsUIRowCount, blockCellsUIColumnCount];

        Sprite blockSprite = Resources.Load<Sprite>("Sprites/Block/" + block.Type.ToString());
        // Place blocks according to shape data
        foreach (Vector2Int pos in block.Shape)
        {
            int normalizedX = pos.x - minX;
            int normalizedY = pos.y - minY;
            
            GameObject blockCell = Instantiate(prefabBlockCellUI);
            blockCell.GetComponent<Image>().sprite = blockSprite;
            blockCellsUI[normalizedY, normalizedX] = blockCell;
        }

        // Position blocks (minX, minY) is left top
        for (int row = 0; row < blockCellsUIRowCount; row++)
        {
            for (int col = 0; col < blockCellsUIColumnCount; col++)
            {
                if (blockCellsUI[row, col] != null)
                {
                    blockCellsUI[row, col].transform.SetParent(transform, false);
                    blockCellsUI[row, col].GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(col, -row) * block_size;
                }
            }
        }

        makeSmall(true);
    }

    void makeSmall(bool toSmall)
    {
        Vector3 targetScale = toSmall ? new Vector3(0.5f, 0.5f, 1) : new Vector3(1, 1, 1);
        transform.DOScale(targetScale, 0.2f).SetEase(Ease.OutQuad);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        makeSmall(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );
        dragOffset = localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint))
        {
            rectTransform.localPosition = localPoint - dragOffset;
        }

        //ClearShadowCells();
        
        BoardCellUI closestValidCell = FindClosestValidCell();
        if (closestValidCell != null)
        {
            Vector2Int boardPosition = closestValidCell.GetCellIndex();
            // 보드 셀의 위치를 기준으로 그림자를 표시합니다
            ShowShadowAtPosition(boardPosition);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ClearShadowCells();
        
        BoardCellUI closestValidCell = FindClosestValidCell();
        if (closestValidCell != null)
        {
            Vector2Int boardPosition = closestValidCell.GetCellIndex() - new Vector2Int(minX, minY);
            if (GameUIManager.instance.TryPlaceBlock(idx, boardPosition, gameObject))
            {
                Vector2 targetPosition = closestValidCell.GetComponent<RectTransform>().anchoredPosition
                    + boardUI.GetComponent<RectTransform>().anchoredPosition;
                rectTransform.anchoredPosition = targetPosition;
            }
            else
            {
                ReturnToHand();
            }
        }
        else
        {
            ReturnToHand();
        }
    }

    private void ClearShadowCells()
    {
        foreach (var cell in currentShadowCells)
        {
            cell.HideShadow();
        }
        currentShadowCells.Clear();
    }

    private void ShowShadowAtPosition(Vector2Int boardPosition)
    {
        bool isValid = true;
        List<BoardCellUI> prevShadowCells = new List<BoardCellUI>(currentShadowCells);
        currentShadowCells.Clear();
        
        // 전체 블록 모양이 보드 안에 들어가는지 확인
        for (int blockRow = 0; blockRow < blockCellsUI.GetLength(0) && isValid; blockRow++)
        {
            for (int blockCol = 0; blockCol < blockCellsUI.GetLength(1); blockCol++)
            {
                if (blockCellsUI[blockRow, blockCol] != null)
                {
                    int targetRow = boardPosition.y + blockRow;
                    int targetCol = boardPosition.x + blockCol;
                    
                    if (targetRow < 0 || targetRow >= boardUI.boardCellsUI.GetLength(0) ||
                        targetCol < 0 || targetCol >= boardUI.boardCellsUI.GetLength(1))
                    {
                        isValid = false;
                        break;
                    }
                }
            }
        }

        if (isValid)
        {
            for (int blockRow = 0; blockRow < blockCellsUI.GetLength(0); blockRow++)
            {
                for (int blockCol = 0; blockCol < blockCellsUI.GetLength(1); blockCol++)
                {
                    if (blockCellsUI[blockRow, blockCol] != null)
                    {
                        int targetRow = boardPosition.y + blockRow;
                        int targetCol = boardPosition.x + blockCol;
                        
                        var cell = boardUI.boardCellsUI[targetRow, targetCol];
                        cell.ShowShadow();
                        currentShadowCells.Add(cell);
                    }
                }
            }
        }

        foreach (var cell in prevShadowCells)
        {
            if (!currentShadowCells.Contains(cell))
            {
                cell.HideShadow();
            }
        }
    }

    private BoardCellUI FindClosestValidCell()
    {
        BoardCellUI[,] boardCellsUI = boardUI.boardCellsUI;
        BoardCellUI closestBoardCellUI = null;
        float minDistance = float.MaxValue;

        // 블록의 RectTransform 위치 사용
        Vector2 blockPosition = transform.position;

        float maxAllowedDistance = block_size * 2f; 

        for (int row = 0; row < boardCellsUI.GetLength(0); row++)
        {
            for (int col = 0; col < boardCellsUI.GetLength(1); col++)
            {
                // RectTransform 기반 거리 계산
                Vector2 cellPosition = boardCellsUI[row, col].transform.position;

                float distance = Vector2.Distance(blockPosition, cellPosition);

                if (distance > maxAllowedDistance) continue;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestBoardCellUI = boardCellsUI[row, col];
                }
            }
        }

        return closestBoardCellUI;
    }
    
    private void ReturnToHand()
    {
        makeSmall(true);
        rectTransform.DOLocalMove(originalPosition, 0.3f).SetEase(Ease.OutBack);
    }

    private void OnDestroy()
    {
        ClearShadowCells();
    }
}
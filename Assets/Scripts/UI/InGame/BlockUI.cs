using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

public class BlockUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject prefabBlockCellUI;
    private GameObject[,] blockCellsUI;
    private int minX;
    private int minY;
    private int blockCellsUIColumnCount;
    private int blockCellsUIRowCount;
    private const float block_size = 96f;
    private const float MAX_DISTANCE_TO_BOARD = 200f; // 보드로부터 최대 허용 거리
    private const float ROTATION_THRESHOLD = 30f; // 회전을 위한 최소 드래그 거리

    private BoardUI boardUI;
    private BoardCellUI lastHighlightedCell;
    private List<BoardCellUI> currentShadowCells = new List<BoardCellUI>();

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 dragOffset;
    private Vector3 originalPosition;
    private int idx;
    private Image raycastImage; // 추가: Raycast를 위한 이미지 컴포넌트
    private Vector2 centerOffset; // 중앙 정렬을 위한 오프셋
    private bool isDragging = false;
    float boardCellSize;

    Block myBlock;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        boardUI = GameObject.Find("BoardUI").GetComponent<BoardUI>();
        rectTransform = GetComponent<RectTransform>();
        raycastImage = GetComponent<Image>();
        if (raycastImage == null)
        {
            raycastImage = gameObject.AddComponent<Image>();
        }
        raycastImage.color = new Color(0, 0, 0, 0); // 완전 투명
        raycastImage.raycastTarget = true;
    }

    void Rotate()
    {
        GameUIManager.instance.OnRotateBlock(idx);
    }

    public void Initialize(Block block, int index)
    {
        myBlock = block;
        originalPosition = transform.localPosition;
        idx = index;

        // Destroy existing block cells
        if (blockCellsUI != null)
        {
            foreach (var cell in blockCellsUI)
            {
                if (cell != null)
                {
                    Destroy(cell);
                }
            }
        }

        // Calculate dimensions based on shape data
        minX = block.Shape.Min(pos => pos.x);
        minY = block.Shape.Min(pos => pos.y);

        int maxX = block.Shape.Max(pos => pos.x);
        int maxY = block.Shape.Max(pos => pos.y);

        blockCellsUIRowCount = maxY - minY + 1;
        blockCellsUIColumnCount = maxX - minX + 1;
        
        blockCellsUI = new GameObject[blockCellsUIRowCount, blockCellsUIColumnCount];

        // Place blocks according to shape data
        foreach (Vector2Int pos in block.Shape)
        {
            int normalizedX = pos.x - minX;
            int normalizedY = pos.y - minY;
            
            GameObject blockCell = Instantiate(prefabBlockCellUI);
            blockCell.GetComponent<Image>().sprite = CellEffectManager.instance.ApplyEffect(block.Type);
            blockCellsUI[normalizedY, normalizedX] = blockCell;
        }

        // Position blocks - 중앙 정렬을 위해 오프셋 계산
        centerOffset = new Vector2(
            (blockCellsUIColumnCount - 1) * block_size * 0.5f,
            (blockCellsUIRowCount - 1) * block_size * 0.5f
        );

        for (int row = 0; row < blockCellsUIRowCount; row++)
        {
            for (int col = 0; col < blockCellsUIColumnCount; col++)
            {
                if (blockCellsUI[row, col] != null)
                {
                    blockCellsUI[row, col].transform.SetParent(transform, false);
                    blockCellsUI[row, col].GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(col * block_size - centerOffset.x, -(row * block_size - centerOffset.y));
                }
            }
        }

        // 추가: Raycast 이미지 크기 설정
        float width = blockCellsUIColumnCount * block_size * 1.5f;
        float height = blockCellsUIRowCount * block_size * 1.5f;
        rectTransform.sizeDelta = new Vector2(width, height);

        makeSmall(true);
    }

    void makeSmall(bool toSmall)
    {
        transform.DOKill();
        Vector3 targetScale = toSmall ? new Vector3(0.5f, 0.5f, 1) : new Vector3(1, 1, 1);
        transform.DOScale(targetScale, 0.2f).SetEase(Ease.OutQuad);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        makeSmall(false);
        boardCellSize = boardUI.boardCellsUI[0, 1].GetComponent<RectTransform>().position.x - boardUI.boardCellsUI[0, 0].GetComponent<RectTransform>().position.x;

        AudioManager.instance.SFXSelectBlock();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 드래그 거리가 너무 짧으면 회전
        if (!isDragging || Vector2.Distance(eventData.pressPosition, eventData.position) < ROTATION_THRESHOLD)
        {
            ReturnToHand(true);
            Rotate();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameManager.instance.OnBeginDragBlock(myBlock);

        isDragging = true;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );
        
        // 드래그 오프셋을 중앙 기준으로 계산
        dragOffset = localPoint - (Vector2)transform.localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint))
        {
            transform.localPosition = localPoint - dragOffset;
        }
        
        BoardCellUI closestValidCell = FindClosestValidCell();
        if (closestValidCell != null)
        {
            ShowShadowAtPosition(closestValidCell.GetCellIndex());
        }
        else
        {
            ClearShadowCells();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameManager.instance.OnEndDragBlock(myBlock);

        isDragging = false;
        ClearShadowCells();
        if (Vector2.Distance(eventData.pressPosition, eventData.position) < ROTATION_THRESHOLD)
        {
            return;
        }
        BoardCellUI closestValidCell = FindClosestValidCell();
        if (closestValidCell != null)
        {
            Vector2Int boardPosition = closestValidCell.GetCellIndex() - new Vector2Int(minX, minY);
            if (GameUIManager.instance.TryPlaceBlock(idx, boardPosition, gameObject))
            {
                // 중앙 오프셋을 고려한 최종 위치 계산
                Vector2 targetPosition = closestValidCell.GetComponent<RectTransform>().anchoredPosition
                    + boardUI.GetComponent<RectTransform>().anchoredPosition;
                rectTransform.anchoredPosition = targetPosition;

                AudioManager.instance.SFXPlaceBlock();
                CellEffectManager.instance.PlaceEffect(myBlock);
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

        // 보드와의 거리 체크
        Vector2 boardCenter = boardUI.GetComponent<RectTransform>().position;
        float distanceToBoard = Vector2.Distance(transform.position, boardCenter);
        
        if (distanceToBoard > MAX_DISTANCE_TO_BOARD)
        {
            ClearShadowCells();
            return;
        }
        
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
        int refX = 0, refY = 0;
        bool flag = false;
        Vector2 boardCellRef;
        Vector2 blockCellRef;
        BoardCellUI closestBoardCellUI = null;
        
        for (refX = 0; refX < blockCellsUIRowCount; refX++)
        {
            for (refY = 0; refY < blockCellsUIColumnCount; refY++)
            {
                if (blockCellsUI[refX, refY] != null)
                {
                    flag = true;
                    break;
                }
            }
            if (flag) break;
        }

        blockCellRef = blockCellsUI[refX, refY].GetComponent<RectTransform>().position;
        boardCellRef = boardUI.boardCellsUI[refX, refY].GetComponent<RectTransform>().position;

        Vector2 distance = blockCellRef - boardCellRef;

        Vector2Int pos = new Vector2Int(
            Mathf.RoundToInt(distance.x / boardCellSize),
            Mathf.RoundToInt(-distance.y / boardCellSize)
        );

        if (pos.x < 0 || pos.y < 0 || pos.x >= boardUI.boardCellsUI.GetLength(0) || pos.y >= boardUI.boardCellsUI.GetLength(1))
        {
            closestBoardCellUI = null;
        }
        else
        {
            closestBoardCellUI = boardUI.boardCellsUI[pos.y, pos.x];
        }
        return closestBoardCellUI;
    }
    
    private void ReturnToHand(bool isInstant = false)
    {
        makeSmall(true);
        if (isInstant)
        {
            transform.localPosition = originalPosition;
        }
        else
        {
            rectTransform.DOLocalMove(originalPosition, 0.3f).SetEase(Ease.OutBack);
            AudioManager.instance.SFXPlaceFail();
        }
    }

    private void OnDestroy()
    {
        ClearShadowCells();
    }
}
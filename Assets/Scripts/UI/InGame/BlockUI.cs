using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class BlockUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject prefabBlockCellUI;
    private List<List<GameObject>> blockCellsUI = new List<List<GameObject>>();
    private int blockCellsUIRowCount = 0;
    private int blockCellsUIColumnCount = 0;
    private const float block_size = 96f;
    [SerializeField] private GameObject shadowContainer;
    [SerializeField] private GameObject prefabBlockCellShadowUI;

    private BoardUI boardUI;

    [SerializeField] private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 offset;

    private Vector3 originalPosition;
    private int idx;

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

        // Clear existing blocks
        for (int row = 0; row < blockCellsUIRowCount; row++)
        {
            for (int column = 0; column < blockCellsUIColumnCount; column++)
            {
                if (blockCellsUI[row][column] != null)
                {
                    Destroy(blockCellsUI[row][column]);
                }
            }
        }
        blockCellsUI.Clear();

        // Calculate dimensions based on shape data
        int minX = block.Shape.Min(pos => pos.x);
        int maxX = block.Shape.Max(pos => pos.x);
        int minY = block.Shape.Min(pos => pos.y);
        int maxY = block.Shape.Max(pos => pos.y);

        blockCellsUIColumnCount = maxX - minX + 1;
        blockCellsUIRowCount = maxY - minY + 1;

        // Initialize empty grid
        for (int row = 0; row < blockCellsUIRowCount; row++)
        {
            List<GameObject> listRow = new List<GameObject>();
            for (int column = 0; column < blockCellsUIColumnCount; column++)
            {
                listRow.Add(null);
            }
            blockCellsUI.Add(listRow);
        }

        // Place blocks according to shape data
        foreach (Vector2Int pos in block.Shape)
        {
            int normalizedX = pos.x - minX;
            int normalizedY = pos.y - minY;
            
            GameObject blockCell = Instantiate(prefabBlockCellUI);
            blockCellsUI[normalizedY][normalizedX] = blockCell;
        }

        // Position blocks
        for (int row = 0; row < blockCellsUIRowCount; row++)
        {
            for (int column = 0; column < blockCellsUIColumnCount; column++)
            {
                if (blockCellsUI[row][column] != null)
                {
                    blockCellsUI[row][column].transform.SetParent(this.transform, false);
                    blockCellsUI[row][column].GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(column - (blockCellsUIColumnCount - 1) / 2f, -(row - (blockCellsUIRowCount - 1) / 2f)) * block_size;
                
                    GameObject shadow = Instantiate(prefabBlockCellShadowUI);
                    shadow.transform.SetParent(shadowContainer.transform, false);
                    shadow.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(column - (blockCellsUIColumnCount - 1) / 2f, -(row - (blockCellsUIRowCount - 1) / 2f)) * block_size;
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        shadowContainer.SetActive(true);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out offset
        );
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
            rectTransform.localPosition = localPoint - offset;
        }

        BoardCellUI closestValidCell = FindClosestValidCell();
        if (closestValidCell != null)
        {
            Vector2 zeroOffset = new Vector2((blockCellsUIColumnCount - 1) / 2f, (blockCellsUIRowCount - 1) / 2f * (-1)) * block_size;
            shadowContainer.GetComponent<RectTransform>().anchoredPosition =
                closestValidCell.GetComponent<RectTransform>().anchoredPosition
                + boardUI.GetComponent<RectTransform>().anchoredPosition
                + zeroOffset
                - rectTransform.anchoredPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        shadowContainer.SetActive(false);
        
        BoardCellUI closestValidCell = FindClosestValidCell();
        if (closestValidCell != null)
        {
            Vector2Int boardPosition = closestValidCell.GetCellIndex();
            Debug.Log(boardPosition);
            
            if (GameUIManager.instance.TryPlaceBlock(idx, boardPosition))
            {
                Vector2 zeroOffset = new Vector2((blockCellsUIColumnCount - 1) / 2f, (blockCellsUIRowCount - 1) / 2f * (-1)) * block_size;
                rectTransform.anchoredPosition =
                    closestValidCell.GetComponent<RectTransform>().anchoredPosition
                    + boardUI.GetComponent<RectTransform>().anchoredPosition
                    + zeroOffset;
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

    private BoardCellUI FindClosestValidCell()
    {
        List<List<BoardCellUI>> boardCellsUI = boardUI.boardCellsUI;
        BoardCellUI closestBoardCellUI = null;
        float minDistance = float.MaxValue;

        Vector2 zeroOffset = new Vector2((blockCellsUIColumnCount - 1) / 2f, (blockCellsUIRowCount - 1) / 2f * (-1)) * block_size;
        Vector2 blockCenter = rectTransform.anchoredPosition - zeroOffset;

        for (int row = 0; row < boardCellsUI.Count; row++)
        {
            for (int col = 0; col < boardCellsUI[0].Count; col++)
            {
                Vector2 cellPos = boardCellsUI[row][col].GetComponent<RectTransform>().anchoredPosition
                    + boardUI.GetComponent<RectTransform>().anchoredPosition;

                bool isValid = true;
                float totalDistance = 0;

                // 블록의 각 셀에 대해 보드 위치 확인
                for (int blockRow = 0; blockRow < blockCellsUI.Count; blockRow++)
                {
                    for (int blockCol = 0; blockCol < blockCellsUI[0].Count; blockCol++)
                    {
                        if (blockCellsUI[blockRow][blockCol] != null)
                        {
                            int targetRow = row + blockRow - (int)((blockCellsUIRowCount - 1) / 2f);
                            int targetCol = col + blockCol - (int)((blockCellsUIColumnCount - 1) / 2f);

                            // 보드 범위 체크
                            if (targetRow < 0 || targetRow >= boardCellsUI.Count ||
                                targetCol < 0 || targetCol >= boardCellsUI[0].Count)
                            {
                                isValid = false;
                                break;
                            }

                            Vector2 blockCellPos = blockCellsUI[blockRow][blockCol].GetComponent<RectTransform>().anchoredPosition;
                            Vector2 targetPos = cellPos + blockCellPos;
                            totalDistance += Vector2.Distance(blockCenter + blockCellPos, targetPos);
                        }
                    }
                    if (!isValid) break;
                }

                if (isValid && totalDistance < minDistance)
                {
                    minDistance = totalDistance;
                    closestBoardCellUI = boardCellsUI[row][col];
                }
            }
        }

        return closestBoardCellUI;
    }
    
    private void ReturnToHand()
    {
        transform.localPosition = originalPosition;
    }
}
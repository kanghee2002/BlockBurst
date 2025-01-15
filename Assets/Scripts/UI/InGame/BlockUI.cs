using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class BlockUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private BlockType blockType;
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

    BlockData blockData;

    private Vector3 originalPosition;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        boardUI = GameObject.Find("BoardUI").GetComponent<BoardUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(BlockData data)
    {
        blockData = data;
        blockType = blockData.type;
        originalPosition = transform.localPosition;  // 초기 위치 저장

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
        int minX = blockData.shape.Min(pos => pos.x);
        int maxX = blockData.shape.Max(pos => pos.x);
        int minY = blockData.shape.Min(pos => pos.y);
        int maxY = blockData.shape.Max(pos => pos.y);

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
        foreach (Vector2Int pos in blockData.shape)
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
                        = new Vector2(column - (blockCellsUIColumnCount - 1) / 2f, row - (blockCellsUIRowCount - 1) / 2f) * block_size;
                
                    GameObject shadow = Instantiate(prefabBlockCellShadowUI);
                    shadow.transform.SetParent(shadowContainer.transform, false);
                    shadow.GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(column - (blockCellsUIColumnCount - 1) / 2f, row - (blockCellsUIRowCount - 1) / 2f) * block_size;
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // (�ʿ��ϴٸ�) Ŭ�� �� � ���带 ����Ѵٰų�, �ٸ� ������ ������ �� ����
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        shadowContainer.SetActive(true);
        // �巡�� ���� ��, ���� Ŭ���� ����Ʈ���� ������Ʈ �߽ɱ����� �������� ���
        // RectTransformUtility.ScreenPointToLocalPointInRectangle �Լ���
        // ���� ��ǥ�� �� �� �ִ�.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out offset
        );
    }
    public void OnDrag(PointerEventData eventData)
    {
        // �巡�� ���� ���콺(�Ǵ� ��ġ) ��ġ�� Canvas ������ ���� ��ǥ�� ��ȯ
        Vector2 localPoint;
        // canvas.transform as RectTransform -> Canvas�� RectTransform
        // eventData.pressEventCamera -> ī�޶� (Screen Space - Overlay��� null�� ���� ������ ����)
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint))
        {
            // �������� ���� ������ ���� ���� �������� �̵�
            rectTransform.localPosition = localPoint - offset;
        }

        //������ �׸��ڸ� �׷�����

        List<List<BoardCellUI>> boardCellsUI = boardUI.boardCellsUI;
        BoardCellUI closestBoardCellUI = null;
        float minDistance = float.MaxValue;

        Vector2 zeroOffset = new Vector2((blockCellsUIColumnCount - 1) / 2f, (blockCellsUIRowCount - 1) / 2f * (-1)) * block_size;

        foreach (var cell in boardCellsUI.SelectMany(row => row))
        {
            float dist = Vector2.Distance(
                rectTransform.anchoredPosition - zeroOffset,
                cell.GetComponent<RectTransform>().anchoredPosition
                + boardUI.GetComponent<RectTransform>().anchoredPosition
            );

            if (dist < minDistance)
            {
                minDistance = dist;
                closestBoardCellUI = cell;
            }
        }

        if (closestBoardCellUI != null)
        {
            shadowContainer.GetComponent<RectTransform>().anchoredPosition =
                closestBoardCellUI.GetComponent<RectTransform>().anchoredPosition
                + boardUI.GetComponent<RectTransform>().anchoredPosition
                + zeroOffset
                - rectTransform.anchoredPosition;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        shadowContainer.SetActive(false);

        List<List<BoardCellUI>> boardCellsUI = boardUI.boardCellsUI;
        BoardCellUI closestBoardCellUI = null;
        float minDistance = float.MaxValue;

        Vector2 zeroOffset = new Vector2((blockCellsUIColumnCount - 1) / 2f, (blockCellsUIRowCount - 1) / 2f * (-1)) * block_size;

        foreach (var cell in boardCellsUI.SelectMany(row => row)) {
            float dist = Vector2.Distance(
                rectTransform.anchoredPosition - zeroOffset,
                cell.GetComponent<RectTransform>().anchoredPosition
                + boardUI.GetComponent<RectTransform>().anchoredPosition
            );

            if (dist < minDistance) {
                minDistance = dist;
                closestBoardCellUI = cell;
            }
        }

        if (closestBoardCellUI != null) {
            Vector2Int boardPosition = closestBoardCellUI.GetCellIndex();
            
            // Board에 배치 요청
            if (GameManager.instance.TryPlaceBlock(blockData, boardPosition)) {
                // 배치 성공 시 UI 위치만 업데이트 (실제 처리는 BoardUI에서)
                rectTransform.anchoredPosition =
                    closestBoardCellUI.GetComponent<RectTransform>().anchoredPosition
                    + boardUI.GetComponent<RectTransform>().anchoredPosition
                    + zeroOffset;
            } else {
                // 배치 실패 시 원위치
                ReturnToHand();
            }
        }
    }
    
    private void ReturnToHand() {
        transform.localPosition = originalPosition;
    }
}

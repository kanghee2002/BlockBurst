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
    /*
    public void SetBlockType(BlockType blockTypeToSet)
    {
        blockType = blockTypeToSet;

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

        if (blockType == BlockType.I)
        {
            blockCellsUIRowCount = 4;
            blockCellsUIColumnCount = 1;
            for (int row = 0; row < blockCellsUIRowCount; row++)
            {
                List<GameObject> listRow = new List<GameObject>();
                listRow.Add(Instantiate(prefabBlockCellUI));
                blockCellsUI.Add(listRow);
            }
        }
        else if (blockType == BlockType.O)
        {
            blockCellsUIRowCount = 2;
            blockCellsUIColumnCount = 2;
            for (int row = 0; row < blockCellsUIRowCount; row++)
            {
                List<GameObject> listRow = new List<GameObject>();
                for (int column = 0; column < blockCellsUIColumnCount; column++)
                {
                    listRow.Add(Instantiate(prefabBlockCellUI));
                }
                blockCellsUI.Add(listRow);
            }
        }
        else if (blockType == BlockType.T)
        {
            blockCellsUIRowCount = 3;
            blockCellsUIColumnCount = 3;

            List<GameObject> listRow1st = new List<GameObject>();
            listRow1st.Add(null);
            listRow1st.Add(Instantiate(prefabBlockCellUI));
            listRow1st.Add(null);
            blockCellsUI.Add(listRow1st);

            List<GameObject> listRow2nd = new List<GameObject>();
            listRow2nd.Add(null);
            listRow2nd.Add(Instantiate(prefabBlockCellUI));
            listRow2nd.Add(null);
            blockCellsUI.Add(listRow2nd);

            List<GameObject> listRow3rd = new List<GameObject>();
            for (int column = 0; column < blockCellsUIColumnCount; column++)
            {
                listRow3rd.Add(Instantiate(prefabBlockCellUI));
            }
            blockCellsUI.Add(listRow3rd);
        }
        else if (blockType == BlockType.L)
        {
            blockCellsUIRowCount = 3;
            blockCellsUIColumnCount = 2;

            List<GameObject> listRow1st = new List<GameObject>();
            listRow1st.Add(Instantiate(prefabBlockCellUI));
            listRow1st.Add(Instantiate(prefabBlockCellUI));
            blockCellsUI.Add(listRow1st);

            List<GameObject> listRow2nd = new List<GameObject>();
            listRow2nd.Add(Instantiate(prefabBlockCellUI));
            listRow2nd.Add(null);
            blockCellsUI.Add(listRow2nd);

            List<GameObject> listRow3rd = new List<GameObject>();
            listRow3rd.Add(Instantiate(prefabBlockCellUI));
            listRow3rd.Add(null);
            blockCellsUI.Add(listRow3rd);
        }
        else if (blockType == BlockType.J)
        {
            blockCellsUIRowCount = 3;
            blockCellsUIColumnCount = 2;

            List<GameObject> listRow1st = new List<GameObject>();
            listRow1st.Add(Instantiate(prefabBlockCellUI));
            listRow1st.Add(Instantiate(prefabBlockCellUI));
            blockCellsUI.Add(listRow1st);

            List<GameObject> listRow2nd = new List<GameObject>();
            listRow2nd.Add(null);
            listRow2nd.Add(Instantiate(prefabBlockCellUI));
            blockCellsUI.Add(listRow2nd);

            List<GameObject> listRow3rd = new List<GameObject>();
            listRow3rd.Add(null);
            listRow3rd.Add(Instantiate(prefabBlockCellUI));
            blockCellsUI.Add(listRow3rd);
        }
        else if (blockType == BlockType.S)
        {
            blockCellsUIRowCount = 2;
            blockCellsUIColumnCount = 3;

            List<GameObject> listRow1st = new List<GameObject>();
            listRow1st.Add(Instantiate(prefabBlockCellUI));
            listRow1st.Add(Instantiate(prefabBlockCellUI));
            listRow1st.Add(null);
            blockCellsUI.Add(listRow1st);

            List<GameObject> listRow2nd = new List<GameObject>();
            listRow2nd.Add(null);
            listRow2nd.Add(Instantiate(prefabBlockCellUI));
            listRow2nd.Add(Instantiate(prefabBlockCellUI));
            blockCellsUI.Add(listRow2nd);
        }
        else if (blockType == BlockType.Z)
        {
            blockCellsUIRowCount = 2;
            blockCellsUIColumnCount = 3;

            List<GameObject> listRow1st = new List<GameObject>();
            listRow1st.Add(null);
            listRow1st.Add(Instantiate(prefabBlockCellUI));
            listRow1st.Add(Instantiate(prefabBlockCellUI));
            blockCellsUI.Add(listRow1st);

            List<GameObject> listRow2nd = new List<GameObject>();
            listRow2nd.Add(Instantiate(prefabBlockCellUI));
            listRow2nd.Add(Instantiate(prefabBlockCellUI));
            listRow2nd.Add(null);
            blockCellsUI.Add(listRow2nd);
        }

        // ��ġ �����ֱ�
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
    */
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
        if (GameUIManager.instance.OnBlockSet(blockData))
        {
            shadowContainer.SetActive(false);
            // 1) �� Cell�� ��ġ(���� ��ǥ�� ���� ��ǥ)�� ��ȸ�ϸ�,
            // 2) �� ������ RectTransform.localPosition���� �Ÿ� ���� ��,
            // 3) ���� ����� Cell�� ã�´�.

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

            // 4) ���� ����� Cell�� ����
            if (closestBoardCellUI != null)
            {
                rectTransform.anchoredPosition =
                    closestBoardCellUI.GetComponent<RectTransform>().anchoredPosition
                    + boardUI.GetComponent<RectTransform>().anchoredPosition
                    + zeroOffset;
            }

            // ���� �ش� Cell�� �̹� �� ������(Block�� ������) ���� �� ���� �ϴ� ��
            // �߰� ������ �ʿ��� �� ����
        }
    }
}

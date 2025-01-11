using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private BlockType blockType;
    [SerializeField] private GameObject prefabBlockCellUI;
    private List<List<GameObject>> blockCellsUI = new List<List<GameObject>>();
    private int blockCellsUIRowCount = 0;
    private int blockCellsUIColumnCount = 0;
    private const float block_size = 96f;

    private BoardUI boardUI;
    private List<BoardCellUI> boardCellsUI;

    [SerializeField] private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 offset;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        boardUI = FindObjectOfType<BoardUI>();
        SetBlockType(BlockType.Z);
    }

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

        // 위치 맞춰주기
        for (int row = 0; row < blockCellsUIRowCount; row++)
        {
            for (int column = 0; column < blockCellsUIColumnCount; column++)
            {
                if (blockCellsUI[row][column] != null)
                {
                    blockCellsUI[row][column].transform.SetParent(this.transform, false);
                    blockCellsUI[row][column].GetComponent<RectTransform>().anchoredPosition
                        = new Vector2(column - (blockCellsUIColumnCount - 1) / 2f, row - (blockCellsUIRowCount - 1) / 2f) * block_size;
                }
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        // (필요하다면) 클릭 시 어떤 사운드를 재생한다거나, 다른 로직을 수행할 수 있음
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 시, 현재 클릭된 포인트에서 오브젝트 중심까지의 오프셋을 계산
        // RectTransformUtility.ScreenPointToLocalPointInRectangle 함수로
        // 로컬 좌표를 얻어낼 수 있다.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out offset
        );
    }
    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 중인 마우스(또는 터치) 위치를 Canvas 기준의 로컬 좌표로 변환
        Vector2 localPoint;
        // canvas.transform as RectTransform -> Canvas의 RectTransform
        // eventData.pressEventCamera -> 카메라 (Screen Space - Overlay라면 null일 수도 있으니 유의)
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint))
        {
            // 오프셋을 빼서 실제로 누른 지점 기준으로 이동
            rectTransform.localPosition = localPoint - offset;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        // 1) 각 Cell의 위치(월드 좌표나 로컬 좌표)를 순회하며,
        // 2) 내 블록의 RectTransform.localPosition과의 거리 차를 비교,
        // 3) 가장 가까운 Cell을 찾는다.

        BoardCellUI closestBoardCellUI = null;
        float minDistance = float.MaxValue;

        Vector2 zeroOffset = new Vector2((blockCellsUIColumnCount - 1) / 2f, (blockCellsUIRowCount - 1) / 2f * (-1)) * block_size;

        boardCellsUI = boardUI.boardCellsUI;
        foreach (var cell in boardCellsUI) // allCells: 모든 격자칸(Cell)을 담은 리스트
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

        // 4) 가장 가까운 Cell에 스냅
        if (closestBoardCellUI != null)
        {
            rectTransform.anchoredPosition =
                closestBoardCellUI.GetComponent<RectTransform>().anchoredPosition
                + boardUI.GetComponent<RectTransform>().anchoredPosition
                + zeroOffset;
            Debug.Log(closestBoardCellUI.GetComponent<BoardCellUI>().cellIndex);
        }

        // 만약 해당 Cell이 이미 차 있으면(Block이 있으면) 놓을 수 없게 하는 등
        // 추가 로직이 필요할 수 있음
    }
}

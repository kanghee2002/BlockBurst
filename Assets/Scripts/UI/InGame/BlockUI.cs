using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private BlockType blockType;
    [SerializeField] private GameObject prefabBlockCellUI;
    private List<GameObject> blockCellsUI = new List<GameObject>();

    private BoardUI boardUI;
    private List<BoardCellUI> boardCellsUI = new List<BoardCellUI>();

    [SerializeField] private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 offset;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        boardUI = FindObjectOfType<BoardUI>();
        SetBlockType(BlockType.I);
    }

    public void SetBlockType(BlockType blockTypeToSet)
    {
        blockType = blockTypeToSet;
        const float block_size = 96f;
        if (blockType == BlockType.I)
        {
            for (int i = 0; i < 4; i++)
            {
                blockCellsUI.Add(Instantiate(prefabBlockCellUI));
                blockCellsUI[i].transform.SetParent(this.transform, false);
                blockCellsUI[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, block_size * (i - 1.5f));
            }
        }
        else if (blockType == BlockType.O)
        {
            for (int i = 0; i < 4; i++)
            {
                blockCellsUI.Add(Instantiate(prefabBlockCellUI));
                blockCellsUI[i].transform.SetParent(this.transform, false);
                blockCellsUI[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(block_size * (i % 2 - 0.5f), block_size * (i / 2 - 0.5f));
            }
        }
        else if (blockType == BlockType.T)
        {
            for (int i = 0; i < 3; i++)
            {
                blockCellsUI.Add(Instantiate(prefabBlockCellUI));
                blockCellsUI[i].transform.SetParent(this.transform, false);
                blockCellsUI[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, block_size * (i - 1));
            }
            blockCellsUI.Add(Instantiate(prefabBlockCellUI));
            blockCellsUI[3].transform.SetParent(this.transform, false);
            blockCellsUI[3].GetComponent<RectTransform>().anchoredPosition = new Vector2(block_size * (-1f), block_size * 1f);

            blockCellsUI.Add(Instantiate(prefabBlockCellUI));
            blockCellsUI[4].transform.SetParent(this.transform, false);
            blockCellsUI[4].GetComponent<RectTransform>().anchoredPosition = new Vector2(block_size * (1f), block_size * 1f);
        }
        else if (blockType == BlockType.L)
        {
            for (int i = 0; i < 3; i++)
            {
                blockCellsUI.Add(Instantiate(prefabBlockCellUI));
                blockCellsUI[i].transform.SetParent(this.transform, false);
                blockCellsUI[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(block_size * (-0.5f), block_size * (i - 1));
            }
            blockCellsUI.Add(Instantiate(prefabBlockCellUI));
            blockCellsUI[3].transform.SetParent(this.transform, false);
            blockCellsUI[3].GetComponent<RectTransform>().anchoredPosition = new Vector2(block_size * (0.5f), block_size * (-1f));
        }
        else if (blockType == BlockType.J)
        {
            for (int i = 0; i < 3; i++)
            {
                blockCellsUI.Add(Instantiate(prefabBlockCellUI));
                blockCellsUI[i].transform.SetParent(this.transform, false);
                blockCellsUI[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(block_size * (0.5f), block_size * (i - 1));
            }
            blockCellsUI.Add(Instantiate(prefabBlockCellUI));
            blockCellsUI[3].transform.SetParent(this.transform, false);
            blockCellsUI[3].GetComponent<RectTransform>().anchoredPosition = new Vector2(block_size * (-0.5f), block_size * (-1f));
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        // (�ʿ��ϴٸ�) Ŭ�� �� � ���带 ����Ѵٰų�, �ٸ� ������ ������ �� ����
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
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
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        // 1) �� Cell�� ��ġ(���� ��ǥ�� ���� ��ǥ)�� ��ȸ�ϸ�,
        // 2) �� ����� RectTransform.localPosition���� �Ÿ� ���� ��,
        // 3) ���� ����� Cell�� ã�´�.

        BoardCellUI closestBoardCellUI = null;
        float minDistance = float.MaxValue;

        boardCellsUI = boardUI.boardCellsUI;
        foreach (var cell in boardCellsUI) // allCells: ��� ����ĭ(Cell)�� ���� ����Ʈ
        {
            float dist = Vector2.Distance(
                rectTransform.anchoredPosition,
                cell.GetComponent<RectTransform>().anchoredPosition + boardUI.GetComponent<RectTransform>().anchoredPosition
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
                closestBoardCellUI.GetComponent<RectTransform>().anchoredPosition + boardUI.GetComponent<RectTransform>().anchoredPosition;
            Debug.Log(closestBoardCellUI.GetComponent<BoardCellUI>().cellIndex);
        }

        // ���� �ش� Cell�� �̹� �� ������(Block�� ������) ���� �� ���� �ϴ� ��
        // �߰� ������ �ʿ��� �� ����
    }
}

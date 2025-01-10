using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public BoardUI boardUI;
    public List<BoardCellUI> boardCellsUI = new List<BoardCellUI>();

    [SerializeField] private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 offset;
    private int draw_index;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
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
                cell.GetComponent<RectTransform>().anchoredPosition
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
                closestBoardCellUI.GetComponent<RectTransform>().anchoredPosition;
        }

        // ���� �ش� Cell�� �̹� �� ������(Block�� ������) ���� �� ���� �ϴ� ��
        // �߰� ������ �ʿ��� �� ����
    }
}

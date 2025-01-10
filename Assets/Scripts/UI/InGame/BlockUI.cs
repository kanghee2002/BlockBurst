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

        boardCellsUI = boardUI.boardCellsUI;
        foreach (var cell in boardCellsUI) // allCells: 모든 격자칸(Cell)을 담은 리스트
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

        // 4) 가장 가까운 Cell에 스냅
        if (closestBoardCellUI != null)
        {
            rectTransform.anchoredPosition =
                closestBoardCellUI.GetComponent<RectTransform>().anchoredPosition;
        }

        // 만약 해당 Cell이 이미 차 있으면(Block이 있으면) 놓을 수 없게 하는 등
        // 추가 로직이 필요할 수 있음
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
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
        // 드래그 종료 시 필요한 로직 (ex. 위치 제한, 스냅(snap) 등)을 처리
    }
}

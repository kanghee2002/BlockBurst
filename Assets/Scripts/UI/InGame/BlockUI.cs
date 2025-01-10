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
        // �巡�� ���� �� �ʿ��� ���� (ex. ��ġ ����, ����(snap) ��)�� ó��
    }
}

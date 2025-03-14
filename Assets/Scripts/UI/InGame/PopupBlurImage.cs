using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupBlurImage : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image image;
    [SerializeField] private UnityEvent onClick;

    Tween currentTween;
    public void OpenPopupBlurImage(Color colorToSet, float duration = 0.2f)
    {
        image.raycastTarget = true;
        currentTween?.Kill();
        currentTween = image.DOColor(colorToSet, duration);
    }
    public void ClosePopupBlurImage(float duration = 0.2f)
    {
        image.raycastTarget = false;
        currentTween?.Kill();
        currentTween = image.DOColor(Color.clear, duration);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }
}

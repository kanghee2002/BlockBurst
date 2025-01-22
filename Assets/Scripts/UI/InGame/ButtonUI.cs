using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private bool isPressed = false;
    
    private Image image;
    private RectTransform rectTransform;

    private Color originalColor;
    private Vector2 originalScale;
    private Vector2 originalPosition;

    private const float brightnessIncrease = 0.2f;
    private Color hoverColor = Color.gray;

    private const float brightnessDecrease = 0.2f;
    private Color pressedColor;
    private Vector2 pressedScale;
    private Vector2 pressedPosition;

    void Start()
    {
        image = GetComponent<Image>();

        rectTransform = GetComponent<RectTransform>();
        originalColor = image.color;
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.anchoredPosition;

        hoverColor = Color.Lerp(originalColor, Color.white, brightnessIncrease);

        pressedColor = Color.Lerp(originalColor, Color.black, brightnessDecrease);
        pressedScale = originalScale * new Vector2(0.99f, 0.99f);
        pressedPosition = originalPosition + new Vector2(0f, -0.08f * rectTransform.rect.height);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isPressed == true)
        {
            image.color = pressedColor;
            rectTransform.anchoredPosition = pressedPosition;
            rectTransform.localScale = pressedScale;
        }
        else if (isPressed == false)
        {
            image.color = hoverColor;
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = originalColor;
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        image.color = pressedColor;
        rectTransform.anchoredPosition = pressedPosition;
        rectTransform.localScale = pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        image.color = originalColor;
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = originalScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isPressed = false;
        image.color = originalColor;
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = originalScale;
        OnClick();
    }

    public virtual void OnClick()
    {
        // Please override this method.
    }

}

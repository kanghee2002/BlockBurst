using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private bool isPressed = false;
    private Color originalColor;

    private const float brightnessIncrease = 0.2f;
    private Color hoverColor = Color.gray;

    private const float brightnessDecrease = 0.2f;
    private Color pressedColor;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        originalColor = image.color;

        hoverColor = Color.Lerp(originalColor, Color.white, brightnessIncrease);
        
        pressedColor = Color.Lerp(originalColor, Color.black, brightnessDecrease);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isPressed == true)
        {
            image.color = pressedColor;
        }
        else if (isPressed == false)
        {
            image.color = hoverColor;
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = originalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        image.color = pressedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        image.color = originalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
        Debug.Log("asdf");
    }

    public void OnClick()
    {
        // Please override this method.
    }

}

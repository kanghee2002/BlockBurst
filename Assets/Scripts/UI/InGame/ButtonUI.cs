using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private bool isPressed = false;
    
    private GameObject button;
    private Image image;
    private RectTransform rectTransform;

    private Color originalColor;
    private Vector2 originalScale;
    private Vector2 originalPosition;

    private const float brightnessIncrease = 0.2f;
    private Color hoverColor;

    private const float brightnessDecrease = 0.2f;
    private Color pressedColor;
    private Vector2 pressedScale;
    private Vector2 pressedPositionOffset;
    private Vector2 pressedPosition;

    private GameObject shadowObject;

    [SerializeField] private UnityEvent onClick;
    
    void Awake()
    {
        button = transform.GetChild(0).gameObject;
    }

    void Start()
    {
        Initialize();

        CreateShadow();

        CreateHitbox();
    }

    private void Initialize()
    {
        image = button.GetComponent<Image>();

        rectTransform = button.GetComponent<RectTransform>();
        originalColor = image.color;
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.anchoredPosition;

        hoverColor = Color.Lerp(originalColor, Color.white, brightnessIncrease);

        pressedColor = Color.Lerp(originalColor, Color.black, brightnessDecrease);
        pressedScale = originalScale * new Vector2(0.99f, 0.99f);
        pressedPositionOffset = new Vector2(0f, -0.08f * rectTransform.rect.height);
        pressedPosition = originalPosition + pressedPositionOffset;
    }
    private void CreateShadow()
    {
        // 새 GameObject 생성
        shadowObject = new GameObject("Shadow");
        shadowObject.transform.SetParent(transform); // 부모를 현재 오브젝트로 설정
        shadowObject.transform.SetAsFirstSibling(); // 부모의 첫 번째 자식으로 설정 (이미지 뒤에 배치)

        // RectTransform 설정
        RectTransform shadowRect = shadowObject.AddComponent<RectTransform>();
        shadowRect.sizeDelta = rectTransform.sizeDelta; // 원본 크기와 동일
        shadowRect.anchoredPosition = pressedPositionOffset; // 그림자 위치 설정
        shadowRect.localScale = pressedScale; // 그림자 크기 설정

        // Image 설정
        Image shadowImage = shadowObject.AddComponent<Image>();
        shadowImage.sprite = image.sprite; // 원본 이미지와 동일한 Sprite 사용
        shadowImage.color = new Color(0f, 0f, 0f, 0.8f); // 반투명한 검정색

        // 그림자를 Raycast 타겟에서 제외 (클릭 감지 방지)
        shadowImage.raycastTarget = false;
    }

    private void CreateHitbox()
    {
        GameObject hitbox = new GameObject("HitBox");
        hitbox.transform.SetParent(transform);
        hitbox.transform.SetAsFirstSibling();

        RectTransform hitboxRect = hitbox.AddComponent<RectTransform>();
        hitboxRect.sizeDelta = rectTransform.sizeDelta;
        hitboxRect.anchoredPosition = originalPosition;
        hitboxRect.localScale = originalScale;

        Image hitboxImage = hitbox.AddComponent<Image>();
        hitboxImage.sprite = image.sprite;
        hitboxImage.color = new Color(0f, 0f, 0f, 0f);

        //hitboxImage.raycastTarget = true;
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

        onClick?.Invoke();

        AudioManager.instance.SFXSelectMenu();
    }
}

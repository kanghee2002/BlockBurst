using UnityEngine;
using UnityEngine.UI;

public class CellEffectUI : MonoBehaviour
{
    private Image borderImage;
    [SerializeField] private Sprite borderSprite;
    [SerializeField] private Color baseColor = Color.white;
    [SerializeField, Range(0f, 1f)] private float intensity = 0f;

    private void Awake()
    {
        if (borderImage == null)
        {
            GameObject border = new GameObject("Border");
            border.transform.SetParent(transform);
            border.transform.localPosition = Vector3.zero;
            border.transform.localScale = Vector3.one;
            
            borderImage = border.AddComponent<Image>();
            borderImage.sprite = borderSprite;
        }
        
        UpdateBorderEffect();
    }

    public void SetScoreEffect(int score)
    {
        intensity = Mathf.Clamp01(Mathf.Log(score - 19, 6));
        UpdateBorderEffect();
    }
    
    public void SetBorderEffect(float effectIntensity)
    {
        intensity = Mathf.Clamp01(effectIntensity);
        UpdateBorderEffect();
    }
    
    private void UpdateBorderEffect()
    {
        if (borderImage != null)
        {
            Color newColor = baseColor;
            newColor.a *= intensity;
            borderImage.color = newColor;
        }
    }
}
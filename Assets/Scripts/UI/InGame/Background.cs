using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Background : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private const float rotationSpeed = 4.0f;

    private Tween currentTween;
    private static readonly Color DefaultColor = Color.white; // 기본 색상

    private void Update()
    {
        if (material != null)
        {
            float rotationValue = Mathf.Repeat(Time.time * rotationSpeed, 360f);
            material.SetFloat("_Rotation", rotationValue);
        }
    }

    public void SetRandomColor()
    {
        Color randomColor = new Color(Random.value, Random.value, Random.value);
        SetColor(randomColor);
    }

    public void SetColor(Color colorToSet, float duration = 2.0f)
    {
        currentTween?.Kill();
        currentTween = spriteRenderer.DOColor(colorToSet, duration);
    }

    private void OnApplicationQuit()
    {
        ResetValues();
    }

    private void OnDestroy()
    {
        ResetValues();
    }

    private void ResetValues()
    {
        // 기본 색상으로 변경
        spriteRenderer.color = DefaultColor;

        // 머티리얼의 Rotation 값 초기화
        if (material != null)
        {
            material.SetFloat("_Rotation", 0f);
        }

        // Tween 제거
        currentTween?.Kill();
    }
}

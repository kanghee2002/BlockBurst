using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Background : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private const float rotationSpeed = 4.0f;

    /*
    private void Start()
    {
        StartCoroutine(ChangeColorPeriodically());
    }
    private IEnumerator ChangeColorPeriodically()
    {
        while (true)
        {
            // 랜덤 색상 생성
            Color randomColor = new Color(Random.value, Random.value, Random.value);

            // DOTween으로 구현된 SetColor 호출
            SetColor(randomColor);

            yield return new WaitForSeconds(2.0f);
        }
    }
    */

    private void Update()
    {
        if (material != null)
        {
            // 시간에 따라 Rotation 값을 변경
            float rotationValue = Mathf.Repeat(Time.time * rotationSpeed, 360f); // 0~360도 반복
            material.SetFloat("_Rotation", rotationValue); // Shader Graph의 Rotation 변수에 값 전달
        }
    }

    Tween currentTween;
    public void SetColor(Color colorToSet, float duration = 2.0f)
    {
        currentTween?.Kill();
        currentTween = spriteRenderer.DOColor(colorToSet, duration);
    }
}

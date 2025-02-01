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
            // ���� ���� ����
            Color randomColor = new Color(Random.value, Random.value, Random.value);

            // DOTween���� ������ SetColor ȣ��
            SetColor(randomColor);

            yield return new WaitForSeconds(2.0f);
        }
    }
    */

    private void Update()
    {
        if (material != null)
        {
            // �ð��� ���� Rotation ���� ����
            float rotationValue = Mathf.Repeat(Time.time * rotationSpeed, 360f); // 0~360�� �ݺ�
            material.SetFloat("_Rotation", rotationValue); // Shader Graph�� Rotation ������ �� ����
        }
    }

    Tween currentTween;
    public void SetColor(Color colorToSet, float duration = 2.0f)
    {
        currentTween?.Kill();
        currentTween = spriteRenderer.DOColor(colorToSet, duration);
    }
}

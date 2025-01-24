using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private const float rotationSpeed = 4.0f;

    /*
    private void Start()
    {
        Color colorToSet = new Color(0.0627f, 0.8235f, 0.4588f, 1.0f);
        SetColor(colorToSet);
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

    public void SetColor(Color colorToSet)
    {
        spriteRenderer.color = colorToSet;
    }
}

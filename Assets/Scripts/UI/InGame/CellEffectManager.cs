using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CellEffectManager : MonoBehaviour
{
    public static CellEffectManager instance;
    const float SCORE_EFFECT_INNER = 0.7f;
    const float SCORE_EFFECT_OUTER = 1.0f;
    Color SCORE_EFFECT_COLOR = new Color(0f, 0f, 0f, 1f);

    void Awake()
    {
        // singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        /* test code
        Image image = GetComponent<Image>();
        Texture2D baseTexture = image.sprite.texture;
        Texture2D maskedTexture = CreateRectangularGradientMask(baseTexture, Color.red, .8f, .9f);
        Sprite newSprite = Sprite.Create(
            maskedTexture,
            new Rect(0, 0, maskedTexture.width, maskedTexture.height),
            new Vector2(0.5f, 0.5f)
        );
        image.sprite = newSprite;
        */
    }

    private float CalculateIntensity(int score)
    {
        float intensity = Mathf.Log(score - 10, 30);
        
        if (float.IsNaN(intensity) || intensity < 0)
            intensity = 0;
        else if (intensity > 1)
            intensity = 1;

        return intensity;
    }

    public void PlaceEffect(Block block)
    {
        // 점수에 따라 화면을 흔듦, 로그스케일로 계산
        float intensity = CalculateIntensity(block.Score);
        ShakeEverything(intensity * 30f);
    }

    public Sprite ApplyEffect(Sprite baseSprite, Block block)
    {
        Texture2D baseTexture = baseSprite.texture;
        Texture2D maskedTexture = ScoreEffect(baseTexture, block.Score);
        return Sprite.Create(
            maskedTexture,
            new Rect(0, 0, maskedTexture.width, maskedTexture.height),
            new Vector2(0.5f, 0.5f)
        );
    }

    public Texture2D ScoreEffect(Texture2D baseTexture, int score)
    {
        // 점수가 0이하인 경우 전체 색상 변경
        if (score <= 0)
        {
            return ApplyColorToTexture(baseTexture, new Color(0f, 0f, 0f, 0.5f));
        }

        // 양수 점수는 기존 그라데이션 효과 적용
        float intensity = CalculateIntensity(score);

        float gap = (SCORE_EFFECT_OUTER - SCORE_EFFECT_INNER) * (1 - intensity);
        
        return CreateRectangularGradientMask(baseTexture, SCORE_EFFECT_COLOR, SCORE_EFFECT_INNER + gap, SCORE_EFFECT_OUTER + gap);
    }

    /*
    private Texture2D UpgradeEffect(Texture2D baseTexture, int level)
    {

    }
    */

    // base functions
    private Texture2D ApplyColorToTexture(Texture2D baseTexture, Color color)
    {
        // 새로운 텍스처 생성
        Texture2D resultTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.RGBA32, false);
        
        // 기존 텍스처의 픽셀을 복사
        Color[] basePixels = baseTexture.GetPixels();
        Color[] resultPixels = new Color[basePixels.Length];
        System.Array.Copy(basePixels, resultPixels, basePixels.Length);
        
        // 각 픽셀에 새로운 색상 블렌딩
        for (int i = 0; i < basePixels.Length; i++)
        {
            // 알파값이 0인 픽셀은 건너뛰기
            if (basePixels[i].a <= 0)
                continue;
            
            // 기존 색상과 새로운 색상을 블렌딩
            resultPixels[i] = Color.Lerp(basePixels[i], color, color.a);
        }
        
        resultTexture.SetPixels(resultPixels);
        resultTexture.Apply();
        return resultTexture;
    }

    private Texture2D CreateRectangularGradientMask(Texture2D baseTexture, Color color, float innerRatio, float outerRatio)
    {
        // 새로운 텍스처 생성
        Texture2D resultTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.RGBA32, false);
        
        // 기존 텍스처의 픽셀을 복사
        Color[] basePixels = baseTexture.GetPixels();
        Color[] resultPixels = new Color[basePixels.Length];
        System.Array.Copy(basePixels, resultPixels, basePixels.Length);
        
        // 텍스처의 중심점과 크기
        Vector2 center = new Vector2(resultTexture.width / 2f, resultTexture.height / 2f);
        float halfWidth = resultTexture.width / 2f;
        float halfHeight = resultTexture.height / 2f;

        // 내부/외부 사각형 크기 계산
        float innerWidth = halfWidth * innerRatio;
        float innerHeight = halfHeight * innerRatio;
        float outerWidth = halfWidth * outerRatio;
        float outerHeight = halfHeight * outerRatio;
        
        // 각 픽셀 순회
        for (int x = 0; x < resultTexture.width; x++)
        {
            for (int y = 0; y < resultTexture.height; y++)
            {
                int index = y * resultTexture.width + x;
                
                // 기존 픽셀의 알파값이 0이면 건너뛰기
                if (basePixels[index].a <= 0)
                    continue;
                    
                // 중심점으로부터의 상대적 위치
                float distX = Mathf.Abs(x - center.x);
                float distY = Mathf.Abs(y - center.y);
                
                // 사각형 영역 판정을 위한 비율 계산
                float ratioX = distX / halfWidth;
                float ratioY = distY / halfHeight;
                float maxRatio = Mathf.Max(ratioX, ratioY);
                
                if (maxRatio <= innerRatio)
                {
                    // 내부 사각형: 원본 유지
                    continue;
                }
                else if (maxRatio >= outerRatio)
                {
                    // 외부 사각형: 지정된 색상
                    resultPixels[index] = new Color(
                        color.r,
                        color.g,
                        color.b,
                        color.a * basePixels[index].a
                    );
                }
                else
                {
                    // 중간 영역: 그라데이션
                    float t = (maxRatio - innerRatio) / (outerRatio - innerRatio);
                    Color gradientColor = Color.Lerp(basePixels[index], color, t);
                    gradientColor.a *= basePixels[index].a;
                    resultPixels[index] = gradientColor;
                }
            }
        }
        
        resultTexture.SetPixels(resultPixels);
        resultTexture.Apply();
        return resultTexture;
    }

    private Texture2D CreateCircularGradientMask(Texture2D baseTexture, Color color, float innerRatio, float outerRatio)
    {
        // 새로운 텍스처 생성
        Texture2D resultTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.RGBA32, false);
        
        // 기존 텍스처의 픽셀을 복사
        Color[] basePixels = baseTexture.GetPixels();
        Color[] resultPixels = new Color[basePixels.Length];
        System.Array.Copy(basePixels, resultPixels, basePixels.Length);
        
        // 텍스처의 중심점
        Vector2 center = new Vector2(resultTexture.width / 2f, resultTexture.height / 2f);

        // 대각선의 길이
        float baseRadius = Vector2.Distance(Vector2.zero, center);
        float innerRadius = innerRatio * baseRadius;
        float outerRadius = outerRatio * baseRadius;
        
        // 각 픽셀 순회
        for (int x = 0; x < resultTexture.width; x++)
        {
            for (int y = 0; y < resultTexture.height; y++)
            {
                int index = y * resultTexture.width + x;
                
                // 기존 픽셀의 알파값이 0이면 건너뛰기
                if (basePixels[index].a <= 0)
                    continue;
                    
                // 현재 픽셀과 중심점 사이의 거리 계산
                float distance = Vector2.Distance(new Vector2(x, y), center);
                
                if (distance <= innerRadius)
                {
                    // innerRadius 안쪽은 원본 유지
                    continue;
                }
                else if (distance >= outerRadius)
                {
                    // outerRadius 바깥쪽은 지정된 색상으로 설정
                    resultPixels[index] = new Color(
                        color.r,
                        color.g,
                        color.b,
                        color.a * basePixels[index].a
                    );
                }
                else
                {
                    // 중간 영역은 그라데이션 처리
                    float t = (distance - innerRadius) / (outerRadius - innerRadius);
                    Color gradientColor = Color.Lerp(basePixels[index], color, t);
                    gradientColor.a *= basePixels[index].a;
                    resultPixels[index] = gradientColor;
                }
            }
        }
        
        resultTexture.SetPixels(resultPixels);
        resultTexture.Apply();
        return resultTexture;
    }

    public void ShakeEverything(float intensity = 1f)
    {
        float duration = 0.3f;
        int vibrato = 10;
        float randomness = 90f;
        bool fadeOut = true;
        Vector3 strength = new Vector3(0.2f * intensity, 0.2f * intensity, 0);

        RectTransform[] uiElements = FindObjectsOfType<RectTransform>();
        
        foreach(RectTransform rt in uiElements)
        {
            // null, destroyed, 비활성화 체크
            if(rt == null || !rt || !rt.gameObject.activeSelf) continue;
            
            // Canvas의 직계 자식이 아닌 실제 UI 요소들만 처리
            Transform parent = rt.parent;
            if(parent != null && !parent.TryGetComponent<Canvas>(out _))
            {
                rt.DOShakeAnchorPos(duration, strength, vibrato, randomness, fadeOut)
                .SetUpdate(true);
            }
        }
    }
}

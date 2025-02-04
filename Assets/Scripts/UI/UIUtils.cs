using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public static class UIUtils
{
    
    // 효과 타입별 색상 정의
    public static readonly Dictionary<ItemEffectType, Color> effectColors = new Dictionary<ItemEffectType, Color>()
    {
        { ItemEffectType.SCORE, new Color(0x0b/255f, 0xa9/255f, 0x05/255f) }, // #0ba905
        { ItemEffectType.DECK, new Color(0x47/255f, 0x38/255f, 0xff/255f) },  // #4738ff
        { ItemEffectType.GOLD, new Color(0xd9/255f, 0xa7/255f, 0x38/255f) },  // #d9a738
        { ItemEffectType.OTHER, new Color(0xfc/255f, 0x8b/255f, 0x4d/255f) }  // #fc8b4d
    };

    // 레어도별 색상 정의
    public static readonly Dictionary<ItemRarity, Color> rarityColors = new Dictionary<ItemRarity, Color>()
    {
        { ItemRarity.SILVER, new Color(0xb0/255f, 0xa7/255f, 0xb8/255f) },   // #b0a7b8
        { ItemRarity.GOLD, new Color(0xff/255f, 0xc8/255f, 0x57/255f) },     // #ffc857
        { ItemRarity.PLATINUM, new Color(0xc0/255f, 0xf9/255f, 0xff/255f) }  // #c0f9ff
    };

    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        
        // 첫 단어를 소문자로 시작
        string firstWord = str[0].ToString().ToLowerInvariant();
        
        // 나머지 문자열이 있다면 그대로 붙임
        if (str.Length > 1)
            firstWord += str.Substring(1);
            
        return firstWord;
    }

    /// <summary>
    /// UI를 열 때 사용합니다.
    /// </summary>
    /// <param name="rectTransform">RectTransform</param>
    /// <param name="axis">X축으로 열지 Y축으로 열지</param>
    /// <param name="insidePosition">도착할 위치</param>
    /// <param name="duration">애니메이션 시간</param>
    public static void OpenUI(RectTransform rectTransform, string axis, float insidePosition, float duration = 0.2f)
    {
        if (axis == "X")
        {
            rectTransform.DOAnchorPosX(insidePosition, duration)
                .SetEase(Ease.OutCubic);
        }
        else if (axis == "Y")
        {
            rectTransform.DOAnchorPosY(insidePosition, duration)
                .SetEase(Ease.OutCubic);
        }
    }

    /// <summary>
    /// UI를 닫을 때 사용합니다.
    /// </summary>
    /// <param name="rectTransform">RectTransform</param>
    /// <param name="axis">X축으로 닫을지 Y축으로 닫을지</param>
    /// <param name="insidePosition">도착할 위치</param>
    /// <param name="outsidePositionOffset">숨겨질 위치</param>
    /// <param name="duration">애니메이션 시간</param>
    public static void CloseUI(RectTransform rectTransform, string axis, float insidePosition, float outsidePositionOffset, float duration = 0.2f)
    {
        if (axis == "X")
        {
            rectTransform.DOAnchorPosX(insidePosition + outsidePositionOffset, duration)
                .SetEase(Ease.OutCubic);
        }
        else if (axis == "Y")
        {
            rectTransform.DOAnchorPosY(insidePosition + outsidePositionOffset, duration)
                .SetEase(Ease.OutCubic);
        }
    }

    /// <summary>
    /// Text를 튕기는 애니메이션을 재생합니다.
    /// </summary>
    /// <param name="textTransform">Text의 Transform</param>
    /// <param name="duration">애니메이션 시간</param>
    /// <param name="strength">애니메이션 강도</param>
    public static void BounceText(Transform textTransform, float duration = 0.3f, float strength = 0.2f)
    {
        textTransform.DOPunchScale(Vector3.one * strength, duration, 1, 0.5f)
            .OnKill(() => textTransform.DOScale(Vector3.one, 0.2f));
    }
    /// <summary>
    /// 6자리 16진수 색상 코드를 Color로 변환합니다.
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString("#" + hex, out Color color))
        {
            return color;
        }
        else
        {
            Debug.LogError("유효하지 않은 색상 코드: " + hex);
            return Color.white; // 기본값
        }
    }
}

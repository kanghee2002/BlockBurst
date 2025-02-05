using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public static class UIUtils
{
    
    // 효과 타입별 색상 정의
    public static readonly Dictionary<ItemEffectType, Color> effectColors = new Dictionary<ItemEffectType, Color>()
    {
        { ItemEffectType.SCORE, new Color(0x47/255f, 0x38/255f, 0xff/255f) }, // #4738ff 파랑
        { ItemEffectType.DECK, new Color(0xe3/255f, 0x6f/255f, 0x2a/255f) },  // #e36f2a 주황
        { ItemEffectType.GOLD, new Color(0xd9/255f, 0xa7/255f, 0x38/255f) },  // #d9a738 노랑
        { ItemEffectType.OTHER, new Color(0x8c/255f, 0x8c/255f, 0x8c/255f) }  // #8c8c8c 회색
    };

    // 레어도별 색상 정의
    public static readonly Dictionary<ItemRarity, Color> rarityColors = new Dictionary<ItemRarity, Color>()
    {
        { ItemRarity.SILVER, new Color(0x0b/255f, 0xa9/255f, 0x05/255f) },   // #0ba905 연두
        { ItemRarity.GOLD, new Color(0x70/255f, 0x1b/255f, 0xd2/255f) },     // #701bd2 보라
        { ItemRarity.PLATINUM, new Color(0xb7/255f, 0x1c/255f, 0x31/255f) }  // #b71c31 빨강
    };

    public static readonly Dictionary<ItemType, string> itemTypeNames = new Dictionary<ItemType, string>()
    {
        { ItemType.ITEM, "아이템" },
        { ItemType.UPGRADE, "강화" },
        { ItemType.ADD_BLOCK, "덱" },
        { ItemType.CONVERT_BLOCK, "덱" },
    };

    public static readonly Dictionary<ItemRarity, string> itemRarityNames = new Dictionary<ItemRarity, string>()
    {
        { ItemRarity.SILVER, "일반" },
        { ItemRarity.GOLD, "희귀" },
        { ItemRarity.PLATINUM, "레어" },
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

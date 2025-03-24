using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;

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
        { ItemRarity.COMMON, new Color(0x46/255f, 0xd9/255f, 0x41/255f) },   // #46D941 연두
        { ItemRarity.RARE, new Color(0x4f/255f, 0xad/255f, 0xff/255f) },     // #4FADFF 파랑
        { ItemRarity.EPIC, new Color(0xa3/255f, 0x53/255f, 0xff/255f) },     // #A353FF 보라
        { ItemRarity.LEGENDARY, new Color(0xd2/255f, 0x00/255f, 0x1d/255f) }  // #D2001D 빨강
    };

    public static readonly Dictionary<ItemType, Color> itemTypeColors = new Dictionary<ItemType, Color>()
    {
        { ItemType.ITEM, new Color(0xff/255f, 0xbe/255f, 0x00/255f) },  // FFBE00 노랑
        { ItemType.BOOST, new Color(0x66/255f, 0xc7/255f, 0xe4/255f) },  // 66C7E4 하늘
        { ItemType.ADD_BLOCK, new Color(0xbe/255f, 0xe4/255f, 0x66/255f) },  // BEE466 연두
    };

    public static readonly Dictionary<ItemType, string> itemTypeNames = new Dictionary<ItemType, string>()
    {
        { ItemType.ITEM, "아이템" },
        { ItemType.BOOST, "부스트" },
        { ItemType.UPGRADE, "강화" },
        { ItemType.ADD_BLOCK, "덱" },
        { ItemType.CONVERT_BLOCK, "덱" },
    };

    public static readonly Dictionary<ItemRarity, string> itemRarityNames = new Dictionary<ItemRarity, string>()
    {
        { ItemRarity.COMMON, "일반" },
        { ItemRarity.RARE, "희귀" },
        { ItemRarity.EPIC, "특급" },
        { ItemRarity.LEGENDARY, "전설" },
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
                .SetEase(Ease.OutBack, overshoot: 1f);
        }
        else if (axis == "Y")
        {
            rectTransform.DOAnchorPosY(insidePosition, duration)
                .SetEase(Ease.OutBack, overshoot: 1f);
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
                .SetEase(Ease.InBack, overshoot: 1f);
            //rectTransform.anchoredPosition = new Vector2(insidePosition + outsidePositionOffset, rectTransform.anchoredPosition.y);
        }
        else if (axis == "Y")
        {
            rectTransform.DOAnchorPosY(insidePosition + outsidePositionOffset, duration)
                .SetEase(Ease.InBack, overshoot: 1f);
            //rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, insidePosition + outsidePositionOffset);
        }
    }

    public static Sequence PlaySlowShakeAnimation(Transform transform, float rotateAmount = 0.5f, float duration = 4f, float delay = 0f)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DORotate(new Vector3(0, 0, rotateAmount), duration))
            .Append(transform.DORotate(new Vector3(0, 0, -rotateAmount), duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad))
            .SetDelay(delay);

        sequence.OnKill(() =>
        {
            transform.DORotate(Vector3.zero, 1f);
        });

        return sequence;
    }

    /// <summary>
    /// Image의 색상을 변경합니다.
    /// </summary>
    /// <param name="image">Image</param>
    /// <param name="color">기준 색상</param>
    /// <param name="scalar">색상 변경치</param>
    public static void SetImageColorByScalar(MaskableGraphic image, Color color, float scalar, float duration = 0.5f)
    {
        Color scaledColor = new Color(color.r, color.g, color.b) * scalar;
        Color resultColor = new Color(scaledColor.r, scaledColor.g, scaledColor.b, 1f);
        image.DOColor(resultColor, duration)
            .SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// Text의 색상을 변경합니다.
    /// </summary>
    /// <param name="text">TextMeshProUGUI</param>
    /// <param name="color">기준 색상</param>
    /// <param name="scalar">색상 변경치</param>
    public static void SetTextColorByScalar(TextMeshProUGUI text, Color color, float scalar)
    {
        Color scaledColor = new Color(color.r, color.g, color.b) * scalar;
        Color resultColor = new Color(scaledColor.r, scaledColor.g, scaledColor.b, 1f);
        text.DOColor(resultColor, 0.5f)
            .SetEase(Ease.OutQuad);
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

    public static string SetBlockNameToIcon(string text)
    {
        string[] blockNames = Enum.GetNames(typeof(BlockType));

        foreach (var blockName in blockNames)
        {
            if (text.Contains(blockName))
            {
                if (Enum.TryParse(blockName, out BlockType blockType))
                {
                    string pattern = @"\b" + Regex.Escape(blockName) + @"\b";
                    string replacement = "<sprite=" + (int)blockType + ">";

                    text = Regex.Replace(text, pattern, replacement);
                }
            }
        }

        return text;
    }

    public static string GetEffectValueText(string text, EffectData effectData)
    {

        string value = Math.Abs(effectData.effectValue).ToString();

        text = text.Replace("+EffectValue 블록 점수", "<color=blue>+EffectValue</color> 블록 점수");
        text = text.Replace("+EffectValue 배수", "<color=red>+EffectValue</color> 배수");
        text = text.Replace("XEffectValue 배수", "<color=red>XEffectValue</color> 배수");
        text = text.Replace("+EffectValue 기본 배수", "<color=red>+EffectValue</color> 기본 배수");
        text = text.Replace("+EffectValue 리롤 횟수", "<color=green>+EffectValue</color> 리롤 횟수");
        text = text.Replace("+EffectValue 기본 리롤 횟수", "<color=green>+EffectValue</color> 기본 리롤 횟수");
        text = text.Replace("$EffectValue", "<color=yellow>$EffectValue</color>");

        string result = text.Replace("EffectValue", value);

        return result;
    }

    public static string SetTextColor(string text, string specificColor = "")
    {
        string[] words = text.Split(new char[] {' ', '\n'});

        int index = Array.IndexOf(words, "배수");

        if (index != -1 && index + 1 < words.Length)
        {
            string count = words[index + 1];
            count = "<color=red>" + count + "</color>";
        }

        index = Array.IndexOf(words, "점수");

        if (index != -1 && index + 1 < words.Length)
        {
            string count = words[index + 1];
            count = "<color=blue>" + count + "</color>";
        }

        index = Array.IndexOf(words, "골드");

        if (index != -1 && index + 1 < words.Length)
        {
            Debug.Log("Gold");
            string count = words[index + 1];
            count = "<color=#8E8E00>" + count + "</color>";
        }

        return string.Join(' ', words);
    }
}

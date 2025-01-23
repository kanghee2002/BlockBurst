using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public static class UIUtils
{
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
        textTransform.DOPunchScale(Vector3.one * strength, duration, 1, 0.5f);
    }
}

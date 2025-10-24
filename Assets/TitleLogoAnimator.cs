using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleLogoAnimator : MonoBehaviour
{
    [SerializeField] private List<RectTransform> upBlocks;
    [SerializeField] private List<RectTransform> downBlocks;

    [SerializeField] private float moveDistance = 300f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float moveDelay = 0.2f;
    [SerializeField] private float upDownInterval = 1f;
    [SerializeField] private float animationInterval = 1f;

    private Sequence animationSequence;

    private void Start()
    {
        StartLogoAnimation();
    }

    public void StartLogoAnimation()
    {
        float direction = 1f;

        animationSequence = DOTween.Sequence();

        animationSequence.AppendInterval(animationInterval);

        Sequence upSequence1 = DOTween.Sequence();
        float upInterval = 0f;
        foreach (RectTransform upBlock in upBlocks)
        {
            upSequence1.Insert(upInterval, upBlock.DOAnchorPosX(direction * moveDistance, duration)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad));
            upInterval += moveDelay;
        }

        animationSequence.Append(upSequence1);
        animationSequence.AppendInterval(upDownInterval);

        direction *= -1;

        Sequence downSequence1 = DOTween.Sequence();
        float downInterval = 0f;
        foreach (RectTransform downBlock in downBlocks)
        {
            downSequence1.Insert(downInterval, downBlock.DOAnchorPosX(direction * moveDistance, duration)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad));
            downInterval += moveDelay;
        }
        animationSequence.Append(downSequence1);

        animationSequence.AppendInterval(animationInterval);

        Sequence upSequence2 = DOTween.Sequence();
        upInterval = 0f;
        foreach (RectTransform upBlock in upBlocks)
        {
            upSequence2.Insert(upInterval, upBlock.DOAnchorPosX(direction * moveDistance, duration)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad));
            upInterval += moveDelay;
        }

        animationSequence.Append(upSequence2);
        animationSequence.AppendInterval(upDownInterval);

        direction *= -1;

        Sequence downSequence2 = DOTween.Sequence();
        downInterval = 0f;
        foreach (RectTransform downBlock in downBlocks)
        {
            downSequence2.Insert(downInterval, downBlock.DOAnchorPosX(direction * moveDistance, duration)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad));
            downInterval += moveDelay;
        }
        animationSequence.Append(downSequence2);

        animationSequence.SetLoops(-1);
    }

    public void StopLogoAnimation()
    {
        animationSequence.Kill();
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockNotificationUI : MonoBehaviour
{
    [SerializeField] private Image layout;
    [SerializeField] private Image unlockedImage;

    private const float windowsInsidePositionX = 150;
    private const float mobileInsidePositionX = 150;
    private const float outsidePositionOffsetX = 250;

    private const float duration = 0.2f;
    private const float interval = 0.5f;
    private const float animationDelay = 0.25f;

    private RectTransform rectTransform;

    private Queue<(Sprite, Color)> animationQueue;
    private bool isPlayingAnimation;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        animationQueue = new Queue<(Sprite, Color)>();
        isPlayingAnimation = false;
    }


    public void PlayUnlockAnimation(Sprite sprite, Color uiColor)
    {
        animationQueue.Enqueue((sprite, uiColor));

        if (!isPlayingAnimation)
        {
            isPlayingAnimation = true;
            StartCoroutine(UnlockAnimation());
        }
    }

    private IEnumerator UnlockAnimation()
    {
        while (animationQueue.Count > 0)
        {
            (Sprite sprite, Color uiColor) = animationQueue.Dequeue();

            Sequence shakeSequence = DOTween.Sequence();

            shakeSequence.AppendInterval(animationDelay / 2f);
            shakeSequence.Append(transform.DOLocalRotate(new Vector3(0, 0, 20), animationDelay / 4f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine));
            shakeSequence.Join(transform.DOPunchScale(new Vector3(0.25f, 0.25f, 0.25f), animationDelay / 2f));
            shakeSequence.Append(transform.DOLocalRotate(new Vector3(0, 0, -20), animationDelay / 4f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine))
                .SetAutoKill(false);

            OpenUnlockNotificationUI(uiColor);
            yield return new WaitForSeconds(duration);

            shakeSequence.Play();
            yield return new WaitForSeconds(animationDelay + interval);

            shakeSequence.Restart();
            yield return new WaitForSeconds(animationDelay + interval);

            shakeSequence.Restart();
            yield return new WaitForSeconds(animationDelay + interval);

            CloseUnlockNotificationUI();
            yield return new WaitForSeconds(duration);
        }

        isPlayingAnimation = false;
    }

    private void OpenUnlockNotificationUI(Color uiColor)
    {
        gameObject.SetActive(true);

        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            UIUtils.OpenUI(rectTransform, "X", windowsInsidePositionX, duration);
        }
        else
        {
            UIUtils.OpenUI(rectTransform, "X", mobileInsidePositionX, duration);
        }

        SetLayoutColor(uiColor);
    }

    private void CloseUnlockNotificationUI()
    {
        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            UIUtils.CloseUI(rectTransform, "X", windowsInsidePositionX, outsidePositionOffsetX, duration);
        }
        else
        {
            UIUtils.CloseUI(rectTransform, "X", mobileInsidePositionX, outsidePositionOffsetX, duration);
        }
    }

    private void SetLayoutColor(Color uiColor)
    {
        UIUtils.SetImageColorByScalar(layout, uiColor, 1f / 3f);
    }

    private Sequence GetShakeSequence()
    {
        return null;
    }
}

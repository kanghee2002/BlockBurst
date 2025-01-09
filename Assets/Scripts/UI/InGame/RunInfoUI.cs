using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RunInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject runInfoUI;
    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private GameUIManager gameUIManager;

    private const float centerPositionY = 0;
    private const float outsidePositionY = -1024;

    private const float duration = 0.2f;

    public void OpenRunInfoUI()
    {
        runInfoUI.SetActive(true);
        rectTransform.DOAnchorPosY(centerPositionY, duration)
            .SetEase(Ease.OutSine);
    }

    public void CloseRunInfoUI()
    {
        Debug.Log("RunInfoUI�� ȭ�� �߾ӿ��� �Ʒ��� ������");
        rectTransform.DOAnchorPosY(outsidePositionY, duration)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
             {
                 runInfoUI.SetActive(false);
             });
    }

    public void OnRunInfoBackButtonUIPressed()
    {
        gameUIManager.RunInfoBackButtonUIPressed();
    }
}

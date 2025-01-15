using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckInfoUI : MonoBehaviour
{
    public TextMeshProUGUI deckInfoText;
    [SerializeField] private GameObject deckInfoUI;
    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private GameUIManager gameUIManager;

    private const float insidePositionX = 0;
    private const float outsidePositionX = 1920;
    private const float duration = 0.2f;

    public void OpenDeckInfoUI()
    {
        deckInfoUI.SetActive(true);
        rectTransform.DOAnchorPosX(insidePositionX, duration)
            .SetEase(Ease.OutCubic);
    }

    public void CloseDeckInfoUI()
    {
        rectTransform.DOAnchorPosX(outsidePositionX, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                deckInfoUI.SetActive(false);
            });
    }

    public void OnDeckInfoBackButtonUIPressed()
    {
        gameUIManager.DeckInfoBackButtonUIPressed();
    }

    public void Initialize(RunData runData)
    {
        List<BlockData> availableBlocks = runData.availableBlocks;
        string text = "";
        foreach (BlockData blockData in availableBlocks)
        {
            text += blockData.type.ToString() + "\n";
        }
        deckInfoText.text = text;
    }
}

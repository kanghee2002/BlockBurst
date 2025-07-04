using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckSelectionBoardUI : MonoBehaviour
{
    [SerializeField] private PopupBlurImage popupBlurImage;

    [SerializeField] private TextMeshProUGUI selectedDeckText;
    [SerializeField] private TextMeshProUGUI selectedLevelText;

    private RectTransform rectTransform;

    private const float insidePositionX = 0;
    private const float outsidePositionOffsetX = 1000;
    private const float duration = 0.2f;

    private const int maxLevel = 5;

    private DeckType selectedDeckType;
    private int selectedLevel;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize()
    {
        selectedDeckType = DeckType.Default;
        selectedLevel = 0;

        SetDeckDescription();
        SetLevelDescription();
    }

    public void OnDeckSelectionUpButtonUIPressed()
    {
        int deckIndex = (int)selectedDeckType;
        int deckCount = Enums.GetEnumArray<DeckType>().Length;

        if (deckIndex >= deckCount - 1)
        {
            return;
        }

        deckIndex++;
        selectedDeckType = (DeckType)deckIndex;

        SetDeckDescription();
    }

    public void OnDeckSelectionDownButtonUIPressed()
    {
        int deckIndex = (int)selectedDeckType;

        if (deckIndex <= 0)
        {
            return;
        }

        deckIndex--;
        selectedDeckType = (DeckType)deckIndex;

        SetDeckDescription();
    }

    public void OnLevelUpButtonUIPressed()
    {
        if (selectedLevel >= 5)
        {
            return;
        }

        selectedLevel++;

        SetLevelDescription();
    }

    public void OnLevelDownButtonUIPressed()
    {
        if (selectedLevel <= 0)
        {
            return;
        }

        selectedLevel--;

        SetLevelDescription();
    }

    public void OnPlayButtonUIPressed()
    {
        GameUIManager.instance.OnPlayButtonUIPressed(selectedDeckType, selectedLevel);
    }

    private void SetDeckDescription()
    {
        selectedDeckText.text = "덱: " + selectedDeckType.ToString();
    }

    private void SetLevelDescription()
    {
        selectedLevelText.text = "레벨: " + selectedLevel.ToString();
    }

    public void OpenDeckSelectionBoardUI()
    {
        gameObject.SetActive(true);
        popupBlurImage.OpenPopupBlurImage(new Color(0.0f, 0.0f, 0.0f, 0.9f));
        UIUtils.OpenUI(rectTransform, "Y", insidePositionX, duration);
    }

    public void CloseDeckSelectionBoardUI()
    {
        popupBlurImage.ClosePopupBlurImage();
        UIUtils.CloseUI(rectTransform, "Y", insidePositionX, outsidePositionOffsetX, duration);
    }
}

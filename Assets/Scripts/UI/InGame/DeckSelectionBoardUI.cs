using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckSelectionBoardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI selectedDeckText;

    private RectTransform rectTransform;

    private const float insidePositionX = 0;
    private const float outsidePositionOffsetX = -500;
    private const float duration = 0.2f;


    private DeckType selectedDeckType;
    private int selectedLevel;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // TEST
        Initialize();
    }

    public void Initialize()
    {
        selectedDeckType = DeckType.Default;
        selectedLevel = 0;

        SetDeckDescription();
        SetLevelDescription();
    }

    public void OnDeckSelectionUpButtonPressed()
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

    public void OnDeckSelectionDownButtonPressed()
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

    public void OnLevelUpButtonPressed()
    {

    }

    public void OnLevelDownButtonPressed()
    {

    }

    public void OnPlayButtonPressed()
    {
        GameManager.instance.StartNewGame(selectedDeckType, selectedLevel);
    }

    private void SetDeckDescription()
    {
        selectedDeckText.text = selectedDeckType.ToString();
    }

    private void SetLevelDescription()
    {

    }

    public void OpenDeckSelectionBoardUI()
    {
        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "X", insidePositionX, duration);
    }

    public void CloseDeckSelectionBoardUI()
    {
        UIUtils.CloseUI(rectTransform, "X", insidePositionX, outsidePositionOffsetX, duration);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckSelectionBoardUI : MonoBehaviour
{
    [SerializeField] private PopupBlurImage popupBlurImage;

    [SerializeField] private Image deckImage;
    [SerializeField] private TextMeshProUGUI selectedDeckText;
    [SerializeField] private TextMeshProUGUI deckDescriptionText;
    [SerializeField] private TextMeshProUGUI selectedLevelText;
    [SerializeField] private TextMeshProUGUI levelDescriptionText;
    [SerializeField] private TextMeshProUGUI levelDuplicatedApplyText;

    private struct DeckDescription
    {
        public DeckType deckType;
        public string deckName;
        public string description;
    }

    private struct LevelDescription
    {
        public int level;
        public string description;
    }

    private RectTransform rectTransform;

    private const float insidePositionX = 0;
    private const float outsidePositionOffsetX = 1000;
    private const float duration = 0.2f;

    private const int maxLevel = 5;

    private List<DeckDescription> deckDescriptions;
    private List<LevelDescription> levelDescriptions;

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

    public void InitializeDeckLevelInfo(DeckData[] deckTemplates, LevelData[] levelTemplates)
    {
        deckDescriptions = new();
        foreach (DeckData deck in deckTemplates)
        {
            DeckDescription deckDescription = new();
            deckDescription.deckType = deck.type;
            deckDescription.deckName = deck.deckName;
            deckDescription.description = deck.description;

            deckDescriptions.Add(deckDescription);
        }

        levelDescriptions = new();
        foreach (LevelData level in levelTemplates)
        {
            LevelDescription levelDescription = new();
            levelDescription.level = level.level;
            levelDescription.description = level.description;

            levelDescriptions.Add(levelDescription);
        }
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
        DeckDescription currentDeck = deckDescriptions.Find(deck => deck.deckType == selectedDeckType);

        selectedDeckText.text = currentDeck.deckName;
        deckDescriptionText.text = currentDeck.description;

        SetDeckImage();
    }

    private void SetLevelDescription()
    {
        LevelDescription currentLevel = levelDescriptions.Find(level => level.level == selectedLevel);

        selectedLevelText.text = "레벨 " + currentLevel.level.ToString();
        levelDescriptionText.text = currentLevel.description;

        if (currentLevel.level > 0)
        {
            levelDuplicatedApplyText.gameObject.SetActive(true);
        }
        else
        {
            levelDuplicatedApplyText.gameObject.SetActive(false);
        }
    }

    private void SetDeckImage()
    {
        string path = "Sprites/Deck/" + selectedDeckType.ToString();

        Sprite deckSprite = Resources.Load<Sprite>(path);

        deckImage.sprite = deckSprite;
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

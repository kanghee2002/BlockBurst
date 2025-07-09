using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckSelectionBoardUI : MonoBehaviour
{
    [SerializeField] private PopupBlurImage popupBlurImage;

    [SerializeField] private Transform deckContainer;
    [SerializeField] private ButtonUI playButtonUI;

    [SerializeField] private DeckDescriptionUI deckDescriptionUI;

    private RectTransform rectTransform;

    private const float insidePositionX = 0;
    private const float outsidePositionOffsetX = 1000;
    private const float duration = 0.2f;

    private Color canPlayColor;
    private Color cantPlayColor;

    private List<DeckDescriptionUI> deckDescriptions;

    private int selectedDeckIndex;
    private int selectedLevel;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canPlayColor = new Color(0.35f, 0.89f, 0.25f);
        cantPlayColor = new Color(0.35f, 0.35f, 0.35f);
    }

    // 첫 번째 호출
    public void InitializeDeckLevelInfo(DeckData[] deckTemplates, LevelData[] levelTemplates)
    {
        if (deckContainer.childCount <= 0)
        {
            deckDescriptions = new();
        }

        DeckData[] orderedDeckTemplates = deckTemplates.OrderBy(deck => (int)deck.type).ToArray();
        LevelData[] orderedLevelTemplates = levelTemplates.OrderBy(level => level.level).ToArray();
        foreach (DeckData deck in orderedDeckTemplates)
        {
            DeckDescriptionUI deckUI;
            
            if (deckDescriptions.Count < orderedDeckTemplates.Length)
            {
                deckUI = Instantiate(deckDescriptionUI, deckContainer);
                deckDescriptions.Add(deckUI);
            }
            else
            {
                deckUI = deckDescriptions.Find(x => x.deckType == deck.type);
            }

            deckUI.transform.localPosition = Vector2.zero;
            deckUI.Initialize(deck);

            foreach (LevelData level in orderedLevelTemplates)
            {
                deckUI.InitializeLevel(level, orderedLevelTemplates.Length);
            }

        }
    }

    // 두 번째 호출
    public void InitializePlayerData(PlayerData playerData)
    {
        for (int i = 0; i < deckDescriptions.Count; i++)
        {
            int index = playerData.unlockedDecks.FindIndex(deckInfo => i == (int)deckInfo.deckType);

            if (index != -1)
            {
                deckDescriptions[i].SetUnlock(true, playerData.unlockedDecks[index].level);
            }
            else
            {
                deckDescriptions[i].SetUnlock(false, -1);
            }
        }

        string data = "Start: ";

        foreach (var x in playerData.unlockedDecks)
        {
            data += x.deckName + " " + x.level + "\n";
        }

        GameManager.instance.TEST_TEXT(data);
    }

    // 세 번째 호출
    public void InitializeDeckUnlockInfo(UnlockInfo[] unlockInfoTemplates)
    {
        foreach (DeckDescriptionUI deckUI in deckDescriptions)
        {
            DeckType deckType = deckUI.deckType;

            UnlockInfo unlockInfo = unlockInfoTemplates.FirstOrDefault(x => x.targetName == deckType.ToString());

            if (unlockInfo == null)
            {
                continue;
            }

            deckUI.SetUnlockDescription(unlockInfo.GetDescription());
        }
    }

    public void Initialize()
    {
        selectedDeckIndex = 0;
        selectedLevel = 0;

        SetActiveLevel(0, 0);
    }

    private void SetActiveDeck(int index)
    {
        for (int i = 0; i < deckDescriptions.Count; i++)
        {
            DeckDescriptionUI deckUI = deckDescriptions[i];
            if (i == index)
            {
                deckUI.gameObject.SetActive(true);
            }
            else
            {
                deckUI.gameObject.SetActive(false);
            }
        }
    }

    private void SetActiveLevel(int deckIndex, int levelIndex)
    {
        for (int i = 0; i < deckDescriptions.Count; i++)
        {
            if (i == deckIndex)
            {
                deckDescriptions[i].SetActiveLevel(levelIndex);
            }
        }
        SetActiveDeck(deckIndex);
    }

    private bool CanPlay()
    {
        return deckDescriptions[selectedDeckIndex].CanPlay(selectedLevel);
    }

    private void SetPlayButtonColor()
    {
        if (CanPlay())
        {
            playButtonUI.SetUIColor(canPlayColor);
        }
        else
        {
            playButtonUI.SetUIColor(cantPlayColor);
        }
    }

    public void OnDeckSelectionUpButtonUIPressed()
    {
        int deckCount = Enums.GetEnumArray<DeckType>().Length;

        if (selectedDeckIndex >= deckCount - 1)
        {
            return;
        }

        selectedDeckIndex++;
        selectedLevel = 0;

        SetActiveLevel(selectedDeckIndex, selectedLevel);

        SetPlayButtonColor();
    }

    public void OnDeckSelectionDownButtonUIPressed()
    {
        if (selectedDeckIndex <= 0)
        {
            return;
        }

        selectedDeckIndex--;
        selectedLevel = 0;

        SetActiveLevel(selectedDeckIndex, selectedLevel);

        SetPlayButtonColor();
    }

    public void OnLevelUpButtonUIPressed()
    {
        if (selectedLevel >= 5)
        {
            return;
        }

        selectedLevel++;

        SetActiveLevel(selectedDeckIndex, selectedLevel);

        SetPlayButtonColor();
    }

    public void OnLevelDownButtonUIPressed()
    {
        if (selectedLevel <= 0)
        {
            return;
        }

        selectedLevel--;

        SetActiveLevel(selectedDeckIndex, selectedLevel);

        SetPlayButtonColor();
    }

    public void OnPlayButtonUIPressed()
    {
        if (CanPlay())
        {
            GameUIManager.instance.OnPlayButtonUIPressed((DeckType)selectedDeckIndex, selectedLevel);
        }
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

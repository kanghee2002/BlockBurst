using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckDescriptionUI : MonoBehaviour
{
    [SerializeField] private LevelDescriptionUI levelDescriptionUI;

    [SerializeField] private Image deckImage;
    [SerializeField] private TextMeshProUGUI selectedDeckText;
    [SerializeField] private TextMeshProUGUI deckDescriptionText;

    [SerializeField] private Transform levelConatiner;

    private List<LevelDescriptionUI> levelDescriptions;

    public DeckType deckType { get; private set; }
    private bool isUnlocked;

    private void OnEnable()
    {
        // Level에 따라 SetActiveLevel();
    }

    public void Initialize(DeckData deckData)
    {
        deckImage.sprite = GetDeckImage(deckData.type);

        selectedDeckText.text = deckData.deckName;

        deckDescriptionText.text = deckData.description;

        if (levelConatiner.childCount <= 0)
        {
            levelDescriptions = new();
        }

        deckType = deckData.type;
    }

    public void InitializeLevel(LevelData levelData, int levelCount)
    {
        LevelDescriptionUI levelUI;
        
        if (levelDescriptions.Count < levelCount)
        {
            levelUI = Instantiate(levelDescriptionUI, levelConatiner);
            levelDescriptions.Add(levelUI);
        }
        else
        {
            levelUI = levelDescriptions.Find(x => x.level == levelData.level);
        }
        levelUI.Initialize(levelData);
    }

    public void SetUnlock(bool isUnlocked, int unlockedLevel)
    {
        this.isUnlocked = isUnlocked;

        if (isUnlocked)
        {
            for (int i = 0; i < levelDescriptions.Count; i++)
            {
                if (i > unlockedLevel)
                {
                    levelDescriptions[i].SetLock();
                }
            }
        }
        else
        {
            deckImage.sprite = GetLockImage();
        }
    }

    public void SetUnlockDescription(string description)
    {
        if (isUnlocked)
        {
            return;
        }

        for (int i = 0; i < levelDescriptions.Count; i++)
        {
            levelDescriptions[i].SetDeckLock(description);
        }
    }

    public void SetActiveLevel(int index)
    {
        for (int i = 0; i < levelConatiner.childCount; i++)
        {
            Transform deckUI = levelConatiner.GetChild(i);
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

    public bool CanPlay(int level)
    {
        if (!isUnlocked)
        {
            return false;
        }
        else
        {
            return levelDescriptions[level].CanPlayLevel();
        }
    }

    private Sprite GetDeckImage(DeckType deckType)
    {
        string path = "Sprites/Deck/" + deckType.ToString();

        Sprite deckSprite = Resources.Load<Sprite>(path);

        return deckSprite;
    }

    private Sprite GetLockImage()
    {
        string path = "Sprites/UI/LockDeck";

        Sprite lockSprite = Resources.Load<Sprite>(path);

        return lockSprite;
    }
}

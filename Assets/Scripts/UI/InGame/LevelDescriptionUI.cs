using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelDescriptionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI selectedLevelText;
    [SerializeField] private TextMeshProUGUI levelDescriptionText;
    [SerializeField] private TextMeshProUGUI levelDuplicatedApplyText;

    public int level { get; private set; }

    private bool isUnlocked;

    public void Initialize(LevelData levelData)
    {
        selectedLevelText.text = "레벨 " + levelData.level;

        levelDescriptionText.text = levelData.description;

        if (levelData.level >= 2)
        {
            levelDuplicatedApplyText.gameObject.SetActive(true);
        }
        else
        {
            levelDuplicatedApplyText.gameObject.SetActive(false);
        }

        level = levelData.level;

        isUnlocked = true;
    }

    public void SetLock()
    {
        levelDescriptionText.text = "이전 레벨을 클리어하세요";

        levelDuplicatedApplyText.gameObject.SetActive(false);

        isUnlocked = false;
    }

    public void SetDeckLock(string description)
    {
        selectedLevelText.text = "덱 잠김";

        levelDescriptionText.text = description;

        levelDuplicatedApplyText.gameObject.SetActive(false);
    }

    public bool CanPlayLevel()
    {
        return isUnlocked;
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deckInfoText1;
    [SerializeField] private TextMeshProUGUI deckInfoText2;
    private RectTransform rectTransform;

    private const float insidePositionX = 0;
    private const float outsidePositionX = 1920;
    private const float duration = 0.2f;

    [SerializeField] private GameObject basicContainer;
    [SerializeField] private GameObject specialContainer;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(RunData runData)
    {
        List<BlockData> availableBlocks = runData.availableBlocks;
        Dictionary<BlockType, int> defaultCountDictionary = new Dictionary<BlockType, int>();
        Dictionary<BlockType, int> specialCountDictionary = new Dictionary<BlockType, int>();
        
        foreach (BlockData block in availableBlocks)
        {
            if (Enums.IsDefaultBlockType(block.type))
            {
                if (!defaultCountDictionary.ContainsKey(block.type))
                {
                    defaultCountDictionary.Add(block.type, 0);
                }
                defaultCountDictionary[block.type]++;
            }
            else
            {
                if (!specialCountDictionary.ContainsKey(block.type))
                {
                    specialCountDictionary.Add(block.type, 0);
                }
                specialCountDictionary[block.type]++;
            }
        }

        string text1 = "\t개수\t점수\n";
        foreach ((BlockType blockType, int count)  in defaultCountDictionary)
        {
            if (defaultCountDictionary[blockType] == 0)
            {
                continue;
            } 
            text1 += blockType.ToString() + " 블록\t";
            text1 += "  " + count + "\t";
            text1 += " " + runData.baseBlockScores[blockType] + "\t";
            text1 += "\n";
        }
        deckInfoText1.text = text1;

        string text2 = "\t\t개수\t점수\n";
        foreach ((BlockType blockType, int count) in specialCountDictionary)
        {
            if (specialCountDictionary[blockType] == 0)
            {
                continue;
            }
            if (blockType == BlockType.CORNER) text2 += "CORNER 블록\t";
            else text2 += blockType.ToString() + " 블록\t";
            text2 += "  " + count + "\t";
            text2 += " " + runData.baseBlockScores[blockType] + "\t";
            text2 += "\n";
        }
        text2 += "\n모든 블록 수: " + availableBlocks.Count;
        deckInfoText2.text = text2;
    }

    public void OpenDeckInfoUI()
    {
        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "X", insidePositionX, duration);
    }

    public void CloseDeckInfoUI()
    {
        UIUtils.CloseUI(rectTransform, "X", insidePositionX, outsidePositionX, duration);
    }

    public void OnBasicSwitchButtonUIClick()
    {
        basicContainer.SetActive(true);
        specialContainer.SetActive(false);
    }
    public void OnSpecialSwitchButtonUIClick()
    {
        specialContainer.SetActive(true);
        basicContainer.SetActive(false);
    }
}

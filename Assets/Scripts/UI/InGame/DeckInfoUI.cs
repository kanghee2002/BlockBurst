using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DeckInfoUI : MonoBehaviour
{
    private RectTransform rectTransform;

    private const float insidePositionX = 0;
    private const float outsidePositionX = 1920;
    private const float duration = 0.2f;

    [SerializeField] private GameObject basicContainer;
    [SerializeField] private GameObject specialContainer;
    
    [SerializeField] private GameObject BlockPrefab;

    public Transform[] BlockTransforms;

    const float ROW_OFFSET = 112;
    const float ROW_REF = 112 * 3;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // basicContainer의 자식들을 BlockTransforms에 저장
        BlockTransforms = new Transform[Enum.GetNames(typeof(BlockType)).Length];
        for (int i = 0; i < BlockTransforms.Length; i++)
        {
            if (Enums.IsDefaultBlockType((BlockType)i))
            {
                GameObject newObject = Instantiate(BlockPrefab, basicContainer.transform);
                BlockTransforms[i] = newObject.transform;
            }
            else
            {
                GameObject newObject = Instantiate(BlockPrefab, specialContainer.transform);
                BlockTransforms[i] = newObject.transform;
            }
        }
    }

    public void Initialize(RunData runData, BlockGameData blockGameData, bool isPlaying)
    {
        
        Dictionary<BlockType, int> scoreDictionary = isPlaying ? blockGameData.blockScores : runData.baseBlockScores;
        List<BlockData> availableBlocks = isPlaying ? blockGameData.deck : runData.availableBlocks;
        Dictionary<BlockType, int> countDictionary = new Dictionary<BlockType, int>();
        
        for (int i = 0; i < BlockTransforms.Length; i++)
        {
            countDictionary.Add((BlockType)i, 0);
        }

        foreach (BlockData block in availableBlocks)
        {
            countDictionary[block.type]++;
        }

        int basicPos = 0;
        int specialPos = 0;

        for (int i = 0; i < BlockTransforms.Length; i++)
        {
            if (Enums.IsDefaultBlockType((BlockType)i))
            {
                InitializeBlockUI(BlockTransforms[i], (BlockType)i, countDictionary[(BlockType)i], scoreDictionary[(BlockType)i]);
                BlockTransforms[i].localPosition = new Vector3(0, ROW_REF - ROW_OFFSET * basicPos, 0);
                basicPos++;
            }
            else
            {
                if (countDictionary[(BlockType)i] == 0)
                {
                    // 투명하게
                    BlockTransforms[i].GetComponent<CanvasGroup>().alpha = 0f;
                }
                else
                {
                    BlockTransforms[i].GetComponent<CanvasGroup>().alpha = 1f;
                    InitializeBlockUI(BlockTransforms[i], (BlockType)i, countDictionary[(BlockType)i], scoreDictionary[(BlockType)i]);
                    BlockTransforms[i].localPosition = new Vector3(0, ROW_REF - ROW_OFFSET * specialPos, 0);
                    specialPos++;
                }
            }
        }
    }

    void InitializeBlockUI(Transform blockTransform, BlockType blockType, int count, int score)
    {
        TextMeshProUGUI blockTypeText = blockTransform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI blockCountText = blockTransform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI blockScoreText = blockTransform.GetChild(1).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
        Image blockImage = blockTransform.GetChild(1).GetChild(1).GetComponent<Image>();

        blockTypeText.text = blockType.ToString();
        blockCountText.text = count.ToString() + "개";
        blockScoreText.text = score.ToString() + "점";
        blockImage.sprite = Resources.Load<Sprite>("Sprites/Block/Preset/" + blockType.ToString());

        if (!Enums.IsDefaultBlockType(blockType))
        {
            blockTypeText.fontSize = 32;    // 특수 블록은 글자 크기를 줄인다.
        }
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

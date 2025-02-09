using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

public class DeckInfoUI : MonoBehaviour
{
    [SerializeField] private PopupBlurImage popupBlurImage;

    private RectTransform rectTransform;

    private const float insidePositionX = 0;
    private const float outsidePositionX = 1920;
    private const float duration = 0.2f;

    [SerializeField] private GameObject basicContainer;
    [SerializeField] private GameObject specialContainer;
    private CanvasGroup basicCanvasGroup;
    private CanvasGroup specialCanvasGroup;
    
    [SerializeField] private GameObject BlockPrefab;
    [SerializeField] private GameObject EffectPrefab;

    public Transform[] BlockTransforms;

    const float ROW_OFFSET = 112;
    const float COLUMN_OFFSET = 112;
    const float ROW_REF = 112 * 3;

    Dictionary<string, string> effectNames = new Dictionary<string, string>() {
        {"GOLD_MODIFIER", "GoldUpgrade"},
        {"MULTIPLIER_MODIFIER", "MultiplierUpgrade"},
        {"REROLL_MODIFIER", "RerollUpgrade"},
        {"SCORE_MODIFIER", "ScoreUpgrade"},
    };

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        basicCanvasGroup = basicContainer.GetComponent<CanvasGroup>();
        specialCanvasGroup = specialContainer.GetComponent<CanvasGroup>();

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
        OnBasicSwitchButtonUIClick();
    }

    public void Initialize(RunData runData, BlockGameData blockGameData, bool isPlaying)
    {
        gameObject.SetActive(true);
        Dictionary<BlockType, int> scoreDictionary = isPlaying ? blockGameData.blockScores : runData.baseBlockScores;
        List<BlockData> availableBlocks = isPlaying ? blockGameData.deck : runData.availableBlocks;
        Dictionary<BlockType, int> countDictionary = new Dictionary<BlockType, int>();

        foreach (BlockData block in runData.availableBlocks)
        {
            if (!countDictionary.ContainsKey(block.type))
            {
                countDictionary[block.type] = 0;
            }
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
                if (!countDictionary.ContainsKey((BlockType)i))
                {
                    countDictionary[(BlockType)i] = 0;
                }
                InitializeBlockUI(BlockTransforms[i], (BlockType)i, countDictionary[(BlockType)i], scoreDictionary[(BlockType)i]);
                BlockTransforms[i].localPosition = new Vector3(0, ROW_REF - ROW_OFFSET * basicPos, 0);
                basicPos++;
            }
            else
            {
                if (!countDictionary.ContainsKey((BlockType)i))
                {
                    // 투명하게
                    BlockTransforms[i].GetComponent<CanvasGroup>().alpha = 0f;
                }
                else
                {
                    BlockTransforms[i].GetComponent<CanvasGroup>().alpha = 1f;
                    InitializeBlockUI(BlockTransforms[i], (BlockType)i, countDictionary[(BlockType)i], scoreDictionary[(BlockType)i]);
                    BlockTransforms[i].localPosition = new Vector3(0, ROW_REF - ROW_OFFSET * specialPos, 0);
                    Debug.Log("specialPos: " + specialPos);
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
        Transform effectContainer = blockTransform.GetChild(2);

        blockTypeText.text = blockType.ToString();
        blockCountText.text = count.ToString() + "개";
        blockScoreText.text = score.ToString() + "점";
        blockImage.sprite = Resources.Load<Sprite>("Sprites/Block/Preset/" + blockType.ToString());

        if (!Enums.IsDefaultBlockType(blockType))
        {
            blockTypeText.fontSize = 32;
            TextMeshProUGUI effectText = effectContainer.GetChild(0).GetComponent<TextMeshProUGUI>();
            effectText.text = "";
            List<EffectData> specialEffects = Resources.Load<ItemData>("ScriptableObjects/Item/Block/AddBlock" + UIUtils.ToCamelCase(blockType.ToString())).effects;
            foreach (EffectData effect in specialEffects)
            {
                effectText.text += UIUtils.SetBlockNameToIcon(effect.effectName.Replace("\n", " "));
            }
        }
        else
        {
            // 기존 이펙트 UI 모두 제거
            foreach (Transform child in effectContainer)
            {
                Destroy(child.gameObject);
            }
            Dictionary<string, int> effects = CellEffectManager.instance.blockEffects[(int)blockType];
            if (effects != null && effects.Count > 0)
            {
                // effectNames의 순서대로 고정된 위치에 이펙트 생성
                foreach (var effectPair in effectNames)
                {
                    string effectType = effectPair.Key;
                    if (effects.ContainsKey(effectType))
                    {
                        // 이펙트 아이콘 생성
                        Sprite effectSprite = Resources.Load<Sprite>("Sprites/Item/Upgrade/" + effectPair.Value);
                        GameObject effectObject = Instantiate(EffectPrefab, effectContainer);
                        effectObject.transform.GetChild(0).GetComponent<Image>().sprite = effectSprite;

                        // 개수 표시 Text 생성
                        TextMeshProUGUI effectCountText = effectObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                        effectCountText.text = effects[effectType].ToString();
                        // 색깔 설정
                        effectCountText.color = Color.white;

                        // effectNames에서의 인덱스를 찾아서 위치 설정
                        int effectIndex = Array.IndexOf(effectNames.Keys.ToArray(), effectType);
                        float xOffset = COLUMN_OFFSET * effectIndex;
                        effectObject.transform.localPosition = new Vector3(xOffset, 0, 0);
                    }
                }
            }
        }
    }

    public void OpenDeckInfoUI()
    {
        popupBlurImage.OpenPopupBlurImage(new Color(0.0f, 0.0f, 0.0f, 0.9f));
        UIUtils.OpenUI(rectTransform, "X", insidePositionX, duration);
    }

    public void CloseDeckInfoUI()
    {
        popupBlurImage.ClosePopupBlurImage();
        UIUtils.CloseUI(rectTransform, "X", insidePositionX, outsidePositionX, duration);
    }

    public void OnBasicSwitchButtonUIClick()
    {
        basicCanvasGroup.alpha = 1f;
        basicCanvasGroup.interactable = true;
        basicCanvasGroup.blocksRaycasts = true;

        specialCanvasGroup.alpha = 0f;
        specialCanvasGroup.interactable = false;
        specialCanvasGroup.blocksRaycasts = false;
    }

    public void OnSpecialSwitchButtonUIClick()
    {
        specialCanvasGroup.alpha = 1f;
        specialCanvasGroup.interactable = true;
        specialCanvasGroup.blocksRaycasts = true;

        basicCanvasGroup.alpha = 0f;
        basicCanvasGroup.interactable = false;
        basicCanvasGroup.blocksRaycasts = false;
    }
}

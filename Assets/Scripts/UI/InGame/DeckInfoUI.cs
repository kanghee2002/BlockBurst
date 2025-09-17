using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.EventSystems;

public class DeckInfoUI : MonoBehaviour
{
    [SerializeField] private PopupBlurImage popupBlurImage;

    private RectTransform rectTransform;

    private const float insidePositionX = -224.5f;
    private const float outsidePositionX = -1118;
    private const float duration = 0.2f;

    [Header("Layout")]
    [SerializeField] private Image outerLayout;
    [SerializeField] private Image innerLayout;

    [Header("SwitchButton")]
    [SerializeField] private List<ButtonUI> switchButtonUIList;

    [Header("Container")]
    [SerializeField] private GameObject basicContainer;
    [SerializeField] private GameObject specialContainer;
    private CanvasGroup basicCanvasGroup;
    private CanvasGroup specialCanvasGroup;
    
    [SerializeField] private GameObject BlockPrefab;
    [SerializeField] private GameObject EffectPrefab;

    public Transform[] BlockTransforms;

    [Header("BoostInfo")]
    [SerializeField] private GameObject boostInfoContainer;
    [SerializeField] private Transform boostInfoGridLayout;
    [SerializeField] private GameObject boostIcon;

    public List<TextMeshProUGUI> texts;

    const float WINDOWS_ROW_OFFSET = 112;
    public float MOBILE_ROW_OFFSET = 80;
    const float WINDOWS_COLUMN_OFFSET = 112;
    const float MOBILE_COLUMN_OFFSET = 80;
    const float ROW_REF = 290;

    Dictionary<string, string> effectNames = new Dictionary<string, string>() {
        {"GOLD_MODIFIER", "GoldUpgrade"},
        {"MULTIPLIER_MODIFIER", "MultiplierUpgrade"},
        {"REROLL_MODIFIER", "RerollUpgrade"},
        //{"SCORE_MODIFIER", "ScoreUpgrade"},
    };

    [HideInInspector] public bool isShowingBoostDetail;

    private enum CurrentTab
    {
        Basic, Special, Boost
    };

    private CurrentTab currentTab;
    private Color currentUIColor;

    private Color currentUITextColor;

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
                if (GameManager.instance.applicationType == ApplicationType.Windows)
                {
                    BlockTransforms[i].localPosition = new Vector3(0, ROW_REF - WINDOWS_ROW_OFFSET * basicPos, 0);
                }
                else
                {
                    BlockTransforms[i].localPosition = new Vector3(0, ROW_REF - MOBILE_ROW_OFFSET * basicPos, 0);
                }
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
                    if (GameManager.instance.applicationType == ApplicationType.Windows)
                    {
                        BlockTransforms[i].localPosition = new Vector3(0, ROW_REF - WINDOWS_ROW_OFFSET * specialPos, 0);
                    }
                    else
                    {
                        BlockTransforms[i].localPosition = new Vector3(0, ROW_REF - MOBILE_ROW_OFFSET * specialPos, 0);
                    }
                    Debug.Log("specialPos: " + specialPos);
                    specialPos++;
                }
            }
        }

        // Initialize BoostInfo
        for (int i = boostInfoGridLayout.childCount - 1; i >= 0; i--)
        {
            Destroy(boostInfoGridLayout.GetChild(i).gameObject);
        }

        for (int i = 0; i < runData.activeBoosts.Count; i++)
        {
            ItemData boostData = runData.activeBoosts[i];

            GameObject boost = Instantiate(boostIcon, boostInfoGridLayout);
            boost.GetComponent<Image>().sprite = GetImage(boostData);
            //boost.GetComponent<ItemDescriptionUI>().Initialize(boostData);

            // Event
            EventTrigger trigger = boost.AddComponent<EventTrigger>();
            
            int index = i;
            EventTrigger.Entry clickEntry = new EventTrigger.Entry();
            clickEntry.eventID = EventTriggerType.PointerClick;
            clickEntry.callback.AddListener((data) => {
                isShowingBoostDetail = true;
                GameUIManager.instance.OnDeckInfoBoostUIPressed(index);
            });
            trigger.triggers.Add(clickEntry); 
        }
    }
    
    void InitializeBlockUI(Transform blockTransform, BlockType blockType, int count, int score)
    {
        TextMeshProUGUI blockTypeText = blockTransform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI blockCountText = blockTransform.GetChild(2).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI blockScoreText = blockTransform.GetChild(2).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI blockStateText = blockTransform.GetChild(1).GetComponent<TextMeshProUGUI>();
        Image blockImage = blockTransform.GetChild(2).GetChild(1).GetComponent<Image>();
        Transform effectContainer = blockTransform.GetChild(3);

        blockTypeText.text = blockType.ToString();
        blockCountText.text = count.ToString() + "개";
        blockScoreText.text = score.ToString() + "점";
        blockStateText.text = count.ToString() + "개 • " + score.ToString() + "점";
        blockImage.sprite = Resources.Load<Sprite>("Sprites/Block/Preset/" + blockType.ToString());

        if (!Enums.IsDefaultBlockType(blockType))
        {
            blockTypeText.fontSize = 32;
            TextMeshProUGUI effectText = effectContainer.GetChild(0).GetComponent<TextMeshProUGUI>();
            effectText.text = "";
            List<EffectData> specialEffects = Resources.Load<ItemData>("ScriptableObjects/Item/Block/AddBlock" + UIUtils.ToCamelCase(blockType.ToString())).effects;
            foreach (EffectData effect in specialEffects)
            {
                string tmp = UIUtils.SetBlockNameToIcon(effect.effectName.Replace("\n", " "));
                string tmp2 = UIUtils.GetEffectValueText(tmp, effect);
                string result = UIUtils.GetValueText(tmp2, effect);
                effectText.text += result;
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
                        float xOffset;
                        if (GameManager.instance.applicationType == ApplicationType.Windows)
                        {
                            xOffset = WINDOWS_COLUMN_OFFSET * effectIndex;
                        }
                        else
                        {
                            xOffset = MOBILE_COLUMN_OFFSET * effectIndex;
                        }
                        effectObject.transform.localPosition = new Vector3(xOffset, 0, 0);
                    }
                }
            }
        }
    }

    public void OpenDeckInfoUI()
    {
        popupBlurImage.OpenPopupBlurImage(new Color(0.0f, 0.0f, 0.0f, 0.9f));
        UIUtils.OpenUI(rectTransform, "Y", insidePositionX, duration);
    }

    public void CloseDeckInfoUI()
    {
        popupBlurImage.ClosePopupBlurImage();
        UIUtils.CloseUI(rectTransform, "Y", insidePositionX, outsidePositionX, duration);
    }

    public void OnBasicSwitchButtonUIClick()
    {
        basicCanvasGroup.alpha = 1f;
        basicCanvasGroup.interactable = true;
        basicCanvasGroup.blocksRaycasts = true;

        specialCanvasGroup.alpha = 0f;
        specialCanvasGroup.interactable = false;
        specialCanvasGroup.blocksRaycasts = false;

        boostInfoContainer.SetActive(false);

        currentTab = CurrentTab.Basic;

        SetSwitchButtonUIColor(CurrentTab.Basic, currentUIColor);
    }

    public void OnSpecialSwitchButtonUIClick()
    {
        specialCanvasGroup.alpha = 1f;
        specialCanvasGroup.interactable = true;
        specialCanvasGroup.blocksRaycasts = true;

        basicCanvasGroup.alpha = 0f;
        basicCanvasGroup.interactable = false;
        basicCanvasGroup.blocksRaycasts = false;

        boostInfoContainer.SetActive(false);

        currentTab = CurrentTab.Special;

        SetSwitchButtonUIColor(CurrentTab.Special, currentUIColor);
    }

    public void OnBoostInfoSwitchButtonUIClick()
    {
        specialCanvasGroup.alpha = 0f;
        specialCanvasGroup.interactable = false;
        specialCanvasGroup.blocksRaycasts = false;

        basicCanvasGroup.alpha = 0f;
        basicCanvasGroup.interactable = false;
        basicCanvasGroup.blocksRaycasts = false;

        boostInfoContainer.SetActive(true);

        currentTab = CurrentTab.Boost;

        SetSwitchButtonUIColor(CurrentTab.Boost, currentUIColor);
    }

    public void SetLayoutsColor(Color uiColor, Color textColor)
    {
        currentUIColor = uiColor;

        // 버튼
        SetSwitchButtonUIColor(currentTab, uiColor);

        // 기본 블록
        UIUtils.SetImageColorByScalar(outerLayout, uiColor, 3f / 5f, duration: 0.05f);
        UIUtils.SetImageColorByScalar(innerLayout, uiColor, 4f / 5f, duration: 0.05f);

        float layoutScalar = 1f;
        foreach (Transform child in basicContainer.transform)
        {
            UIUtils.SetImageColorByScalar(child.GetChild(0).GetComponent<Image>(), uiColor, layoutScalar, duration: 0.05f);
        }

        // 특수 블록
        foreach (Transform child in specialContainer.transform)
        {
            UIUtils.SetImageColorByScalar(child.GetChild(0).GetComponent<Image>(), uiColor, layoutScalar, duration: 0.05f);
        }

        // 부스트
        UIUtils.SetImageColorByScalar(boostInfoGridLayout.GetComponent<Image>(), uiColor, layoutScalar, duration: 0.05f);

        foreach (TextMeshProUGUI text in texts)
        {
            UIUtils.SetTextColorByScalar(text, textColor, 1f);
        }

        foreach (Transform BUI in BlockTransforms)
        {
            TextMeshProUGUI t = BUI.GetChild(1).GetComponent<TextMeshProUGUI>();
            UIUtils.SetTextColorByScalar(t, textColor, 1f);
        }
    }

    public void SetTextColor(Color textColor)
    {
        currentUITextColor = textColor;

        // 버튼


        // 기본 블록

        foreach (Transform BUI in BlockTransforms)
        {
            TextMeshProUGUI t = BUI.GetChild(1).GetComponent<TextMeshProUGUI>();
            UIUtils.SetTextColorByScalar(t, textColor, 1f);
        }
        
    }

    private void SetSwitchButtonUIColor(CurrentTab tab, Color uiColor)
    {
        for (int i = 0; i < switchButtonUIList.Count; i++)
        {
            ButtonUI button = switchButtonUIList[i];
            if (i == (int)tab)
            {
                button.SetUIColor(uiColor, scalar: 3f / 5f);
            }
            else
            {
                button.SetUIColor(uiColor, scalar: 2f / 5f);
            }
        }
    }

    private Sprite GetImage(ItemData item)
    {
        string itemPath = "Sprites/Item/Item/";
        return Resources.Load<Sprite>(itemPath + item.id);
    }
}

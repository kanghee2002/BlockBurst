using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class RunInfoUI : MonoBehaviour
{
    public TextMeshProUGUI runInfoText;
    [SerializeField] private GameObject runInfoUI;
    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private GameUIManager gameUIManager;

    private const float insidePositionY = 0;
    private const float outsidePositionY = -1080;
    private const float duration = 0.2f;

    public void OpenRunInfoUI()
    {
        runInfoUI.SetActive(true);
        rectTransform.DOAnchorPosY(insidePositionY, duration)
            .SetEase(Ease.OutCubic);
    }

    public void CloseRunInfoUI()
    {
        rectTransform.DOAnchorPosY(outsidePositionY, duration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
             {
                 runInfoUI.SetActive(false);
             });
    }

    public void OnRunInfoBackButtonUIPressed()
    {
        gameUIManager.RunInfoBackButtonUIPressed();
    }

/*
public class RunData
{
    // 스테이지 진행 데이터
    public Dictionary<BlockType, int> baseBlockScores;          // 기본 블록 점수
    public Dictionary<MatchType, int> baseMatchMultipliers;     // 기본 배수
    public List<BlockData> availableBlocks;                     // 사용 가능한 블록들
    public List<ItemData> activeItems;                          // 활성화된 아이템들
    public List<EffectData> activeEffects;                      // 활성화된 효과들
    public StageData currentStage;                              // 현재 스테이지
    public Dictionary<BlockType, int> blockReuses;              // 블록별 재사용 횟수
    public int gold;                                            // 현재 보유 골드
    public int baseRerollCount;                                 // 기본 리롤 횟수
    public int currentRerollCount;                              // 현재 리롤 횟수
    public int boardSize;                                       // 보드 크기

    public void Initialize(GameData gameData)
    {
        baseBlockScores = new Dictionary<BlockType, int>(gameData.defaultBlockScores);
        baseMatchMultipliers = new Dictionary<MatchType, int>(gameData.defaultMatchMultipliers);
        availableBlocks = new List<BlockData>(gameData.defaultBlocks);
        activeEffects = new List<EffectData>();
        blockReuses = new Dictionary<BlockType, int>();
        gold = gameData.startingGold;
        baseRerollCount = gameData.defaultRerollCount;
        currentRerollCount = baseRerollCount;
    }
}
*/
    public void Initialize(RunData runData)
    {
        string text = "";
        text += "Gold: " + runData.gold + "\n";
        text += "Reroll: " + runData.currentRerollCount + "/" + runData.baseRerollCount + "\n";
        runInfoText.text = text;
    }
}

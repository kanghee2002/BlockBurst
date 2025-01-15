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
        boardSize = 8;
    }
}

*/
    public void Initialize(RunData runData)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        
        // 기본 정보를 2개씩 탭으로 구분
        sb.AppendLine($"Gold: {runData.gold}\t\tReroll: {runData.currentRerollCount}/{runData.baseRerollCount}");
        
        // Block Scores를 2개씩 탭으로 구분
        sb.AppendLine("\nBlock Scores:");
        
        // Dictionary를 List로 변환
        List<KeyValuePair<BlockType, int>> blockScores = new List<KeyValuePair<BlockType, int>>();
        foreach (var pair in runData.baseBlockScores)
        {
            blockScores.Add(pair);
        }

        for (int i = 0; i < blockScores.Count; i += 2)
        {
            if (i + 1 < blockScores.Count)
            {
                // 2개의 항목이 있는 경우
                sb.AppendLine($"  {blockScores[i].Key}: {blockScores[i].Value}\t\t{blockScores[i+1].Key}: {blockScores[i+1].Value}");
            }
            else
            {
                // 마지막 항목이 홀수인 경우
                sb.AppendLine($"  {blockScores[i].Key}: {blockScores[i].Value}");
            }
        }

        runInfoText.text = sb.ToString();
    }
}

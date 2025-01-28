using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class RunInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentElapsedTimeText;
    [SerializeField] private TextMeshProUGUI baseMultiplierText;
    [SerializeField] private TextMeshProUGUI defaultRerollCountText;
    [SerializeField] private Image mostPlacedBlockImage;
    private RectTransform rectTransform;

    private const float insidePositionY = 0;
    private const float outsidePositionY = -1080;
    private const float duration = 0.2f;

    private RunData currentRunData;
    private float runStartTime;
    private bool isUpdatingTime;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(RunData runData, float startTime, BlockType mostPlacedBlockType)
    {
        currentRunData = runData;
        runStartTime = startTime;

        // 기본 데이터 UI에 표시
        baseMultiplierText.text = "x" + runData.baseMatchMultipliers[MatchType.ROW];
        defaultRerollCountText.text = runData.baseRerollCount.ToString();
        mostPlacedBlockImage.sprite = Resources.Load<Sprite>($"Sprites/Block/Preset/{mostPlacedBlockType.ToString()}");
    }

    public void OpenRunInfoUI()
    {
        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
        
        // 경과 시간 업데이트 시작
        StartCoroutine(UpdateElapsedTime());
    }

    public void CloseRunInfoUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionY, duration);
        isUpdatingTime = false; // 경과 시간 업데이트 중지
    }

    private IEnumerator UpdateElapsedTime()
    {
        isUpdatingTime = true;
        while (isUpdatingTime)
        {
            float elapsedTime = Time.time - runStartTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);

            currentElapsedTimeText.text = $"{minutes:00}:{seconds:00}";

            yield return null; // 매 프레임 업데이트
        }
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
}
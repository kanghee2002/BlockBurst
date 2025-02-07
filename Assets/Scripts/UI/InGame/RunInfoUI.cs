using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class RunInfoUI : MonoBehaviour
{
    [SerializeField] private PopupBlurImage popupBlurImage;

    [SerializeField] private TextMeshProUGUI currentElapsedTimeText;
    [SerializeField] private TextMeshProUGUI baseMultiplierText;
    [SerializeField] private TextMeshProUGUI defaultRerollCountText;
    [SerializeField] private Image mostPlacedBlockImage;
    [SerializeField] private TextMeshProUGUI tipText;
    private RectTransform rectTransform;

    private const float insidePositionY = 0;
    private const float outsidePositionY = -1080;
    private const float duration = 0.2f;

    private RunData currentRunData;
    private float runStartTime;
    private bool isUpdatingTime;

    private readonly List<string> tips = new List<string>()
    {
        "무작위 줄 지우기는 지우기 효과를 발동시키지 않습니다!",
        "배수는 점수 계산 후 기본 배수로 초기화됩니다!",
        "한 번에 많은 줄을 지울수록 큰 점수를 얻을 수 있습니다!",
        "우상단 덱을 통해 현재 블록 수와 점수를 알 수 있습니다!",
        "메인 화면으로 가면 튜토리얼을 다시 할 수 있습니다!",
        "스테이지 제한은 해당 스테이지에만 적용됩니다!",
    };

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(RunData runData, float startTime, BlockType? mostPlacedBlockType)
    {
        currentRunData = runData;
        runStartTime = startTime;

        // 기본 데이터 UI에 표시
        baseMultiplierText.text = "x" + runData.baseMatchMultipliers[MatchType.ROW];
        defaultRerollCountText.text = runData.baseRerollCount.ToString();
        if (mostPlacedBlockType != null)
        {
            mostPlacedBlockImage.sprite = Resources.Load<Sprite>($"Sprites/Block/Preset/{mostPlacedBlockType.ToString()}");
            mostPlacedBlockImage.color = new Color(1f, 1f, 1f, 1f);
        }

        // 항상 새로운 팁 표시
        for (int i = 0; i < 10000; i++)
        {
            string tip = tips[Random.Range(0, tips.Count)];
            if (tipText.text != tip)
            {
                tipText.text = tip;
                break;
            }
        }
        
    }

    public void OpenRunInfoUI()
    {
        gameObject.SetActive(true);
        UIUtils.OpenUI(rectTransform, "Y", insidePositionY, duration);
        popupBlurImage.OpenPopupBlurImage(new Color(0.0f, 0.0f, 0.0f, 0.9f));
        
        // 경과 시간 업데이트 시작
        StartCoroutine(UpdateElapsedTime());
    }

    public void CloseRunInfoUI()
    {
        UIUtils.CloseUI(rectTransform, "Y", insidePositionY, outsidePositionY, duration);
        popupBlurImage.ClosePopupBlurImage();
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
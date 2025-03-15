using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class RunInfoUI : MonoBehaviour
{
    [SerializeField] private PopupBlurImage popupBlurImage;

    [Header("RunInfo")]
    [SerializeField] private GameObject runInfoContainer;
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
        "아이템 효과로 인한 줄 지우기는 다른 아이템 효과를 발동시키지 않습니다!",
        "배수는 점수 계산 후 기본 배수로 초기화됩니다!",
        "아이템 효과를 적극적으로 활용하면 많은 점수를 얻을 수 있습니다!",
        "우상단 덱을 통해 현재 블록 수와 점수를 알 수 있습니다!",
        "메인 화면으로 가면 튜토리얼을 다시 할 수 있습니다!",
        "스테이지 제한은 해당 스테이지에만 적용됩니다!",
        "스테이지 제한마다 점수 요구치는 다릅니다!",
        "설정에서 새 게임을 진행할 수 있습니다!",
        "덱이 소진되거나 놓을 수 있는 곳이 없는 경우 게임 오버됩니다!",
        "부스트와 블록 강화는 영구적으로 적용됩니다!",
        "고양이는 귀엽습니다!",
        "진행 정보에서 부스트 목록을 확인할 수 있습니다!",
        "아이템을 통해 쌓인 능력치는 아이템을 팔아도 유지됩니다!",
        "독서는 블록의 양식입니다!",
        "2시 시계는 하루에 2번 맞습니다! 2시 시계는 하루에 2번 맞습니다!",
        "아이템은 일반, 레어, 희귀 등급으로 구분됩니다!",
    };

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(RunData runData, float startTime, BlockType? mostPlacedBlockType)
    {
        // Initialize RunInfo
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
}
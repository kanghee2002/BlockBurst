using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{

    [SerializeField] private RectTransform characterRect;
    [SerializeField] private RectTransform highlightRect;
    [SerializeField] private CutOutMaskUI shadow;
    [SerializeField] private RectTransform textLayoutRect;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private ButtonUI nextButton;

    [Header("Block Click")]
    [SerializeField] private RectTransform clickableRect;
    [SerializeField] private RectTransform[] blocks; // 클릭 막는 4개의 Rect

    [Header("Highlight Area")]
    [SerializeField] private RectTransform highlightAreaRect;

    [Header("For Shopping")]
    [SerializeField] private RectTransform itemShowcaseUI;
    [SerializeField] private RectTransform itemSetUI;

    [Header("Delay")]
    [SerializeField] private float nextButtonDelay;

    private enum Block
    {
        Left,
        Right,
        Top,
        Bottom
    }

    [System.Serializable]
    public class TutorialStep
    {
        public Vector2 characterPosition;
        public RectTransform highlightRect;
        public RectTransform clickableRect;
        public string description;
        public bool isNextButtonInactive;
        public bool isInactive;
    }
    [Header("Tutorial Step")]
    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();

    private int stepCount;

    // TODO: 텍스트에 색깔 넣기
    private readonly List<string> descriptions = new List<string>()
    {
        "안녕하세요! 게임 플레이 방법에 대해 설명드리겠습니다!",
        "Block Block은 각 스테이지마다 목표 점수를 달성하는 게임입니다",
        "이 스테이지는 이 점수만 달성하면 됩니다",
        "매 스테이지는 제한 조건이 있습니다",
        "선택을 클릭해 스테이지를 시작하세요!",
        "블록을 배치해 한 줄을 지우면 점수를 획득합니다",
        "점수는 지워지는 블록들의 '점수 합 x 배수'입니다",
        "한 번에 많이 지울수록 더 큰 점수를 얻겠죠!?",
        "배수는 점수 계산 이후 기본 배수로 돌아갑니다. 기억해두세요!",
        "'덱'을 눌러 블록 이름과 점수를 확인해보세요",
        "매 게임마다 블록을 3개씩 가진 채 시작합니다",
        "블록을 누르면 회전시킬 수 있습니다",
        "'리롤'을 눌러 새로운 블록을 가져올 수 있습니다",
        "횟수가 제한되어 있으니 신중히 사용하세요!",
        "목표 점수 도달 이전에 블록을 다 배치하면 패배합니다",
        "또는 블록을 배치할 곳이 없을 때 패배합니다",
        "이제 블록을 배치해 스테이지를 클리어하세요!",

        "스테이지를 클리어해 돈을 두둑히 받았습니다",
        "이 사기적인 아이템을 구매해보세요",
        "'2시 시계'는 줄을 2번 지울 때마다 +2 배수를 추가합니다!",
        "그럼 줄을 2번 지울 때마다 3배의 점수를 얻을 수 있겠군요!",
        "아이템은 한 번에 5개까지만 가질 수 있습니다",
        "아이템을 클릭하면 버리기 버튼이 활성화됩니다",
        "이제 'Duo 블록'을 구매해보세요",
        "특수 블록을 덱에 1개 추가했습니다\n" +
        "이 특수 블록은 블록이 모두 지워질 때 +2 골드를 획득합니다!",
        "특수 블록은 각각 특별한 효과를 가집니다",
        "이건 'I/O 블록 골드 강화'입니다",
        "강화는 블록에 능력을 부여합니다.\n" +
        "이 강화는 I 또는 O 블록을 배치해서 줄을 지울 때 +1 골드를 획득합니다!",
        "블록 추가와 강화엔 횟수 제한이 없습니다!\n" +
        "자세한 정보는 '덱'을 눌러 확인하세요",
        "이제 다음 스테이지로 이동하세요!",

        "스테이지에서 승리할 때마다 목표 점수가 증가합니다",

        "3번째 스테이지마다 강력한 제한을 가진 보스 스테이지가 등장하니 주의하세요!",
        "그럼 3챕터까지 클리어해 게임을 승리해보세요. 행운을 빕니다!",
    };

    private bool isPlayingTutorial = false;
    private bool isWaitingForClick = false;
    private bool isWaitingForSignal = false;

    private int itemCount = 0;

    private float shadowAlpha;

    public void Initialize()
    {
        characterRect.gameObject.SetActive(true);

        stepCount = 0;
        isPlayingTutorial = true;
        isWaitingForClick = false;
        isWaitingForSignal = false;
        itemCount = 0;
        shadowAlpha = shadow.color.a;

        List<string> firstShopItems = new List<string>()
        {
            "TwoClock",
            "AddBlockDuo",
            "BlockIOGoldUpgrade",
        };
        GameManager.instance.shopManager.AddFirstItem(firstShopItems);

        StartCoroutine(ProcessStepCoroutine());
    }

    IEnumerator ProcessStepCoroutine()
    {
        yield return null;
        ProcessStep();
    }

    public void ProceedNextStep(string sign)
    {
        if (isWaitingForClick && 
           (sign == "StageChoice" || sign == "Deck" || sign == "DeckBack" ||
            sign == "Purchase" || sign == "NextStage"))
        {
            ProcessStep();
        }

        else if (isWaitingForSignal && sign == "EndStage")
        {
            ProcessStep();
        }
    }

    private void ProcessStep()
    {
        if (!isPlayingTutorial) return;

        TutorialStep currentStep = tutorialSteps[stepCount];

        // 캐릭터가 나타날 때 애니메이션
        if (!characterRect.gameObject.activeSelf)
        {
            characterRect.DOPunchScale(Vector3.one * 1.2f, 0.3f);
        }

        // 캐릭터 이동
        characterRect.gameObject.SetActive(true);
        characterRect.localScale = Vector3.one;

        //characterRect.anchoredPosition = currentStep.characterPosition;
        characterRect.DOAnchorPos(currentStep.characterPosition, 0.5f)
            .SetEase(Ease.OutBack, overshoot: 1.2f);

        // 설명 표시
        textLayoutRect.gameObject.SetActive(true);

        descriptionText.text = currentStep.description.Replace("\\n", "\n");

        // 캐릭터와 함께 이동
        textLayoutRect.anchoredPosition = new Vector2(characterRect.anchoredPosition.x, characterRect.anchoredPosition.y - 50f);
        Vector2 nextTextPosition = new Vector2(currentStep.characterPosition.x, currentStep.characterPosition.y - 50f);
        textLayoutRect.DOAnchorPos(nextTextPosition, 0.5f)
            .SetEase(Ease.OutBack, overshoot: 1.2f);

        // 하이라이트 표시
        SetHightlight(currentStep.highlightRect);
     
        // 클릭 방지
        if (currentStep.isInactive)
        {
            DisableBlocks();
        }
        else
        {
            BlockInputExceptRect(currentStep.clickableRect);
        }

        // '다음 버튼' 뜰지 말지
        if (currentStep.isNextButtonInactive)
        {
            isWaitingForClick = true;
            nextButton.gameObject.SetActive(false);
        }
        else
        {
            isWaitingForClick = false;
            nextButton.gameObject.SetActive(false);
            DOVirtual.DelayedCall(nextButtonDelay, PlayShowNextButton);
        }

        // 캐릭터 사라질지 말지
        if (currentStep.isInactive)
        {
            isWaitingForSignal = true;
            PlayDisapearRect(characterRect);
            PlayDisapearRect(textLayoutRect);
        }
        else
        {
            PlayCharacterSpeakAnimation();
        }

        stepCount++;
    }

    private void SetHightlight(RectTransform source)
    {
        // 이전에 꺼져있었다면
        if (!highlightRect.gameObject.activeSelf)
        {
            // 점점 어두워지는 애니메이션
            Color shadowColor = shadow.color;
            shadowColor.a = 0f;
            shadow.color = shadowColor;

            shadow.DOFade(shadowAlpha, 0.5f);
        }

        highlightRect.gameObject.SetActive(true);

        if (source == null)
        {
            // 이전에 켜져있었다면
            if (highlightAreaRect.gameObject.activeSelf)
            {
                // 점점 밝아지는 애니메이션
                shadow.DOFade(0f, 0.5f)
                    .OnComplete(() =>
                    {
                        highlightRect.gameObject.SetActive(false);
                        Color shadowColor = shadow.color;
                        shadowColor.a = shadowAlpha;
                        shadow.color = shadowColor;
                    });
            }
            else
            {
                highlightRect.gameObject.SetActive(false);
            }
            return;
        }
        else if (source == characterRect)
        {
            highlightAreaRect.sizeDelta = Vector2.zero;
            highlightRect.DOMove(highlightAreaRect.position, 0.5f);
            highlightRect.DOSizeDelta(highlightAreaRect.rect.size, 0.5f);
            return;
        }
        else if (source == itemShowcaseUI)
        {
            if (itemCount == 0 || itemCount == 1)
            {
                source = source.GetChild(itemCount).GetComponent<RectTransform>();
            }
            else if (itemCount >= 2)
            {
                source = source.GetChild(2).GetComponent<RectTransform>();
            }
        }
        else if (source == itemSetUI)
        {
            source = source.GetChild(0).GetComponent<RectTransform>();
        }

        Canvas canvas = source.GetComponentInParent<Canvas>();
        float scaleFactor = canvas.scaleFactor;

        // 2. 원본 RectTransform의 크기 직접 가져오기
        Vector2 sourceSize = source.rect.size;

        // 3. 위치와 크기 설정
        highlightAreaRect.position = source.position;
        highlightAreaRect.sizeDelta = sourceSize;
        //highlightRect.position = source.position;
        //highlightRect.sizeDelta = sourceSize;
        //highlightRect.DOMove(source.position, 0.5f);
        //highlightRect.DOSizeDelta(sourceSize, 0.5f);

        // 4. 회전값도 동일하게 설정 (필요한 경우)
        highlightAreaRect.rotation = source.rotation;

        if (source.parent.name == "DeckInfoBackButtonUI" ||
            source.name == "DeckInfoUI")
        {
            highlightAreaRect.anchoredPosition = new Vector2(0f, highlightAreaRect.anchoredPosition.y);
        }
        else if (source.name == "HandUI")
        {
            highlightAreaRect.anchoredPosition = new Vector2(highlightAreaRect.anchoredPosition.x - (source.anchoredPosition.x - (-188f)), highlightAreaRect.anchoredPosition.y);
        }
        else if (stepCount > 30 &&  source.name == "ScoreAndRewardLayout")
        {
            highlightAreaRect.anchoredPosition = new Vector2(highlightAreaRect.anchoredPosition.x, -178f);
            //UIUtils.OpenUI(highlightAreaRect, "Y", -178f, 0.2f);
        }

        highlightRect.DOMove(highlightAreaRect.position, 0.5f);
        highlightRect.DOSizeDelta(highlightAreaRect.rect.size, 0.5f);
    }

    public void BlockInputExceptRect(RectTransform target)
    {
        foreach (RectTransform block in blocks)
        {
            block.gameObject.SetActive(true);
        }

        if (target == null)
        {
            blocks[(int)Block.Left].sizeDelta = new Vector2(Screen.width / 2f, 0);
            blocks[(int)Block.Right].sizeDelta = new Vector2(Screen.width / 2f, 0);
            blocks[(int)Block.Top].sizeDelta = new Vector2(0, Screen.height / 2f);
            blocks[(int)Block.Bottom].sizeDelta = new Vector2(0, Screen.height / 2f);
            return;
        }
        else if (target == itemShowcaseUI)
        {
            if (itemCount == 0 || itemCount == 1)
            {
                target = target.GetChild(itemCount).GetComponent<RectTransform>(); ;
            }
            else if (itemCount >= 2)
            {
                target = target.GetChild(2).GetComponent<RectTransform>();
            }
            itemCount++;
        }
        else if (target == itemSetUI)
        {
            target = target.GetChild(0).GetComponent<RectTransform>();
        }

        /*Debug.Log("PosX: " + target.anchoredPosition.x);
        Debug.Log("PosY: " + target.anchoredPosition.y);
        Debug.Log("Width: " + target.rect.width);
        Debug.Log("Height: " + target.rect.height);*/

        MatchRect(target, clickableRect);

        if (target.parent.name == "DeckInfoBackButtonUI" ||
            target.name == "DeckInfoUI")
        {
            clickableRect.anchoredPosition = new Vector2(0f, clickableRect.anchoredPosition.y);
        }
        else if (target.name == "HandUIasdasdsd")
        {
            clickableRect.anchoredPosition = new Vector2(-188f, clickableRect.anchoredPosition.y);
        }

        blocks[(int)Block.Left].sizeDelta = new Vector2(Screen.width / 2f + clickableRect.anchoredPosition.x - clickableRect.rect.width / 2f, 0);
        blocks[(int)Block.Right].sizeDelta = new Vector2(Screen.width / 2f - clickableRect.anchoredPosition.x - clickableRect.rect.width / 2f, 0);
        blocks[(int)Block.Top].sizeDelta = new Vector2(0, Screen.height / 2f - clickableRect.anchoredPosition.y - clickableRect.rect.height / 2f);
        blocks[(int)Block.Bottom].sizeDelta = new Vector2(0, Screen.height / 2f + clickableRect.anchoredPosition.y - clickableRect.rect.height / 2f);
    }

    private void DisableBlocks()
    {
        foreach (RectTransform block in blocks)
        {
            block.gameObject.SetActive(false);
        }
    }

    private void MatchRect(RectTransform source, RectTransform target)
    {
        Canvas canvas = source.GetComponentInParent<Canvas>();
        float scaleFactor = canvas.scaleFactor;

        // 2. 원본 RectTransform의 크기 직접 가져오기
        Vector2 sourceSize = source.rect.size;

        // 3. 위치와 크기 설정
        target.position = source.position;
        target.sizeDelta = sourceSize;

        // 4. 회전값도 동일하게 설정 (필요한 경우)
        target.rotation = source.rotation;
    }

    private void PlayShowNextButton()
    {
        nextButton.gameObject.SetActive(true);
        nextButton.transform.DOPunchScale(Vector3.one * 0.2f, duration: 0.2f);
    }

    private void PlayDisapearRect(RectTransform rect)
    {
        rect.DOScale(0f, 0.3f).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                rect.localScale = Vector3.one;
                rect.gameObject.SetActive(false);
            });
    }

    private void PlayCharacterSpeakAnimation()
    {
        /*characterRect.DOPunchScale(Vector3.one * 0.3f, duration: 0.7f, vibrato: 10)
            .OnComplete(() =>
            {
                characterRect.localScale = Vector3.one;
            });*/
    }
}

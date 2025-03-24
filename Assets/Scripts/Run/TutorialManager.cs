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

        [Header("Additional")]
        public bool isNextButtonInactive;
        public bool isInactive;
        public float highlightDelay;
    }
    [Header("Tutorial Step")]
    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();

    private int stepCount;

    // TODO: 텍스트에 색깔 넣기

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

        List<string> firstStageName = new List<string>()
        {
            "DebuffSpecial",
            "IncreaseBoardSize",
        };

        GameManager.instance.stageManager.firstStageList.AddRange(firstStageName);

        List<string> firstShopItems = new List<string>()
        {
            "TwoClock",
            "RedEgg",
            "GrayCube",
            "BlockIOGoldUpgrade",
            "AddBlockDuo",
        };
        GameManager.instance.shopManager.AddFirstItem(firstShopItems);

        StartCoroutine(ProcessStepCoroutine());
    }

    IEnumerator ProcessStepCoroutine()
    {
        yield return new WaitForSeconds(0.01f);
        ProcessStep();
    }

    public void ProceedNextStep(string sign)
    {
        if (isWaitingForClick && 
           (sign == "StageChoice" || sign == "Deck" || sign == "DeckBack" ||
            sign == "Purchase" || sign == "NextStage" || sign == "Run" ||
            sign == "RunBack" || sign == "ItemClicked"))
        {
            ProcessStep();
        }

        else if (isWaitingForSignal && sign == "EndStage")
        {
            if (stepCount < 25)
            {
                // 어둠의 경로로 돈 추가
                GameManager.instance.UpdateGold(4);
            }

            ProcessStep();
        }
    }

    private void ProcessStep()
    {
        if (!isPlayingTutorial) return;
        
        if (stepCount >= tutorialSteps.Count)
        {
            isPlayingTutorial = false;
            return;
        }


        TutorialStep currentStep = tutorialSteps[stepCount];

        // 캐릭터가 나타날 때 애니메이션
        if (!characterRect.gameObject.activeSelf)
        {
            characterRect.DOPunchScale(Vector3.one * 1.2f, 0.3f);
        }

        // 캐릭터 이동
        characterRect.gameObject.SetActive(true);
        characterRect.DOKill();
        characterRect.localScale = Vector3.one;

        //characterRect.anchoredPosition = currentStep.characterPosition;
        characterRect.DOAnchorPos(currentStep.characterPosition, 0.5f)
            .SetEase(Ease.OutBack, overshoot: 1.2f);

        // 설명 표시
        textLayoutRect.gameObject.SetActive(true);

        descriptionText.text = currentStep.description.Replace("\\n", "\n");

        // 캐릭터와 함께 이동
        //textLayoutRect.anchoredPosition = new Vector2(characterRect.anchoredPosition.x, characterRect.anchoredPosition.y - 60f);
        Vector2 nextTextPosition = new Vector2(currentStep.characterPosition.x + 210f, currentStep.characterPosition.y);
        textLayoutRect.DOAnchorPos(nextTextPosition, 0.5f)
            .SetEase(Ease.OutBack, overshoot: 1.2f);

        // 하이라이트 표시
        SetHightlight(currentStep.highlightRect, currentStep.highlightDelay);
     
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

        AudioManager.instance.TutorialTalker(currentStep.description);

        stepCount++;
    }

    private void SetHightlight(RectTransform source, float delay = 0f)
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
            if (itemCount == 0)
            {
                source = source.GetChild(0).GetChild(1).GetComponent<RectTransform>();
            }
            else if (itemCount == 1)
            {
                source = source.GetChild(1).GetChild(1).GetComponent<RectTransform>();
            }
            else if (itemCount == 2)
            {
                source = source.GetChild(2).GetChild(1).GetComponent<RectTransform>();
            }
            else if (itemCount == 3)
            {
                source = source.GetChild(2).GetChild(2).GetComponent<RectTransform>();
            }
            else if (itemCount >= 5)
            {
                source = source.GetChild(0).GetComponent<RectTransform>();
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
        else if (source.name == "HandUI" || source.name == "Outer")
        {
            highlightAreaRect.anchoredPosition = new Vector2(0f, highlightAreaRect.anchoredPosition.y);
        }
        else if (stepCount > 30 &&  source.name == "ScoreAndRewardLayout")
        {
            highlightAreaRect.anchoredPosition = new Vector2(highlightAreaRect.anchoredPosition.x, -178f);
            //UIUtils.OpenUI(highlightAreaRect, "Y", -178f, 0.2f);
        }
        else if (source.name == "RunInfoUI" || source.name == "ItemDetailUI")
        {
            highlightAreaRect.anchoredPosition = new Vector2(highlightAreaRect.anchoredPosition.x, 0f);
        }

        highlightRect.DOMove(highlightAreaRect.position, 0.5f + delay);
        highlightRect.DOSizeDelta(highlightAreaRect.rect.size, 0.5f + delay);
    }

    public void BlockInputExceptRect(RectTransform target)
    {
        //Debug.Log("Screen Width: " + Screen.width);
        //Debug.Log("Screen Height: " + Screen.height);

        foreach (RectTransform block in blocks)
        {
            block.gameObject.SetActive(true);
        }

        float resolutionRatioX;
        float resolutionRatioY;

        if (GameManager.instance.applicationType == ApplicationType.Windows)
        {
            resolutionRatioX = Screen.width / 1920f;
            resolutionRatioY = Screen.height / 1080f;
        }
        else
        {
            resolutionRatioX = Screen.width / 462.85f;
            resolutionRatioY = Screen.height / 1080f;
        }

        if (target == null)
        {
            blocks[(int)Block.Left].sizeDelta = new Vector2((Screen.width / 2f) / resolutionRatioX, 0);
            blocks[(int)Block.Right].sizeDelta = new Vector2((Screen.width / 2f) / resolutionRatioX, 0);
            blocks[(int)Block.Top].sizeDelta = new Vector2(0, (Screen.height / 2f) / resolutionRatioY);
            blocks[(int)Block.Bottom].sizeDelta = new Vector2(0, (Screen.height / 2f) / resolutionRatioY);
            return;
        }
        else if (target == itemShowcaseUI)
        {
            if (itemCount == 0)
            {
                target = target.GetChild(0).GetChild(1).GetComponent<RectTransform>();
            }
            else if (itemCount == 1)
            {
                target = target.GetChild(1).GetChild(1).GetComponent<RectTransform>();
            }
            else if (itemCount == 2)
            {
                target = target.GetChild(2).GetChild(1).GetComponent<RectTransform>();
            }
            else if (itemCount == 3)
            {
                target = target.GetChild(2).GetChild(2).GetComponent<RectTransform>();
            }
            else if (itemCount >= 5)
            {
                target = target.GetChild(0).GetComponent<RectTransform>();
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
        else if (target.name == "RunInfoUI")
        {
            clickableRect.anchoredPosition = new Vector2(clickableRect.anchoredPosition.x, 0f);
        }
        else if (target.parent.name == "InteractButtonUI")
        {
            clickableRect.anchoredPosition = new Vector2(clickableRect.anchoredPosition.x, -60f);
        }

        float anchorPosX = clickableRect.anchoredPosition.x * resolutionRatioX;
        float anchorPosY = clickableRect.anchoredPosition.y * resolutionRatioY;
        float rectWidth = clickableRect.rect.width * resolutionRatioX;
        float rectHeight = clickableRect.rect.height * resolutionRatioY;

        blocks[(int)Block.Left].sizeDelta = new Vector2((Screen.width / 2f + anchorPosX - rectWidth / 2f) / resolutionRatioX, 0);
        blocks[(int)Block.Right].sizeDelta = new Vector2((Screen.width / 2f - anchorPosX - rectWidth / 2f) / resolutionRatioX, 0);
        blocks[(int)Block.Top].sizeDelta = new Vector2(0, (Screen.height / 2f - anchorPosY - rectHeight / 2f) / resolutionRatioY);
        blocks[(int)Block.Bottom].sizeDelta = new Vector2(0, (Screen.height / 2f + anchorPosY - rectHeight / 2f) / resolutionRatioY);
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
        rect.DOKill();
        rect.DOScale(0f, 0.3f).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                rect.localScale = Vector3.one;
                rect.gameObject.SetActive(false);
            });
    }

    private void PlayCharacterSpeakAnimation()
    {
        characterRect.DOPunchScale(Vector3.one * 0.3f, duration: 1f, vibrato: 7)
            .OnComplete(() =>
            {
                characterRect.localScale = Vector3.one;
            });
    }
}

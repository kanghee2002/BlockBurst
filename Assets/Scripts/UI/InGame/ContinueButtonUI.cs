using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 이어하기 버튼: 런 저장 파일 존재 시 활성화(<see cref="DataManager.HasValidRunSaveData"/>).
/// 손상·구버전 등은 클릭 후 <see cref="GameManager.ContinueGame"/>의 <c>TryLoadRunData</c>에서 걸러진다.
/// </summary>
public class ContinueButtonUI : MonoBehaviour
{
    [SerializeField] private ButtonUI buttonUI;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] [Range(0.05f, 1f)] private float disabledAlphaMultiplier = 0.38f;

    private void Start()
    {
        if (buttonUI == null)
            buttonUI = GetComponent<ButtonUI>();

        if (buttonUI == null)
        {
            Debug.LogError("ContinueButtonUI: ButtonUI가 필요합니다.", this);
            return;
        }

        bool canContinue = DataManager.instance.HasValidRunSaveData() && !DataManager.instance.IsRunDefeated();

        if (canContinue)
        {
            buttonUI.SetOnClickMethod(OnContinueClicked);
        }
        else
        {
            buttonUI.SetOnClickMethod(null);
            buttonUI.showHover = false;
            buttonUI.enabled = false;

            if (buttonImage != null)
            {
                Color c = buttonImage.color;
                c.a *= disabledAlphaMultiplier;
                buttonImage.color = c;
            }

            if (continueText != null)
            {
                Color tc = continueText.color;
                tc.a *= disabledAlphaMultiplier;
                continueText.color = tc;
            }
        }
    }

    private void OnContinueClicked()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("ContinueButtonUI: GameManager.instance is null.");
            return;
        }

        GameManager.instance.ContinueGame();
    }
}

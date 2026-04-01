using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 이어하기 버튼: <see cref="DataManager.HasValidRunSaveData"/>에 따라 클릭 동작·표시를 맞춘다.
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

        bool canContinue = DataManager.instance.HasValidRunSaveData();

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

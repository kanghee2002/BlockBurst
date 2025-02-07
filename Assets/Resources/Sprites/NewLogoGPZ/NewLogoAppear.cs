using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class NewLogoAppear : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Animator anim;
    [SerializeField] private Button startButton;
    [SerializeField] private Toggle tutorialToggle;
    [SerializeField] private GameObject logoText;

    private EventInstance eventInstance;
    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClick);
        tutorialToggle.onValueChanged.AddListener(OnToggleTutorial);
        logoText.GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width / 2, -1 * Screen.height / 2);
        float scale = Screen.width / 1920f;
        logoText.GetComponent<RectTransform>().localScale = new Vector2(scale, scale);
        StartCoroutine(Wait());
        eventInstance = RuntimeManager.CreateInstance("event:/mainmenuloop");

        // gameManager에 따름
        bool isTutorial = GameManager.instance.GetTutorialValue();
        tutorialToggle.isOn = isTutorial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnStartButtonClick() {
        RuntimeManager.PlayOneShot("event:/game_start_button");
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        SceneTransitionManager.instance.TransitionToScene("GameScene");
    }

    void OnToggleTutorial(bool value) {
        AudioManager.instance.SFXSelectMenu();

        GameManager.instance.SetTutorialValue(value);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("sssssss");
        RuntimeManager.PlayOneShot("event:/newlogo");
        anim.SetBool("Appear", true);
        yield return new WaitForSeconds(2);
        anim.SetBool("Appear", false);
        yield return new WaitForSeconds(2);
        
        StartCoroutine(LogoTextMover());
        StartCoroutine(LogoTextScaler());
        StartCoroutine(AppearStartButton());
        StartCoroutine(AppearTutorialToggle());
    }

    IEnumerator AppearStartButton() {
        float startPos = 361f;
        float endPos = -418f;
        float currentPos = startPos;
        eventInstance.start();

        while(currentPos > endPos + 3f) {
            currentPos += (endPos - currentPos) / 8;
            yield return new WaitForSeconds(0.01f);
            startButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(currentPos, 0);
        }
        currentPos = endPos;
        startButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(currentPos, 0);

    }

    IEnumerator LogoTextMover() {
        float startX = Screen.width / 2;
        float startY = -1 * Screen.height / 2;
        float endX = 960;
        float endY = -540;

        while(Math.Abs(startX - endX) > 0.1f) {
            startX += (endX - startX) / 8;
            startY += (endY - startY) / 8;
            yield return new WaitForSeconds(0.01f);
            logoText.GetComponent<RectTransform>().anchoredPosition = new Vector2(startX, startY);
        }   
         logoText.GetComponent<RectTransform>().anchoredPosition = new Vector2(endX, endY);
        
    }

    IEnumerator LogoTextScaler() {
        float startScale = Screen.width / 1920f;
        float endScale = 1f;

        while(Math.Abs(startScale - endScale) > 0.01f) {
            startScale += (endScale - startScale) / 8;
            yield return new WaitForSeconds(0.01f);
            logoText.GetComponent<RectTransform>().localScale = new Vector2(startScale, startScale);
        }
        logoText.GetComponent<RectTransform>().localScale = new Vector2(endScale, endScale);

    }

    IEnumerator AppearTutorialToggle() {
        float startPos = 361f;
        float endPos = -361f;
        float currentPos = startPos;
        float startY = -100f;

        while(currentPos > endPos + 3f) {
            currentPos += (endPos - currentPos) / 8;
            yield return new WaitForSeconds(0.01f);
            tutorialToggle.GetComponent<RectTransform>().anchoredPosition = new Vector2(currentPos, startY);
        }
        currentPos = endPos;
        tutorialToggle.GetComponent<RectTransform>().anchoredPosition = new Vector2(currentPos, startY);
    }


}

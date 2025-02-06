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

    private EventInstance eventInstance;
    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClick);
        tutorialToggle.onValueChanged.AddListener(OnToggleTutorial);
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

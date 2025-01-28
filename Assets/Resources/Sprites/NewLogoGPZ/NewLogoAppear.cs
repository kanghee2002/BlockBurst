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

    private EventInstance eventInstance;
    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClick);
        StartCoroutine(Wait());
        eventInstance = RuntimeManager.CreateInstance("event:/mainmenuloop");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnStartButtonClick() {
        RuntimeManager.PlayOneShot("event:/game_start_button");
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        //////////////// 여기에 게임 씬으로 넘어가는 부분 추가바람 //////////////// plz add transition to game scene ///////////////////
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


}

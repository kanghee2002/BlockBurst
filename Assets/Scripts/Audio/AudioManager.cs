using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;
using System.Runtime.InteropServices;


public class AudioManager : MonoBehaviour
{
    // AudioManager is a singleton class for controlling the whole audio of the project. Use AudioManager.Instance to get the singleton instance in other scripts.

    class TimelineInfo
    {
        public int currentMusicBar = 0;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }
    public static AudioManager instance;

    private static bool isPlaying = false;
    private RuntimeManager runtimeManager;

    private EventInstance stageSource;

    private GCHandle timelineHandle;

    private TimelineInfo timelineInfo1;


    private float currentTempo;

    private FMOD.Studio.EVENT_CALLBACK stageSourceCallback;

    private int currentChapter;
    

    void Awake() {
        if(instance != null && instance != this) {
            Destroy(this.gameObject);
        }
        else {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        currentChapter = 1;
        timelineInfo1 = new TimelineInfo();
        stageSourceCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        stageSource = RuntimeManager.CreateInstance("event:/stage" + currentChapter);
        stageSource.setParameterByName("shop_transit", 0);
        timelineHandle = GCHandle.Alloc(timelineInfo1, GCHandleType.Pinned);
        stageSource.setUserData(GCHandle.ToIntPtr(timelineHandle));
        stageSource.setCallback(stageSourceCallback, EVENT_CALLBACK_TYPE.TIMELINE_MARKER | EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        

        // gotta add more stages later
        
    }

    private IEnumerator shopTransition;
    private IEnumerator stageTransitionIn;
    private IEnumerator stageTransitionOut;
    private IEnumerator talker;
    void Start()
    {
        /*
        timelineInfo1 = new TimelineInfo();
        stageSourceCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        stageSource.setParameterByName("shop_transit", 0);
        stageSource = RuntimeManager.CreateInstance("event:/stageSource");
        timelineHandle = GCHandle.Alloc(timelineInfo1, GCHandleType.Pinned);
        stageSource.setUserData(GCHandle.ToIntPtr(timelineHandle));
        stageSource.setCallback(stageSourceCallback, EVENT_CALLBACK_TYPE.TIMELINE_MARKER | EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        */
    }

    void OnDestroy() {
        stageSource.setUserData(IntPtr.Zero);
        timelineHandle.Free();
    }

    void ChangeBGMVolume(float volume) {
        // Change FMOD VCA volume
        RuntimeManager.GetVCA("vca:/BGM").setVolume(volume);
    }

    void ChangeSFXVolume(float volume) {
        // Change FMOD VCA volume
        RuntimeManager.GetVCA("vca:/SFX").setVolume(volume);
    }

    public void RestartGame() { // 배경음악 초기화(1단계부터)
        StopBackgroundMusic();
        ChangeChapter(1);
    }

    public void BeginBackgroundMusic() { // 배경음악 재생 시작
        if(!isPlaying) {
            stageSource.start();
            isPlaying = true;
        }
    }

    public void StopBackgroundMusic() { // 배경음악 재생 중지
        if(isPlaying)
        {
            stageSource.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isPlaying = false;
        }

    }

    public void TutorialTalker(string text, float interval = 0.075f) {
        if(talker != null) {
            StopCoroutine(talker);
        }
        talker = TutorialVoiceActuator(text, interval);
        StartCoroutine(talker);
    }

    IEnumerator TutorialVoiceActuator(string text, float interval) {
        // Play the oneshot effect on every letter except space, also stop this after 30 letters
        int count = 0;
        foreach(char c in text) {
            if(c != ' ') {
                RuntimeManager.PlayOneShot("event:/tutorial_talk");
                count++;
                if(count >= 30) {
                    break;
                }
            }
            yield return new WaitForSeconds(interval);
        }
    }

    public void ChangeStage(int stage) {
        shopTransition = ChangeStageParameter(stage);
        StartCoroutine(shopTransition);
        //StartCoroutine(ChangeStageParameter(stage));
    }

    public void ShopTransitionIn() {
        stageTransitionIn = ShopTransition(true);
        StartCoroutine(stageTransitionIn);
    }

    public void ShopTransitionOut() {
        stageTransitionOut = ShopTransition(false);
        StartCoroutine(stageTransitionOut);
    }

    IEnumerator ChangeStageParameter(int stage) {
        
        stageSource.getParameterByName("stage_transit", out float currentStage);
        float targetStage = Convert.ToSingle(stage);
        Debug.Log(targetStage);
        while(Math.Abs(targetStage - currentStage) >= 0.01) {
            currentStage += (targetStage - currentStage) * 0.02f;
            stageSource.setParameterByName("stage_transit", currentStage);
            yield return new WaitForSeconds(0.01f);
        }
        stageSource.setParameterByName("stage_transit", targetStage);

    }

    IEnumerator ShopTransition(bool intoShop) {
        if(intoShop) {
            stageSource.getParameterByName("shop_transit", out float currentStage);
            while(currentStage < 1) {
                currentStage += 0.02f;
                stageSource.setParameterByName("shop_transit", currentStage);
                yield return new WaitForSeconds(0.01f);
                
            }
            currentStage = 1;
            stageSource.setParameterByName("shop_transit", currentStage);
        }
        else {
            stageSource.getParameterByName("shop_transit", out float currentStage);
            while(currentStage > 0) {
                currentStage -= 0.02f;
                stageSource.setParameterByName("shop_transit", currentStage);
                yield return new WaitForSeconds(0.01f);

            }
            currentStage = 0;
            stageSource.setParameterByName("shop_transit", currentStage);
        }
    }

    IEnumerator RechargeDelay(int count) {
        for(int i = 0; i < count; i++) {
            RuntimeManager.PlayOneShot("event:/recharge_block");
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void ChangeChapter(int chapter) {
        stageSource.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        stageSource.setUserData(IntPtr.Zero);
        timelineHandle.Free();
        stageSource.release();
        isPlaying = false;
        try {
            StopCoroutine(shopTransition);
        }
        catch(Exception e) {
            Debug.Log(e);
        }
        try {
            StopCoroutine(stageTransitionIn);
        }
        catch(Exception e) {
            Debug.Log(e);
        }
        try {
            StopCoroutine(stageTransitionOut);
        }
        catch(Exception e) {
            Debug.Log(e);
        }


        currentChapter = chapter % 2;
        if(currentChapter == 0) currentChapter = 2;
        stageSource = RuntimeManager.CreateInstance("event:/stage" + currentChapter);
        timelineHandle = GCHandle.Alloc(timelineInfo1, GCHandleType.Pinned);
        stageSource.setUserData(GCHandle.ToIntPtr(timelineHandle));
        stageSource.setCallback(stageSourceCallback, EVENT_CALLBACK_TYPE.TIMELINE_MARKER | EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
    }


    public void SFXSelectBlock() { // Hand에서 블록 집었을 때 재생
        RuntimeManager.PlayOneShot("event:/select_block");
    }

    public void SFXPlaceBlock() { // Hand에서 블록을 성공적으로 배치했을 때 재생
        RuntimeManager.PlayOneShot("event:/place_block");
    }

    public void SFXPlaceFail() { // 어떤 이유로든 배치에 실패하면 재생
        RuntimeManager.PlayOneShot("event:/place_fail");
    }

    public void SFXRechargeBlock(int count) { // 손으로 블록 카드 드로우할 때 재생(한 장당 하나)
        StartCoroutine(RechargeDelay(count));
    }

    public void SFXSelectMenu() { // 메뉴에서 그냥 버튼을 누르기는 하는데 딱히 중요하지 않은 버튼에 재생
        RuntimeManager.PlayOneShot("event:/select_menu");
    }

    public void SFXShopBuy() { // 상점 구매 효과음
        RuntimeManager.PlayOneShot("event:/shop_buy");
    }

    public void SFXShopFail() { // 상점 구매 실패 효과음
        SFXPlaceFail(); // 임시로 배치 실패 효과음으로 대체
    }

    public void SFXMatch(int quantity = 8) { // 매치해서 터질 때 재생
        EventInstance matchSFX = RuntimeManager.CreateInstance("event:/match");
        matchSFX.setParameterByName("quantity", quantity);
        matchSFX.start();
        matchSFX.release();
    }

    public void SFXtransition() { // 상점 -> 보드 뭐 이런 전환 시에
        EventInstance transitionSFX = RuntimeManager.CreateInstance("event:/transition");
        int barMod = timelineInfo1.currentMusicBar % 8;
        Debug.Log("barMod: " + barMod);
        if(barMod == 1 || barMod == 2) {
            transitionSFX.setParameterByName("transition", 0);
        }
        else if(barMod == 3 || barMod == 4) {
            transitionSFX.setParameterByName("transition", 1);
        }
        else {
            transitionSFX.setParameterByName("transition", 2);
        }
        transitionSFX.start();
        transitionSFX.release();
    }

    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr) {
        EventInstance instance = new EventInstance(instancePtr);
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        if(result != FMOD.RESULT.OK) {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if(timelineInfoPtr != IntPtr.Zero) {
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch(type) {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentMusicBar = parameter.bar;
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.lastMarker = parameter.name;
                    }
                    break;
            }
        }
        return FMOD.RESULT.OK;
   
    }

    

    // Update is called once per frame
    void Update()
    {
    }
}

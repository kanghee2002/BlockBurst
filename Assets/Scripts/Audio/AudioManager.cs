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
    public static AudioManager Instance;

    private static bool isPlaying = false;
    private RuntimeManager runtimeManager;

    private EventInstance stage1;

    private GCHandle timelineHandle;

    private TimelineInfo timelineInfo;


    private float currentTempo;

    private FMOD.Studio.EVENT_CALLBACK stage1Callback;
    

    void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(this.gameObject);
        }
        else {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        
    }
    void Start()
    {
        timelineInfo = new TimelineInfo();
        stage1Callback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        stage1.setParameterByName("shop_transit", 0);
        stage1 = RuntimeManager.CreateInstance("event:/stage1");
        timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
        stage1.setUserData(GCHandle.ToIntPtr(timelineHandle));
        stage1.setCallback(stage1Callback, EVENT_CALLBACK_TYPE.TIMELINE_MARKER | EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
    }

    void OnDestroy() {
        stage1.setUserData(IntPtr.Zero);
        timelineHandle.Free();
    }

    public void BeginBackgroundMusic() {
        if(!isPlaying) {
            stage1.start();
            isPlaying = true;
        }
    }

    public void StopBackgroundMusic() {
        if(isPlaying)
        {
            stage1.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isPlaying = false;
        }

    }

    public void ChangeStage(int stage) {
        StartCoroutine(ChangeStageParameter(stage));
    }

    public void ShopTransitionIn() {
        StartCoroutine(ShopTransition(true));
    }

    public void ShopTransitionOut() {
        StartCoroutine(ShopTransition(false));
    }

    IEnumerator ChangeStageParameter(int stage) {
        
        stage1.getParameterByName("stage_transit", out float currentStage);
        float targetStage = stage;
        while(Math.Abs(targetStage - currentStage) <= 0.01) {
            stage1.setParameterByName("stage_transit", Mathf.Lerp(currentStage, targetStage, 0.02f));
            yield return new WaitForSeconds(0.01f);
        }
        stage1.setParameterByName("stage_transit", targetStage);
    }

    IEnumerator ShopTransition(bool intoShop) {
        if(intoShop) {
            stage1.getParameterByName("shop_transit", out float currentStage);
            while(currentStage < 1) {
                currentStage += 0.02f;
                stage1.setParameterByName("shop_transit", currentStage);
                yield return new WaitForSeconds(0.01f);
                
            }
            currentStage = 1;
            stage1.setParameterByName("shop_transit", currentStage);
        }
        else {
            stage1.getParameterByName("shop_transit", out float currentStage);
            while(currentStage > 0) {
                currentStage -= 0.02f;
                stage1.setParameterByName("shop_transit", currentStage);
                yield return new WaitForSeconds(0.01f);

            }
            currentStage = 0;
            stage1.setParameterByName("shop_transit", currentStage);
        }
    }

    IEnumerator RechargeDelay(int count) {
        for(int i = 0; i < count; i++) {
            RuntimeManager.PlayOneShot("event:/recharge_block");
            yield return new WaitForSeconds(0.2f);
        }
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

    public void SFXMatch(int quantity = 8) { // 매치해서 터질 때 재생
        EventInstance matchSFX = RuntimeManager.CreateInstance("event:/match");
        matchSFX.setParameterByName("quantity", quantity);
        matchSFX.start();
        matchSFX.release();
    }

    public void SFXtransition() { // 상점 -> 보드 뭐 이런 전환 시에
        EventInstance transitionSFX = RuntimeManager.CreateInstance("event:/transition");
        int barMod = timelineInfo.currentMusicBar % 4;
        Debug.Log("barMod: " + barMod);
        if(barMod == 1) {
            transitionSFX.setParameterByName("transition", 0);
        }
        else if(barMod == 2) {
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

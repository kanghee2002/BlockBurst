using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;
using System.Runtime.InteropServices;

public class AudioManager : MonoBehaviour
{
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

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        StartCoroutine(InitializeFMOD());
    }

    private IEnumerator InitializeFMOD()
    {
        // FMOD 시스템이 완전히 초기화될 때까지 대기
        yield return new WaitForSeconds(0.1f);

        Debug.Log("Initializing FMOD...");
        currentChapter = 1;
        timelineInfo1 = new TimelineInfo();
        stageSourceCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        try
        {
            string eventPath = "event:/stage" + currentChapter;
            Debug.Log($"Creating FMOD event instance: {eventPath}");
            stageSource = RuntimeManager.CreateInstance(eventPath);
            
            if (!stageSource.isValid())
            {
                Debug.LogError("Failed to create FMOD event instance");
                yield break;
            }

            Debug.Log("Successfully created FMOD event instance");
            stageSource.setParameterByName("shop_transit", 0);
            timelineHandle = GCHandle.Alloc(timelineInfo1, GCHandleType.Pinned);
            stageSource.setUserData(GCHandle.ToIntPtr(timelineHandle));
            stageSource.setCallback(stageSourceCallback, EVENT_CALLBACK_TYPE.TIMELINE_MARKER | EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        }
        catch (Exception e)
        {
            Debug.LogError($"FMOD initialization error: {e.Message}");
        }
    }

    private IEnumerator shopTransition;
    private IEnumerator stageTransitionIn;
    private IEnumerator stageTransitionOut;
    private IEnumerator talker;

    void OnDestroy()
    {
        if (stageSource.isValid())
        {
            stageSource.setUserData(IntPtr.Zero);
            stageSource.release();
        }
        if (timelineHandle.IsAllocated)
        {
            timelineHandle.Free();
        }
    }

    public void ChangeBGMVolume(float volume)
    {
        RuntimeManager.GetVCA("vca:/BGM").setVolume(volume);
    }

    public void ChangeSFXVolume(float volume)
    {
        RuntimeManager.GetVCA("vca:/SFX").setVolume(volume);
    }

    public void RestartGame()
    {
        StopBackgroundMusic();
        ChangeChapter(1);
    }

    public void BeginBackgroundMusic()
    {
        if (!isPlaying && stageSource.isValid())
        {
            stageSource.start();
            isPlaying = true;
        }
    }

    public void StopBackgroundMusic()
    {
        if (isPlaying && stageSource.isValid())
        {
            stageSource.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isPlaying = false;
        }
    }

    public void TutorialTalker(string text, float interval = 0.075f)
    {
        if (talker != null)
        {
            StopCoroutine(talker);
        }
        talker = TutorialVoiceActuator(text, interval);
        StartCoroutine(talker);
    }

    IEnumerator TutorialVoiceActuator(string text, float interval)
    {
        int count = 0;
        foreach (char c in text)
        {
            if (c != ' ')
            {
                RuntimeManager.PlayOneShot("event:/tutorial_talk");
                count++;
                if (count >= 7)
                {
                    break;
                }
            }
            yield return new WaitForSeconds(interval);
        }
    }

    public void ChangeStage(int stage)
    {
        shopTransition = ChangeStageParameter(stage);
        StartCoroutine(shopTransition);
    }

    public void ShopTransitionIn()
    {
        stageTransitionIn = ShopTransition(true);
        StartCoroutine(stageTransitionIn);
    }

    public void ShopTransitionOut()
    {
        stageTransitionOut = ShopTransition(false);
        StartCoroutine(stageTransitionOut);
    }

    IEnumerator ChangeStageParameter(int stage)
    {
        if (!stageSource.isValid())
        {
            Debug.LogError("Stage source is not valid");
            yield break;
        }

        stageSource.getParameterByName("stage_transit", out float currentStage);
        float targetStage = Convert.ToSingle(stage);
        Debug.Log($"Changing stage parameter from {currentStage} to {targetStage}");

        while (Math.Abs(targetStage - currentStage) >= 0.01)
        {
            currentStage += (targetStage - currentStage) * 0.02f;
            stageSource.setParameterByName("stage_transit", currentStage);
            yield return new WaitForSeconds(0.01f);
        }
        stageSource.setParameterByName("stage_transit", targetStage);
    }

    IEnumerator ShopTransition(bool intoShop)
    {
        if (!stageSource.isValid())
        {
            Debug.LogError("Stage source is not valid");
            yield break;
        }

        if (intoShop)
        {
            stageSource.getParameterByName("shop_transit", out float currentStage);
            while (currentStage < 1)
            {
                currentStage += 0.02f;
                stageSource.setParameterByName("shop_transit", currentStage);
                yield return new WaitForSeconds(0.01f);
            }
            currentStage = 1;
            stageSource.setParameterByName("shop_transit", currentStage);
        }
        else
        {
            stageSource.getParameterByName("shop_transit", out float currentStage);
            while (currentStage > 0)
            {
                currentStage -= 0.02f;
                stageSource.setParameterByName("shop_transit", currentStage);
                yield return new WaitForSeconds(0.01f);
            }
            currentStage = 0;
            stageSource.setParameterByName("shop_transit", currentStage);
        }
    }

    IEnumerator RechargeDelay(int count)
    {
        for (int i = 0; i < count; i++)
        {
            RuntimeManager.PlayOneShot("event:/recharge_block");
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void ChangeChapter(int chapter)
    {
        Debug.Log($"Changing chapter to: {chapter}");

        if (stageSource.isValid())
        {
            Debug.Log("Stopping and cleaning up previous stage source");
            stageSource.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            stageSource.setUserData(IntPtr.Zero);
            stageSource.release();
        }

        if (timelineHandle.IsAllocated)
        {
            timelineHandle.Free();
        }

        isPlaying = false;

        try
        {
            if (shopTransition != null)
                StopCoroutine(shopTransition);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Error stopping shopTransition: {e.Message}");
        }

        try
        {
            if (stageTransitionIn != null)
                StopCoroutine(stageTransitionIn);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Error stopping stageTransitionIn: {e.Message}");
        }

        try
        {
            if (stageTransitionOut != null)
                StopCoroutine(stageTransitionOut);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Error stopping stageTransitionOut: {e.Message}");
        }

        currentChapter = chapter % 2;
        if (currentChapter == 0) currentChapter = 2;

        string eventPath = "event:/stage" + currentChapter;
        Debug.Log($"Creating new instance for stage: {eventPath}");
        
        try
        {
            stageSource = RuntimeManager.CreateInstance(eventPath);
            if (!stageSource.isValid())
            {
                Debug.LogError($"Failed to create valid instance for {eventPath}");
                return;
            }
            
            Debug.Log("Successfully created new stage instance");
            timelineHandle = GCHandle.Alloc(timelineInfo1, GCHandleType.Pinned);
            stageSource.setUserData(GCHandle.ToIntPtr(timelineHandle));
            stageSource.setCallback(stageSourceCallback, EVENT_CALLBACK_TYPE.TIMELINE_MARKER | EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error creating new stage instance: {e.Message}");
        }
    }

    public void SFXSelectBlock()
    {
        RuntimeManager.PlayOneShot("event:/select_block");
    }

    public void SFXPlaceBlock()
    {
        RuntimeManager.PlayOneShot("event:/place_block");
    }

    public void SFXPlaceFail()
    {
        RuntimeManager.PlayOneShot("event:/place_fail");
    }

    public void SFXRechargeBlock(int count)
    {
        StartCoroutine(RechargeDelay(count));
    }

    public void SFXSelectMenu()
    {
        RuntimeManager.PlayOneShot("event:/select_menu");
    }

    public void SFXShopBuy()
    {
        RuntimeManager.PlayOneShot("event:/shop_buy");
    }

    public void SFXShopFail()
    {
        SFXPlaceFail();
    }

    public void SFXMatch(int quantity = 8)
    {
        EventInstance matchSFX = RuntimeManager.CreateInstance("event:/match");
        matchSFX.setParameterByName("quantity", quantity);
        matchSFX.start();
        matchSFX.release();
    }

    public void SFXtransition()
    {
        EventInstance transitionSFX = RuntimeManager.CreateInstance("event:/transition");
        int barMod = timelineInfo1.currentMusicBar % 8;
        Debug.Log("barMod: " + barMod);
        
        if (barMod == 1 || barMod == 2)
        {
            transitionSFX.setParameterByName("transition", 0);
        }
        else if (barMod == 3 || barMod == 4)
        {
            transitionSFX.setParameterByName("transition", 1);
        }
        else
        {
            transitionSFX.setParameterByName("transition", 2);
        }
        
        transitionSFX.start();
        transitionSFX.release();
    }

    public void SFXStageClear()
    {
        RuntimeManager.PlayOneShot("event:/stage_clear");
    }

    public void SFXGameOver()
    {
        RuntimeManager.PlayOneShot("event:/game_over");
        if (stageSource.isValid())
        {
            stageSource.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void SFXGameWin()
    {
        RuntimeManager.PlayOneShot("event:/game_win");
    }

    public void SFXEffectON()
    {
        RuntimeManager.PlayOneShot("event:/effect_on");
    }

    public void SFXThrowItem()
    {
        RuntimeManager.PlayOneShot("event:/throw_item");
    }

    public void SFXGold()
    {
        RuntimeManager.PlayOneShot("event:/gold");
    }

    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        EventInstance instance = new EventInstance(instancePtr);
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
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
}
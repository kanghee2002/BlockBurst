using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering.LookDev;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance;
    [SerializeField] private Material material;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(FadeIn(1f));
    }

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // (선택 사항) 전환 효과를 위해 페이드 아웃 코루틴 호출 가능
        yield return StartCoroutine(FadeOut(0.5f));

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        // 로딩이 완료될 때까지 대기
        while (!asyncOperation.isDone)
        {
            // 로딩 진행 상황에 따라 UI 업데이트 가능
            // 예: progressBar.fillAmount = asyncOperation.progress;
            yield return null;
        }
        
        // (선택 사항) 씬 로딩 완료 후 페이드 인 코루틴 호출 가능
        yield return StartCoroutine(FadeIn(0.5f));
    }
    private IEnumerator FadeOut(float duration = 1f)
    {
        float timer = duration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            material.SetFloat("_Scroll", timer / duration);
            yield return null;
        }
    }
    private IEnumerator FadeIn(float duration = 1f)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            material.SetFloat("_Scroll", timer / duration);
            yield return null;
        }
    }
}

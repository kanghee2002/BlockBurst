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
        //StartCoroutine(FadeIn(0.1f));
    }

    private bool transitionMutex = true;
    public void TransitionToScene(string sceneName)
    {
        if (transitionMutex == true)
        {
            transitionMutex = false;
            StartCoroutine(LoadSceneAsync(sceneName));
        }
    }
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // (���� ����) ��ȯ ȿ���� ���� ���̵� �ƿ� �ڷ�ƾ ȣ�� ����
        yield return StartCoroutine(FadeOut(0.5f));

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        // �ε��� �Ϸ�� ������ ���
        while (!asyncOperation.isDone)
        {
            // �ε� ���� ��Ȳ�� ���� UI ������Ʈ ����
            // ��: progressBar.fillAmount = asyncOperation.progress;
            yield return null;
        }
        
        // (���� ����) �� �ε� �Ϸ� �� ���̵� �� �ڷ�ƾ ȣ�� ����
        yield return StartCoroutine(FadeIn(0.5f));

        transitionMutex = true;
        yield return null;
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

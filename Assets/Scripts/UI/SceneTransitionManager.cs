using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance;
    [SerializeField] private Image transitionImage;
    [SerializeField] private Image topPanel;
    [SerializeField] private Image bottomPanel;
    [SerializeField] private Image leftPanel;
    [SerializeField] private Image rightPanel;
    
    private Vector2 originalSize;
    private RectTransform rectTransform;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            rectTransform = transitionImage.rectTransform;
            SetTransitionImageSize();
            originalSize = rectTransform.sizeDelta;
            SetPanelsInitialPosition();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetTransitionImageSize()
    {
        // 화면의 대각선 길이보다 더 크게 설정하여 회전해도 빈 공간이 안 생기게 함
        float screenDiagonal = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
        float imageSize = screenDiagonal * 1.5f; // 여유있게 1.5배
        rectTransform.sizeDelta = new Vector2(imageSize, imageSize);
    }
    private void SetPanelsInitialPosition()
    {
        // 패널들을 처음부터 화면보다 훨씬 크게 설정
        float screenSize = Mathf.Max(Screen.width, Screen.height) * 3f;
        float overlap = 2f;  // 겹치는 정도

        // 모든 패널 크기 설정
        topPanel.rectTransform.sizeDelta = new Vector2(screenSize, screenSize);
        bottomPanel.rectTransform.sizeDelta = new Vector2(screenSize, screenSize);
        leftPanel.rectTransform.sizeDelta = new Vector2(screenSize, screenSize);
        rightPanel.rectTransform.sizeDelta = new Vector2(screenSize, screenSize);

        // pivot 설정
        topPanel.rectTransform.pivot = new Vector2(0.5f, 0f);
        bottomPanel.rectTransform.pivot = new Vector2(0.5f, 1f);
        leftPanel.rectTransform.pivot = new Vector2(1f, 0.5f);
        rightPanel.rectTransform.pivot = new Vector2(0f, 0.5f);

        // 시작 위치 설정 (약간 겹치게)
        topPanel.rectTransform.anchoredPosition = new Vector2(0, originalSize.y * 0.5f - overlap);
        bottomPanel.rectTransform.anchoredPosition = new Vector2(0, -originalSize.y * 0.5f + overlap);
        leftPanel.rectTransform.anchoredPosition = new Vector2(-originalSize.x * 0.5f + overlap, 0);
        rightPanel.rectTransform.anchoredPosition = new Vector2(originalSize.x * 0.5f - overlap, 0);
    }

    private void UpdatePanelsPosition(Vector2 currentSize)
    {
        float halfSize = currentSize.x * 0.5f;
        float overlap = 2f;  // 겹치는 정도
        
        // 패널 위치 업데이트 (약간 겹치게)
        topPanel.rectTransform.anchoredPosition = new Vector2(0, halfSize - overlap);
        bottomPanel.rectTransform.anchoredPosition = new Vector2(0, -halfSize + overlap);
        leftPanel.rectTransform.anchoredPosition = new Vector2(-halfSize + overlap, 0);
        rightPanel.rectTransform.anchoredPosition = new Vector2(halfSize - overlap, 0);
    }

    private bool transitionMutex = true;
    public void TransitionToScene(string sceneName)
    {
        if (transitionMutex)
        {
            transitionMutex = false;
            StartCoroutine(LoadSceneAsync(sceneName));
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        yield return StartCoroutine(Shrink(0.5f));

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(Expand(0.5f));
        transitionMutex = true;
    }

    private IEnumerator Shrink(float duration)
    {
        float timer = duration;
        Vector2 targetSize = originalSize * 0.01f;
        
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            float progress = timer / duration;
            Vector2 newSize = Vector2.Lerp(targetSize, originalSize, progress);
            rectTransform.sizeDelta = newSize;
            UpdatePanelsPosition(newSize);
            yield return null;
        }
    }

    private IEnumerator Expand(float duration)
    {
        float timer = 0f;
        Vector2 targetSize = originalSize * 0.01f;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            Vector2 newSize = Vector2.Lerp(targetSize, originalSize, progress);
            rectTransform.sizeDelta = newSize;
            UpdatePanelsPosition(newSize);
            yield return null;
        }
    }
}
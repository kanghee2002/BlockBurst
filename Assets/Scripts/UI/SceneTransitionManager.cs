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
    private Scene currentLoadedScene;  // 현재 로드된 씬을 추적

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
            // 초기 씬 설정
            currentLoadedScene = SceneManager.GetActiveScene();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetTransitionImageSize()
    {
        float screenDiagonal = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
        float imageSize = screenDiagonal * 1.5f;
        rectTransform.sizeDelta = new Vector2(imageSize, imageSize);
    }

    private void SetPanelsInitialPosition()
    {
        float screenSize = Mathf.Max(Screen.width, Screen.height) * 3f;
        float overlap = 2f;

        topPanel.rectTransform.sizeDelta = new Vector2(screenSize, screenSize);
        bottomPanel.rectTransform.sizeDelta = new Vector2(screenSize, screenSize);
        leftPanel.rectTransform.sizeDelta = new Vector2(screenSize, screenSize);
        rightPanel.rectTransform.sizeDelta = new Vector2(screenSize, screenSize);

        topPanel.rectTransform.pivot = new Vector2(0.5f, 0f);
        bottomPanel.rectTransform.pivot = new Vector2(0.5f, 1f);
        leftPanel.rectTransform.pivot = new Vector2(1f, 0.5f);
        rightPanel.rectTransform.pivot = new Vector2(0f, 0.5f);

        topPanel.rectTransform.anchoredPosition = new Vector2(0, originalSize.y * 0.5f - overlap);
        bottomPanel.rectTransform.anchoredPosition = new Vector2(0, -originalSize.y * 0.5f + overlap);
        leftPanel.rectTransform.anchoredPosition = new Vector2(-originalSize.x * 0.5f + overlap, 0);
        rightPanel.rectTransform.anchoredPosition = new Vector2(originalSize.x * 0.5f - overlap, 0);
    }

    private void UpdatePanelsPosition(Vector2 currentSize)
    {
        float halfSize = currentSize.x * 0.5f;
        float overlap = 2f;
        
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
        // 1. 전환 효과 시작
        yield return StartCoroutine(Shrink(0.5f));

        Scene currentScene = SceneManager.GetActiveScene();
        bool isSameScene = currentScene.name == sceneName;

        // 2. 새 씬을 로드
        AsyncOperation asyncLoad;
        if (isSameScene)
        {
            // 같은 씬일 경우 Single 모드로 로드하여 완전한 초기화 보장
            asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }
        else 
        {
            asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (!isSameScene)
        {
            // 다른 씬일 경우에만 Additive 로드 후처리
            Scene newScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(newScene);

            if (currentLoadedScene.isLoaded)
            {
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentLoadedScene);
                while (!asyncUnload.isDone)
                {
                    yield return null;
                }
            }
            currentLoadedScene = newScene;
        }
        else
        {
            // 같은 씬일 경우 현재 씬 참조 업데이트
            currentLoadedScene = SceneManager.GetActiveScene();
        }

        // 3. 전환 효과 종료
        yield return StartCoroutine(Expand(0.5f));
        transitionMutex = true;

        // 4. 가비지 컬렉션 요청
        System.GC.Collect();
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
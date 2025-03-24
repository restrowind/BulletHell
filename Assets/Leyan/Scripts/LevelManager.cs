using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("UI 过场设置")]
    public CanvasGroup blackoutCanvas; // 子物体上的黑幕Canvas（含 Image + CanvasGroup）
    public float fadeDuration = 0.5f;

    public int levelIndex = 0;
    public bool choosingCard = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void LoadNextLevel()
    {
        if (levelIndex == 0)
        {
            levelIndex = 1;
            LoadLevel("Level1");
        }
        else if(levelIndex<3)
        {

            levelIndex++;
            LoadLevel("Level" + levelIndex.ToString());

        }
        else
        {
            LoadLevel("StartLevel");
        }
    }

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(TransitionAndLoad(sceneName));
    }

    private IEnumerator TransitionAndLoad(string sceneName)
    {
        // 黑幕淡入
        yield return StartCoroutine(FadeCanvas(1f));

        // 异步加载关卡
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 等待一帧，确保新场景激活
        yield return null;

        // 黑幕淡出
        yield return StartCoroutine(FadeCanvas(0f));

        // 场景加载完成后的初始化逻辑可以写在新场景的 Start/Awake 中
    }

    private IEnumerator FadeCanvas(float targetAlpha)
    {
        float startAlpha = blackoutCanvas.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            blackoutCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            yield return null;
        }

        blackoutCanvas.alpha = targetAlpha;
        blackoutCanvas.blocksRaycasts = targetAlpha > 0.5f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            LoadNextLevel();
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("UI ��������")]
    public CanvasGroup blackoutCanvas; // �������ϵĺ�ĻCanvas���� Image + CanvasGroup��
    public float fadeDuration = 0.5f;

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

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(TransitionAndLoad(sceneName));
    }

    private IEnumerator TransitionAndLoad(string sceneName)
    {
        // ��Ļ����
        yield return StartCoroutine(FadeCanvas(1f));

        // �첽���عؿ�
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // �ȴ�һ֡��ȷ���³�������
        yield return null;

        // ��Ļ����
        yield return StartCoroutine(FadeCanvas(0f));

        // ����������ɺ�ĳ�ʼ���߼�����д���³����� Start/Awake ��
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
}

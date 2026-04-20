using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public static SceneFader instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeIn()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            t = t * t * (3f - 2f * t);

            SetAlpha(1f - t);

            time += Time.deltaTime;
            yield return null;
        }

        SetAlpha(0f);
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            t = t * t * (3f - 2f * t);

            SetAlpha(t);

            time += Time.deltaTime;
            yield return null;
        }

        SetAlpha(1f);
        SceneManager.LoadScene(sceneName);
    }

    private void SetAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}
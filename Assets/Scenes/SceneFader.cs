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
        SetAlpha(1f);
    }
    private void Start()
    {
        StartCoroutine(FadeInRoutine());
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoadRoutine(sceneName));
    }

    public void QuitWithFade()
    {
        StartCoroutine(QuitRoutine());
    }

    public void ShowGuideThenLoad(Image guideImage, string sceneName, float waitSeconds = 10f)
    {
        StartCoroutine(ShowGuideThenLoadRoutine(guideImage, sceneName, waitSeconds));
    }

    public void FadeToBlack()
    {
        StartCoroutine(FadeToBlackRoutine());
    }

    public void FadeFromBlack()
    {
        StartCoroutine(FadeFromBlackRoutine());
    }

    public IEnumerator ChangeImageSmooth(Image oldImage = null, Image newImage = null)
    {
        yield return StartCoroutine(FadeToBlackRoutine());

        if (oldImage != null)
        {
            oldImage.color = new Color(oldImage.color.r, oldImage.color.g, oldImage.color.b, 0f);
        }

        if (newImage != null)
        {
            newImage.color = new Color(newImage.color.r, newImage.color.g, newImage.color.b, 1f);
        }

        yield return StartCoroutine(FadeFromBlackRoutine());
    }

    private IEnumerator ShowGuideThenLoadRoutine(Image guideImage, string sceneName, float waitSeconds)
    {
        yield return StartCoroutine(FadeToBlackRoutine());

        if (guideImage != null)
        {
            guideImage.color = new Color(guideImage.color.r, guideImage.color.g, guideImage.color.b, 1f);
        }

        yield return StartCoroutine(FadeFromBlackRoutine());

        yield return new WaitForSecondsRealtime(waitSeconds);

        yield return StartCoroutine(FadeToBlackRoutine());

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeInRoutine()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            t = SmoothStep(t);

            SetAlpha(1f - t);

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        SetAlpha(0f);
    }

    private IEnumerator FadeOutAndLoadRoutine(string sceneName)
    {
        yield return StartCoroutine(FadeToBlackRoutine());
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator QuitRoutine()
    {
        yield return StartCoroutine(FadeToBlackRoutine());
        Application.Quit();
    }

    private IEnumerator FadeToBlackRoutine()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            t = SmoothStep(t);

            SetAlpha(t);

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        SetAlpha(1f);
    }

    private IEnumerator FadeFromBlackRoutine()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            t = SmoothStep(t);

            SetAlpha(1f - t);

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        if (fadeImage == null)
            return;

        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }

    private float SmoothStep(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t * (3f - 2f * t);
    }
}
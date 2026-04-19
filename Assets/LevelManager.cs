using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public Light2D light2D;

    public float minSize = 2f;
    public float maxSize = 6f;
    public float signalDuration = 1.5f;
    public float pauseDuration = 1f;

    public float startIntensity = 1f;
    public float endIntensity = 0f;

    public bool canUseSignal = true;

    void Start()
    {
        instance = this;

        light2D.pointLightInnerRadius = minSize;
        light2D.pointLightOuterRadius = minSize;
        light2D.intensity = 0f;
    }

    public void GameOver()
    {
        SceneManager.LoadScene("Level1");
    }

    public void GameFinished()
    {
    }

    public void MenuReturn()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void MakeSignal()
    {
        if (!canUseSignal)
            return;

        StartCoroutine(PulseRoutine());
    }

    IEnumerator PulseRoutine()
    {
        canUseSignal = false;

        light2D.pointLightInnerRadius = minSize;
        light2D.pointLightOuterRadius = minSize;
        light2D.intensity = startIntensity;

        yield return StartCoroutine(ExpandSignal());
        yield return new WaitForSeconds(pauseDuration);
        yield return StartCoroutine(FadeSignalOut());

        light2D.pointLightInnerRadius = minSize;
        light2D.pointLightOuterRadius = minSize;
        light2D.intensity = 0f;

        canUseSignal = true;
    }

    IEnumerator ExpandSignal()
    {
        float time = 0f;

        while (time < signalDuration)
        {
            float t = time / signalDuration;

            light2D.pointLightInnerRadius = 0f;
            light2D.pointLightOuterRadius = Mathf.Lerp(minSize, maxSize, t);
            light2D.intensity = startIntensity;

            time += Time.deltaTime;
            yield return null;
        }

        light2D.pointLightInnerRadius = 0f;
        light2D.pointLightOuterRadius = maxSize;
        light2D.intensity = startIntensity;
    }

    IEnumerator FadeSignalOut()
    {
        float time = 0f;
        float extraDistance = maxSize - minSize;

        while (time < signalDuration)
        {
            float t = time / signalDuration;

            light2D.pointLightInnerRadius = Mathf.Lerp(0f, maxSize, t);
            light2D.pointLightOuterRadius = Mathf.Lerp(maxSize, maxSize + extraDistance, t);
            light2D.intensity = Mathf.Lerp(startIntensity, endIntensity, t);

            time += Time.deltaTime;
            yield return null;
        }

        light2D.pointLightInnerRadius = maxSize + extraDistance;
        light2D.pointLightOuterRadius = maxSize + extraDistance;
        light2D.intensity = endIntensity;
    }
}
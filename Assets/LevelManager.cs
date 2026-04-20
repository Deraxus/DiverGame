using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Signal Light")]
    public Light2D signalLight;

    [Header("Signal Settings")]
    public float minRadius = 0f;
    public float maxRadius = 12f;
    public float expandDuration = 1f;
    public float holdDuration = 1.5f;
    public float shrinkDuration = 1f;
    public float signalIntensity = 1f;

    [Header("State")]
    public bool canUseSignal = true;

    [Header("UI")]
    public List<Image> bonusesUI = new List<Image>();

    private Coroutine signalCoroutine;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ResetSignalLight();
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

        if (signalCoroutine != null)
        {
            StopCoroutine(signalCoroutine);
        }

        signalCoroutine = StartCoroutine(SignalRoutine());
    }

    public Image ChooseLastActiveBonus()
    {
        for (int i = bonusesUI.Count - 1; i >= 0; i--)
        {
            if (bonusesUI[i] != null && bonusesUI[i].gameObject.activeInHierarchy)
            {
                return bonusesUI[i];
            }
        }

        return null;
    }

    public Image ChooseLastNonActiveBonus()
    {
        for (int i = 0; i < bonusesUI.Count; i++)
        {
            if (bonusesUI[i] != null && !bonusesUI[i].gameObject.activeInHierarchy)
            {
                return bonusesUI[i];
            }
        }

        return null;
    }

    private IEnumerator SignalRoutine()
    {
        canUseSignal = false;

        signalLight.enabled = true;
        signalLight.intensity = signalIntensity;
        signalLight.pointLightInnerRadius = 0f;
        signalLight.pointLightOuterRadius = minRadius;

        yield return StartCoroutine(ExpandSignal());
        yield return new WaitForSeconds(holdDuration);
        yield return StartCoroutine(ShrinkSignal());

        ResetSignalLight();

        canUseSignal = true;
        signalCoroutine = null;
    }

    private IEnumerator ExpandSignal()
    {
        float time = 0f;

        while (time < expandDuration)
        {
            float t = time / expandDuration;
            t = SmoothStep(t);

            float radius = Mathf.Lerp(minRadius, maxRadius, t);

            signalLight.pointLightInnerRadius = 0f;
            signalLight.pointLightOuterRadius = radius;
            signalLight.intensity = signalIntensity;

            time += Time.deltaTime;
            yield return null;
        }

        signalLight.pointLightInnerRadius = 0f;
        signalLight.pointLightOuterRadius = maxRadius;
        signalLight.intensity = signalIntensity;
    }

    private IEnumerator ShrinkSignal()
    {
        float time = 0f;

        while (time < shrinkDuration)
        {
            float t = time / shrinkDuration;
            t = SmoothStep(t);

            float radius = Mathf.Lerp(maxRadius, minRadius, t);

            signalLight.pointLightInnerRadius = 0f;
            signalLight.pointLightOuterRadius = radius;
            signalLight.intensity = signalIntensity;

            time += Time.deltaTime;
            yield return null;
        }

        signalLight.pointLightInnerRadius = 0f;
        signalLight.pointLightOuterRadius = minRadius;
        signalLight.intensity = signalIntensity;
    }

    private void ResetSignalLight()
    {
        signalLight.pointLightInnerRadius = 0f;
        signalLight.pointLightOuterRadius = 0f;
        signalLight.intensity = 0f;
        signalLight.enabled = false;
    }

    private float SmoothStep(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t * (3f - 2f * t);
    }
}
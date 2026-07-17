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
    public float minScale = 0f;
    public float maxScale = 12f;
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
        AudioManager.instance.GetComponent<AudioSource>().Stop();
        AudioManager.instance.firstsource.Stop();
        AudioManager.instance.secondsource.Stop();
        SceneManager.LoadScene("Dying");
    }

    public void GameFinished()
    {
    }

    public void MenuReturn()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void MakeSignal(bool isLevelEnd = false)
    {
        if (!canUseSignal)
            return;

        if (signalCoroutine != null)
        {
            StopCoroutine(signalCoroutine);
        }

        signalCoroutine = StartCoroutine(SignalRoutine(isLevelEnd));
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

    private IEnumerator SignalRoutine(bool isLevelEnd)
    {
        canUseSignal = false;

        signalLight.enabled = true;
        signalLight.intensity = signalIntensity;
        signalLight.transform.localScale = Vector3.one * minScale;

        yield return StartCoroutine(ExpandSignal(isLevelEnd ? 3f : 1f));

        if (!isLevelEnd)
        {
            yield return new WaitForSeconds(holdDuration);
            yield return StartCoroutine(ShrinkSignal());

            ResetSignalLight();
            canUseSignal = true;
            signalCoroutine = null;
        }
        else
        {
            signalLight.transform.localScale = Vector3.one * maxScale;
            signalLight.intensity = signalIntensity;

            signalCoroutine = null;
        }
    }

    private IEnumerator ExpandSignal(float durationMultiplier = 1f)
    {
        float time = 0f;
        float targetDuration = expandDuration * durationMultiplier;

        while (time < targetDuration)
        {
            float t = time / targetDuration;
            t = SmoothStep(t);

            float currentScale = Mathf.Lerp(minScale, maxScale, t);

            signalLight.transform.localScale = Vector3.one * currentScale;
            signalLight.intensity = signalIntensity;

            time += Time.deltaTime;
            yield return null;
        }

        signalLight.transform.localScale = Vector3.one * maxScale;
        signalLight.intensity = signalIntensity;
    }

    private IEnumerator ShrinkSignal()
    {
        float time = 0f;

        while (time < shrinkDuration)
        {
            float t = time / shrinkDuration;
            t = SmoothStep(t);

            float currentScale = Mathf.Lerp(maxScale, minScale, t);

            signalLight.transform.localScale = Vector3.one * currentScale;
            signalLight.intensity = signalIntensity;

            time += Time.deltaTime;
            yield return null;
        }

        signalLight.transform.localScale = Vector3.one * minScale;
        signalLight.intensity = signalIntensity;
    }

    private void ResetSignalLight()
    {
        signalLight.transform.localScale = Vector3.one * minScale;
        signalLight.intensity = 0f;
        signalLight.enabled = false;
    }

    private float SmoothStep(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t * (3f - 2f * t);
    }
}
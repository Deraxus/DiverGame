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
    public float signalDuration = 2f;
    public float pauseDuration = 5f;

    public bool canUseSignal = true;
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameOver()
    {
        // Затемнение экрана
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
        // Проиграть звук
        // Осветить локацию
        StartCoroutine(PulseRoutine());
    }
    
    IEnumerator PulseRoutine()
    {
        canUseSignal = false;
        // УВЕЛИЧЕНИЕ
        yield return StartCoroutine(LerpLight(minSize, maxSize, signalDuration));

        // ПАУЗА
        yield return new WaitForSeconds(pauseDuration);

        // УМЕНЬШЕНИЕ
        yield return StartCoroutine(LerpLight(maxSize, minSize, signalDuration, true));
    }
    
    IEnumerator LerpLight(float start, float end, float duration, bool changeSignal = false)
    {
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;

            light2D.pointLightInnerRadius = Mathf.Lerp(start, end, t);
            light2D.pointLightOuterRadius = Mathf.Lerp(start, end, t);

            time += Time.deltaTime;
            yield return null;
        }

        // чтобы точно дошло до конца
        light2D.pointLightOuterRadius = end;
        if (changeSignal)
        {
            canUseSignal = !canUseSignal;
        }
    }
    
}

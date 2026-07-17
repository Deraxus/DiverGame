using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FishMouth : MonoBehaviour
{
    [SerializeField] private Transform upperJaw;
    [SerializeField] private Transform lowerJaw;
    [SerializeField] private float delayBeforeClose = 0.5f; // Задержка перед закрытием в секундах
    [SerializeField] private float closeSpeed = 300f; // Скорость закрытия пасти (градусов в секунду)
    [SerializeField] private float openSpeed = 300f; // Скорость открытия пасти (градусов в секунду)
    
    [Header("End Game Settings")]
    [SerializeField] private Image endGameImage; // Image для затемнения
    [SerializeField] private float endGameFadeSpeed = 0.5f; // Скорость появления Image (в секундах)
    [SerializeField] private float delayAfterClose = 3f; // Задержка после закрытия пасти перед завершением
    
    private Vector3 upperJawStartRot;
    private Vector3 lowerJawStartRot;
    private Coroutine currentAnimation;
    
    public static FishMouth instance;
    
    private void Start()
    {
        upperJawStartRot = upperJaw.localEulerAngles;
        lowerJawStartRot = lowerJaw.localEulerAngles;
        instance = this;
        
        // Убедимся, что Image изначально полностью прозрачный
        if (endGameImage != null)
        {
            Color color = endGameImage.color;
            color.a = 0f;
            endGameImage.color = color;
        }
    }
    
    public void CloseMouth(float angleInDegrees)
    {
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(AnimateMouthWithDelay(angleInDegrees));
    }
    
    public void OpenMouth()
    {
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(AnimateMouthOpen());
    }
    
    /// <summary>
    /// Метод завершения игры - плавно делает Image непрозрачным
    /// </summary>
    public void EndGame()
    {
        if (endGameImage == null)
        {
            Debug.LogError("FishMouth: endGameImage не назначен в инспекторе!");
            return;
        }
        
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        StartCoroutine(FadeInImage());
    }
    
    /// <summary>
    /// Плавное появление Image
    /// </summary>
    private IEnumerator FadeInImage()
    {
        Color color = endGameImage.color;
        color.a = 0f;
        endGameImage.color = color;
        endGameImage.gameObject.SetActive(true);
        
        float elapsed = 0f;
        
        while (elapsed < endGameFadeSpeed)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / endGameFadeSpeed);
            color.a = alpha;
            endGameImage.color = color;
            yield return null;
        }
        
        // Финальная установка полностью непрозрачного
        color.a = 1f;
        endGameImage.color = color;
        
        // Здесь можно добавить дополнительную логику, например:
        // Time.timeScale = 0f; // Остановить игру
        // SceneManager.LoadScene("MenuScene"); // Загрузить меню
        Debug.Log("Игра завершена!");
        SceneManager.LoadScene("Win");
    }
    
    // Анимация с задержкой перед закрытием
    private IEnumerator AnimateMouthWithDelay(float targetAngle)
    {
        // Ждем задержку перед началом анимации
        yield return new WaitForSeconds(delayBeforeClose);
        
        upperJaw.GetComponent<PolygonCollider2D>().enabled = false;
        lowerJaw.GetComponent<PolygonCollider2D>().enabled = false;
        // Запускаем саму анимацию
        yield return StartCoroutine(AnimateMouthClose(targetAngle));
        
        // Ждем указанную задержку после закрытия пасти
        yield return new WaitForSeconds(delayAfterClose);
        
        // Вызываем завершение игры
        EndGame();
        
        currentAnimation = null;
    }
    
    // Анимация закрытия пасти
    private IEnumerator AnimateMouthClose(float targetAngle)
    {
        float upperTargetZ = upperJawStartRot.z - targetAngle;
        float lowerTargetZ = lowerJawStartRot.z + targetAngle;
        
        // Продолжаем анимацию пока не достигнем целевых углов
        while (Mathf.Abs(Mathf.DeltaAngle(upperJaw.localEulerAngles.z, upperTargetZ)) > 0.01f ||
               Mathf.Abs(Mathf.DeltaAngle(lowerJaw.localEulerAngles.z, lowerTargetZ)) > 0.01f)
        {
            // Поворачиваем челюсти с заданной скоростью
            float upperCurrentZ = upperJaw.localEulerAngles.z;
            float lowerCurrentZ = lowerJaw.localEulerAngles.z;
            
            float newUpperZ = Mathf.MoveTowardsAngle(upperCurrentZ, upperTargetZ, closeSpeed * Time.deltaTime);
            float newLowerZ = Mathf.MoveTowardsAngle(lowerCurrentZ, lowerTargetZ, closeSpeed * Time.deltaTime);
            
            upperJaw.localEulerAngles = new Vector3(upperJawStartRot.x, upperJawStartRot.y, newUpperZ);
            lowerJaw.localEulerAngles = new Vector3(lowerJawStartRot.x, lowerJawStartRot.y, newLowerZ);
            
            yield return null;
        }
        
        // Финальная установка точных значений
        upperJaw.localEulerAngles = new Vector3(upperJawStartRot.x, upperJawStartRot.y, upperTargetZ);
        lowerJaw.localEulerAngles = new Vector3(lowerJawStartRot.x, lowerJawStartRot.y, lowerTargetZ);

        DiverMovement.instance.maxSpeed = 0;
        DiverMovement.instance.acceleration = 0;
    }
    
    // Анимация открытия пасти
    private IEnumerator AnimateMouthOpen()
    {
        // Продолжаем анимацию пока не вернемся в начальное положение
        while (Mathf.Abs(Mathf.DeltaAngle(upperJaw.localEulerAngles.z, upperJawStartRot.z)) > 0.01f ||
               Mathf.Abs(Mathf.DeltaAngle(lowerJaw.localEulerAngles.z, lowerJawStartRot.z)) > 0.01f)
        {
            float upperCurrentZ = upperJaw.localEulerAngles.z;
            float lowerCurrentZ = lowerJaw.localEulerAngles.z;
            
            float newUpperZ = Mathf.MoveTowardsAngle(upperCurrentZ, upperJawStartRot.z, openSpeed * Time.deltaTime);
            float newLowerZ = Mathf.MoveTowardsAngle(lowerCurrentZ, lowerJawStartRot.z, openSpeed * Time.deltaTime);
            
            upperJaw.localEulerAngles = new Vector3(upperJawStartRot.x, upperJawStartRot.y, newUpperZ);
            lowerJaw.localEulerAngles = new Vector3(lowerJawStartRot.x, lowerJawStartRot.y, newLowerZ);
            
            yield return null;
        }
        
        // Финальная установка точных значений
        upperJaw.localEulerAngles = upperJawStartRot;
        lowerJaw.localEulerAngles = lowerJawStartRot;
        
        currentAnimation = null;
    }
}
using UnityEngine;
using System.Collections;

public class FishMouth : MonoBehaviour
{
    [SerializeField] private Transform upperJaw;
    [SerializeField] private Transform lowerJaw;
    
    private Vector3 upperJawStartRot;
    private Vector3 lowerJawStartRot;
    private Coroutine currentAnimation;
    
    public static FishMouth instance;
    private void Start()
    {
        upperJawStartRot = upperJaw.localEulerAngles;
        lowerJawStartRot = lowerJaw.localEulerAngles;
        instance = this;
    }
    
    public void CloseMouth(float angleInDegrees, float duration = 0.1f)
    {
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(AnimateMouth(angleInDegrees, duration));
    }
    
    public void OpenMouth(float duration = 0.1f)
    {
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(AnimateMouth(0f, duration));
    }
    
    private IEnumerator AnimateMouth(float targetAngle, float duration)
    {
        Vector3 upperStart = upperJaw.localEulerAngles;
        Vector3 lowerStart = lowerJaw.localEulerAngles;
        
        float upperTargetZ = upperJawStartRot.z - targetAngle;
        float lowerTargetZ = lowerJawStartRot.z + targetAngle;
        
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            float upperZ = Mathf.LerpAngle(upperStart.z, upperTargetZ, t);
            float lowerZ = Mathf.LerpAngle(lowerStart.z, lowerTargetZ, t);
            
            upperJaw.localEulerAngles = new Vector3(upperJawStartRot.x, upperJawStartRot.y, upperZ);
            lowerJaw.localEulerAngles = new Vector3(lowerJawStartRot.x, lowerJawStartRot.y, lowerZ);
            
            yield return null;
        }
        
        upperJaw.localEulerAngles = new Vector3(upperJawStartRot.x, upperJawStartRot.y, upperTargetZ);
        lowerJaw.localEulerAngles = new Vector3(lowerJawStartRot.x, lowerJawStartRot.y, lowerTargetZ);
        
        currentAnimation = null;
    }
}
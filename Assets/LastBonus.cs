using System;
using UnityEngine;

public class LastBonus : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLastBonusTaken()
    {
        AudioManager.instance.secondsource.Stop();
        DiverMovement.instance.signalCount += 1;
        LevelManager.instance.canUseSignal = true;
        DiverMovement.instance.OnSignal(true);
        DiverMovement.instance.signalCount = 0;
        LevelManager.instance.canUseSignal = false;
        //FishMouth.instance.CloseMouth(-8.8f);
        FishMouth.instance.CloseMouth(-12.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnLastBonusTaken();
    }
}

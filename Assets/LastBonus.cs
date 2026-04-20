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
        DiverMovement.instance.OnSignal();
        FishMouth.instance.CloseMouth(-8);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnLastBonusTaken();
    }
}

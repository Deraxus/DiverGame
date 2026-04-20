using UnityEngine;

public class bgEnabled : MonoBehaviour
{
    public GameObject bg;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bg.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using UnityEngine;

public class PuzirSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float spawnPeriod = 0.5f;
    public GameObject puzirPrefab;

    public float puzirRazbros = 1f;
    void Start()
    {
        StartCoroutine(SpawnPuzir());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator SpawnPuzir()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnPeriod);
            PuzirSpawn(transform.position);   
        }
    }

    public void PuzirSpawn(Vector2 spawnPosition)
    {
        spawnPosition = new Vector2(Random.Range(-puzirRazbros, puzirRazbros) + spawnPosition.x, spawnPosition.y);
        Instantiate(puzirPrefab, spawnPosition, Quaternion.identity);
    }
}

using System.Collections;
using UnityEngine;

public class PuzirSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float spawnPeriod = 0.5f;
    public GameObject puzirPrefab;

    public enum Direction
    {
        LeftRight,
        DownUp,
        RightLeft,
        UpDown,
    }

    public Direction directionType;

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
        if (directionType == Direction.DownUp)
        {
            spawnPosition = new Vector2(Random.Range(-puzirRazbros, puzirRazbros) + spawnPosition.x, spawnPosition.y);   
        }

        if (directionType == Direction.LeftRight)
        {
            spawnPosition = new Vector2(spawnPosition.x, Random.Range(-puzirRazbros, puzirRazbros) + spawnPosition.y);
        }
        GameObject spawnedPuzir = Instantiate(puzirPrefab, spawnPosition, Quaternion.identity);
        if (directionType == Direction.LeftRight)
        {
            spawnedPuzir.GetComponent<PuzirLogic>().directionType = Direction.LeftRight;
        }

        if (directionType == Direction.DownUp)
        {
            spawnedPuzir.GetComponent<PuzirLogic>().directionType = Direction.DownUp;
        }
        
        if (directionType == Direction.RightLeft)
        {
            spawnedPuzir.GetComponent<PuzirLogic>().directionType = Direction.RightLeft;
        }

        if (directionType == Direction.UpDown)
        {
            spawnedPuzir.GetComponent<PuzirLogic>().directionType = Direction.UpDown;
        }
    }
}

using System.Collections;
using UnityEngine;

public class PuzirSpawner : MonoBehaviour
{
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

    [Header("Rotation")]
    public float spawnAngle = 0f;

    private void Start()
    {
        StartCoroutine(SpawnPuzir());
    }

    private IEnumerator SpawnPuzir()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnPeriod);
            PuzirSpawn(transform.position);
        }
    }

    public void PuzirSpawn(Vector2 spawnPosition)
    {
        if (directionType == Direction.DownUp || directionType == Direction.UpDown)
        {
            spawnPosition = new Vector2(
                spawnPosition.x + Random.Range(-puzirRazbros, puzirRazbros),
                spawnPosition.y
            );
        }

        if (directionType == Direction.LeftRight || directionType == Direction.RightLeft)
        {
            spawnPosition = new Vector2(
                spawnPosition.x,
                spawnPosition.y + Random.Range(-puzirRazbros, puzirRazbros)
            );
        }

        Quaternion spawnRotation;

        if (Mathf.Approximately(spawnAngle, 0f))
        {
            spawnRotation = puzirPrefab.transform.rotation;
        }
        else
        {
            spawnRotation = Quaternion.Euler(0f, 0f, spawnAngle);
        }

        GameObject spawnedPuzir = Instantiate(puzirPrefab, spawnPosition, spawnRotation);

        PuzirLogic puzirLogic = spawnedPuzir.GetComponent<PuzirLogic>();
        if (puzirLogic != null)
        {
            puzirLogic.directionType = directionType;
        }
    }
}
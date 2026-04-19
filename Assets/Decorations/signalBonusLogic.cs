using UnityEngine;

public class signalBonusLogic : MonoBehaviour
{
    public float amplitude = 0.3f;
    public float frequency = 1f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;

        transform.position = new Vector3(
            startPos.x,
            startPos.y + yOffset,
            startPos.z
        );
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<DiverMovement>().TakeSignalBonus();
            Destroy(gameObject);
        }
    }
}
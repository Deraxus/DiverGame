using UnityEngine;

public class spikeLogic : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamage(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other.gameObject);
    }

    private void TryDamage(GameObject obj)
    {
        DiverMovement player = obj.GetComponent<DiverMovement>();

        if (player == null)
            return;

        player.TakeDamageOrUseSignal();
    }
}
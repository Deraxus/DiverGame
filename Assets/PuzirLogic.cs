using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PuzirLogic : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    public float baseSpeed = 1f;

    public PuzirSpawner.Direction directionType = PuzirSpawner.Direction.DownUp;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (sprites.Count > 0)
        {
            int randIndex = Random.Range(0, sprites.Count);
            spriteRenderer.sprite = sprites[randIndex];
        }

        float finalSpeed = baseSpeed + Random.Range(0.1f, 1.5f);
        Vector2 moveDirection = GetWorldMoveDirection();

        rb.linearVelocity = moveDirection * finalSpeed;
    }

    private Vector2 GetWorldMoveDirection()
    {
        Vector2 localDirection = GetLocalDirection();
        Vector3 worldDirection = transform.TransformDirection(localDirection);
        return ((Vector2)worldDirection).normalized;
    }

    private Vector2 GetLocalDirection()
    {
        switch (directionType)
        {
            case PuzirSpawner.Direction.DownUp:
                return Vector2.up;

            case PuzirSpawner.Direction.UpDown:
                return Vector2.down;

            case PuzirSpawner.Direction.LeftRight:
                return Vector2.right;

            case PuzirSpawner.Direction.RightLeft:
                return Vector2.left;

            default:
                return Vector2.up;
        }
    }
}
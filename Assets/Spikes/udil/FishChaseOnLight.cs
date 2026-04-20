using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FishChaseOnLight : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Flashlight")]
    public Transform flashlightOrigin;
    public float lightDistance = 10f;
    public float lightAngle = 80f;
    public LayerMask obstacleMask;
    public bool ignoreObstacles = false;

    [Header("Aggro")]
    public float chaseMemoryTime = 2f;
    public float extraAggroDistance = 1.5f;
    public float extraAggroAngle = 20f;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 8f;

    [Header("State")]
    public bool isChasing;

    private Rigidbody2D rb;
    private float chaseTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        bool inLight = IsInLightCone();

        if (inLight)
        {
            isChasing = true;
            chaseTimer = chaseMemoryTime;
        }
        else if (isChasing)
        {
            chaseTimer -= Time.deltaTime;

            if (chaseTimer <= 0f)
            {
                isChasing = false;
                chaseTimer = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isChasing || player == null)
            return;

        Vector2 directionToPlayer = ((Vector2)player.position - rb.position).normalized;
        Vector2 newPosition = rb.position + directionToPlayer * (moveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(newPosition);

        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        float smoothedAngle = Mathf.LerpAngle(rb.rotation, angle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(smoothedAngle);
    }

    private bool IsInLightCone()
    {
        if (player == null || flashlightOrigin == null)
            return false;

        Vector2 fishPosition = transform.position;
        Vector2 lightPosition = flashlightOrigin.position;

        Vector2 toFish = fishPosition - lightPosition;
        float distance = toFish.magnitude;

        float allowedDistance = lightDistance + extraAggroDistance;
        if (distance > allowedDistance)
            return false;

        Vector2 lightForward = flashlightOrigin.right;
        float angleToFish = Vector2.Angle(lightForward, toFish);

        float allowedAngle = (lightAngle * 0.5f) + extraAggroAngle;
        if (angleToFish > allowedAngle)
            return false;

        if (!ignoreObstacles)
        {
            RaycastHit2D hit = Physics2D.Raycast(lightPosition, toFish.normalized, distance, obstacleMask);

            if (hit.collider != null)
                return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (flashlightOrigin == null)
            return;

        Gizmos.color = isChasing ? Color.red : Color.yellow;

        float allowedDistance = lightDistance + extraAggroDistance;
        float allowedHalfAngle = (lightAngle * 0.5f) + extraAggroAngle;

        Vector3 origin = flashlightOrigin.position;
        Vector3 rightBorder = Quaternion.Euler(0f, 0f, allowedHalfAngle) * flashlightOrigin.right * allowedDistance;
        Vector3 leftBorder = Quaternion.Euler(0f, 0f, -allowedHalfAngle) * flashlightOrigin.right * allowedDistance;

        Gizmos.DrawLine(origin, origin + rightBorder);
        Gizmos.DrawLine(origin, origin + leftBorder);
    }
}
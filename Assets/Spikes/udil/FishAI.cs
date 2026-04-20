using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D))]
public class FishAI : MonoBehaviour
{
    private enum FishState
    {
        Patrol,
        Chase,
        WaitBeforePatrol
    }

    [Header("References")]
    public Transform player;
    public Transform flashlightOrigin;

    [Header("Main Flashlight Aggro")]
    public float flashlightDistance = 10f;
    public float flashlightAngle = 90f;
    public float extraAggroDistance = 2f;
    public float extraAggroAngle = 20f;
    public LayerMask sightObstacleMask;
    public bool ignoreSightObstacles = false;

    [Header("Signal Aggro")]
    public bool reactToSignalLight = true;

    [Header("Patrol")]
    public float patrolSpeed = 2f;
    public float patrolCheckDistance = 0.5f;
    public LayerMask patrolObstacleMask;
    public bool startMovingRight = true;

    [Header("Chase")]
    public float chaseSpeed = 3.5f;
    public float rotationSpeed = 8f;
    public float lostTargetDelay = 2f;

    [Header("Debug")]
    public bool isAggroed;

    private Rigidbody2D rb;
    private FishState currentState;

    private Vector2 patrolDirection;
    private float lostTargetTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        patrolDirection = startMovingRight ? Vector2.right : Vector2.left;
        currentState = FishState.Patrol;
        UpdateVisualDirection(patrolDirection);
    }

    private void Update()
    {
        bool seesPlayerFromAnyLight = IsInMainFlashlight() || IsInSignalLight();

        switch (currentState)
        {
            case FishState.Patrol:
                if (seesPlayerFromAnyLight)
                {
                    StartChase();
                }
                break;

            case FishState.Chase:
                if (seesPlayerFromAnyLight)
                {
                    lostTargetTimer = lostTargetDelay;
                }
                else
                {
                    lostTargetTimer -= Time.deltaTime;

                    if (lostTargetTimer <= 0f)
                    {
                        StartWaitingBeforePatrol();
                    }
                }
                break;

            case FishState.WaitBeforePatrol:
                if (seesPlayerFromAnyLight)
                {
                    StartChase();
                }
                break;
        }

        isAggroed = currentState == FishState.Chase;
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case FishState.Patrol:
                HandlePatrol();
                break;

            case FishState.Chase:
                HandleChase();
                break;

            case FishState.WaitBeforePatrol:
                HandleWaitBeforePatrol();
                break;
        }
    }

    private void HandlePatrol()
    {
        if (IsPatrolObstacleAhead())
        {
            ReversePatrolDirection();
        }

        Vector2 newPosition = rb.position + patrolDirection * (patrolSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
        UpdateVisualDirection(patrolDirection);
    }

    private void HandleChase()
    {
        if (player == null)
            return;

        Vector2 directionToPlayer = ((Vector2)player.position - rb.position).normalized;
        Vector2 newPosition = rb.position + directionToPlayer * (chaseSpeed * Time.fixedDeltaTime);

        rb.MovePosition(newPosition);

        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        float smoothedAngle = Mathf.LerpAngle(rb.rotation, angle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(smoothedAngle);
    }

    private void HandleWaitBeforePatrol()
    {
    }

    private void StartChase()
    {
        currentState = FishState.Chase;
        lostTargetTimer = lostTargetDelay;
    }

    private void StartWaitingBeforePatrol()
    {
        currentState = FishState.WaitBeforePatrol;
        rb.linearVelocity = Vector2.zero;
        StartCoroutine(ReturnToPatrolAfterDelay());
    }

    private System.Collections.IEnumerator ReturnToPatrolAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        if (currentState == FishState.WaitBeforePatrol)
        {
            currentState = FishState.Patrol;

            rb.rotation = 0f;

            if (Mathf.Abs(transform.localScale.x) > 0.001f)
            {
                patrolDirection = transform.localScale.x >= 0f ? Vector2.right : Vector2.left;
            }
            else
            {
                patrolDirection = Vector2.right;
            }

            UpdateVisualDirection(patrolDirection);
        }
    }

    private bool IsInMainFlashlight()
    {
        if (player == null || flashlightOrigin == null)
            return false;

        Vector2 fishPosition = transform.position;
        Vector2 lightPosition = flashlightOrigin.position;

        Vector2 toFish = fishPosition - lightPosition;
        float distance = toFish.magnitude;

        float allowedDistance = flashlightDistance + extraAggroDistance;
        if (distance > allowedDistance)
            return false;

        Vector2 lightForward = flashlightOrigin.right;
        float angleToFish = Vector2.Angle(lightForward, toFish);

        float allowedHalfAngle = (flashlightAngle * 0.5f) + extraAggroAngle;
        if (angleToFish > allowedHalfAngle)
            return false;

        if (!ignoreSightObstacles)
        {
            RaycastHit2D hit = Physics2D.Raycast(lightPosition, toFish.normalized, distance, sightObstacleMask);

            if (hit.collider != null)
                return false;
        }

        return true;
    }

    private bool IsInSignalLight()
    {
        if (!reactToSignalLight)
            return false;

        if (LevelManager.instance == null)
            return false;

        Light2D signalLight = LevelManager.instance.signalLight;
        if (signalLight == null)
            return false;

        if (!signalLight.enabled || signalLight.intensity <= 0.01f)
            return false;

        float distanceToSignalCenter = Vector2.Distance(transform.position, signalLight.transform.position);

        if (distanceToSignalCenter > signalLight.pointLightOuterRadius)
            return false;

        return true;
    }

    private bool IsPatrolObstacleAhead()
    {
        Vector2 origin = rb.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, patrolDirection, patrolCheckDistance, patrolObstacleMask);
        return hit.collider != null;
    }

    private void ReversePatrolDirection()
    {
        patrolDirection *= -1f;
        UpdateVisualDirection(patrolDirection);
    }

    private void UpdateVisualDirection(Vector2 direction)
    {
        if (currentState == FishState.Chase)
            return;

        if (direction.x > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            rb.rotation = 0f;
        }
        else if (direction.x < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            rb.rotation = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState != FishState.Patrol)
            return;

        if (((1 << collision.gameObject.layer) & patrolObstacleMask) != 0)
        {
            ReversePatrolDirection();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (flashlightOrigin != null)
        {
            Gizmos.color = Color.yellow;

            float allowedDistance = flashlightDistance + extraAggroDistance;
            float allowedHalfAngle = (flashlightAngle * 0.5f) + extraAggroAngle;

            Vector3 origin = flashlightOrigin.position;
            Vector3 rightBorder = Quaternion.Euler(0f, 0f, allowedHalfAngle) * flashlightOrigin.right * allowedDistance;
            Vector3 leftBorder = Quaternion.Euler(0f, 0f, -allowedHalfAngle) * flashlightOrigin.right * allowedDistance;

            Gizmos.DrawLine(origin, origin + rightBorder);
            Gizmos.DrawLine(origin, origin + leftBorder);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, patrolDirection * patrolCheckDistance);
    }
}
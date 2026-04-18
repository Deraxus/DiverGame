using UnityEngine;

public class PuzirLogicGPT : MonoBehaviour
{
    [Header("Base Movement")]
    public float upSpeed = 2f;

    [Header("Side Drift")]
    public float sideDistance = 0.5f;
    public float sideMoveSpeed = 2f;
    public float minTimeBeforeSideMove = 0.5f;
    public float maxTimeBeforeSideMove = 1.5f;
    public float minPauseBetweenMoves = 0.3f;
    public float maxPauseBetweenMoves = 1f;

    private Vector2 currentSideTarget;
    private bool movingSideways;
    private float nextSideMoveTimer;

    private void Start()
    {
        ScheduleNextSideMove();
    }

    private void Update()
    {
        MoveUp();
        HandleSideMovement();
    }

    private void MoveUp()
    {
        transform.position += Vector3.up * (upSpeed * Time.deltaTime);
    }

    private void HandleSideMovement()
    {
        if (!movingSideways)
        {
            nextSideMoveTimer -= Time.deltaTime;

            if (nextSideMoveTimer <= 0f)
            {
                StartSideMove();
            }

            return;
        }

        Vector2 currentPosition = transform.position;

        Vector2 newPosition = Vector2.MoveTowards(
            currentPosition,
            currentSideTarget,
            sideMoveSpeed * Time.deltaTime
        );

        transform.position = new Vector3(newPosition.x, transform.position.y, transform.position.z);

        if (Mathf.Abs(transform.position.x - currentSideTarget.x) < 0.01f)
        {
            movingSideways = false;
            ScheduleNextSideMoveWithPause();
        }
    }

    private void StartSideMove()
    {
        float randomDirection = Random.value < 0.5f ? -1f : 1f;
        float randomOffset = Random.Range(sideDistance * 0.5f, sideDistance);

        currentSideTarget = new Vector2(
            transform.position.x + randomDirection * randomOffset,
            transform.position.y
        );

        movingSideways = true;
    }

    private void ScheduleNextSideMove()
    {
        nextSideMoveTimer = Random.Range(minTimeBeforeSideMove, maxTimeBeforeSideMove);
    }

    private void ScheduleNextSideMoveWithPause()
    {
        nextSideMoveTimer = Random.Range(minPauseBetweenMoves, maxPauseBetweenMoves);
    }
}
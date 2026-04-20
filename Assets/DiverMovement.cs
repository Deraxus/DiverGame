using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class DiverMovement : MonoBehaviour
{
    public float acceleration = 8f;
    public float maxSpeed = 5f;
    public float drag = 2f;

    public int signalCount = 3;

    [Header("Invulnerability")]
    public float invulnerabilityDuration = 3f;
    public float blinkInterval = 0.15f;

    [HideInInspector] public bool isInvulnerable = false;

    private Rigidbody2D rb;
    private Vector2 input;
    private InputActions controls;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = drag;

        controls = new InputActions();
        controls.Player.Signal.started += ctx => OnSignal();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        input = controls.Player.Move.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        if (input != Vector2.zero)
        {
            rb.AddForce(input * acceleration);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        if (rb.linearVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }
    }

    public void OnSignal()
    {
        if (LevelManager.instance != null && LevelManager.instance.canUseSignal && signalCount > 0)
        {
            SpendSignal();
            LevelManager.instance.MakeSignal();
        }
    }

    public void TakeDamageOrUseSignal()
    {
        if (isInvulnerable)
            return;

        if (signalCount > 0)
        {
            SpendSignal();
            StartCoroutine(InvulnerabilityCoroutine());
            return;
        }

        if (LevelManager.instance != null)
        {
            LevelManager.instance.GameOver();
        }
    }

    private void SpendSignal()
    {
        signalCount--;

        if (LevelManager.instance != null)
        {
            Image bonusImage = LevelManager.instance.ChooseLastActiveBonus();
            if (bonusImage != null)
            {
                bonusImage.gameObject.SetActive(false);
            }
        }
    }

    public void TakeSignalBonus()
    {
        signalCount++;

        if (LevelManager.instance != null)
        {
            Image bonusImage = LevelManager.instance.ChooseLastNonActiveBonus();
            if (bonusImage != null)
            {
                bonusImage.gameObject.SetActive(true);
            }
        }
    }

    private System.Collections.IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;

        float time = 0f;
        bool visible = true;

        while (time < invulnerabilityDuration)
        {
            visible = !visible;

            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = visible;
            }

            yield return new WaitForSeconds(blinkInterval);
            time += blinkInterval;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        isInvulnerable = false;
    }
}
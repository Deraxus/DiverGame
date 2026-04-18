using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class DiverMovement : MonoBehaviour
{
    public float acceleration = 8f;
    public float maxSpeed = 5f;
    public float drag = 2f;

    private Rigidbody2D rb;
    private Vector2 input;

    private InputActions controls;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = drag;

        controls = new InputActions();

        controls.Player.Signal.performed += ctx => OnSignal();
    }

    private void Start()
    {
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

        // поворот
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }
    }
    
    void OnSignal()
    {
        if (LevelManager.instance != null && LevelManager.instance.canUseSignal)
        {
            LevelManager.instance.MakeSignal();
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float Speed = 5f;
    private Vector2 moveInput;

    [Header("Hit Reaction Settings")]
    private bool isHit = false;
    private float hitTimer = 0f;

    private Animator animator;
    private Camera mainCamera;

    // Unity Event Methods
    private void Start()
    {
        mainCamera = Camera.main;
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        HandleHitTimer();
        HandleMovement();
        HandleRotation();
        HandleHitTrigger();
        UpdateAnimatorParameters();
    }

    // Input System Events
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Movement & Rotation
    private void HandleMovement()
    {
        if (isHit) return;

        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        transform.Translate(moveDirection * Speed * Time.deltaTime, Space.World);
    }

    private void HandleRotation()
    {
        if (isHit) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 direction = (hitPoint - transform.position).normalized;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 0.1f);
            }
        }
    }

    // Hit Reaction
    private void HandleHitTrigger()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame && !isHit)
        {
            animator.SetTrigger("Hit");
            isHit = true;
            hitTimer = 1.4f; // Hit animasyon süresi
        }
    }

    private void HandleHitTimer()
    {
        if (!isHit) return;

        hitTimer -= Time.deltaTime;
        if (hitTimer <= 0f)
        {
            isHit = false;
        }
    }

    // Animator
    private void UpdateAnimatorParameters()
    {
        if (isHit)
        {
            animator.SetFloat("Speed", 0f);
        }
        else
        {
            float speed = moveInput.magnitude;
            animator.SetFloat("Speed", speed);
        }
    }
}

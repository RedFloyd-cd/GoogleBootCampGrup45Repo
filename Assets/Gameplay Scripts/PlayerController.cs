using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float Speed = 5f;
    private Vector2 moveInput;

    [Header("Hit Reaction Settings")]
    private bool isHit = false;
    private float hitTimer = 0f;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    public Slider healthSlider;
    private Coroutine poisonCoroutine;

    [Header("Dash Settings")]
    public float dashForce = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 3f;
    private Rigidbody rb;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float cooldownTimer = 0f;
    private bool isOnCooldown = false;
    private Vector3 dashDirection;

    [Header("Time Warp Settings")]
    public float timeWarpTimeScale = 0.2f;
    public float timeWarpDuration = 3f;
    public float timeWarpCooldown = 10f;
    public GameObject timeWarpPanel;
    private float timeWarpCooldownTimer = 0f;
    private bool isTimeWarpOnCooldown = false;
    private bool isTimeWarpActive = false;
    private float timeWarpTimer;

    private Animator animator;
    private Camera mainCamera;

    // Unity Event Methods
    void Start()
    {
        mainCamera = Camera.main;
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        // Stat değerlerini PlayerStats'tan al
        maxHealth = PlayerStats.Instance.maxHealth;
        dashCooldown = PlayerStats.Instance.dashCooldown;
        timeWarpCooldown = PlayerStats.Instance.timeWarpCooldown;

        currentHealth = maxHealth;
        UpdateUI();
    }


    private void Update()
    {
        HandleHitTimer();
        HandleMovement();
        HandleRotation();
        HandleHitTrigger();
        UpdateAnimatorParameters();
        HandleDashCooldown();
        HandleTimeWarpCooldown();
        HandleDashInput();
        HandleTimeWarpInput();
        HandleDashing();
        HandleTimeWarpActive();
    }

    // Input System Events
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Movement & Rotation
    private void HandleMovement()
    {
        if (isHit || isDashing) return;
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

    // Health System
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UpdateUI();
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("Öldün!");
        enabled = false;
        gameObject.SetActive(false);
        GameOverManager.Instance.ShowGameOver();
    }

    public void ApplyPoison(float tickDamage, float duration, float tickInterval)
    {
        if (poisonCoroutine != null)
        {
            StopCoroutine(poisonCoroutine);
        }
        poisonCoroutine = StartCoroutine(PoisonEffect(tickDamage, duration, tickInterval));
    }

    private IEnumerator PoisonEffect(float tickDamage, float duration, float tickInterval)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            TakeDamage(tickDamage);
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }
        poisonCoroutine = null;
    }

    // Dash System
    private void HandleDashInput()
    {
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame && !isDashing && !isOnCooldown)
        {
            StartDash();
        }
    }

    private void HandleDashing()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                StopDash();
            }
        }
    }

    private void StartDash()
    {
        isDashing = true;
        isOnCooldown = true;
        dashTimer = dashDuration;
        cooldownTimer = dashCooldown;
        dashDirection = GetDashDirection();
        rb.linearVelocity = dashDirection * dashForce;
        Debug.Log("Dash başladı!");
    }

    private void StopDash()
    {
        isDashing = false;
        rb.linearVelocity = Vector3.zero;
    }

    private void HandleDashCooldown()
    {
        if (!isOnCooldown) return;
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            isOnCooldown = false;
            Debug.Log("Dash yeniden kullanılabilir.");
        }
    }

    private Vector3 GetDashDirection()
    {
        Vector3 move = Vector3.zero;
        if (Keyboard.current.wKey.isPressed) move += Vector3.forward;
        if (Keyboard.current.sKey.isPressed) move += Vector3.back;
        if (Keyboard.current.aKey.isPressed) move += Vector3.left;
        if (Keyboard.current.dKey.isPressed) move += Vector3.right;
        if (move == Vector3.zero)
            move = transform.forward;
        return move.normalized;
    }

    // Time Warp System
    private void HandleTimeWarpInput()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame && !isTimeWarpActive && !isTimeWarpOnCooldown)
        {
            ActivateTimeWarp();
        }
    }

    private void HandleTimeWarpActive()
    {
        if (isTimeWarpActive)
        {
            timeWarpTimer -= Time.unscaledDeltaTime;

            if (timeWarpTimer <= 0f)
            {
                ResetTimeScale();
            }
        }
    }

    private void ActivateTimeWarp()
    {
        Time.timeScale = timeWarpTimeScale;
        Time.fixedDeltaTime = 0.02f * timeWarpTimeScale;
        timeWarpTimer = timeWarpDuration;
        isTimeWarpActive = true;

        if (timeWarpPanel != null)
            timeWarpPanel.SetActive(true);

        isTimeWarpOnCooldown = true;
        timeWarpCooldownTimer = timeWarpCooldown;

        Debug.Log("Zaman yavaşlatma aktif!");
    }

    private void ResetTimeScale()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        isTimeWarpActive = false;

        if (timeWarpPanel != null)
            timeWarpPanel.SetActive(false);

        Debug.Log("Zaman normale döndü.");
    }

    private void HandleTimeWarpCooldown()
    {
        if (!isTimeWarpOnCooldown) return;

        timeWarpCooldownTimer -= Time.unscaledDeltaTime;

        if (timeWarpCooldownTimer <= 0f)
        {
            isTimeWarpOnCooldown = false;
            timeWarpCooldownTimer = 0f;

            Debug.Log("TimeWarp yeniden kullanılabilir.");
        }
    }
}

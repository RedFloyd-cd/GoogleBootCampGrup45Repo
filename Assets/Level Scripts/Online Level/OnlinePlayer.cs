using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class OnlinePlayer : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float Speed = 5f;
    private Vector2 moveInput;

    [Header("Hit Reaction Settings")]
    private bool isHit = false;
    private float hitTimer = 0f;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public TextMeshProUGUI healthText;
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



    [Header("Audio Settings")]
    public AudioSource walkAudioSource;
    public AudioClip walkClip;
    public float walkVolume = 0.5f;
    private bool isWalking = false;

    [Header("Skill UI")]
    public SkillCooldownUI dashUI;

    [Header("Combat Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce = 20f;
    public float fireRate = 0.4f;
    private float lastFireTime = 0f;

    // Network Variables
    public NetworkVariable<float> health = new NetworkVariable<float>(100f);

    private Animator animator;
    private Camera mainCamera;

    void Start()
    {
        if (IsOwner)
        {
            mainCamera = Camera.main;
            animator = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody>();

            // Load stats from PlayerStats if available
            if (PlayerStats.Instance != null)
            {
                maxHealth = PlayerStats.Instance.maxHealth;
                dashCooldown = PlayerStats.Instance.dashCooldown;
            }

            health.Value = maxHealth;
            UpdateUI();

            if (walkAudioSource != null && walkClip != null)
            {
                walkAudioSource.clip = walkClip;
                walkAudioSource.loop = true;
                walkAudioSource.volume = walkVolume;
            }
        }
    }

    void Update()
    {
        if (!IsOwner) return; // Sadece kendi karakterini kontrol et

        HandleHitTimer();
        HandleMovement();
        HandleRotation();
        HandleHitTrigger();
        UpdateAnimatorParameters();
        HandleDashCooldown();
        HandleDashInput();
        HandleDashing();
        HandleFire();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        moveInput = context.ReadValue<Vector2>();
    }

    private void HandleMovement()
    {
        if (isHit || isDashing)
        {
            StopWalkSound();
            return;
        }

        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        transform.Translate(moveDirection * Speed * Time.deltaTime, Space.World);

        if (moveInput.magnitude > 0.1f)
        {
            PlayWalkSound();
        }
        else
        {
            StopWalkSound();
        }
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

    private void HandleHitTrigger()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame && !isHit)
        {
            animator.SetTrigger("Hit");
            isHit = true;
            hitTimer = 1.4f;
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

    void HandleFire()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastFireTime > fireRate)
        {
            lastFireTime = Time.time;
            FireServerRpc();
        }
    }

    [ServerRpc]
    void FireServerRpc(ServerRpcParams rpcParams = default)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody>().linearVelocity = firePoint.forward * fireForce;
        bullet.GetComponent<OnlineBullet>().ownerId = OwnerClientId;
        bullet.GetComponent<NetworkObject>().Spawn();
    }

    public void TakeDamage(float amount)
    {
        if (IsServer)
        {
            health.Value -= amount;
            if (health.Value <= 0)
            {
                Die();
            }
        }
    }

    void UpdateUI()
    {
        if (healthText != null && IsOwner)
        {
            healthText.text = Mathf.RoundToInt(health.Value) + " / " + Mathf.RoundToInt(maxHealth);
        }
    }

    void Die()
    {
        if (IsServer)
        {
            Debug.Log("Oyuncu öldü!");
            NetworkObject.Despawn();
        }
        
        if (IsOwner)
        {
            GameOverManager.Instance.ShowGameOver();
        }
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

        if (dashUI != null)
            dashUI.StartCooldown(dashCooldown);

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



    private void PlayWalkSound()
    {
        if (walkAudioSource != null && !walkAudioSource.isPlaying)
        {
            walkAudioSource.Play();
            isWalking = true;
        }
    }

    private void StopWalkSound()
    {
        if (walkAudioSource != null && walkAudioSource.isPlaying)
        {
            walkAudioSource.Stop();
            isWalking = false;
        }
    }
}

using UnityEngine;

public class DashSkill : MonoBehaviour
{
    [Header("Dash Ayarlarý")]
    public float dashForce = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 3f;

    private Rigidbody rb;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float cooldownTimer = 0f;
    private bool isOnCooldown = false;

    private Vector3 dashDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleDashCooldown();

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && !isOnCooldown)
        {
            StartDash();
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                StopDash();
            }
        }
    }

    void StartDash()
    {
        isDashing = true;
        isOnCooldown = true;
        dashTimer = dashDuration;
        cooldownTimer = dashCooldown;

        dashDirection = GetDirection();
        rb.linearVelocity = dashDirection * dashForce;

        Debug.Log("Dash baþladý!");
    }

    void StopDash()
    {
        isDashing = false;
        rb.linearVelocity = Vector3.zero;
    }

    void HandleDashCooldown()
    {
        if (!isOnCooldown) return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            isOnCooldown = false;
            Debug.Log("Dash yeniden kullanýlabilir.");
        }
    }

    Vector3 GetDirection()
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) move += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) move += Vector3.back;
        if (Input.GetKey(KeyCode.A)) move += Vector3.left;
        if (Input.GetKey(KeyCode.D)) move += Vector3.right;

        if (move == Vector3.zero)
            move = transform.forward;

        return move.normalized;
    }
}

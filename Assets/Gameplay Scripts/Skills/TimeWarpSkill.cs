using UnityEngine;

public class TimeWarpSkill : MonoBehaviour
{
    [Header("Slow Motion Settings")]
    public float slowTimeScale = 0.2f;
    public float slowDuration = 3f;

    [Header("Cooldown Settings")]
    public float cooldownDuration = 10f;
    private float cooldownTimer = 0f;
    private bool isOnCooldown = false;

    private bool isSlowing = false;
    private float slowTimer;

    [Header("UI References")]
    public GameObject slowMotionPanel; // Sa� �stteki ikon + yaz�

    void Update()
    {
        HandleCooldown();

        if (Input.GetKeyDown(KeyCode.Q) && !isSlowing && !isOnCooldown)
        {
            ActivateSlowMotion();
        }

        if (isSlowing)
        {
            slowTimer -= Time.unscaledDeltaTime;

            if (slowTimer <= 0f)
            {
                ResetTimeScale();
            }
        }
    }

    void ActivateSlowMotion()
    {
        Time.timeScale = slowTimeScale;
        Time.fixedDeltaTime = 0.02f * slowTimeScale;
        slowTimer = slowDuration;
        isSlowing = true;

        if (slowMotionPanel != null)
            slowMotionPanel.SetActive(true);

        isOnCooldown = true;
        cooldownTimer = cooldownDuration;

        Debug.Log("Zaman yava�latma aktif!");
    }

    void ResetTimeScale()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        isSlowing = false;

        if (slowMotionPanel != null)
            slowMotionPanel.SetActive(false);

        Debug.Log("Zaman normale d�nd�.");
    }

    void HandleCooldown()
    {
        if (!isOnCooldown) return;

        cooldownTimer -= Time.unscaledDeltaTime;

        if (cooldownTimer <= 0f)
        {
            isOnCooldown = false;
            cooldownTimer = 0f;

            Debug.Log("TimeWarp yeniden kullan�labilir.");
        }
    }
}

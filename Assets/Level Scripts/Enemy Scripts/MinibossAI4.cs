using UnityEngine;

public class MinibossAI4 : MonoBehaviour
{
    public Transform target;
    public float detectionRadius = 18f;
    public float moveSpeed = 5f; // Hızlı
    public float teleportCooldown = 2.5f;
    public float health = 120f; // Düşük can
    public GameObject projectilePrefab;
    public float projectileSpeed = 12f;
    public float projectileDamage = 18f;
    public GameObject ammoPickupPrefab;
    public Animator animator;
    public Vector3 teleportAreaCenter;
    public Vector3 teleportAreaSize = new Vector3(12, 0, 12); // Kare alan

    private float lastTeleportTime;
    private bool isDead = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        teleportAreaCenter = transform.position; // Başlangıç pozisyonunu merkez olarak al
    }

    void Update()
    {
        if (isDead) return;
        if (target == null) {
            if (animator != null) animator.SetBool("isMoving", false);
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= detectionRadius)
        {
            if (Time.time - lastTeleportTime > teleportCooldown)
            {
                TeleportRandomly();
                lastTeleportTime = Time.time;
                if (animator != null) animator.SetTrigger("teleport");
                if (animator != null) animator.SetBool("isAttacking", true);
                if (animator != null) animator.SetBool("isMoving", false);
                Invoke(nameof(FireProjectile), 0.3f); // Kısa gecikme ile ateş et
            }
        }
        else
        {
            if (animator != null) animator.SetBool("isMoving", false);
            if (animator != null) animator.SetBool("isAttacking", false);
        }
    }

    void TeleportRandomly()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-teleportAreaSize.x / 2, teleportAreaSize.x / 2),
            0,
            Random.Range(-teleportAreaSize.z / 2, teleportAreaSize.z / 2)
        );
        Vector3 newPos = teleportAreaCenter + randomOffset;
        transform.position = newPos;
    }

    void FireProjectile()
    {
        if (projectilePrefab != null && target != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward, Quaternion.identity);
            if (projectile.GetComponent<EnemyProjectile>() == null)
            {
                projectile.AddComponent<EnemyProjectile>().damage = projectileDamage;
            }
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (target.position - transform.position).normalized;
                rb.linearVelocity = dir * projectileSpeed;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        if (animator != null) animator.SetBool("isDead", true);
        // %20 ihtimalle ammo drop
        if (ammoPickupPrefab != null && Random.value <= 0.2f)
        {
            Instantiate(ammoPickupPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(teleportAreaCenter, teleportAreaSize);
    }
}

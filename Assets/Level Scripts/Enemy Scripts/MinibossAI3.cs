using UnityEngine;

public class MinibossAI3 : MonoBehaviour
{
    public Transform target;
    public float detectionRadius = 18f;
    public float areaAttackRadius = 5f; // Geniş AoE saldırı yarıçapı
    public float moveSpeed = 1.2f; // Çok yavaş
    public float attackCooldown = 3.5f; // Saldırı aralığı
    public float health = 600f; // Çok yüksek can
    public float areaAttackDamage = 40f;
    public Animator animator;
    public GameObject ammoPickupPrefab;

    private float maxHealth;
    private float lastAttackTime;
    private bool isDead = false;

    void Start()
    {
        maxHealth = health;
        if (animator == null)
            animator = GetComponent<Animator>();
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
            // Hedefe bak
            Vector3 lookDir = (target.position - transform.position).normalized;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            }

            // Yaklaş
            if (distance > areaAttackRadius)
            {
                Vector3 direction = (target.position - transform.position);
                if (direction.magnitude > 0.05f)
                {
                    direction = direction.normalized;
                    transform.position += direction * moveSpeed * Time.deltaTime;
                    if (animator != null) animator.SetBool("isMoving", true);
                }
                else
                {
                    if (animator != null) animator.SetBool("isMoving", false);
                }
                if (animator != null) animator.SetBool("isAttacking", false);
            }
            // Alan saldırısı
            if (distance <= areaAttackRadius)
            {
                if (Time.time - lastAttackTime > attackCooldown)
                {
                    if (animator != null) animator.SetTrigger("areaAttack");
                    if (animator != null) animator.SetBool("isAttacking", true);
                    if (animator != null) animator.SetBool("isMoving", false);
                    lastAttackTime = Time.time;
                    AreaAttack();
                }
            }
        }
        else
        {
            if (animator != null) animator.SetBool("isMoving", false);
            if (animator != null) animator.SetBool("isAttacking", false);
        }
    }

    void AreaAttack()
    {
        // Belirli bir yarıçap içindeki tüm Player'lara hasar ver
        Collider[] hits = Physics.OverlapSphere(transform.position, areaAttackRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth ph = hit.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    ph.TakeDamage(areaAttackDamage);
                }
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, areaAttackRadius);
    }
}

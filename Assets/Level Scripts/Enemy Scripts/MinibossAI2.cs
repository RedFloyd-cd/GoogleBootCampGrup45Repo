using UnityEngine;

public class MinibossAI2 : MonoBehaviour
{
    public Transform target;
    public float detectionRadius = 20f;
    public float meleeRange = 2.5f;
    public float moveSpeed = 6f; // Normalde hızlı
    public float attackCooldown = 1.2f; // Normalde hızlı saldırı
    public float health = 250f;
    public float meleeDamage = 30f;
    public Animator animator;
    public GameObject ammoPickupPrefab;

    private float maxHealth;
    private float lastAttackTime;
    private bool isDead = false;
    private bool isUltraEnraged = false;

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

        // Can %25'in altına inince ultra agresif moda geç
        if (!isUltraEnraged && health <= maxHealth * 0.25f)
        {
            isUltraEnraged = true;
            moveSpeed *= 1.7f; // Daha da hızlı koş
            attackCooldown *= 0.5f; // Saldırı aralığı daha da kısalır
        }

        if (distance <= detectionRadius)
        {
            // Hedefe bak (DÜZ BAKMASI İÇİN 180 DERECE DÖNDÜRMEYİ KALDIRIYORUM)
            Vector3 lookDir = (target.position - transform.position);
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            }

            // Yaklaş
            if (distance > meleeRange)
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

            // Saldırı
            if (distance <= meleeRange)
            {
                if (Time.time - lastAttackTime > attackCooldown)
                {
                    if (animator != null) animator.SetTrigger("meleeAttack");
                    if (animator != null) animator.SetBool("isAttacking", true);
                    if (animator != null) animator.SetBool("isMoving", false);
                    lastAttackTime = Time.time;
                    MeleeAttack();
                }
            }
        }
        else
        {
            if (animator != null) animator.SetBool("isMoving", false);
            if (animator != null) animator.SetBool("isAttacking", false);
        }
    }

    void MeleeAttack()
    {
        if (target != null)
        {
            PlayerHealth ph = target.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(meleeDamage);
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
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}

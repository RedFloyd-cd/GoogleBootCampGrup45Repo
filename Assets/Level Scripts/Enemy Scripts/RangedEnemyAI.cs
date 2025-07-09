using UnityEngine;

public class RangedEnemyAI : MonoBehaviour
{
    public Transform target; // Takip edilecek karakter (Player)
    public float detectionRadius = 15f; // Algılama yarıçapı
    public float attackRange = 12f; // Saldırı menzili (uzaktan)
    public float attackCooldown = 1.5f; // Saldırılar arası bekleme süresi
    public GameObject projectilePrefab; // Fırlatılacak mermi prefabı
    public float projectileSpeed = 10f; // Mermi hızı
    public float health = 80f; // Düşman canı
    public float damage = 8f; // Düşman hasarı
    public GameObject ammoPickupPrefab; // Inspector'dan atanacak
    public Animator animator; // Animator referansı

    private float maxHealth;
    private bool isFleeing = false;
    private float lastAttackTime;
    private bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = health;
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        if (target == null) {
            if (animator != null) animator.SetBool("isMoving", false);
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        // Can %30'un altına düştüyse kaçma moduna geç
        if (!isFleeing && health <= maxHealth * 0.3f)
        {
            isFleeing = true;
        }

        if (distance <= detectionRadius && HasLineOfSight())
        {
            if (isFleeing)
            {
                // Kaç: oyuncudan uzaklaş
                Vector3 direction = (transform.position - target.position);
                if (direction.magnitude > 0.05f)
                {
                    direction = direction.normalized;
                    transform.position += direction * projectileSpeed * Time.deltaTime;
                    if (animator != null) animator.SetBool("isMoving", true);
                }
                else
                {
                    if (animator != null) animator.SetBool("isMoving", false);
                }
                if (animator != null) animator.SetBool("isAttacking", false);
                // Kaçış yönüne dön
                if (direction != Vector3.zero)
                {
                    Quaternion toRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
                }
                // Kaçarken de ateş etmeye devam et
                if (Time.time - lastAttackTime > attackCooldown && distance <= attackRange)
                {
                    if (animator != null) animator.SetBool("isAttacking", true);
                    lastAttackTime = Time.time;
                    Attack();
                }
            }
            else if (distance > attackRange)
            {
                // Takip et
                Vector3 direction = (target.position - transform.position);
                if (direction.magnitude > 0.05f)
                {
                    direction = direction.normalized;
                    transform.position += direction * projectileSpeed * Time.deltaTime;
                    if (animator != null) animator.SetBool("isMoving", true);
                }
                else
                {
                    if (animator != null) animator.SetBool("isMoving", false);
                }
                if (animator != null) animator.SetBool("isAttacking", false);
                // Oyuncuya bak
                Vector3 lookDir = (target.position - transform.position).normalized;
                lookDir.y = 0;
                if (lookDir != Vector3.zero)
                {
                    Quaternion toRotation = Quaternion.LookRotation(lookDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
                }
            }
            else // Saldırı menzili içindeyse
            {
                // Hedefe bak
                Vector3 lookDir = (target.position - transform.position).normalized;
                lookDir.y = 0;
                if (lookDir != Vector3.zero)
                {
                    Quaternion toRotation = Quaternion.LookRotation(lookDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
                }
                // Saldırı
                if (Time.time - lastAttackTime > attackCooldown)
                {
                    if (animator != null) animator.SetBool("isAttacking", true);
                    if (animator != null) animator.SetBool("isMoving", false);
                    lastAttackTime = Time.time;
                    Attack();
                }
            }
        }
        else
        {
            if (animator != null) animator.SetBool("isMoving", false);
            if (animator != null) animator.SetBool("isAttacking", false);
        }
    }

    void Attack()
    {
        if (projectilePrefab != null && target != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward, Quaternion.identity);

            // Eğer prefab'e önceden eklemediysen:
            if (projectile.GetComponent<EnemyProjectile>() == null)
            {
                projectile.AddComponent<EnemyProjectile>().damage = damage;
            }

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (target.position - transform.position).normalized;
                rb.linearVelocity = dir * projectileSpeed;
            }
        }

        Debug.Log($"Ranged düşman ateş etti! {damage} hasar verdi.");
    }

    // Düşman ile oyuncu arasında engel var mı kontrolü
    bool HasLineOfSight()
    {
        RaycastHit hit;
        Vector3 direction = (target.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target.position);
        if (Physics.Raycast(transform.position, direction, out hit, distance))
        {
            if (hit.transform == target)
                return true;
            else
                return false;
        }
        return false;
    }

    // Algılama ve saldırı bölgelerini sahnede görmek için
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        // Bullet objesinden gelen hasar
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
        Destroy(gameObject, 2f); // 2 saniye sonra yok et, animasyon için zaman tanı
    }
}

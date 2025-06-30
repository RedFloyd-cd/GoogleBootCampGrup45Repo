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

    private float maxHealth;
    private bool isFleeing = false;
    private float lastAttackTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        // Can %30'un altına düştüyse kaçma moduna geç
        if (!isFleeing && health <= maxHealth * 0.3f)
        {
            isFleeing = true;
        }

        if (distance <= detectionRadius)
        {
            if (isFleeing)
            {
                // Kaç: oyuncudan uzaklaş
                Vector3 direction = (transform.position - target.position).normalized;
                transform.position += direction * projectileSpeed * Time.deltaTime;

                // Kaçarken de ateş etmeye devam et
                if (Time.time - lastAttackTime > attackCooldown && distance <= attackRange)
                {
                    Attack();
                    lastAttackTime = Time.time;
                }
            }
            else if (distance <= attackRange)
            {
                // Hedefe bak
                Vector3 lookDir = (target.position - transform.position).normalized;
                lookDir.y = 0; // Sadece yatay düzlemde bak
                if (lookDir != Vector3.zero)
                    transform.forward = lookDir;

                // Saldırı
                if (Time.time - lastAttackTime > attackCooldown)
                {
                    Attack();
                    lastAttackTime = Time.time;
                }
            }
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
        // Bullet objesinden gelen hasar
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Düşman öldüğünde yapılacaklar (animasyon, efekt vs.)
        Destroy(gameObject);
    }
}

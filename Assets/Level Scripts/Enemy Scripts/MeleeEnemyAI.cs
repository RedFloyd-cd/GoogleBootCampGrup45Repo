using UnityEngine;

public class MeleeEnemyAI : MonoBehaviour
{
    public Transform target; // Takip edilecek karakter (Player)
    public float detectionRadius = 10f; // Algılama yarıçapı
    public float attackRange = 2f; // Saldırı menzili
    public float moveSpeed = 3f; // Düşman hareket hızı
    public float attackCooldown = 1.5f; // Saldırılar arası bekleme süresi
    public float health = 100f; // Düşman canı
    public float damage = 10f; // Düşman hasarı

    private float lastAttackTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= detectionRadius)
        {
            // Takip et
            if (distance > attackRange)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
            else
            {
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
        if (target != null)
        {
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }


    public void TakeDamage(float amount)
    {
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

    // Algılama ve saldırı bölgelerini sahnede görmek için
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

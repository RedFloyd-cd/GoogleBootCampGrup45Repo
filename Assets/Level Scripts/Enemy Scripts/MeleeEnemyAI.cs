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
    public GameObject ammoPickupPrefab; // Inspector'dan atanacak

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

        if (distance <= detectionRadius && HasLineOfSight())
        {
            if (isFleeing)
            {
                // Kaç: oyuncudan uzaklaş
                Vector3 direction = (transform.position - target.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
            else
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
        // %20 ihtimalle ammo drop
        if (ammoPickupPrefab != null && Random.value <= 0.2f)
        {
            Instantiate(ammoPickupPrefab, transform.position, Quaternion.identity);
        }
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

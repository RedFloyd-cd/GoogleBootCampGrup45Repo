using UnityEngine;

public class MinibossAI : MonoBehaviour
{
    public Transform target; // Takip edilecek karakter (Player)
    public float detectionRadius = 18f; // Algılama yarıçapı
    public float meleeRange = 2.5f; // Yakın dövüş menzili
    public float rangedRange = 12f; // Uzaktan saldırı menzili
    public float moveSpeed = 2f; // Yavaş hareket hızı
    public float meleeCooldown = 2f; // Yakın saldırı bekleme süresi
    public float rangedCooldown = 2.5f; // Uzaktan saldırı bekleme süresi
    public GameObject projectilePrefab; // Fırlatılacak mermi prefabı
    public float projectileSpeed = 8f; // Mermi hızı
    public float health = 300f; // Miniboss canı
    public float meleeDamage = 25f; // Yakın dövüş hasarı
    public float rangedDamage = 15f; // Uzaktan saldırı hasarı
    public GameObject ammoPickupPrefab; // Inspector'dan atanacak

    private float maxHealth;
    private bool isEnraged = false;
    private float lastMeleeTime;
    private float lastRangedTime;

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

        // Can %30'un altına düştüyse agresif moda geç
        if (!isEnraged && health <= maxHealth * 0.3f)
        {
            isEnraged = true;
            meleeCooldown *= 0.5f;
            rangedCooldown *= 0.5f;
        }

        if (distance <= detectionRadius && HasLineOfSight())
        {
            // Hedefe bak
            Vector3 lookDir = (target.position - transform.position).normalized;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                transform.forward = lookDir;

            // Hareket: melee menzili dışında ise yaklaş
            if (distance > meleeRange)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
            }

            // Melee saldırı
            if (distance <= meleeRange)
            {
                if (Time.time - lastMeleeTime > meleeCooldown)
                {
                    MeleeAttack();
                    lastMeleeTime = Time.time;
                }
            }
            // Ranged saldırı (melee menzili dışında ve ranged menzili içinde)
            else if (distance <= rangedRange)
            {
                if (Time.time - lastRangedTime > rangedCooldown)
                {
                    RangedAttack();
                    lastRangedTime = Time.time;
                }
            }
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

    void RangedAttack()
    {
        if (projectilePrefab != null && target != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward, Quaternion.identity);
            projectile.AddComponent<EnemyProjectile>().damage = rangedDamage;

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (target.position - transform.position).normalized;
                rb.linearVelocity = dir * projectileSpeed;
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

    // Algılama ve saldırı bölgelerini sahnede görmek için
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rangedRange);
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
}

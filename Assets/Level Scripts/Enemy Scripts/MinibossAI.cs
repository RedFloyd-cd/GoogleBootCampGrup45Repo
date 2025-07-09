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
    public Animator animator; // Animator referansı

    private float maxHealth;
    private bool isEnraged = false;
    private float lastMeleeTime;
    private float lastRangedTime;
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

        // Can %30'un altına düştüyse agresif moda geç
        if (!isEnraged && health <= maxHealth * 0.3f)
        {
            isEnraged = true;
            meleeCooldown *= 0.5f;
            rangedCooldown *= 0.5f;
        }

        if (distance <= detectionRadius && HasLineOfSight())
        {
            // Hedefe bak (yumuşak dönüş)
            Vector3 lookDir = (target.position - transform.position).normalized;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(lookDir) * Quaternion.Euler(0, 180, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            }

            // Hareket: melee menzili dışında ise yaklaş
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

            // Melee saldırı
            if (distance <= meleeRange)
            {
                if (Time.time - lastMeleeTime > meleeCooldown)
                {
                    if (animator != null) animator.SetTrigger("meleeAttack");
                    if (animator != null) animator.SetBool("isAttacking", true);
                    if (animator != null) animator.SetBool("isMoving", false);
                    lastMeleeTime = Time.time;
                    MeleeAttack();
                }
            }
            // Ranged saldırı (melee menzili dışında ve ranged menzili içinde)
            else if (distance <= rangedRange)
            {
                if (Time.time - lastRangedTime > rangedCooldown)
                {
                    if (animator != null) animator.SetTrigger("rangedAttack");
                    if (animator != null) animator.SetBool("isAttacking", true);
                    if (animator != null) animator.SetBool("isMoving", false);
                    lastRangedTime = Time.time;
                    RangedAttack();
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

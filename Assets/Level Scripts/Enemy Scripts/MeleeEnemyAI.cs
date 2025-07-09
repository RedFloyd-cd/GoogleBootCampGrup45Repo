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

        if (isFleeing)
        {
            // Kaç: oyuncudan uzaklaş
            Vector3 fleeDirection = (transform.position - target.position);
            if (fleeDirection.magnitude > 0.05f) // Çok yakınsa hareket etme
            {
                fleeDirection = fleeDirection.normalized;
                transform.position += fleeDirection * moveSpeed * Time.deltaTime;
                if (animator != null) animator.SetBool("isMoving", true);
            }
            else
            {
                if (animator != null) animator.SetBool("isMoving", false);
            }
            if (animator != null) animator.SetBool("isAttacking", false);
            // Kaçış yönüne dön
            if (fleeDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(fleeDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            }
        }
        else
        {
            if (distance > attackRange)
            {
                Vector3 direction = (target.position - transform.position);
                if (direction.magnitude > 0.05f) // Çok yakınsa hareket etme
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
                // Oyuncuya bak
                Vector3 lookDir = (target.position - transform.position).normalized;
                if (lookDir != Vector3.zero)
                {
                    Quaternion toRotation = Quaternion.LookRotation(lookDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
                }
            }
            else
            {
                // Saldırı
                if (Time.time - lastAttackTime > attackCooldown)
                {
                    if (animator != null) animator.SetBool("isAttacking", true);
                    if (animator != null) animator.SetBool("isMoving", false);
                    lastAttackTime = Time.time;
                    // Hasar animasyon eventiyle verilecekse Attack() burada çağrılmaz
                    // Attack(); // Eğer animasyon eventi yoksa burayı açın
                }
                // Saldırı sırasında da oyuncuya bak
                Vector3 lookDir = (target.position - transform.position).normalized;
                if (lookDir != Vector3.zero)
                {
                    Quaternion toRotation = Quaternion.LookRotation(lookDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
                }
            }
        }
    }

    // Animasyon eventiyle çağrılabilir
    public void Attack()
    {
        if (isDead) return;
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

    // Algılama ve saldırı bölgelerini sahnede görmek için
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

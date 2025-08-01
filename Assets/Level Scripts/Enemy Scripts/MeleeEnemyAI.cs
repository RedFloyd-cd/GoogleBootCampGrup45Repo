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

    [Header("Obstacle Avoidance")]
    public float obstacleCheckDistance = 1.5f; // Engel kontrol mesafesi
    public float avoidanceStrength = 2f; // Kaçınma gücü
    public LayerMask obstacleLayerMask = -1; // Engel layer'ları

    private float maxHealth;
    private bool isFleeing = false;
    private float lastAttackTime;
    private bool isDead = false;

    public AudioSource audioSource;
    public AudioClip enemyVoiceClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = health;
        if (animator == null)
            animator = GetComponent<Animator>();

        audioSource.volume = 0.1f;
        audioSource.clip = enemyVoiceClip;
        audioSource.Play();
        
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
                // Engel kontrolü ve kaçınma
                Vector3 adjustedDirection = AvoidObstacles(fleeDirection);
                transform.position += adjustedDirection * moveSpeed * Time.deltaTime;
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
                    // Engel kontrolü ve kaçınma
                    Vector3 adjustedDirection = AvoidObstacles(direction);
                    transform.position += adjustedDirection * moveSpeed * Time.deltaTime;
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
                    Attack(); // Animasyon eventi yoksa burada çağrılır
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

    // Engel kaçınma fonksiyonu
    Vector3 AvoidObstacles(Vector3 desiredDirection)
    {
        Vector3 avoidanceDirection = Vector3.zero;
        
        // Önünde engel var mı kontrol et
        if (Physics.Raycast(transform.position, desiredDirection, obstacleCheckDistance, obstacleLayerMask))
        {
            // Sağa ve sola raycast atarak alternatif yollar ara
            Vector3 rightDirection = Quaternion.Euler(0, 45, 0) * desiredDirection;
            Vector3 leftDirection = Quaternion.Euler(0, -45, 0) * desiredDirection;
            
            bool rightBlocked = Physics.Raycast(transform.position, rightDirection, obstacleCheckDistance, obstacleLayerMask);
            bool leftBlocked = Physics.Raycast(transform.position, leftDirection, obstacleCheckDistance, obstacleLayerMask);
            
            if (!rightBlocked && leftBlocked)
            {
                // Sağa git
                avoidanceDirection = rightDirection;
            }
            else if (rightBlocked && !leftBlocked)
            {
                // Sola git
                avoidanceDirection = leftDirection;
            }
            else if (!rightBlocked && !leftBlocked)
            {
                // Her iki taraf da açık, daha iyi olanı seç
                float rightDistance = Vector3.Distance(transform.position + rightDirection * obstacleCheckDistance, target.position);
                float leftDistance = Vector3.Distance(transform.position + leftDirection * obstacleCheckDistance, target.position);
                
                avoidanceDirection = rightDistance < leftDistance ? rightDirection : leftDirection;
            }
            else
            {
                // Her iki taraf da kapalı, geri git
                avoidanceDirection = -desiredDirection;
            }
        }
        else
        {
            // Engel yok, normal yönde git
            avoidanceDirection = desiredDirection;
        }
        
        return avoidanceDirection.normalized;
    }

    // Animasyon eventiyle çağrılabilir
    public void Attack()
    {
        if (isDead) return;
        if (target != null)
        {
            PlayerController playerController = target.GetComponentInChildren<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(damage);
                Debug.Log($"Player'a {damage} hasar verildi!");
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
        // %40 ihtimalle ammo drop
        if (ammoPickupPrefab != null && Random.value <= 0.4f)
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
        
        // Engel kontrol raycast'lerini göster
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, direction * obstacleCheckDistance);
            
            Vector3 rightDirection = Quaternion.Euler(0, 45, 0) * direction;
            Vector3 leftDirection = Quaternion.Euler(0, -45, 0) * direction;
            
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, rightDirection * obstacleCheckDistance);
            Gizmos.DrawRay(transform.position, leftDirection * obstacleCheckDistance);
        }
    }
}

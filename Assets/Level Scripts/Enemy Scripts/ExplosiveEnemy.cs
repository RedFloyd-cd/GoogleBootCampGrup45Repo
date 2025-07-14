using UnityEngine;

public class ExplosiveEnemy : MonoBehaviour
{
    public Transform target;
    public float detectionRadius = 12f;
    public float explosionRange = 2.2f;
    public float moveSpeed = 6f;
    public float health = 60f;
    public float explosionDamage = 40f;
    public GameObject ammoPickupPrefab;
    public GameObject explosionEffectPrefab;
    public Animator animator;

    private float maxHealth;
    private bool isDead = false;
    private bool isExploded = false;

    void Start()
    {
        maxHealth = health;
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead || isExploded) return;
        if (target == null) {
            if (animator != null) animator.SetBool("isMoving", false);
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= detectionRadius)
        {
            // Hedefe bak
            Vector3 lookDir = (target.position - transform.position).normalized;
            if (lookDir != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            }

            // Hedefe hızlıca koş
            if (distance > explosionRange)
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
            }
            else
            {
                // Patlat
                Explode();
            }
        }
        else
        {
            if (animator != null) animator.SetBool("isMoving", false);
        }
    }

    void Explode()
    {
        if (isExploded) return;
        isExploded = true;
        if (animator != null) animator.SetTrigger("explode");
        // Patlama efekti
        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        // Oyuncuya hasar ver
        if (target != null && Vector3.Distance(transform.position, target.position) <= explosionRange + 0.5f)
        {
            PlayerHealth ph = target.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(explosionDamage);
            }
        }
        // %20 ihtimalle ammo drop
        if (ammoPickupPrefab != null && Random.value <= 0.2f)
        {
            Instantiate(ammoPickupPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject, 0.2f); // Patlama animasyonu için kısa bir gecikme
    }

    public void TakeDamage(float amount)
    {
        if (isDead || isExploded) return;
        health -= amount;
        if (health <= 0)
        {
            isDead = true;
            Explode(); // Ölünce de patla
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}

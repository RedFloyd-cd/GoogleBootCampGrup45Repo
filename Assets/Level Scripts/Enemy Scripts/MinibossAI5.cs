using UnityEngine;

public class MinibossAI5 : MonoBehaviour
{
    public Transform target;
    public float detectionRadius = 20f;
    public float meleeRange = 2.5f;
    public float moveSpeed = 2f;
    public float attackCooldown = 1.5f;
    public float health = 1000f; // En fazla can
    public float meleeDamage = 30f;
    public float areaAttackRadius = 5f;
    public float areaAttackDamage = 45f;
    public float teleportCooldown = 2.5f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 12f;
    public float projectileDamage = 20f;
    public GameObject ammoPickupPrefab;
    public GameObject explosionEffectPrefab;
    public float explosionRange = 3f;
    public float explosionDamage = 60f;
    public Animator animator;
    public Vector3 teleportAreaCenter;
    public Vector3 teleportAreaSize = new Vector3(12, 0, 12);

    private float maxHealth;
    private float lastAttackTime;
    private float lastTeleportTime;
    private bool isDead = false;
    private int currentPhase = 1;

    void Start()
    {
        maxHealth = health;
        teleportAreaCenter = transform.position;
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

        UpdatePhase();

        switch (currentPhase)
        {
            case 1:
                Phase1_Basic(); // Standart miniboss
                break;
            case 2:
                Phase2_FastAggressive(); // Hızlı saldırı
                break;
            case 3:
                Phase3_AreaAttack(); // AoE saldırı
                break;
            case 4:
                Phase4_TeleportProjectile(); // Teleport + projectile
                break;
            case 5:
                Phase5_Explode(); // Patlama
                break;
        }
    }

    void UpdatePhase()
    {
        float healthPercent = health / maxHealth;
        int newPhase = 1;
        if (healthPercent <= 0.2f) newPhase = 5;
        else if (healthPercent <= 0.4f) newPhase = 4;
        else if (healthPercent <= 0.6f) newPhase = 3;
        else if (healthPercent <= 0.8f) newPhase = 2;
        else newPhase = 1;
        if (newPhase != currentPhase)
        {
            currentPhase = newPhase;
            if (animator != null) animator.SetTrigger("phaseChange");
        }
    }

    // Faz 1: Standart miniboss
    void Phase1_Basic()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= detectionRadius)
        {
            MoveTowardsTarget(distance);
            if (distance <= meleeRange && Time.time - lastAttackTime > attackCooldown)
            {
                if (animator != null) animator.SetTrigger("meleeAttack");
                lastAttackTime = Time.time;
                MeleeAttack();
            }
        }
    }

    // Faz 2: Hızlı saldırı
    void Phase2_FastAggressive()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        float fastMove = moveSpeed * 2.2f;
        float fastCooldown = attackCooldown * 0.5f;
        if (distance <= detectionRadius)
        {
            MoveTowardsTarget(distance, fastMove);
            if (distance <= meleeRange && Time.time - lastAttackTime > fastCooldown)
            {
                if (animator != null) animator.SetTrigger("meleeAttack");
                lastAttackTime = Time.time;
                MeleeAttack();
            }
        }
    }

    // Faz 3: AoE saldırı
    void Phase3_AreaAttack()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        float slowMove = moveSpeed * 0.7f;
        if (distance <= detectionRadius)
        {
            MoveTowardsTarget(distance, slowMove);
            if (distance <= areaAttackRadius && Time.time - lastAttackTime > attackCooldown * 1.2f)
            {
                if (animator != null) animator.SetTrigger("areaAttack");
                lastAttackTime = Time.time;
                AreaAttack();
            }
        }
    }

    // Faz 4: Teleport + projectile
    void Phase4_TeleportProjectile()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= detectionRadius)
        {
            if (Time.time - lastTeleportTime > teleportCooldown)
            {
                TeleportRandomly();
                lastTeleportTime = Time.time;
                if (animator != null) animator.SetTrigger("teleport");
                Invoke(nameof(FireProjectile), 0.3f);
            }
        }
    }

    // Faz 5: Patlama (ölümcül saldırı)
    void Phase5_Explode()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= explosionRange && Time.time - lastAttackTime > attackCooldown)
        {
            if (animator != null) animator.SetTrigger("explode");
            lastAttackTime = Time.time;
            Explode();
        }
    }

    void MoveTowardsTarget(float distance, float speedOverride = -1f)
    {
        float speed = speedOverride > 0 ? speedOverride : moveSpeed;
        if (distance > meleeRange)
        {
            Vector3 direction = (target.position - transform.position);
            if (direction.magnitude > 0.05f)
            {
                direction = direction.normalized;
                transform.position += direction * speed * Time.deltaTime;
                if (animator != null) animator.SetBool("isMoving", true);
            }
            else
            {
                if (animator != null) animator.SetBool("isMoving", false);
            }
        }
        else
        {
            if (animator != null) animator.SetBool("isMoving", false);
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

    void AreaAttack()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, areaAttackRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth ph = hit.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    ph.TakeDamage(areaAttackDamage);
                }
            }
        }
    }

    void TeleportRandomly()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-teleportAreaSize.x / 2, teleportAreaSize.x / 2),
            0,
            Random.Range(-teleportAreaSize.z / 2, teleportAreaSize.z / 2)
        );
        Vector3 newPos = teleportAreaCenter + randomOffset;
        transform.position = newPos;
    }

    void FireProjectile()
    {
        if (projectilePrefab != null && target != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward, Quaternion.identity);
            if (projectile.GetComponent<EnemyProjectile>() == null)
            {
                projectile.AddComponent<EnemyProjectile>().damage = projectileDamage;
            }
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (target.position - transform.position).normalized;
                rb.linearVelocity = dir * projectileSpeed;
            }
        }
    }

    void Explode()
    {
        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth ph = hit.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    ph.TakeDamage(explosionDamage);
                }
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
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, areaAttackRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(teleportAreaCenter, teleportAreaSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}

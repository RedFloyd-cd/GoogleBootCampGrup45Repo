using Unity.Netcode;
using UnityEngine;

public class OnlinePlayer : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float maxHealth = 100f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce = 20f;
    public float fireRate = 0.4f;
    private float lastFireTime = 0f;

    public NetworkVariable<float> health = new NetworkVariable<float>(100f);

    void Start()
    {
        if (IsOwner)
        {
            health.Value = maxHealth;
        }
    }

    void Update()
    {
        if (!IsOwner) return; // Sadece kendi karakterini kontrol et

        HandleMovement();
        HandleFire();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);

        // Mouse ile yön dönme (isteğe bağlı)
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        if (ground.Raycast(ray, out float enter))
        {
            Vector3 point = ray.GetPoint(enter);
            Vector3 dir = (point - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.forward = dir;
        }
    }

    void HandleFire()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastFireTime > fireRate)
        {
            lastFireTime = Time.time;
            FireServerRpc();
        }
    }

    [ServerRpc]
    void FireServerRpc(ServerRpcParams rpcParams = default)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody>().linearVelocity = firePoint.forward * fireForce;
        bullet.GetComponent<OnlineBullet>().ownerId = OwnerClientId;
        bullet.GetComponent<NetworkObject>().Spawn();
    }

    public void TakeDamage(float amount)
    {
        if (IsServer)
        {
            health.Value -= amount;
            if (health.Value <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        // Oyuncuyu öldür
        if (IsServer)
            NetworkObject.Despawn();
    }
}

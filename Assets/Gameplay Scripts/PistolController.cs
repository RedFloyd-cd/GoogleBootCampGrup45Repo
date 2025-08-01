using UnityEngine;
using TMPro;

public class PistolController : MonoBehaviour
{ 

    [Header("Pistol Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce = 20f;
    public float fireRate = 0.4f; // Atışlar arası süre
    public int magazineSize = 8;
    public float reloadTime = 1.5f;
    public float bulletDamage = 50f;
    public TextMeshProUGUI ammoText;
    public int maxAmmo = 32;
    private int currentTotalAmmo;

    private int currentAmmo;
    private float lastFireTime;
    private bool isReloading = false;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip fireClip;
    public AudioClip reloadClip;
    public float fireVolume = 0.7f;
    public float reloadVolume = 0.7f;

    void Start()
    {
        bulletDamage = PlayerStats.Instance.damage;
        currentAmmo = magazineSize;
        currentTotalAmmo = maxAmmo;
        UpdateAmmoUI();
        
        // Debug: Pistol ayarlarını kontrol et
        Debug.Log($"Pistol başlatıldı - BulletDamage: {bulletDamage}");
        Debug.Log($"BulletPrefab: {(bulletPrefab != null ? bulletPrefab.name : "NULL")}");
        Debug.Log($"FirePoint: {(firePoint != null ? firePoint.name : "NULL")}");
        Debug.Log($"FireForce: {fireForce}");
        Debug.Log($"MagazineSize: {magazineSize}, MaxAmmo: {maxAmmo}");
    }

    void Update()
    {
        if (isReloading) return;

        // Eğer şarjör ve toplam cephane bitti ise ateş etme
        if (currentAmmo <= 0 && currentTotalAmmo <= 0)
        {
            return;
        }

        if (currentAmmo <= 0)
        {
            if (Input.GetKeyDown(KeyCode.R) && currentTotalAmmo > 0)
            {
                StartCoroutine(Reload());
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magazineSize && currentTotalAmmo > 0)
        {
            StartCoroutine(Reload());
        }

        if (Input.GetMouseButtonDown(0) && Time.time - lastFireTime > fireRate)
        {
            Fire();
        }
    }

    void Fire()
    {
        if (currentAmmo <= 0) return;
        lastFireTime = Time.time;
        currentAmmo--;
        UpdateAmmoUI();

        // Debug: Mermi prefab kontrolü
        if (bulletPrefab == null)
        {
            Debug.LogError("BulletPrefab atanmamış! Inspector'da bulletPrefab'ı atayın.");
            return;
        }

        // Debug: FirePoint kontrolü
        if (firePoint == null)
        {
            Debug.LogError("FirePoint atanmamış! Inspector'da firePoint'i atayın.");
            return;
        }

        // Ateş sesi çal
        if (audioSource != null && fireClip != null)
        {
            audioSource.PlayOneShot(fireClip, fireVolume);
        }

        Vector3 direction = GetFiringDirection();
        Debug.Log($"Ateş yönü: {direction}");

        // Debug için raycast çizgisi (sadece geliştirme sırasında)
        Debug.DrawRay(firePoint.position, direction * 10f, Color.red, 1f);

        // Mermi oluştur
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
        
        if (bullet == null)
        {
            Debug.LogError("Mermi oluşturulamadı!");
            return;
        }
        
        Debug.Log($"Mermi oluşturuldu: {bullet.name} pozisyon: {bullet.transform.position}");

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * fireForce;
            // Collision detection'ı aç
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            Debug.Log($"Mermi hızı: {rb.linearVelocity}");
        }
        else
        {
            Debug.LogWarning("Mermi prefabında Rigidbody yok!");
        }

        // Bullet scripti yerine dinamik olarak çarpışma ve hasar scripti ekle
        bullet.AddComponent<BulletCollision>().Init(bulletDamage);
        Destroy(bullet, 3f); // mermiyi 3 saniye sonra yok et
        
        Debug.Log($"Mermi ateşlendi - Hasar: {bulletDamage}, Kalan mermi: {currentAmmo}");
    }

    // Daha güvenilir ateş yönü hesaplama
    Vector3 GetFiringDirection()
    {
        // Mouse pozisyonunu dünya koordinatlarına çevir
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.y; // Kameranın yüksekliğini kullan
        
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        
        // FirePoint'ten mouse pozisyonuna yön hesapla
        Vector3 direction = (worldMousePos - firePoint.position).normalized;
        
        // Eğer yön çok küçükse (mouse firePoint'in üzerindeyse), kameranın ileri yönünü kullan
        if (direction.magnitude < 0.1f)
        {
            direction = Camera.main.transform.forward;
        }
        
        return direction;
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        // Reload sesi çal
        if (audioSource != null && reloadClip != null)
        {
            audioSource.PlayOneShot(reloadClip, reloadVolume);
        }
        // Burada reload animasyonu oynatılabilir
        yield return new WaitForSeconds(reloadTime);
        int neededAmmo = magazineSize - currentAmmo;
        int ammoToReload = Mathf.Min(neededAmmo, currentTotalAmmo);
        currentAmmo += ammoToReload;
        currentTotalAmmo -= ammoToReload;
        UpdateAmmoUI();
        isReloading = false;
    }

    void UpdateAmmoUI()
    {
        ammoText.text = currentAmmo + " / " + magazineSize + " | " + currentTotalAmmo;
    }

    public void AddAmmo(int amount)
    {
        currentTotalAmmo = Mathf.Min(currentTotalAmmo + amount, maxAmmo);
        UpdateAmmoUI();
    }

    public int GetCurrentTotalAmmo()
    {
        return currentTotalAmmo;
    }
}

public class BulletCollision : MonoBehaviour
{
    private float damage;
    public void Init(float dmg) { damage = dmg; }
    
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Mermi collision: {collision.gameObject.name}");
        CheckForEnemy(collision.gameObject);
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Mermi trigger: {other.gameObject.name}");
        CheckForEnemy(other.gameObject);
    }
    
    void CheckForEnemy(GameObject target)
    {
        // Debug: Collider bilgilerini yazdır
        var collider = target.GetComponent<Collider>();
        if (collider != null)
        {
            Debug.Log($"Hedef: {target.name}, IsTrigger: {collider.isTrigger}, Layer: {target.layer}");
        }
        
        // PoisonEnemyAI
        var poison = target.GetComponent<PoisonEnemyAI>();
        if (poison != null)
        {
            Debug.Log($"PoisonEnemy'ye çarptı, hasar: {damage}");
            poison.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        // MeleeEnemyAI
        var melee = target.GetComponent<MeleeEnemyAI>();
        if (melee != null)
        {
            Debug.Log($"MeleeEnemy'ye çarptı, hasar: {damage}");
            melee.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        // RangedEnemyAI
        var ranged = target.GetComponent<RangedEnemyAI>();
        if (ranged != null)
        {
            Debug.Log($"RangedEnemy'ye çarptı, hasar: {damage}");
            ranged.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        // MinibossAI
        var miniboss = target.GetComponent<MinibossAI>();
        if (miniboss != null)
        {
            Debug.Log($"MinibossAI'ye çarptı, hasar: {damage}");
            miniboss.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        // MinibossAI4
        var miniboss4 = target.GetComponent<MinibossAI4>();
        if (miniboss4 != null)
        {
            Debug.Log($"MinibossAI4'e çarptı, hasar: {damage}");
            miniboss4.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        
        // Eğer düşman değilse ve duvar/engel ise mermiyi yok et
        if (target.layer == LayerMask.NameToLayer("Default") || 
            target.layer == LayerMask.NameToLayer("Ground") ||
            target.CompareTag("Wall"))
        {
            Debug.Log($"Duvara çarptı: {target.name}");
            Destroy(gameObject);
        }
    }
}

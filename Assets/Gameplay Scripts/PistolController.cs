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
    public float bulletDamage = 20f;
    public float minFireDistance = 1.5f; // Minimum ateş mesafesi
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

        // Ateş sesi çal
        if (audioSource != null && fireClip != null)
        {
            audioSource.PlayOneShot(fireClip, fireVolume);
        }

        Vector3 direction = GetFiringDirection();

        // Hedef çok yakınsa ateş etme (firePoint'ten belirli bir mesafede kontrol)
        if (Vector3.Distance(firePoint.position, firePoint.position + direction * minFireDistance) < minFireDistance)
            return;

        // Debug için raycast çizgisi (sadece geliştirme sırasında)
        Debug.DrawRay(firePoint.position, direction * 10f, Color.red, 1f);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * fireForce;
        }
        // Bullet scripti yerine dinamik olarak çarpışma ve hasar scripti ekle
        bullet.AddComponent<BulletCollision>().Init(bulletDamage);
        Destroy(bullet, 3f); // mermiyi 3 saniye sonra yok et
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
        // PoisonEnemyAI
        var poison = collision.gameObject.GetComponent<PoisonEnemyAI>();
        if (poison != null)
        {
            poison.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        // MeleeEnemyAI
        var melee = collision.gameObject.GetComponent<MeleeEnemyAI>();
        if (melee != null)
        {
            melee.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        // RangedEnemyAI
        var ranged = collision.gameObject.GetComponent<RangedEnemyAI>();
        if (ranged != null)
        {
            ranged.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        // MinibossAI
        var miniboss = collision.gameObject.GetComponent<MinibossAI>();
        if (miniboss != null)
        {
            miniboss.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        // MinibossAI4
        var miniboss4 = collision.gameObject.GetComponent<MinibossAI4>();
        if (miniboss4 != null)
        {
            miniboss4.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        // İsteğe bağlı: başka bir şeye çarparsa da yok et
        Destroy(gameObject);
    }
}

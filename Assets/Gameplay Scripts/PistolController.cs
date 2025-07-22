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

        // Mouse pozisyonunu bul
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100f);
        }
        // Yalnızca yatay düzlemde hedefle
        targetPoint.y = firePoint.position.y;

        // Hedef çok yakınsa ateş etme
        if (Vector3.Distance(firePoint.position, targetPoint) < minFireDistance)
            return;

        Vector3 direction = (targetPoint - firePoint.position).normalized;
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

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
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
    void OnTriggerEnter(Collider other)
    {
        // MeleeEnemyAI
        var melee = other.GetComponent<MeleeEnemyAI>();
        if (melee != null)
        {
            melee.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        // RangedEnemyAI
        var ranged = other.GetComponent<RangedEnemyAI>();
        if (ranged != null)
        {
            ranged.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        // MinibossAI
        var miniboss = other.GetComponent<MinibossAI>();
        if (miniboss != null)
        {
            miniboss.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        // MinibossAI4
        var miniboss4 = other.GetComponent<MinibossAI4>();
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

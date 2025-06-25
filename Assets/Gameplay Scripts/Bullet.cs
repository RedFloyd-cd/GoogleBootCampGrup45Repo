using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 20f;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
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
        // İsteğe bağlı: başka bir şeye çarparsa da yok et
        Destroy(gameObject);
    }
}

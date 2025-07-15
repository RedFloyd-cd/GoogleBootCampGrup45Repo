using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 20f;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

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

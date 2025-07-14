using UnityEngine;

public class PoisonProjectile : MonoBehaviour
{
    public float damage = 6f;
    public float poisonDuration = 4f;
    public float poisonTickInterval = 0.5f;
    public float poisonTickDamage = 2f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
                ph.ApplyPoison(poisonTickDamage, poisonDuration, poisonTickInterval);
            }
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
} 
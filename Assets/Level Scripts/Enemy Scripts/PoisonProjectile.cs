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
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.TakeDamage(damage);
                pc.ApplyPoison(poisonTickDamage, poisonDuration, poisonTickInterval);
            }
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
} 
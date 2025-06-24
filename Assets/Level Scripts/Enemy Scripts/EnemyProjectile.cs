using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage = 8f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Çarptýðý þey: {other.name}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Oyuncuya çarptý!");
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject); // sadece oyuncuya çarptýðýnda sil
        }
    }
}

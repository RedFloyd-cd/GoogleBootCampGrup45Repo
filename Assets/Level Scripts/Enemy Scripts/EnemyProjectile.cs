using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage = 8f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"�arpt��� �ey: {other.name}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Oyuncuya �arpt�!");
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject); // sadece oyuncuya �arpt���nda sil
        }
    }
}

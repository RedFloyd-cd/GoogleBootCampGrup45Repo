using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage = 8f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"�arpt��� �ey: {other.name}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Oyuncuya çarptı!");
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(damage);
            }
            Destroy(gameObject); // sadece oyuncuya çarptığında sil
        }
    }
}

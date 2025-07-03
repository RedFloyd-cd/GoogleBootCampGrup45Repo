using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 6;

    private void OnTriggerEnter(Collider other)
    {
        PistolController pistol = other.GetComponentInChildren<PistolController>();
        if (pistol != null)
        {
            if (pistol.GetCurrentTotalAmmo() < pistol.maxAmmo)
            {
                pistol.AddAmmo(ammoAmount);
                Destroy(gameObject);
            }
        }
    }
} 
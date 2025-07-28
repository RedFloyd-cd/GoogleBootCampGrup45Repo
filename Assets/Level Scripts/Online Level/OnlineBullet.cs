using Unity.Netcode;
using UnityEngine;

public class OnlineBullet : NetworkBehaviour
{
    public float damage = 20f;
    [HideInInspector] public ulong ownerId;

    void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        var player = other.GetComponent<OnlinePlayer>();
        if (player != null && player.OwnerClientId != ownerId)
        {
            player.TakeDamage(damage);
            NetworkObject.Despawn();
        }
        else if (!other.CompareTag("Player"))
        {
            NetworkObject.Despawn();
        }
    }
}

using Unity.Netcode;
using UnityEngine;
using System.Linq;

public class NetworkGameManager : NetworkBehaviour
{
    public void CheckForWinner()
    {
        var players = FindObjectsOfType<OnlinePlayer>();
        var alivePlayers = players.Where(p => p.health.Value > 0).ToList();

        if (alivePlayers.Count == 1)
        {
            // Kazananı ilan et
            Debug.Log("Kazanan: " + alivePlayers[0].OwnerClientId);
            // Burada UI ile kazananı gösterebilirsin
        }
    }
}

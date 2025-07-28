using Unity.Netcode;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    public void Host()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void Join(string ip)
    {
        NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>().ConnectionData.Address = ip;
        NetworkManager.Singleton.StartClient();
    }
}

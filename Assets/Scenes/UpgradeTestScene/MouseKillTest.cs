using UnityEngine;

public class MouseKillTest : MonoBehaviour
{
    void OnMouseDown()
    {
        Destroy(gameObject);
    }
}

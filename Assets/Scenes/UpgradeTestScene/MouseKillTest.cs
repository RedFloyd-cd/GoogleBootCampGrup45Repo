using UnityEngine;

public class MouseKillTest : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Test Ayarlarý")]
    public bool testAktif = true;

    void OnMouseDown()
    {
        if (testAktif)
        {
            Destroy(gameObject);
        }
    }
#endif
}

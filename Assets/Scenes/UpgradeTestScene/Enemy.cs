using UnityEngine;

public class Enemy : MonoBehaviour
{
    private void OnDestroy()
    {
        if (EnemyTracker.Instance != null)
            EnemyTracker.Instance.NotifyEnemyKilled();
    }
}

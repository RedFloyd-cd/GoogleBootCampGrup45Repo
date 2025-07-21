using UnityEngine;
using System;

public class EnemyTracker : MonoBehaviour
{
    public static EnemyTracker Instance;
    private int aliveEnemies;

    public static event Action OnAllEnemiesDead;

    private void Awake()
    {
        Instance = this;
    }

    public void InitializeLevel()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        aliveEnemies = enemies.Length;
        Debug.Log("Düþman sayýsý: " + aliveEnemies);
    }

    public void NotifyEnemyKilled()
    {
        aliveEnemies--;
        Debug.Log("Düþman öldü! Kalan: " + aliveEnemies);
        if (aliveEnemies <= 0)
        {
            OnAllEnemiesDead?.Invoke();
        }
    }
}

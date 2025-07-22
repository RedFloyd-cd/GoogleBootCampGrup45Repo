using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Stat Values")]
    public float maxHealth = 100f;
    public float damage = 20f;
    public float dashCooldown = 3f;
    public float timeWarpCooldown = 10f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ApplyUpgrade(string upgradeType)
    {
        switch (upgradeType)
        {
            case "Health":
                maxHealth += 50f;
                break;
            case "Damage":
                damage += 10f;
                break;
            case "Cooldown":
                dashCooldown = Mathf.Max(0.5f, dashCooldown - 0.5f);
                timeWarpCooldown = Mathf.Max(1f, timeWarpCooldown - 1f);
                break;

        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("UI")]
    public Button healthButton;
    public Button damageButton;
    public Button cooldownButton;
    public GameObject upgradePanel;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        healthButton.onClick.AddListener(ApplyHealthUpgrade);
        damageButton.onClick.AddListener(ApplyDamageUpgrade);
        cooldownButton.onClick.AddListener(ApplyCooldownUpgrade);
    }

    void ApplyHealthUpgrade()
    {
        PlayerStats.Instance.ApplyUpgrade("Health");
        Debug.Log($"Yeni maxHealth: {PlayerStats.Instance.maxHealth}");
        ContinueToCutscene();
    }

    void ApplyDamageUpgrade()
    {
        PlayerStats.Instance.ApplyUpgrade("Damage");
        Debug.Log($"Yeni damage: {PlayerStats.Instance.damage}");
        ContinueToCutscene();
    }

    void ApplyCooldownUpgrade()
    {
        PlayerStats.Instance.ApplyUpgrade("Cooldown");
        Debug.Log($"Yeni cooldown: {PlayerStats.Instance.dashCooldown}");
        Debug.Log($"Yeni cooldown: {PlayerStats.Instance.timeWarpCooldown}");
        ContinueToCutscene();
    }

    void ContinueToCutscene()
    {
        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        Time.timeScale = 1f;
        LevelManager.Instance.StartCutscene();
    }
}

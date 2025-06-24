using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public Slider healthSlider;

    private PlayerController controller;

    private void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<PlayerController>();
        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UpdateUI();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("Öldün!");
        controller.enabled = false;
        gameObject.SetActive(false);
        GameOverManager.Instance.ShowGameOver();
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
        Debug.Log("�ld�n!");
        controller.enabled = false;
        gameObject.SetActive(false);
        GameOverManager.Instance.ShowGameOver();
    }

    private Coroutine poisonCoroutine;

    public void ApplyPoison(float tickDamage, float duration, float tickInterval)
    {
        if (poisonCoroutine != null)
        {
            StopCoroutine(poisonCoroutine);
        }
        poisonCoroutine = StartCoroutine(PoisonEffect(tickDamage, duration, tickInterval));
    }

    private IEnumerator PoisonEffect(float tickDamage, float duration, float tickInterval)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            TakeDamage(tickDamage);
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }
        poisonCoroutine = null;
    }
}

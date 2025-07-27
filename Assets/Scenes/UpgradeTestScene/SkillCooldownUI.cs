using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCooldownUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI cooldownText;
    public Sprite activeSprite;
    public Sprite cooldownSprite;

    private float cooldownTime;
    private float cooldownRemaining;
    private bool isOnCooldown = false;

    void Start()
    {
        iconImage.sprite = activeSprite;
        cooldownText.gameObject.SetActive(false);
    }

    public void StartCooldown(float duration)
    {
        cooldownTime = duration;
        cooldownRemaining = duration;
        isOnCooldown = true;
        iconImage.sprite = cooldownSprite;
        cooldownText.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!isOnCooldown) return;

        cooldownRemaining -= Time.unscaledDeltaTime;
        cooldownText.text = Mathf.CeilToInt(cooldownRemaining).ToString();

        if (cooldownRemaining <= 0f)
        {
            iconImage.sprite = activeSprite;
            cooldownText.gameObject.SetActive(false);
            isOnCooldown = false;
        }
    }
}

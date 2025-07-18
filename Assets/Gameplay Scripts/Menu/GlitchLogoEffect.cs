using UnityEngine;
using UnityEngine.UI;

public class LogoGlitch : MonoBehaviour
{
    public Image logoImage;
    public Sprite[] glitchFrames;
    public float glitchInterval = 0.1f;
    public float shakeAmount = 5f;

    private float glitchTimer;
    private Vector2 originalPosition;
    private Vector3 originalScale;

    void Start()
    {
        // Ba�lang�� pozisyonu ve scale'� kaydet
        originalPosition = logoImage.rectTransform.anchoredPosition;
        originalScale = logoImage.rectTransform.localScale;
    }

    void Update()
    {
        glitchTimer += Time.deltaTime;

        if (glitchTimer >= glitchInterval)
        {
            // Sprite de�i�imi
            if (glitchFrames.Length > 0)
            {
                int index = Random.Range(0, glitchFrames.Length);
                logoImage.sprite = glitchFrames[index];
            }

            // Pozisyon sars�nt�s� (orijinal pozisyon etraf�nda)
            logoImage.rectTransform.anchoredPosition = originalPosition + new Vector2(
                Random.Range(-shakeAmount, shakeAmount),
                Random.Range(-shakeAmount, shakeAmount)
            );

            // �l�ek sars�nt�s� (orijinal scale etraf�nda)
            float scale = Random.Range(0.97f, 1.03f);
            logoImage.rectTransform.localScale = originalScale * scale;

            glitchTimer = 0f;
        }
    }
}

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
        // Baþlangýç pozisyonu ve scale'ý kaydet
        originalPosition = logoImage.rectTransform.anchoredPosition;
        originalScale = logoImage.rectTransform.localScale;
    }

    void Update()
    {
        glitchTimer += Time.deltaTime;

        if (glitchTimer >= glitchInterval)
        {
            // Sprite deðiþimi
            if (glitchFrames.Length > 0)
            {
                int index = Random.Range(0, glitchFrames.Length);
                logoImage.sprite = glitchFrames[index];
            }

            // Pozisyon sarsýntýsý (orijinal pozisyon etrafýnda)
            logoImage.rectTransform.anchoredPosition = originalPosition + new Vector2(
                Random.Range(-shakeAmount, shakeAmount),
                Random.Range(-shakeAmount, shakeAmount)
            );

            // Ölçek sarsýntýsý (orijinal scale etrafýnda)
            float scale = Random.Range(0.97f, 1.03f);
            logoImage.rectTransform.localScale = originalScale * scale;

            glitchTimer = 0f;
        }
    }
}

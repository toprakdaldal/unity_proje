using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathScreen : MonoBehaviour
{
    public static DeathScreen Instance;

    [Header("── UI ──")]
    [SerializeField] GameObject  root;          // tüm panel
    [SerializeField] CanvasGroup canvasGroup;   // alpha kontrolü
    [SerializeField] TMP_Text    deathText;     // "SEN ÖLDÜN"

    [Header("── Animasyon Süreleri ──")]
    [SerializeField] float fadeInDuration  = 1.2f;
    [SerializeField] float holdDuration    = 2f;
    [SerializeField] float startScale      = 2.5f; // metin önce büyük, sonra küçülür

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (root != null) root.SetActive(false);
    }

    public void Show()
    {
        if (root == null) return;
        root.SetActive(true);
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        if (canvasGroup != null) canvasGroup.alpha = 0f;

        // Hafif slow motion + ekran sallanması
        CameraController.Instance?.Shake(0.3f, 0.15f);

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t  = elapsed / fadeInDuration;

            if (canvasGroup != null) canvasGroup.alpha = t;
            if (deathText != null)
            {
                float s = Mathf.Lerp(startScale, 1f, t);
                deathText.transform.localScale = Vector3.one * s;
            }
            yield return null;
        }

        if (canvasGroup != null) canvasGroup.alpha = 1f;
        if (deathText   != null) deathText.transform.localScale = Vector3.one;

        yield return new WaitForSecondsRealtime(holdDuration);
    }
}

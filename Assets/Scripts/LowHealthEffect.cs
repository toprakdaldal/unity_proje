using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LowHealthEffect : MonoBehaviour
{
    [Header("── Referanslar ──")]
    [SerializeField] Image            vignetteImage;
    [SerializeField] HealthController health;

    [Header("── Düşük Can ──")]
    [SerializeField] float thresholdPercent = 0.35f; // bu yüzdenin altında devreye girer
    [SerializeField] float maxAlpha         = 0.55f;
    [SerializeField] float basePulseSpeed   = 2f;    // kalp atışı hızı
    [SerializeField] float maxPulseSpeed    = 6f;    // can çok azken hızlanır
    [SerializeField] Color vignetteColor    = new Color(0.8f, 0f, 0f);

    [Header("── Hasar Flaşı ──")]
    [SerializeField] float damageFlashAlpha    = 0.7f;
    [SerializeField] float damageFlashDuration = 0.25f;

    int   lastHealth = -1;
    float damageFlashTimer = 0f;

    void Start()
    {
        if (health == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) health = p.GetComponent<HealthController>();
        }

        if (health != null) lastHealth = health.CurrentHealth;
        if (vignetteImage != null) SetAlpha(0f);
    }

    void Update()
    {
        if (health == null || vignetteImage == null) return;

        // Hasar algıla
        if (health.CurrentHealth < lastHealth)
            damageFlashTimer = damageFlashDuration;
        lastHealth = health.CurrentHealth;

        // Düşük can vignette
        float hp = health.HealthPercent;
        float lowHealthAlpha = 0f;

        if (hp <= thresholdPercent)
        {
            float intensity = 1f - (hp / thresholdPercent);          // 0..1
            float pulseSpeed = Mathf.Lerp(basePulseSpeed, maxPulseSpeed, intensity);
            float pulse      = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) * 0.5f;
            lowHealthAlpha   = intensity * pulse * maxAlpha;
        }

        // Hasar flaşı
        float flashAlpha = 0f;
        if (damageFlashTimer > 0f)
        {
            damageFlashTimer -= Time.unscaledDeltaTime;
            float t = damageFlashTimer / damageFlashDuration;
            flashAlpha = t * damageFlashAlpha;
        }

        // İkisinin maksimumu
        float finalAlpha = Mathf.Max(lowHealthAlpha, flashAlpha);
        SetAlpha(finalAlpha);
    }

    void SetAlpha(float a)
    {
        if (vignetteImage == null) return;
        Color c = vignetteColor;
        c.a = Mathf.Clamp01(a);
        vignetteImage.color = c;
    }
}

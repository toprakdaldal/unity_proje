using System.Collections;
using UnityEngine;

// Düşmanlara eklenir — zehir, yakma, dondurma efektlerini yönetir
public class StatusEffectController : MonoBehaviour
{
    [Header("── Zehir ──")]
    [SerializeField] int   poisonDamagePerTick = 3;
    [SerializeField] float poisonTickInterval  = 0.5f;
    [SerializeField] Color poisonColor         = new Color(0.3f, 0.85f, 0.2f);

    [Header("── Yakma ──")]
    [SerializeField] int   burnDamagePerTick   = 4;
    [SerializeField] float burnTickInterval    = 0.4f;
    [SerializeField] Color burnColor           = new Color(1f, 0.4f, 0.1f);

    [Header("── Dondurma ──")]
    [SerializeField] Color freezeColor         = new Color(0.5f, 0.85f, 1f);

    EnemyHealth    enemyHealth;
    SpriteRenderer sr;
    Color          normalColor;

    bool isPoisoned;
    bool isBurning;
    bool isFrozen;

    Coroutine poisonCoroutine;
    Coroutine burnCoroutine;
    Coroutine freezeCoroutine;

    public bool IsFrozen   => isFrozen;
    public bool IsPoisoned => isPoisoned;
    public bool IsBurning  => isBurning;

    void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        sr          = GetComponent<SpriteRenderer>();
        normalColor = sr != null ? sr.color : Color.white;
    }

    // ── Zehir ────────────────────────────────────────────────
    public void ApplyPoison(float duration)
    {
        if (poisonCoroutine != null) StopCoroutine(poisonCoroutine);
        poisonCoroutine = StartCoroutine(PoisonRoutine(duration));
    }

    IEnumerator PoisonRoutine(float duration)
    {
        isPoisoned = true;
        UpdateColor();

        float elapsed  = 0f;
        float nextTick = 0f;

        while (elapsed < duration)
        {
            elapsed  += Time.deltaTime;
            nextTick -= Time.deltaTime;

            if (nextTick <= 0f)
            {
                nextTick = poisonTickInterval;
                enemyHealth?.TakeDamage(poisonDamagePerTick);
            }
            yield return null;
        }

        isPoisoned = false;
        UpdateColor();
    }

    // ── Yakma ────────────────────────────────────────────────
    public void ApplyBurn(float duration)
    {
        if (burnCoroutine != null) StopCoroutine(burnCoroutine);
        burnCoroutine = StartCoroutine(BurnRoutine(duration));
    }

    IEnumerator BurnRoutine(float duration)
    {
        isBurning = true;
        UpdateColor();

        float elapsed  = 0f;
        float nextTick = 0f;

        while (elapsed < duration)
        {
            elapsed  += Time.deltaTime;
            nextTick -= Time.deltaTime;

            if (nextTick <= 0f)
            {
                nextTick = burnTickInterval;
                enemyHealth?.TakeDamage(burnDamagePerTick);
            }

            // Titreme efekti
            if (sr != null && !isFrozen && !isPoisoned)
                sr.color = Color.Lerp(burnColor, Color.yellow,
                           Mathf.PingPong(elapsed * 6f, 1f));

            yield return null;
        }

        isBurning = false;
        UpdateColor();
    }

    // ── Dondurma ─────────────────────────────────────────────
    public void ApplyFreeze(float duration)
    {
        if (freezeCoroutine != null) StopCoroutine(freezeCoroutine);
        freezeCoroutine = StartCoroutine(FreezeRoutine(duration));
    }

    IEnumerator FreezeRoutine(float duration)
    {
        isFrozen = true;
        UpdateColor();

        yield return new WaitForSeconds(duration);

        isFrozen = false;
        UpdateColor();
    }

    // ── Renk yönetimi ─────────────────────────────────────────
    void UpdateColor()
    {
        if (sr == null) return;

        if (isFrozen)        { sr.color = freezeColor; return; }
        if (isPoisoned)      { sr.color = poisonColor;  return; }
        if (!isBurning)      { sr.color = normalColor;  return; }
        // Yanıyorsa BurnRoutine kendi rengini yönetir
    }
}

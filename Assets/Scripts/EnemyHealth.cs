using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("── Can ──")]
    public int maxHealth = 60;

    [Header("── Knockback ──")]
    public float knockbackForce    = 5f;
    public float knockbackDuration = 0.15f;

    [Header("── Drop ──")]
    [SerializeField] GameObject ghostStepPickupPrefab;
    [SerializeField] float      ghostStepDropChance = 0.4f;

    [Header("── Ruh Ödülü ──")]
    [SerializeField] int soulReward = 8;

    private int            currentHealth;
    private SpriteRenderer sr;
    private Rigidbody2D    rb;
    private bool           isKnockedBack;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int amount, Vector2 hitDirection)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        StartCoroutine(FlashWhite());
        StartCoroutine(Knockback(hitDirection));

        if (currentHealth <= 0)
            Die();
    }

    // Eski çağrılarla uyumluluk (yön olmadan)
    public void TakeDamage(int amount)
    {
        TakeDamage(amount, Vector2.zero);
    }

    public System.Action OnDied;

    void Die()
    {
        // Hayalet taşı düşür
        if (ghostStepPickupPrefab != null && Random.value <= ghostStepDropChance)
            Instantiate(ghostStepPickupPrefab, transform.position, Quaternion.identity);

        // Ruh ödülü (kart bonusu uygulanır)
        int finalSouls = soulReward;
        if (CardSystem.Instance != null)
        {
            float mult = 1f + CardSystem.Instance.GetBonus(CardEffect.SoulMultiplier);
            finalSouls = Mathf.RoundToInt(finalSouls * mult);
        }
        SoulCurrency.Instance?.AddSouls(finalSouls);

        // Kart: öldürmede iyileş
        if (CardSystem.Instance != null)
        {
            int healAmt = Mathf.RoundToInt(CardSystem.Instance.GetBonus(CardEffect.HealOnKill));
            if (healAmt > 0)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                player?.GetComponent<HealthController>()?.Heal(healAmt);
            }
        }

        OnDied?.Invoke();
        Destroy(gameObject);
    }

    IEnumerator Knockback(Vector2 dir)
    {
        if (rb == null || dir == Vector2.zero) yield break;

        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir.normalized * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }

    IEnumerator FlashWhite()
    {
        if (sr != null) sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        if (sr != null) sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (sr != null) sr.color = Color.white;
    }

    public bool IsKnockedBack => isKnockedBack;
}

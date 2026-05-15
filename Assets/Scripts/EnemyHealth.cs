using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("── Can ──")]
    public int maxHealth = 60;

    [Header("── Knockback ──")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.15f;

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

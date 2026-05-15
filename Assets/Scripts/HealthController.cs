using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    [Header("── Can ──")]
    [SerializeField] int maxHealth = 100;
    [SerializeField] float iframeDuration = 0.8f;

    [Header("── Olaylar ──")]
    public UnityEvent<int, int> OnHealthChanged; // (current, max)
    public UnityEvent           OnDeath;

    int   currentHealth;
    bool  isInvincible;

    PlayerController playerController;

    public int   CurrentHealth => currentHealth;
    public int   MaxHealth     => maxHealth;
    public float HealthPercent => (float)currentHealth / maxHealth;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        currentHealth    = maxHealth;
    }

    public void TakeDamage(int amount, float knockbackForce = 6f)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        playerController?.TakeDamage(knockbackForce);
        StartCoroutine(IFrameRoutine());
    }

    public void Heal(int amount)
    {
        if (currentHealth <= 0) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Die()
    {
        playerController?.Die();
        OnDeath?.Invoke();
        StartCoroutine(ReloadSceneDelayed(1.5f));
    }

    IEnumerator IFrameRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(iframeDuration);
        isInvincible = false;
    }

    IEnumerator ReloadSceneDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // Eski tag="Damage" sistemiyle uyumluluk (çarpışma hasarı)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Damage"))
            TakeDamage(25);
    }
}

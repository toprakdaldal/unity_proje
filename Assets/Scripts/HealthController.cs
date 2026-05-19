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
    GhostStep        ghostStep;

    public int   CurrentHealth => currentHealth;
    public int   MaxHealth     => maxHealth;
    public float HealthPercent => (float)currentHealth / maxHealth;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        ghostStep        = GetComponent<GhostStep>();
        currentHealth    = maxHealth;
    }

    public void TakeDamage(int amount, float knockbackForce = 3f, float knockbackDirX = 0f)
    {
        if (isInvincible || currentHealth <= 0) return;
        if (ghostStep != null && ghostStep.IsGhosting) return; // ghost modda hasar alma

        // Yüzükten hasar azaltma
        if (RingController.Instance != null)
        {
            float reduction = RingController.Instance.GetBonus(RingEffect.DamageReduction);
            amount = Mathf.Max(1, Mathf.RoundToInt(amount * (1f - reduction)));
        }

        // Karttan hasar azaltma
        if (CardSystem.Instance != null)
        {
            float reduction = CardSystem.Instance.GetBonus(CardEffect.DamageReduction);
            amount = Mathf.Max(1, Mathf.RoundToInt(amount * (1f - reduction)));
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        CameraController.Instance?.Shake(0.15f, 0.2f);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        playerController?.TakeDamage(knockbackForce, knockbackDirX);
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
        DeathScreen.Instance?.Show();
        StartCoroutine(ReloadSceneDelayed(3.5f));
    }

    IEnumerator IFrameRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(iframeDuration);
        isInvincible = false;
    }

    IEnumerator ReloadSceneDelayed(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
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

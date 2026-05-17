using UnityEngine;

public class BreakableCrate : MonoBehaviour
{
    [Header("── Can ──")]
    [SerializeField] int maxHealth = 1;   // 1 = tek vuruşta kırılır

    [Header("── Drop ──")]
    [SerializeField] GameObject[] dropPrefabs;   // birden fazla olabilir
    [SerializeField] float        dropChance = 1f;  // 1 = kesin düşer

    [Header("── Görsel ──")]
    [SerializeField] GameObject     intactVisual;   // sağlam kasa sprite/obje
    [SerializeField] GameObject     brokenVisual;   // kırık parçalar (opsiyonel)
    [SerializeField] ParticleSystem breakParticles;

    int currentHealth;

    void Start() => currentHealth = maxHealth;

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;
        currentHealth -= amount;
        if (currentHealth <= 0)
            Break();
    }

    void Break()
    {
        // Drop
        if (dropPrefabs != null && dropPrefabs.Length > 0 && Random.value <= dropChance)
        {
            int idx = Random.Range(0, dropPrefabs.Length);
            if (dropPrefabs[idx] != null)
                Instantiate(dropPrefabs[idx], transform.position, Quaternion.identity);
        }

        // Partiküller
        if (breakParticles != null)
        {
            breakParticles.transform.SetParent(null); // dünyada bırak
            breakParticles.Play();
            Destroy(breakParticles.gameObject, breakParticles.main.duration + 0.5f);
        }

        // Görsel geçiş
        if (intactVisual  != null) intactVisual.SetActive(false);
        if (brokenVisual  != null)
        {
            brokenVisual.SetActive(true);
            Destroy(brokenVisual, 2f); // kırık parçalar 2 saniye sonra kaybolur
        }

        // Collider'ı kapat, kısa süre sonra sil
        if (TryGetComponent<Collider2D>(out var col)) col.enabled = false;
        Destroy(gameObject, brokenVisual != null ? 2.1f : 0.05f);
    }
}

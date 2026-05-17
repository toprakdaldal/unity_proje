using UnityEngine;

public class HealthPotionPickup : MonoBehaviour
{
    [Header("── İyileşme ──")]
    [SerializeField] int   healAmount = 40;

    [Header("── Animasyon ──")]
    [SerializeField] float bobSpeed  = 2.5f;
    [SerializeField] float bobHeight = 0.12f;

    [Header("── Yaşam Süresi ──")]
    [SerializeField] float lifetime = 12f;   // 0 = sonsuz

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        if (lifetime > 0f) Destroy(gameObject, lifetime);
    }

    void Update()
    {
        float newY         = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var health = other.GetComponent<HealthController>();
        if (health == null) return;

        // Tam candaysa alma
        if (health.CurrentHealth >= health.MaxHealth) return;

        health.Heal(healAmount);
        Destroy(gameObject);
    }
}

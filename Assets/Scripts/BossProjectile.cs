using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [HideInInspector] public int    damage   = 15;
    [HideInInspector] public float  speed    = 9f;
    [HideInInspector] public float  lifetime = 4f;

    Rigidbody2D rb;

    public void Init(Vector2 dir)
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir.normalized * speed;

        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Boss'a çarpınca geç
        if (other.GetComponent<BossController>() != null) return;
        // Diğer mermilere çarpınca geç
        if (other.GetComponent<BossProjectile>() != null) return;
        if (other.GetComponent<EnemyProjectile>() != null) return;

        // Oyuncuya hasar ver
        if (other.CompareTag("Player"))
            other.GetComponent<HealthController>()?.TakeDamage(damage);

        Destroy(gameObject);
    }
}

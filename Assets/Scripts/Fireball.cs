using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed    = 12f;
    public int   damage   = 20;
    public float lifetime = 3f;

    private Vector2 direction;

    public void Init(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Düşmana çarptı
        var enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, direction);
            Destroy(gameObject);
            return;
        }

        // Zemine çarptı
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}

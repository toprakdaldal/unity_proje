using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed        = 12f;
    public int   damage       = 20;
    public float lifetime     = 3f;
    public float burnChance   = 0.4f;
    public float burnDuration = 2.5f;
    public bool  burnUnlocked = false;

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
        var enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, direction);

            // Yakma (kilit açıksa)
            if (burnUnlocked && Random.value <= burnChance)
                other.GetComponent<StatusEffectController>()?.ApplyBurn(burnDuration);

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Ground"))
            Destroy(gameObject);
    }
}

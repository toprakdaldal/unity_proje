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
            float chance   = burnChance;
            float duration = burnDuration;
            if (SkillTree.Instance != null)
            {
                chance   *= SkillTree.Instance.BurnChanceMult;
                duration *= SkillTree.Instance.BurnDurationMult;
            }
            if (burnUnlocked && Random.value <= chance)
                other.GetComponent<StatusEffectController>()?.ApplyBurn(duration);
            Destroy(gameObject);
            return;
        }

        var boss = other.GetComponent<BossController>();
        if (boss != null)
        {
            boss.TakeDamage(damage, direction);
            Destroy(gameObject);
            return;
        }

        var crate = other.GetComponent<BreakableCrate>();
        if (crate != null)
        {
            crate.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Oyuncu, diğer mermiler ve pickup'ları yok say
        if (other.CompareTag("Player"))  return;
        if (other.GetComponent<Fireball>()         != null) return;
        if (other.GetComponent<EnemyProjectile>()  != null) return;
        if (other.GetComponent<BossProjectile>()   != null) return;
        if (other.GetComponent<HealthPotionPickup>() != null) return;
        if (other.GetComponent<GhostStepPickup>()    != null) return;
        if (other.GetComponent<RingPickup>()         != null) return;
        if (other.GetComponent<EnchantmentPickup>()  != null) return;
        if (other.GetComponent<AbilityPickup>()      != null) return;

        // Zemin/duvar — yok ol
        Destroy(gameObject);
    }
}

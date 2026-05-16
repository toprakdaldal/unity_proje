using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("── Devriye ──")]
    public float speed = 2f;
    public Transform[] points;

    [Header("── Takip ──")]
    public float chaseSpeed     = 4f;
    public float detectionRange = 5f;
    public float attackRange    = 0.8f;

    [Header("── Saldırı ──")]
    public int   attackDamage        = 10;
    public float attackCooldown      = 1.2f;
    public float playerKnockbackForce = 3f;

    private int              i;
    private SpriteRenderer   spriteRenderer;
    private Transform        player;
    private HealthController playerHealth;
    private EnemyHealth      enemyHealth;
    private float            attackTimer;
    private Transform        decoyTarget;
    private float            decoyTimer;
    private bool                   isStunned;
    private float                  stunTimer;
    private StatusEffectController statusEffect;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyHealth    = GetComponent<EnemyHealth>();
        statusEffect   = GetComponent<StatusEffectController>();

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player       = playerObj.transform;
            playerHealth = playerObj.GetComponent<HealthController>();
        }
    }

    public void SetDecoy(Transform decoy, float duration)
    {
        decoyTarget = decoy;
        decoyTimer  = duration;
    }

    public void Stun(float duration)
    {
        isStunned = true;
        stunTimer = duration;
    }

    void Update()
    {
        // Knockback veya dondurma — hareket etme
        if (enemyHealth != null && enemyHealth.IsKnockedBack) return;
        if (statusEffect != null && statusEffect.IsFrozen) return;

        // Stun kontrolü
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f) isStunned = false;
            return;
        }

        // Decoy süresi güncelle
        if (decoyTimer > 0f)
        {
            decoyTimer -= Time.deltaTime;
            if (decoyTimer <= 0f) decoyTarget = null;
        }

        // Decoy varsa onu takip et
        Transform target = (decoyTarget != null) ? decoyTarget : player;

        attackTimer -= Time.deltaTime;

        float distToPlayer = target != null
            ? Vector2.Distance(transform.position, target.position)
            : Mathf.Infinity;

        // Saldırı menzilinde
        if (distToPlayer <= attackRange)
        {
            if (attackTimer <= 0f)
            {
                attackTimer = attackCooldown;
                // Decoy'u hedefliyorsa oyuncuya hasar verme
                if (decoyTarget == null)
                {
                    // Düşmanın konumuna göre knockback yönü hesapla
                    float dir = transform.position.x < player.position.x ? 1f : -1f;
                    playerHealth?.TakeDamage(attackDamage, playerKnockbackForce, dir);
                }
            }
            return;
        }

        // Algılama menzilinde — hedefi takip et (decoy varsa onu)
        if (distToPlayer <= detectionRange || decoyTarget != null)
        {
            ChaseTarget(target);
            return;
        }

        // Normal devriye
        Patrol();
    }

    void Patrol()
    {
        if (points == null || points.Length == 0) return;

        if (Vector2.Distance(transform.position, points[i].position) < 0.25f)
        {
            i++;
            if (i == points.Length) i = 0;
        }

        transform.position = Vector2.MoveTowards(
            transform.position, points[i].position, speed * Time.deltaTime);

        spriteRenderer.flipX = (transform.position.x - points[i].position.x) < 0f;
    }

    void Chase() => ChaseTarget(player);

    void ChaseTarget(Transform target)
    {
        if (target == null) return;
        transform.position = Vector2.MoveTowards(
            transform.position, target.position, chaseSpeed * Time.deltaTime);
        spriteRenderer.flipX = (transform.position.x - target.position.x) < 0f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

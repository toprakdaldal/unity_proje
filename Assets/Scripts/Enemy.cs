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
    public int   attackDamage   = 10;
    public float attackCooldown = 1.2f;

    private int              i;
    private SpriteRenderer   spriteRenderer;
    private Transform        player;
    private HealthController playerHealth;
    private EnemyHealth      enemyHealth;
    private float            attackTimer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyHealth    = GetComponent<EnemyHealth>();

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player       = playerObj.transform;
            playerHealth = playerObj.GetComponent<HealthController>();
        }
    }

    void Update()
    {
        // Knockback yenirken hareket etme
        if (enemyHealth != null && enemyHealth.IsKnockedBack) return;

        attackTimer -= Time.deltaTime;

        float distToPlayer = player != null
            ? Vector2.Distance(transform.position, player.position)
            : Mathf.Infinity;

        // Saldırı menzilinde
        if (distToPlayer <= attackRange)
        {
            if (attackTimer <= 0f)
            {
                attackTimer = attackCooldown;
                playerHealth?.TakeDamage(attackDamage);
            }
            return;
        }

        // Algılama menzilinde — oyuncuyu takip et
        if (distToPlayer <= detectionRange)
        {
            Chase();
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

    void Chase()
    {
        transform.position = Vector2.MoveTowards(
            transform.position, player.position, chaseSpeed * Time.deltaTime);

        spriteRenderer.flipX = (transform.position.x - player.position.x) < 0f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

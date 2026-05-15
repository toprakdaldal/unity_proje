using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyAI : MonoBehaviour
{
    [Header("── Devriye ──")]
    [SerializeField] Transform patrolPointA;
    [SerializeField] Transform patrolPointB;
    [SerializeField] float     patrolSpeed = 2f;

    [Header("── Algılama ──")]
    [SerializeField] float detectionRange = 5f;
    [SerializeField] float attackRange    = 0.8f;
    [SerializeField] LayerMask playerLayer;

    [Header("── Saldırı ──")]
    [SerializeField] int   attackDamage   = 10;
    [SerializeField] float attackCooldown = 1.2f;
    [SerializeField] float knockbackForce = 5f;

    [Header("── Takip ──")]
    [SerializeField] float chaseSpeed = 3.5f;

    // Componentler
    Rigidbody2D      rb;
    SpriteRenderer   sr;
    Animator         anim;
    Transform        player;
    HealthController playerHealth;

    // Durum
    enum State { Patrol, Chase, Attack, Dead }
    State state = State.Patrol;

    Transform patrolTarget;
    bool      canAttack     = true;
    bool      facingRight   = true;
    bool      patrollingRight = true;
    float     patrolSwitchTimer = 0f;

    void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        sr   = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;

        // Oyuncuyu bul
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player       = playerObj.transform;
            playerHealth = playerObj.GetComponent<HealthController>();
        }

        // Düşman ile oyuncu fiziksel olarak çakışmasın (tepine çıkma sorunu)
        int enemyLayer  = gameObject.layer;
        int playerLayer = player != null ? player.gameObject.layer : LayerMask.NameToLayer("Default");
        Physics2D.IgnoreLayerCollision(enemyLayer, playerLayer, true);

        patrolTarget = patrolPointB;
    }

    void Update()
    {
        if (state == State.Dead) return;

        float distToPlayer = player != null
            ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;

        switch (state)
        {
            case State.Patrol:
                Patrol();
                SetAnim(true);
                if (distToPlayer <= detectionRange) state = State.Chase;
                break;

            case State.Chase:
                Chase();
                SetAnim(true);
                if (distToPlayer <= attackRange)    state = State.Attack;
                if (distToPlayer >  detectionRange) state = State.Patrol;
                break;

            case State.Attack:
                rb.linearVelocityX = 0f;
                SetAnim(false);
                if (distToPlayer > attackRange)     state = State.Chase;
                if (canAttack) StartCoroutine(AttackRoutine());
                break;
        }
    }

    // ── DEVRIYE ───────────────────────────────────────────────

    void Patrol()
    {
        if (patrolPointA == null || patrolPointB == null) return;

        patrolSwitchTimer -= Time.deltaTime;

        if (patrolSwitchTimer > 0f)
        {
            rb.linearVelocityX = 0f;
            return;
        }

        float dirToTarget = patrolTarget.position.x - transform.position.x;

        if (Mathf.Abs(dirToTarget) < 0.4f)
        {
            // Hedefe ulaştık — hedefi değiştir, yön çevir
            rb.linearVelocityX = 0f;
            patrolTarget = patrolTarget == patrolPointA ? patrolPointB : patrolPointA;
            patrolSwitchTimer = 0.5f;

            // Yeni hedefe göre yönü belirle (bir kez)
            float newDir = patrolTarget.position.x - transform.position.x;
            SetFacing(newDir > 0);
        }
        else
        {
            rb.linearVelocityX = Mathf.Sign(dirToTarget) * patrolSpeed;
        }
    }

    void SetFacing(bool right)
    {
        if (facingRight == right) return;
        facingRight = right;
        Vector3 s = transform.localScale;
        s.x = right ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
        transform.localScale = s;
    }

    // ── TAKİP ─────────────────────────────────────────────────

    void Chase()
    {
        if (player == null) return;
        float dir = player.position.x - transform.position.x;
        if (Mathf.Abs(dir) < 0.1f) { rb.linearVelocityX = 0f; return; }
        rb.linearVelocityX = Mathf.Sign(dir) * chaseSpeed;
        FlipTowards(dir);
    }

    // ── SALDIRI ───────────────────────────────────────────────

    IEnumerator AttackRoutine()
    {
        canAttack = false;

        // HealthController null ise tekrar bulmayı dene
        if (playerHealth == null && player != null)
            playerHealth = player.GetComponent<HealthController>();

        if (playerHealth != null)
            playerHealth.TakeDamage(attackDamage, knockbackForce);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // ── YARDIMCI ──────────────────────────────────────────────

    void FlipTowards(float dirX)
    {
        if (Mathf.Abs(dirX) < 0.3f) return;
        SetFacing(dirX > 0);
    }

    void SetAnim(bool isRunning)
    {
        if (anim == null) return;
        if (isRunning) anim.Play("Enemy_Run");
        else           anim.Play("Enemy_Idle");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

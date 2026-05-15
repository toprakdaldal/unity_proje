using System.Collections;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [Header("── Saldırı ──")]
    [SerializeField] Transform attackPoint;
    [SerializeField] float     attackRadius   = 0.6f;
    [SerializeField] int       attackDamage   = 15;
    [SerializeField] float     attackCooldown = 0.35f;
    [SerializeField] LayerMask enemyLayer;

    [Header("── İlahi Ateş Saldırısı ──")]
    [SerializeField] Transform fireAttackPoint;
    [SerializeField] float     fireAttackRadius = 1.2f;
    [SerializeField] int       fireAttackDamage = 30;
    [SerializeField] float     divineFireCost   = 25f;

    [Header("── Ateş Topu ──")]
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Transform  fireballSpawnPoint;
    [SerializeField] float      fireballFireCost  = 20f;
    [SerializeField] float      fireballCooldown  = 0.8f;
    [SerializeField] int        fireballDamage    = 20;

    float fireballTimer = 0f;

    [Header("── İlahi Ateş Dolumu ──")]
    [SerializeField] float divineFirePerKill = 12.5f; // 2 kill = çeyrek bar

    PlayerController playerController;
    SpriteRenderer   sr;
    bool             canAttack = true;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        sr               = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && canAttack)
            StartCoroutine(AttackRoutine());

        if (Input.GetKeyDown(KeyCode.X))
            StartCoroutine(FireAttackRoutine());

        fireballTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.C) && fireballTimer <= 0f)
            ShootFireball();
    }

    // ── NORMAL SALDIRI ────────────────────────────────────────

    IEnumerator AttackRoutine()
    {
        canAttack = false;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position, attackRadius, enemyLayer);

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyHealth>();
            if (enemy == null) continue;
            Vector2 dir = (hit.transform.position - transform.position).normalized;
            enemy.OnDied += () => playerController?.AddDivineFire(divineFirePerKill);
            enemy.TakeDamage(attackDamage, dir);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // ── İLAHİ ATEŞ SALDIRISI ─────────────────────────────────

    IEnumerator FireAttackRoutine()
    {
        // Yeterli ateş yoksa çalışma
        if (playerController == null) yield break;
        if (playerController.DivineFire < divineFireCost) yield break;

        // Ateşi harca
        playerController.AddDivineFire(-divineFireCost);

        // Turuncu flaş efekti
        StartCoroutine(FlashColor(new Color(1f, 0.5f, 0f)));

        // Geniş alanda hasar ver
        Transform point = fireAttackPoint != null ? fireAttackPoint : attackPoint;
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            point.position, fireAttackRadius, enemyLayer);

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyHealth>();
            if (enemy == null) continue;
            Vector2 dir = (hit.transform.position - transform.position).normalized;
            enemy.OnDied += () => playerController?.AddDivineFire(divineFirePerKill);
            enemy.TakeDamage(fireAttackDamage, dir);
        }

        yield return null;
    }

    void ShootFireball()
    {
        if (playerController == null || fireballPrefab == null) return;
        if (playerController.DivineFire < fireballFireCost) return;

        playerController.AddDivineFire(-fireballFireCost);
        fireballTimer = fireballCooldown;

        // Fırlatma noktası ve yön
        Transform spawnPoint = fireballSpawnPoint != null ? fireballSpawnPoint : attackPoint;
        Vector2   dir        = playerController.IsFacingRight ? Vector2.right : Vector2.left;

        GameObject fb = Instantiate(fireballPrefab, spawnPoint.position, Quaternion.identity);
        var fireball = fb.GetComponent<Fireball>();
        if (fireball != null)
        {
            fireball.damage = fireballDamage;
            fireball.Init(dir);
        }
    }

    IEnumerator FlashColor(Color color)
    {
        if (sr == null) yield break;
        sr.color = color;
        yield return new WaitForSeconds(0.15f);
        sr.color = Color.white;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
        if (fireAttackPoint != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f);
            Gizmos.DrawWireSphere(fireAttackPoint.position, fireAttackRadius);
        }
    }
}

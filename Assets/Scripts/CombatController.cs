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

    [Header("── Büyü Kilitleri ──")]
    [HideInInspector] public bool poisonUnlocked = false;
    [HideInInspector] public bool burnUnlocked   = false;
    [HideInInspector] public bool freezeUnlocked = false;

    [Header("── Büyüler ──")]
    [SerializeField] float poisonChance    = 0.35f;
    [SerializeField] float poisonDuration  = 3f;
    [SerializeField] float freezeChance    = 0.45f;
    [SerializeField] float freezeDuration  = 2f;

    [Header("── Yankı Darbesi ──")]
    [SerializeField] float          chargeTime    = 0.5f;
    [SerializeField] int            chargeDamage  = 25;
    [SerializeField] float          echoDelay     = 0.3f;
    [SerializeField] int            echoDamage    = 12;
    [SerializeField] float          echoRadius    = 0.9f;
    [SerializeField] ParticleSystem echoParticles;  // yankı çarpma efekti

    [Header("── İlahi Ateş Dolumu ──")]
    [SerializeField] float divineFirePerKill = 12.5f;

    PlayerController playerController;
    Stomp            stomp;
    SpriteRenderer   sr;
    bool             canAttack  = true;
    bool             isCharging;
    float            chargeTimer;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        stomp            = GetComponent<Stomp>();
        sr               = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        bool stompActive = stomp != null && stomp.IsStomping;

        // Z tuşu basıldığında şarjı başlat
        if (Input.GetKeyDown(KeyCode.Z) && canAttack && !stompActive)
        {
            isCharging  = true;
            chargeTimer = 0f;
        }

        // Şarj devam ediyor — sprite sarıya kayar
        if (isCharging)
        {
            chargeTimer += Time.deltaTime;
            if (sr != null)
                sr.color = Color.Lerp(Color.white, Color.yellow, chargeTimer / chargeTime);
        }

        // Z bırakıldığında: yeterince tutulduysa Yankı Darbesi, değilse normal saldırı
        if (Input.GetKeyUp(KeyCode.Z) && isCharging)
        {
            isCharging   = false;
            if (sr != null) sr.color = Color.white;

            if (chargeTimer >= chargeTime)
                StartCoroutine(EchoStrikeRoutine());
            else
                StartCoroutine(AttackRoutine(applyPoison: true));
        }

        if (Input.GetKeyDown(KeyCode.X))
            StartCoroutine(FireAttackRoutine());

        fireballTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.C) && fireballTimer <= 0f)
            ShootFireball();
    }

    // ── NORMAL SALDIRI ────────────────────────────────────────

    IEnumerator AttackRoutine(bool applyPoison = false)
    {
        canAttack = false;
        HitEnemies(attackPoint.position, attackRadius, attackDamage, applyPoison);
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // ── YANKI DARBESİ ─────────────────────────────────────────

    IEnumerator EchoStrikeRoutine()
    {
        canAttack = false;

        // Güçlü ilk vuruş — sarı flaş
        Vector2 hitPos = attackPoint.position;
        StartCoroutine(FlashColor(new Color(1f, 0.9f, 0f)));
        HitEnemies(hitPos, attackRadius, chargeDamage);

        // Yankı gecikmesi
        yield return new WaitForSeconds(echoDelay);

        // İkinci yankı vuruşu — cyan flaş + kıvılcım efekti
        StartCoroutine(FlashColor(new Color(0.4f, 0.9f, 1f)));
        if (echoParticles != null)
        {
            echoParticles.transform.position = hitPos;
            echoParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            echoParticles.Play();
        }
        HitEnemies(hitPos, echoRadius, echoDamage);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // ── ORTAK HASAR FONKSİYONU ────────────────────────────────

    void HitEnemies(Vector2 pos, float radius, int damage, bool applyPoison = false)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, radius, enemyLayer);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyHealth>();
            if (enemy == null) continue;
            Vector2 dir = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
            enemy.OnDied += () => playerController?.AddDivineFire(divineFirePerKill);
            enemy.TakeDamage(damage, dir);

            // Zehir (sadece normal melee, kilit açıksa)
            if (applyPoison && poisonUnlocked && Random.value <= poisonChance)
                hit.GetComponent<StatusEffectController>()?.ApplyPoison(poisonDuration);
        }
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

            // Dondurma (kilit açıksa)
            if (freezeUnlocked && Random.value <= freezeChance)
                hit.GetComponent<StatusEffectController>()?.ApplyFreeze(freezeDuration);
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
            fireball.damage       = fireballDamage;
            fireball.burnUnlocked = burnUnlocked;
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

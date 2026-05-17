using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    // ── Can ──────────────────────────────────────────────────────
    [Header("── Can ──")]
    [SerializeField] int   maxHealth      = 300;
    [SerializeField] float phase2Threshold = 0.30f;  // %30

    // ── Hareket ──────────────────────────────────────────────────
    [Header("── Hareket ──")]
    [SerializeField] float moveSpeed      = 1.8f;
    [SerializeField] float detectionRange = 12f;
    [SerializeField] float keepDistance   = 5f;   // ideal duruş mesafesi
    [SerializeField] float dashTriggerDist = 4f;  // dash için max mesafe

    // ── Mermi ────────────────────────────────────────────────────
    [Header("── Mermi Saldırısı ──")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform  shootPoint;
    [SerializeField] float      shootCooldown    = 2.5f;
    [SerializeField] float      shootMinDist     = 4f;   // bu mesafeden yakınsa ateş etmez
    [SerializeField] float      projectileSpeed  = 9f;
    [SerializeField] int        projectileDamage = 15;

    // ── Zemin Çarpmışı ────────────────────────────────────────────
    [Header("── Zemin Çarpmışı ──")]
    [SerializeField] float          slamCooldown = 4.5f;
    [SerializeField] float          slamRadius   = 3.5f;
    [SerializeField] int            slamDamage   = 25;
    [SerializeField] ParticleSystem slamParticles;

    // ── Dash ─────────────────────────────────────────────────────
    [Header("── Dash Saldırısı ──")]
    [SerializeField] float dashCooldown  = 3.5f;
    [SerializeField] float dashSpeed     = 14f;
    [SerializeField] float dashDuration  = 0.25f;
    [SerializeField] int   dashDamage    = 20;
    [SerializeField] float dashHitRadius = 0.8f;

    // ── Faz 2 ────────────────────────────────────────────────────
    [Header("── Faz 2 ──")]
    [SerializeField] float phase2SpeedMult = 1.6f;
    [SerializeField] Color phase2Color     = new Color(1f, 0.3f, 0.3f);
    [SerializeField] ParticleSystem phaseTransitionParticles;

    // ── Hasar Sayısı ─────────────────────────────────────────────
    [Header("── Hasar Sayısı ──")]
    [SerializeField] GameObject damageNumberPrefab;
    [SerializeField] Color      damageNumberColor = new Color(1f, 0.9f, 0.2f);

    // ── İç Değişkenler ───────────────────────────────────────────
    int   currentHealth;
    bool  isPhase2;
    bool  isActive;
    bool  isDashing;
    bool  isAttacking;

    float shootTimer;
    float slamTimer;
    float dashTimer;

    Transform      player;
    Rigidbody2D    rb;
    SpriteRenderer sr;
    Animator       anim;

    // HUD için
    public int CurrentHealth => currentHealth;
    public int MaxHealth     => maxHealth;
    public System.Action<int, int> OnHealthChanged;
    public System.Action           OnDeath;
    public System.Action           OnPhase2Entered;
    public System.Action           OnActivated;

    // ─────────────────────────────────────────────────────────────

    void Awake()
    {
        rb            = GetComponent<Rigidbody2D>();
        sr            = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        anim          = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
    }

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        // Player ile fiziksel çarpışmayı kapat (BlinkDash bozulmasın)
        int bossLayer   = gameObject.layer;
        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer >= 0)
            Physics2D.IgnoreLayerCollision(bossLayer, playerLayer, true);

        // Cooldown'ları dağıt — aynı anda üst üste gelmesin
        shootTimer = 1f;
        slamTimer  = 2f;
        dashTimer  = 3f;
    }

    void Update()
    {
        if (player == null || currentHealth <= 0) return;

        // Algılama
        float dist = Vector2.Distance(transform.position, player.position);
        if (!isActive)
        {
            if (dist <= detectionRange)
            {
                isActive = true;
                OnActivated?.Invoke();
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
            }
            return;
        }

        FacePlayer();

        float coolMult = isPhase2 ? (1f / phase2SpeedMult) : 1f;

        // Timer'lar her zaman azalsın — saldırı sırasında da
        shootTimer -= Time.deltaTime;
        slamTimer  -= Time.deltaTime;
        dashTimer  -= Time.deltaTime;

        if (isDashing || isAttacking) return;

        float spd = moveSpeed * (isPhase2 ? phase2SpeedMult : 1f);

        // Öncelik: Mermi → Slam (yakınsa) → Dash (yakınsa) → Hareket
        if (shootTimer <= 0f)
        {
            shootTimer = shootCooldown * coolMult;
            StartCoroutine(ShootRoutine());
        }
        else if (slamTimer <= 0f && dist <= slamRadius + 0.5f)
        {
            slamTimer = slamCooldown * coolMult;
            StartCoroutine(SlamRoutine());
        }
        else if (dashTimer <= 0f && dist <= dashTriggerDist)
        {
            dashTimer = dashCooldown * coolMult;
            StartCoroutine(DashRoutine());
        }
        else
        {
            // Çok uzaktaysa yaklaş, ideal mesafedeyse dur, çok yakınsa geri çekil
            if (dist > keepDistance + 1f)
            {
                float dir = player.position.x > transform.position.x ? 1f : -1f;
                rb.linearVelocityX = dir * spd;
            }
            else if (dist < keepDistance - 1f)
            {
                float dir = player.position.x > transform.position.x ? -1f : 1f;
                rb.linearVelocityX = dir * spd * 0.6f;
            }
            else
            {
                rb.linearVelocityX = 0f;
            }
        }

        // Animasyon güncelle
        bool isMoving = Mathf.Abs(rb.linearVelocityX) > 0.1f;
        anim?.SetBool("Walk", isMoving && !isDashing && !isAttacking);
    }

    // ── Yüz Yönü ─────────────────────────────────────────────────
    void FacePlayer()
    {
        if (player == null) return;
        float scaleX = Mathf.Abs(transform.localScale.x);
        transform.localScale = player.position.x < transform.position.x
            ? new Vector3( scaleX, transform.localScale.y, 1f)
            : new Vector3(-scaleX, transform.localScale.y, 1f);
    }

    // ── Telgraf Yardımcısı ───────────────────────────────────────
    IEnumerator Telegraph(Color color, float duration)
    {
        Color original = sr != null ? sr.color : Color.white;
        float elapsed  = 0f;

        // Rengi hızlıca telgraf rengine götür
        while (elapsed < duration * 0.4f)
        {
            elapsed += Time.unscaledDeltaTime;
            if (sr != null)
                sr.color = Color.Lerp(original, color, elapsed / (duration * 0.4f));
            yield return null;
        }

        // O renkte bekle
        yield return new WaitForSecondsRealtime(duration * 0.6f);
        if (sr != null) sr.color = original;
    }

    // ── Mermi Saldırısı ──────────────────────────────────────────
    IEnumerator ShootRoutine()
    {
        isAttacking = true;
        rb.linearVelocityX = 0f;

        // Telgraf: sarı → "ateş edecek"
        yield return StartCoroutine(Telegraph(new Color(1f, 0.95f, 0.2f), 0.55f));

        if (projectilePrefab != null && shootPoint != null)
        {
            int   count  = isPhase2 ? 3 : 1;
            float spread = 18f;

            Vector2 baseDir = ((Vector2)player.position - (Vector2)shootPoint.position).normalized;

            for (int i = 0; i < count; i++)
            {
                float angle = count == 1 ? 0f : -spread + (spread * i);
                Vector2 dir = RotateVector(baseDir, angle);

                GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

                // Önce BossProjectile dene, yoksa EnemyProjectile dene
                var bp = proj.GetComponent<BossProjectile>();
                if (bp != null)
                {
                    bp.damage = projectileDamage;
                    bp.speed  = projectileSpeed;
                    bp.Init(dir);
                }
                else
                {
                    var ep = proj.GetComponent<EnemyProjectile>();
                    if (ep != null)
                    {
                        ep.damage = projectileDamage;
                        ep.speed  = projectileSpeed;
                        ep.Init(dir);
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }

    // ── Zemin Çarpmışı ───────────────────────────────────────────
    IEnumerator SlamRoutine()
    {
        isAttacking = true;
        rb.linearVelocityX = 0f;

        // Telgraf: turuncu → "çarpacak"
        yield return StartCoroutine(Telegraph(new Color(1f, 0.45f, 0f), 0.6f));

        anim?.SetTrigger("Cast");
        yield return new WaitForSeconds(0.2f); // cast animasyonunun başı oynasın

        if (slamParticles != null)
        {
            slamParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            slamParticles.Play();
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, slamRadius);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;
            float dir = transform.position.x < player.position.x ? 1f : -1f;
            hit.GetComponent<HealthController>()?.TakeDamage(slamDamage, 6f, dir);
        }

        CameraController.Instance?.Shake(0.3f, 0.35f);
        HitStop.Instance?.Stop(0.08f);

        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    // ── Dash Saldırısı ───────────────────────────────────────────
    IEnumerator DashRoutine()
    {
        isDashing = false; // telgraf sırasında hareket etmesin
        isAttacking = true;
        rb.linearVelocityX = 0f;

        // Telgraf: kırmızı → "dash atacak"
        yield return StartCoroutine(Telegraph(new Color(1f, 0.15f, 0.15f), 0.5f));

        anim?.SetTrigger("Attack");
        isDashing = true;

        float dir      = player.position.x > transform.position.x ? 1f : -1f;
        float spd      = dashSpeed * (isPhase2 ? phase2SpeedMult : 1f);
        float elapsed  = 0f;
        bool  hitPlayer = false;

        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            rb.linearVelocity = new Vector2(dir * spd, rb.linearVelocityY);

            // Her frame'de çarpma kontrolü
            if (!hitPlayer)
            {
                Collider2D hit = Physics2D.OverlapCircle(transform.position, dashHitRadius,
                    LayerMask.GetMask("Player"));
                if (hit != null && hit.CompareTag("Player"))
                {
                    hit.GetComponent<HealthController>()?.TakeDamage(dashDamage, 7f, dir);
                    CameraController.Instance?.Shake(0.2f, 0.25f);
                    HitStop.Instance?.Stop(0.06f);
                    hitPlayer = true;
                }
            }

            yield return null;
        }

        rb.linearVelocityX = 0f;
        isDashing   = false;
        isAttacking = false;
    }

    // ── Hasar Al ─────────────────────────────────────────────────
    public void TakeDamage(int amount, Vector2 hitDir = default)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        StartCoroutine(FlashWhite());
        anim?.SetTrigger("Hurt");

        // Hasar sayısı
        if (damageNumberPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 1.2f;
            var go = Instantiate(damageNumberPrefab, spawnPos, Quaternion.identity);
            go.GetComponent<DamageNumber>()?.Init(amount, damageNumberColor);
        }

        if (!isPhase2 && (float)currentHealth / maxHealth <= phase2Threshold)
            StartCoroutine(EnterPhase2());

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator EnterPhase2()
    {
        isPhase2   = true;
        isAttacking = true;

        HitStop.Instance?.Stop(0.25f);
        CameraController.Instance?.Shake(0.6f, 0.5f);

        if (phaseTransitionParticles != null) phaseTransitionParticles.Play();

        // Rengi faza 2 rengine geçir
        if (sr != null)
        {
            float t = 0f;
            Color start = sr.color;
            while (t < 0.8f)
            {
                t += Time.unscaledDeltaTime;
                sr.color = Color.Lerp(start, phase2Color, t / 0.8f);
                yield return null;
            }
            sr.color = phase2Color;
        }

        OnPhase2Entered?.Invoke();
        isAttacking = false;
    }

    void Die()
    {
        anim?.SetTrigger("Death");
        OnDeath?.Invoke();
        StartCoroutine(DieDelayed());
    }

    IEnumerator DieDelayed()
    {
        yield return new WaitForSeconds(1.5f); // ölüm animasyonu oynasın
        Destroy(gameObject);
    }

    IEnumerator FlashWhite()
    {
        if (sr == null) yield break;
        Color orig = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(0.08f);
        sr.color = orig;
    }

    // ── Yardımcı ─────────────────────────────────────────────────
    Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(rad) - v.y * Mathf.Sin(rad),
            v.x * Mathf.Sin(rad) + v.y * Mathf.Cos(rad));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, slamRadius);
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, dashHitRadius);
    }
}

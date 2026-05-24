using System.Collections;
using UnityEngine;

public class ExploderEnemy : MonoBehaviour
{
    [Header("── Devriye ──")]
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] float patrolSpeed = 1.5f;

    [Header("── Hareket ──")]
    [SerializeField] float chaseSpeed     = 3.5f;
    [SerializeField] float detectionRange = 7f;
    [SerializeField] float triggerRange   = 1.5f;   // bu mesafede sayım başlar

    int currentPatrolIndex = 0;

    [Header("── Patlama ──")]
    [SerializeField] float countdownDuration = 1.2f;
    [SerializeField] float explosionRadius   = 2.2f;
    [SerializeField] int   explosionDamage   = 35;
    [SerializeField] float playerKnockback   = 7f;
    [SerializeField] ParticleSystem explosionParticles;

    [Header("── Görsel ──")]
    [SerializeField] Color flashColor = new Color(1f, 0.3f, 0.2f);
    [SerializeField] float flashSpeedBase = 4f;   // sayım sırasında flash hızı
    [SerializeField] float flashSpeedMax  = 18f;  // sona yaklaşırken hızlanır

    SpriteRenderer    sr;
    Color             originalColor;
    Rigidbody2D       rb;
    Transform         player;
    EnemyHealth       enemyHealth;
    StatusEffectController statusEffect;

    bool isCountingDown = false;
    bool isExploded     = false;

    void Start()
    {
        sr            = GetComponent<SpriteRenderer>();
        rb            = GetComponent<Rigidbody2D>();
        enemyHealth   = GetComponent<EnemyHealth>();
        statusEffect  = GetComponent<StatusEffectController>();

        if (sr != null) originalColor = sr.color;

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        // Player ile fiziksel çarpışmayı kapat
        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer >= 0)
            Physics2D.IgnoreLayerCollision(gameObject.layer, playerLayer, true);
    }

    void Update()
    {
        if (player == null || isExploded) return;
        if (statusEffect != null && statusEffect.IsFrozen) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // Sayım sırasında hareket etmesin
        if (isCountingDown) return;

        // Algılama menzilinde değilse devriye at
        if (dist > detectionRange)
        {
            Patrol();
            return;
        }

        // Tetik mesafesine ulaştıysa patla
        if (dist <= triggerRange)
        {
            StartCoroutine(CountdownAndExplode());
            return;
        }

        // Aksi takdirde oyuncuya yaklaş — direkt pozisyon ile (fizik bypass)
        transform.position = Vector2.MoveTowards(
            transform.position, player.position, chaseSpeed * Time.deltaTime);

        if (sr != null) sr.flipX = (transform.position.x - player.position.x) < 0f;
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPatrolIndex];
        if (target == null) return;

        if (Vector2.Distance(transform.position, target.position) < 0.25f)
        {
            currentPatrolIndex++;
            if (currentPatrolIndex >= patrolPoints.Length) currentPatrolIndex = 0;
            target = patrolPoints[currentPatrolIndex];
            if (target == null) return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position, target.position, patrolSpeed * Time.deltaTime);

        if (sr != null) sr.flipX = (transform.position.x - target.position.x) < 0f;
    }

    IEnumerator CountdownAndExplode()
    {
        isCountingDown = true;
        if (rb != null) rb.linearVelocityX = 0f;

        float elapsed = 0f;
        while (elapsed < countdownDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / countdownDuration;

            // Flash hızı sona yaklaşırken artar
            float flashSpeed = Mathf.Lerp(flashSpeedBase, flashSpeedMax, t);
            float pulse = Mathf.PingPong(elapsed * flashSpeed, 1f);
            if (sr != null) sr.color = Color.Lerp(originalColor, flashColor, pulse);

            yield return null;
        }

        Explode();
    }

    void Explode()
    {
        isExploded = true;

        // Partikül
        if (explosionParticles != null)
        {
            explosionParticles.transform.SetParent(null);
            explosionParticles.Play();
            Destroy(explosionParticles.gameObject, explosionParticles.main.duration + 1f);
        }

        // Alan hasarı - oyuncu
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                float dir = transform.position.x < hit.transform.position.x ? 1f : -1f;
                hit.GetComponent<HealthController>()?.TakeDamage(explosionDamage, playerKnockback, dir);
            }
        }

        // Ekran sallanması + hit-stop
        CameraController.Instance?.Shake(0.3f, 0.35f);
        HitStop.Instance?.Stop(0.08f);

        // Kendi ölümü
        if (enemyHealth != null)
            enemyHealth.TakeDamage(99999, Vector2.zero);
        else
            Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, triggerRange);
        Gizmos.color = new Color(1f, 0.2f, 0.1f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

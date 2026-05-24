using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonerEnemy : MonoBehaviour
{
    [Header("── Devriye ──")]
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] float patrolSpeed = 1.5f;

    [Header("── Algılama ──")]
    [SerializeField] float detectionRange = 9f;
    [SerializeField] float fleeRange      = 5f;    // bu mesafeden yakınsa kaçar
    [SerializeField] float fleeSpeed      = 2.5f;

    int currentPatrolIndex = 0;

    [Header("── Çağırma ──")]
    [SerializeField] GameObject[] summonPrefabs;   // çağrılabilecek düşman tipleri
    [SerializeField] float        summonInterval     = 3f;
    [SerializeField] int          maxActiveSummons   = 4;
    [SerializeField] float        summonRadius       = 2.5f;  // oyuncuya doğru çağrılacak mesafe
    [SerializeField] float        summonChannelTime  = 1.2f;  // çağırma sırasında bekleme

    [Header("── Savunma ──")]
    [SerializeField] float channelingDamageReduction = 0.7f; // çağırma sırasında %70 az hasar

    [Header("── Görsel ──")]
    [SerializeField] ParticleSystem channelParticles;   // çağırma sırasında
    [SerializeField] ParticleSystem summonParticles;    // düşman çıkarken
    [SerializeField] Color          channelColor = new Color(0.5f, 0.2f, 0.8f);

    SpriteRenderer   sr;
    Rigidbody2D      rb;
    Transform        player;
    EnemyHealth      enemyHealth;
    StatusEffectController statusEffect;

    Color  originalColor;
    float  summonTimer;
    bool   isChanneling = false;

    List<GameObject> activeSummons = new List<GameObject>();

    public bool  IsChanneling    => isChanneling;
    public float DamageReduction => channelingDamageReduction;

    void Start()
    {
        sr           = GetComponent<SpriteRenderer>();
        rb           = GetComponent<Rigidbody2D>();
        enemyHealth  = GetComponent<EnemyHealth>();
        statusEffect = GetComponent<StatusEffectController>();

        if (sr != null) originalColor = sr.color;

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        // Player ile fiziksel çarpışmayı kapat
        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer >= 0)
            Physics2D.IgnoreLayerCollision(gameObject.layer, playerLayer, true);

        summonTimer = summonInterval * 0.5f; // ilk çağırma daha hızlı
    }

    void Update()
    {
        if (player == null) return;
        if (statusEffect != null && statusEffect.IsFrozen) return;

        // Ölü summon'ları listeden temizle
        activeSummons.RemoveAll(s => s == null);

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > detectionRange)
        {
            Patrol();
            return;
        }
        if (isChanneling) return;

        // Yakındaysa oyuncudan KAÇIŞ yönünde uzaklaş
        if (dist < fleeRange)
        {
            float dirAway = player.position.x > transform.position.x ? -1f : 1f;
            Vector3 fleeTarget = transform.position + new Vector3(dirAway * 2f, 0f, 0f);
            transform.position = Vector2.MoveTowards(
                transform.position, fleeTarget, fleeSpeed * Time.deltaTime);
            if (sr != null) sr.flipX = player.position.x > transform.position.x;
        }
        else
        {
            if (sr != null) sr.flipX = player.position.x > transform.position.x;
        }

        // Çağırma zamanlayıcı
        summonTimer -= Time.deltaTime;
        if (summonTimer <= 0f && activeSummons.Count < maxActiveSummons)
        {
            summonTimer = summonInterval;
            StartCoroutine(SummonRoutine());
        }
    }

    IEnumerator SummonRoutine()
    {
        isChanneling = true;
        if (rb != null) rb.linearVelocityX = 0f;

        // Görsel telgraf: rengi mor'a çevir + partikül
        if (channelParticles != null)
        {
            channelParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            channelParticles.Play();
        }

        float elapsed = 0f;
        while (elapsed < summonChannelTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / summonChannelTime;
            if (sr != null) sr.color = Color.Lerp(originalColor, channelColor, Mathf.PingPong(t * 4f, 1f));
            yield return null;
        }
        if (sr != null) sr.color = originalColor;

        // Spawn — oyuncuya doğru tarafa
        if (summonPrefabs != null && summonPrefabs.Length > 0)
        {
            GameObject prefab = summonPrefabs[Random.Range(0, summonPrefabs.Length)];
            if (prefab != null)
            {
                float dirToPlayer = player != null && player.position.x > transform.position.x ? 1f : -1f;
                float jitter      = Random.Range(-0.4f, 0.4f); // hafif rastgelelik
                Vector3 spawnPos  = transform.position
                                  + new Vector3(dirToPlayer * summonRadius + jitter, 0.5f, 0f);

                GameObject summoned = Instantiate(prefab, spawnPos, Quaternion.identity);
                activeSummons.Add(summoned);

                if (summonParticles != null)
                {
                    summonParticles.transform.position = spawnPos;
                    summonParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    summonParticles.Play();
                }
            }
        }

        isChanneling = false;
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

    void OnDestroy()
    {
        // Tellal ölürse çağırdığı düşmanlar kalır — istersen burada onları da öldürebilirsin
        // Şimdilik kalsınlar, oyuncuya zorluk
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0.2f, 0.8f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, fleeRange);
        Gizmos.color = new Color(0.8f, 0.4f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, summonRadius);
    }
}

using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [Header("── Devriye ──")]
    public float       speed  = 1.5f;
    public Transform[] points;

    [Header("── Algılama ──")]
    public float detectionRange = 7f;
    public float minRange       = 3f;  // bu kadar yaklaşınca geri çekil

    [Header("── Mermi ──")]
    public GameObject projectilePrefab;
    public Transform  shootPoint;
    public float      shootCooldown   = 2f;
    public float      projectileSpeed = 8f;
    public int        projectileDamage = 10;

    int            i;
    SpriteRenderer sr;
    Transform      player;
    EnemyHealth    enemyHealth;
    float          shootTimer;

    void Start()
    {
        sr          = GetComponent<SpriteRenderer>();
        enemyHealth = GetComponent<EnemyHealth>();
        shootTimer  = shootCooldown;

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Player ile fiziksel çarpışmayı devre dışı bırak
        int enemyLayer  = gameObject.layer;
        int playerLayer = playerObj != null ? playerObj.layer : LayerMask.NameToLayer("Player");
        if (enemyLayer >= 0 && playerLayer >= 0)
            Physics2D.IgnoreLayerCollision(enemyLayer, playerLayer, true);
    }

    void Update()
    {
        if (enemyHealth != null && enemyHealth.IsKnockedBack) return;
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            FacePlayer();

            // Çok yakınsa geri çekil
            if (dist < minRange)
            {
                Retreat();
                return;
            }

            // Ateş et
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                shootTimer = shootCooldown;
                Shoot();
            }
        }
        else
        {
            Patrol();
        }
    }

    void FacePlayer()
    {
        sr.flipX = player.position.x > transform.position.x;
    }

    void Retreat()
    {
        Vector2 dir        = ((Vector2)transform.position - (Vector2)player.position).normalized;
        Vector2 retreatPos = (Vector2)transform.position + dir * speed * Time.deltaTime;
        transform.position = retreatPos;
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

        sr.flipX = (transform.position.x - points[i].position.x) < 0f;
    }

    void Shoot()
    {
        if (projectilePrefab == null || player == null) return;

        Transform origin = shootPoint != null ? shootPoint : transform;
        Vector2   dir    = ((Vector2)player.position - (Vector2)origin.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, origin.position, Quaternion.identity);
        var ep = proj.GetComponent<EnemyProjectile>();
        if (ep != null)
        {
            ep.damage = projectileDamage;
            ep.speed  = projectileSpeed;
            ep.Init(dir);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minRange);
    }
}

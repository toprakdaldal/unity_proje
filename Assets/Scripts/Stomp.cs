using System.Collections;
using UnityEngine;

public class Stomp : MonoBehaviour
{
    [Header("── Stomp ──")]
    public  bool            isUnlocked   = true;
    [SerializeField] float  stompSpeed   = 22f;
    [SerializeField] float  stompRadius  = 1.8f;
    [SerializeField] int    stompDamage  = 8;
    [SerializeField] float  stunDuration = 1.5f;

    [Header("── Efekt ──")]
    [SerializeField] ParticleSystem groundCrackParticles;
    [SerializeField] LayerMask      groundLayer;

    PlayerController playerController;
    Rigidbody2D      rb;

    bool isStomping;
    bool landTriggered;

    public bool IsStomping => isStomping;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb               = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isUnlocked) return;

        // Yere değince landTriggered sıfırla (bir sonraki stomp için)
        if (playerController.IsGrounded && !isStomping)
            landTriggered = false;

        if (isStomping) return;

        bool downHeld = Input.GetAxisRaw("Vertical") < -0.5f;
        bool zPressed  = InputBindings.GetKeyDown(InputAction.Attack);

        if (!playerController.IsGrounded && downHeld && zPressed)
            StartCoroutine(StompRoutine());
    }

    void FixedUpdate()
    {
        if (isStomping)
            rb.linearVelocity = new Vector2(0f, -stompSpeed);
    }

    IEnumerator StompRoutine()
    {
        isStomping    = true;
        landTriggered = false;
        yield return new WaitForSeconds(0.06f);
        // Çarpma OnCollisionEnter2D ile yakalanacak
        // Yedek: IsGrounded ile de kontrol et
        while (isStomping)
            yield return null;
    }

    // Hızlı düşüşlerde OnCollisionEnter2D çok daha güvenilir
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!isStomping || landTriggered) return;

        // Yukarıdan mı çarptı? (normal Y > 0 = zemin)
        foreach (var contact in col.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                landTriggered = true;
                isStomping    = false;
                OnStompLand();
                return;
            }
        }
    }

    void OnStompLand()
    {
        if (groundCrackParticles != null)
        {
            groundCrackParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            groundCrackParticles.Play();
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, stompRadius);
        foreach (var hit in hits)
        {
            Vector2 dir = (hit.transform.position - transform.position).normalized;

            var enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.TakeDamage(stompDamage, dir);

            var enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
                enemy.Stun(stunDuration);

            var crate = hit.GetComponent<BreakableCrate>();
            if (crate != null)
                crate.TakeDamage(stompDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.8f, 0.4f, 0f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, stompRadius);
    }
}

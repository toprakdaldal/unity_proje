using System.Collections;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [Header("── Saldırı ──")]
    [SerializeField] Transform  attackPoint;
    [SerializeField] float      attackRadius   = 0.6f;
    [SerializeField] int        attackDamage   = 15;
    [SerializeField] float      attackCooldown = 0.35f;
    [SerializeField] LayerMask  enemyLayer;

    [Header("── İlahi Ateş Saldırısı ──")]
    [SerializeField] Transform  fireAttackPoint;
    [SerializeField] float      fireAttackRadius = 1.2f;
    [SerializeField] int        fireAttackDamage = 30;

    [Header("── İlahi Ateş Dolumu (düşman başına) ──")]
    [SerializeField] float divineFirePerKill = 20f;

    PlayerController playerController;
    bool             canAttack = true;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void OnAttackHit()
    {
        if (!canAttack) return;
        StartCoroutine(AttackCooldown());

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position, attackRadius, enemyLayer);

        foreach (var hit in hits)
        {
            // Düşman scripti eklenince buraya GetComponent eklenecek
            playerController?.AddDivineFire(divineFirePerKill);
        }
    }

    public void OnFireAttackHit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            fireAttackPoint != null ? fireAttackPoint.position : attackPoint.position,
            fireAttackRadius, enemyLayer);

        foreach (var hit in hits)
        {
            playerController?.AddDivineFire(divineFirePerKill);
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
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

using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [HideInInspector] public int   damage   = 10;
    [HideInInspector] public float speed    = 8f;
    [SerializeField]  float        maxRange = 10f;

    Vector2 direction;
    float   traveled = 0f;
    bool    canHit   = false;

    public void Init(Vector2 dir)
    {
        direction = dir.normalized;
        StartCoroutine(EnableHit());
    }

    IEnumerator EnableHit()
    {
        yield return new WaitForSeconds(0.15f);
        canHit = true;
    }

    void Update()
    {
        float step = speed * Time.deltaTime;

        // Sadece hareket yönünde tarama — üstten düşen player'a çarpmaz
        RaycastHit2D hit = Physics2D.CircleCast(
            transform.position, 0.12f, direction, step);

        if (hit.collider != null
            && hit.collider.GetComponent<EnemyHealth>()     == null
            && hit.collider.GetComponent<EnemyProjectile>() == null
            && hit.collider.GetComponent<BossController>()  == null)
        {
            if (canHit && hit.collider.CompareTag("Player"))
                hit.collider.GetComponent<HealthController>()?.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }

        transform.Translate(direction * step);
        traveled += step;
        if (traveled >= maxRange)
            Destroy(gameObject);
    }
}

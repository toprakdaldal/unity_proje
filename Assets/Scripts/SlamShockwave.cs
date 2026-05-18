using UnityEngine;

public class SlamShockwave : MonoBehaviour
{
    [HideInInspector] public float speed     = 6f;
    [HideInInspector] public int   damage    = 25;
    [HideInInspector] public float maxRange  = 8f;
    [HideInInspector] public float direction = 1f;  // 1 = sağ, -1 = sol

    float traveled = 0f;
    bool  hasHit   = false;

    ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position += new Vector3(direction * step, 0f, 0f);
        traveled += step;

        if (traveled >= maxRange)
            StartDie();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        if (!other.CompareTag("Player")) return;

        hasHit = true;
        float kbDir = direction;
        other.GetComponent<HealthController>()?.TakeDamage(damage, 5f, kbDir);
        CameraController.Instance?.Shake(0.2f, 0.25f);
        StartDie();
    }

    void StartDie()
    {
        // Collider'ı kapat, partiküller bitince yok ol
        if (TryGetComponent<Collider2D>(out var col)) col.enabled = false;
        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Destroy(gameObject, ps.main.duration + 0.5f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

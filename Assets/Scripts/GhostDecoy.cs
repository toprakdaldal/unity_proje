using UnityEngine;

// Hayalet objelerine eklenir — yakındaki düşmanları geçici olarak kendine çeker
public class GhostDecoy : MonoBehaviour
{
    public float decoyDuration = 0.4f; // hayaletin yaşam süresiyle aynı olmalı
    public float decoyRadius   = 6f;   // sadece bu yarıçap içindeki düşmanlar etkilenir

    void Start()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist <= decoyRadius)
                enemy.SetDecoy(transform, decoyDuration);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 0.8f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, decoyRadius);
    }
}

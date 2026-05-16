using UnityEngine;

// Seviyenin altına yerleştir — içine giren düşmanlar anında ölür, divine fire dolar
public class DeathZone : MonoBehaviour
{
    [SerializeField] float divineFirePerKill = 12.5f;

    PlayerController playerController;

    void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerController = playerObj.GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth == null) return;

        // Önce divine fire aboneliğini ekle, sonra öldür
        var pc = playerController;
        enemyHealth.OnDied += () => pc?.AddDivineFire(divineFirePerKill);
        enemyHealth.TakeDamage(9999);
    }
}

using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 60;

    public UnityEvent OnDeath;

    int currentHealth;

    public int CurrentHealth => currentHealth;

    void Awake() => currentHealth = maxHealth;

    // true döner: düşman öldü
    public bool TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject, 0.1f);
            return true;
        }
        return false;
    }
}

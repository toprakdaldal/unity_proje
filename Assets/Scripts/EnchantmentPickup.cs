using UnityEngine;

public class EnchantmentPickup : MonoBehaviour
{
    public enum EnchantmentType { Poison, Burn, Freeze }

    [Header("── Büyü ──")]
    [SerializeField] EnchantmentType enchantmentType;
    [SerializeField] string          displayName = "Zehir Büyüsü";
    [SerializeField] string          keyHint     = "Z";
    [SerializeField] Sprite          icon;

    [Header("── Animasyon ──")]
    [SerializeField] float bobSpeed    = 2f;
    [SerializeField] float bobHeight   = 0.15f;
    [SerializeField] float rotateSpeed = 90f;

    Vector3 startPos;

    void Start() => startPos = transform.position;

    void Update()
    {
        float newY         = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var combat = other.GetComponent<CombatController>();
        if (combat != null)
        {
            switch (enchantmentType)
            {
                case EnchantmentType.Poison: combat.poisonUnlocked = true; break;
                case EnchantmentType.Burn:   combat.burnUnlocked   = true; break;
                case EnchantmentType.Freeze: combat.freezeUnlocked = true; break;
            }
        }

        // HUD bildirimi
        AbilityHUD.Instance?.ShowAbility(displayName, keyHint, icon);

        Destroy(gameObject);
    }
}

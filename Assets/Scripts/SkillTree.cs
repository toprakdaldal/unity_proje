using UnityEngine;

public class SkillTree : MonoBehaviour
{
    public static SkillTree Instance;

    [Header("── Maliyet ──")]
    [SerializeField] int baseCost      = 15;
    [SerializeField] int costIncrement = 10;
    [SerializeField] int maxLevel      = 10;

    [Header("── Mevcut Seviyeler ──")]
    public int combatLevel   = 0;  // melee hasarı
    public int magicLevel    = 0;  // divine fire max
    public int movementLevel = 0;  // hareket hızı

    [Header("── Seviye Başı Bonus ──")]
    public int   damagePerLevel       = 3;
    public float divineFireMaxPerLvl  = 10f;
    public float speedPerLevel        = 0.3f;

    public System.Action OnSkillsChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public int GetCost(int level) => baseCost + (level * costIncrement);
    public int MaxLevel           => maxLevel;

    public bool UpgradeCombat()
    {
        if (combatLevel >= maxLevel) return false;
        int cost = GetCost(combatLevel);
        if (SoulCurrency.Instance == null || !SoulCurrency.Instance.SpendSouls(cost)) return false;
        combatLevel++;
        OnSkillsChanged?.Invoke();
        return true;
    }

    public bool UpgradeMagic()
    {
        if (magicLevel >= maxLevel) return false;
        int cost = GetCost(magicLevel);
        if (SoulCurrency.Instance == null || !SoulCurrency.Instance.SpendSouls(cost)) return false;
        magicLevel++;
        OnSkillsChanged?.Invoke();
        return true;
    }

    public bool UpgradeMovement()
    {
        if (movementLevel >= maxLevel) return false;
        int cost = GetCost(movementLevel);
        if (SoulCurrency.Instance == null || !SoulCurrency.Instance.SpendSouls(cost)) return false;
        movementLevel++;
        OnSkillsChanged?.Invoke();
        return true;
    }

    // Yardımcı sorgular
    public int   ExtraDamage     => combatLevel   * damagePerLevel;
    public float ExtraDivineMax  => magicLevel    * divineFireMaxPerLvl;
    public float ExtraMoveSpeed  => movementLevel * speedPerLevel;
}

using System.Collections.Generic;
using UnityEngine;

public enum SkillID
{
    None,
    // ── Savaş ──
    SharpBlade,
    DoubleStrike,
    CriticalHit,
    Vampire,
    Berserk,
    // ── Maji ──
    DivineFlow,
    FastRegen,
    DoubleFireball,
    FireMaster,
    Apocalypse,
    // ── Hareket ──
    QuickStep,
    Glide,        // eski: HigherJump
    FastFall,     // eski: DoubleJump
    DashStrike,   // eski: FastDash
    Phantom
}

[System.Serializable]
public class SkillNode
{
    public SkillID id;
    public SkillID prerequisite = SkillID.None;
    public string  skillName    = "Skill";
    [TextArea] public string description;
    public int     cost         = 20;
    public Sprite  icon;
}

public class SkillTree : MonoBehaviour
{
    public static SkillTree Instance;

    [Header("── Tüm Skill'ler ──")]
    public List<SkillNode> allSkills = new List<SkillNode>();

    HashSet<SkillID> unlocked = new HashSet<SkillID>();

    public System.Action OnSkillsChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public bool IsUnlocked(SkillID id) => unlocked.Contains(id);

    public bool CanUnlock(SkillID id)
    {
        var node = GetNode(id);
        if (node == null) return false;
        if (unlocked.Contains(id)) return false;
        if (node.prerequisite != SkillID.None && !unlocked.Contains(node.prerequisite)) return false;
        if (SoulCurrency.Instance == null) return false;
        return SoulCurrency.Instance.CanAfford(node.cost);
    }

    public bool TryUnlock(SkillID id)
    {
        if (!CanUnlock(id)) return false;
        var node = GetNode(id);
        SoulCurrency.Instance.SpendSouls(node.cost);
        unlocked.Add(id);
        OnSkillsChanged?.Invoke();
        return true;
    }

    public SkillNode GetNode(SkillID id)
    {
        foreach (var s in allSkills)
            if (s.id == id) return s;
        return null;
    }

    // ── Bonus Sorguları ──────────────────────────────────────────
    public int   ExtraDamage     => IsUnlocked(SkillID.SharpBlade) ? 5 : 0;
    public int   ExtraHitCount   => IsUnlocked(SkillID.DoubleStrike) ? 1 : 0;
    public float CritChance      => IsUnlocked(SkillID.CriticalHit) ? 0.25f : 0f;
    public bool  HasVampire      => IsUnlocked(SkillID.Vampire);
    public bool  HasBerserk      => IsUnlocked(SkillID.Berserk);

    public float ExtraDivineMax  => IsUnlocked(SkillID.DivineFlow) ? 20f : 0f;
    public float DivineRegenRate => IsUnlocked(SkillID.FastRegen) ? 1f : 0f; // saniye başına
    public bool  HasDoubleFireball => IsUnlocked(SkillID.DoubleFireball);
    public float BurnChanceMult  => IsUnlocked(SkillID.FireMaster) ? 2f : 1f;
    public float BurnDurationMult => IsUnlocked(SkillID.FireMaster) ? 1.5f : 1f;
    public float FireRadiusMult  => IsUnlocked(SkillID.Apocalypse) ? 1.5f : 1f;

    public float ExtraMoveSpeed  => IsUnlocked(SkillID.QuickStep) ? 1.5f : 0f;
    public bool  HasGlide        => IsUnlocked(SkillID.Glide);
    public bool  HasFastFall     => IsUnlocked(SkillID.FastFall);
    public bool  HasDashStrike   => IsUnlocked(SkillID.DashStrike);
    public bool  HasPhantom      => IsUnlocked(SkillID.Phantom);
}

using UnityEngine;

public enum RingEffect
{
    None,
    BonusDamage,        // +N melee hasarı
    DamageReduction,    // -% hasar alma (0.15 = %15)
    FasterRunning,      // +N hareket hızı
    MaxGhostSteps,      // +N max ghost step şarjı
    DivineFireOnKill,   // her öldürmede +N divine fire
    BonusHealth         // +N max can
}

[System.Serializable]
public class Ring
{
    public string     ringName  = "Yüzük";
    public RingEffect effect    = RingEffect.None;
    public float      value     = 0f;
    public Sprite     icon;
    [TextArea]
    public string     description;
}

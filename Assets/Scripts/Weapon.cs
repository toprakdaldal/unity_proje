using UnityEngine;

[System.Serializable]
public class Weapon
{
    [Header("── Genel ──")]
    public string weaponName    = "Silah";
    public Sprite icon;

    [Header("── Hasar ──")]
    public int    damage         = 15;
    public float  attackRadius   = 0.6f;
    public float  attackCooldown = 0.35f;
    public int    hitCount       = 1;    // combo vuruş sayısı (1, 2, 3)

    [Header("── Özel ──")]
    public bool   createsShockwave = false; // çekiç için yere vuruş efekti
    public float  knockbackForce   = 5f;
    public Color  flashColor       = Color.white;

    [Header("── Açıklama ──")]
    [TextArea]
    public string description;
}

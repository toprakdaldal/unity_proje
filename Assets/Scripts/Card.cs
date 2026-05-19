using System.Collections.Generic;
using UnityEngine;

public enum CardEffect
{
    BurnChanceBonus,    // +N burn şansı (0.35 = +%35)
    DamageBonus,        // saldırı çarpanı (0.5 = +%50 hasar)
    MaxHPMultiplier,    // max can çarpanı (-0.3 = -%30)
    SoulMultiplier,     // ruh ödülü çarpanı (1.0 = 2x)
    HealOnKill,         // her öldürmede +N can
    MoveSpeedBonus,     // +N hareket hızı
    DamageReduction     // -% hasar alma (0.25 = -%25)
}

[System.Serializable]
public class CardModifier
{
    public CardEffect effect;
    public float      value;
}

[System.Serializable]
public class Card
{
    public string cardName;
    [TextArea] public string description;
    public Color cardColor = Color.white;
    public List<CardModifier> modifiers = new List<CardModifier>();
}

using System.Collections.Generic;
using UnityEngine;

public class CardSystem : MonoBehaviour
{
    public static CardSystem Instance;

    [Header("── Kart Havuzu ──")]
    public List<Card> allCards = new List<Card>();

    [Header("── Aktif Kart ──")]
    public Card activeCard;

    public System.Action<Card> OnCardChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Havuzdan rastgele N kart çek (tekrar etmez)
    public List<Card> DrawRandom(int count)
    {
        var pool   = new List<Card>(allCards);
        var result = new List<Card>();
        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            result.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
        return result;
    }

    public void Select(Card card)
    {
        activeCard = card;
        OnCardChanged?.Invoke(card);
    }

    public float GetBonus(CardEffect effect)
    {
        if (activeCard == null) return 0f;
        float total = 0f;
        foreach (var m in activeCard.modifiers)
            if (m.effect == effect) total += m.value;
        return total;
    }
}

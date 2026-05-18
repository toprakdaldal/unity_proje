using UnityEngine;

public class RingController : MonoBehaviour
{
    public static RingController Instance;

    [Header("── Yüzük Slotları ──")]
    [SerializeField] int slotCount = 3;
    public Ring[] equipped;

    public System.Action OnRingsChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (equipped == null || equipped.Length != slotCount)
            equipped = new Ring[slotCount];
    }

    // Boş slot var mı?
    public int FirstEmptySlot()
    {
        for (int i = 0; i < equipped.Length; i++)
            if (equipped[i] == null) return i;
        return -1;
    }

    // Otomatik boş slota ekle, doluysa false dön
    public bool TryEquip(Ring ring)
    {
        int slot = FirstEmptySlot();
        if (slot < 0) return false;
        equipped[slot] = ring;
        OnRingsChanged?.Invoke();
        return true;
    }

    public void Unequip(int slot)
    {
        if (slot < 0 || slot >= equipped.Length) return;
        equipped[slot] = null;
        OnRingsChanged?.Invoke();
    }

    // Belirli bir efekt için toplam bonus
    public float GetBonus(RingEffect effect)
    {
        float total = 0f;
        foreach (var r in equipped)
            if (r != null && r.effect == effect)
                total += r.value;
        return total;
    }
}

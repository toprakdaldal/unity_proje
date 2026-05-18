using UnityEngine;

public class SoulCurrency : MonoBehaviour
{
    public static SoulCurrency Instance;

    [SerializeField] int startingSouls = 0;

    int currentSouls;

    public int CurrentSouls => currentSouls;
    public System.Action<int> OnSoulsChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        currentSouls = startingSouls;
    }

    public void AddSouls(int amount)
    {
        currentSouls += amount;
        OnSoulsChanged?.Invoke(currentSouls);
    }

    public bool SpendSouls(int amount)
    {
        if (currentSouls < amount) return false;
        currentSouls -= amount;
        OnSoulsChanged?.Invoke(currentSouls);
        return true;
    }

    public bool CanAfford(int amount) => currentSouls >= amount;
}

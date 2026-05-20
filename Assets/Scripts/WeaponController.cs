using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public static WeaponController Instance;

    [Header("── Silahlar ──")]
    public List<Weapon> weapons = new List<Weapon>();

    [Header("── Tuş ──")]
    [SerializeField] KeyCode switchKey = KeyCode.E;

    int currentIndex = 0;

    public Weapon Current
    {
        get
        {
            if (weapons == null || weapons.Count == 0) return null;
            return weapons[currentIndex];
        }
    }

    public System.Action OnWeaponChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        OnWeaponChanged?.Invoke();
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey) && weapons.Count > 1)
            SwitchNext();
    }

    public void SwitchNext()
    {
        currentIndex = (currentIndex + 1) % weapons.Count;
        OnWeaponChanged?.Invoke();
    }

    public void AddWeapon(Weapon w)
    {
        weapons.Add(w);
        OnWeaponChanged?.Invoke();
    }
}

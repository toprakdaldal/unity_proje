using UnityEngine;
using TMPro;

public class SkillTreeUI : MonoBehaviour
{
    [Header("── Panel ──")]
    [SerializeField] GameObject panel;
    [SerializeField] KeyCode    toggleKey = KeyCode.Tab;

    [Header("── Ruh Sayacı ──")]
    [SerializeField] TMP_Text soulText;

    [Header("── Tüm Slot'lar ──")]
    [SerializeField] SkillSlotUI[] allSlots;

    void Start()
    {
        if (panel != null) panel.SetActive(false);

        if (SkillTree.Instance     != null) SkillTree.Instance.OnSkillsChanged   += Refresh;
        if (SoulCurrency.Instance  != null) SoulCurrency.Instance.OnSoulsChanged += _ => Refresh();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            Toggle();
    }

    void Toggle()
    {
        if (panel == null) return;
        bool open = !panel.activeSelf;
        panel.SetActive(open);
        if (open) Refresh();
    }

    public void Refresh()
    {
        if (SoulCurrency.Instance != null && soulText != null)
            soulText.text = SoulCurrency.Instance.CurrentSouls.ToString();

        if (allSlots == null) return;
        foreach (var slot in allSlots)
            if (slot != null) slot.Refresh();
    }
}

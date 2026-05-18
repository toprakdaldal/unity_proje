using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTreeUI : MonoBehaviour
{
    [Header("── Panel ──")]
    [SerializeField] GameObject panel;
    [SerializeField] KeyCode    toggleKey = KeyCode.Tab;

    [Header("── Ruh Sayacı ──")]
    [SerializeField] TMP_Text soulText;

    [Header("── Combat ──")]
    [SerializeField] TMP_Text combatLevelText;
    [SerializeField] TMP_Text combatCostText;
    [SerializeField] Button   combatUpgradeBtn;

    [Header("── Magic ──")]
    [SerializeField] TMP_Text magicLevelText;
    [SerializeField] TMP_Text magicCostText;
    [SerializeField] Button   magicUpgradeBtn;

    [Header("── Movement ──")]
    [SerializeField] TMP_Text movementLevelText;
    [SerializeField] TMP_Text movementCostText;
    [SerializeField] Button   movementUpgradeBtn;

    void Start()
    {
        if (panel != null) panel.SetActive(false);

        if (combatUpgradeBtn   != null) combatUpgradeBtn  .onClick.AddListener(OnCombatClick);
        if (magicUpgradeBtn    != null) magicUpgradeBtn   .onClick.AddListener(OnMagicClick);
        if (movementUpgradeBtn != null) movementUpgradeBtn.onClick.AddListener(OnMovementClick);

        if (SkillTree.Instance     != null) SkillTree.Instance.OnSkillsChanged += Refresh;
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

    void Refresh()
    {
        if (SoulCurrency.Instance != null && soulText != null)
            soulText.text = SoulCurrency.Instance.CurrentSouls.ToString();

        var st = SkillTree.Instance;
        if (st == null) return;

        SetRow(combatLevelText,   combatCostText,   combatUpgradeBtn,   st.combatLevel,   st.GetCost(st.combatLevel),   st.MaxLevel);
        SetRow(magicLevelText,    magicCostText,    magicUpgradeBtn,    st.magicLevel,    st.GetCost(st.magicLevel),    st.MaxLevel);
        SetRow(movementLevelText, movementCostText, movementUpgradeBtn, st.movementLevel, st.GetCost(st.movementLevel), st.MaxLevel);
    }

    void SetRow(TMP_Text levelTxt, TMP_Text costTxt, Button btn, int level, int cost, int max)
    {
        if (levelTxt != null) levelTxt.text = $"Lv {level}/{max}";

        bool maxed = level >= max;
        if (costTxt != null) costTxt.text = maxed ? "MAX" : cost.ToString();

        if (btn != null)
        {
            bool canBuy = !maxed && SoulCurrency.Instance != null && SoulCurrency.Instance.CanAfford(cost);
            btn.interactable = canBuy;
        }
    }

    void OnCombatClick()   => SkillTree.Instance?.UpgradeCombat();
    void OnMagicClick()    => SkillTree.Instance?.UpgradeMagic();
    void OnMovementClick() => SkillTree.Instance?.UpgradeMovement();
}

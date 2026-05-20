using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSlotUI : MonoBehaviour
{
    [Header("── Skill ──")]
    public SkillID skillId = SkillID.None;

    [Header("── Görseller ──")]
    [SerializeField] Button   button;
    [SerializeField] Image    iconImage;
    [SerializeField] Image    background;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text costText;

    [Header("── Renkler ──")]
    [SerializeField] Color lockedColor    = new Color(0.2f, 0.2f, 0.25f);
    [SerializeField] Color availableColor = new Color(0.4f, 0.4f, 0.5f);
    [SerializeField] Color unlockedColor  = new Color(0.9f, 0.7f, 0.2f);

    void Start()
    {
        if (button != null) button.onClick.AddListener(OnClick);
        Refresh();
    }

    public void Refresh()
    {
        var st = SkillTree.Instance;
        if (st == null) return;

        var node = st.GetNode(skillId);
        if (node == null) return;

        if (iconImage != null && node.icon != null) iconImage.sprite = node.icon;
        if (nameText  != null) nameText.text = node.skillName;

        bool isUnlocked   = st.IsUnlocked(skillId);
        bool prereqMet    = node.prerequisite == SkillID.None || st.IsUnlocked(node.prerequisite);
        bool canBuy       = st.CanUnlock(skillId);

        if (costText != null)
        {
            if (isUnlocked) { costText.text = "✓"; }
            else            { costText.text = node.cost.ToString(); }
        }

        if (background != null)
        {
            if      (isUnlocked) background.color = unlockedColor;
            else if (prereqMet)  background.color = availableColor;
            else                 background.color = lockedColor;
        }

        if (button != null)
            button.interactable = canBuy;
    }

    void OnClick()
    {
        SkillTree.Instance?.TryUnlock(skillId);
    }
}

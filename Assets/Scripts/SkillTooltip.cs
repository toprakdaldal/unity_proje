using UnityEngine;
using TMPro;

public class SkillTooltip : MonoBehaviour
{
    public static SkillTooltip Instance;

    [Header("── Panel ──")]
    [SerializeField] GameObject panel;
    [SerializeField] RectTransform panelRect;

    [Header("── İçerik ──")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descText;
    [SerializeField] TMP_Text statusText;  // "Maliyet: 15" veya "Sahip olunan"

    [Header("── Ayar ──")]
    [SerializeField] Vector2 offset = new Vector2(20f, -20f);

    Canvas parentCanvas;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        parentCanvas = GetComponentInParent<Canvas>();
        Hide();
    }

    public void Show(SkillNode node, bool isUnlocked, bool canBuy)
    {
        if (node == null || panel == null) return;

        if (nameText != null) nameText.text = node.skillName;
        if (descText != null) descText.text = node.description;
        if (statusText != null)
        {
            if (isUnlocked)
                statusText.text = "<color=#FFD96B>✓ Sahip olunan</color>";
            else if (canBuy)
                statusText.text = $"<color=#FFD96B>Maliyet: {node.cost} ruh</color>";
            else
                statusText.text = $"<color=#888888>Maliyet: {node.cost} ruh (kilitli)</color>";
        }

        panel.SetActive(true);
        UpdatePosition();
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        if (panel != null && panel.activeSelf) UpdatePosition();
    }

    void UpdatePosition()
    {
        if (panelRect == null) return;
        Vector2 mousePos = Input.mousePosition;

        // Ekran kenarına çıkmasın
        float w = panelRect.rect.width;
        float h = panelRect.rect.height;
        float xMax = Screen.width  - w;
        float yMin = h;

        Vector2 pos = mousePos + offset;
        pos.x = Mathf.Clamp(pos.x, 10f, xMax - 10f);
        pos.y = Mathf.Clamp(pos.y, yMin + 10f, Screen.height - 10f);

        panelRect.position = pos;
    }
}

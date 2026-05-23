using System.Collections;
using UnityEngine;
using TMPro;

public class GhostStep : MonoBehaviour
{
    [Header("── Hayalet Adımı ──")]
    [SerializeField] int      maxCharges    = 3;
    [SerializeField] float    ghostDuration = 1.5f;
    [SerializeField] KeyCode  activateKey   = KeyCode.V;

    [Header("── Görsel ──")]
    [SerializeField] Color ghostColor = new Color(0.6f, 0.8f, 1f, 0.35f);

    [Header("── HUD ──")]
    [SerializeField] GameObject hudPanel;    // charge paneli (başta kapalı)
    [SerializeField] TMP_Text   chargeText;  // "x2" gibi

    int            currentCharges;
    bool           isGhosting;
    SpriteRenderer sr;
    Color          normalColor;

    public bool IsGhosting     => isGhosting;
    public int  CurrentCharges => currentCharges;

    void Awake()
    {
        sr          = GetComponent<SpriteRenderer>();
        normalColor = sr != null ? sr.color : Color.white;
    }

    void Start()
    {
        if (hudPanel != null) hudPanel.SetActive(false);
        UpdateHUD();
    }

    void Update()
    {
        if (InputBindings.GetKeyDown(InputAction.GhostStep) && currentCharges > 0 && !isGhosting)
            StartCoroutine(GhostRoutine());
    }

    public void AddCharge(int amount = 1)
    {
        currentCharges = Mathf.Min(currentCharges + amount, maxCharges);
        if (hudPanel != null) hudPanel.SetActive(currentCharges > 0);
        UpdateHUD();
    }

    IEnumerator GhostRoutine()
    {
        isGhosting = true;
        currentCharges--;
        UpdateHUD();

        // Sprite yarı saydam
        if (sr != null) sr.color = ghostColor;

        // Düşman katmanıyla fiziksel çarpışmayı kapat
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if (enemyLayer >= 0)
            Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayer, true);

        yield return new WaitForSeconds(ghostDuration);

        // Geri al
        isGhosting = false;
        if (sr != null) sr.color = normalColor;
        if (enemyLayer >= 0)
            Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayer, false);

        if (hudPanel != null && currentCharges == 0)
            hudPanel.SetActive(false);
    }

    void UpdateHUD()
    {
        if (chargeText != null)
            chargeText.text = "x" + currentCharges;
    }
}

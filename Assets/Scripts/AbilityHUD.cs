using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityHUD : MonoBehaviour
{
    public static AbilityHUD Instance;

    [Header("── Yetenek Slotu (Sol Alt) ──")]
    [SerializeField] GameObject slotPanel;       // tüm slot objesi
    [SerializeField] Image      slotIcon;        // yetenek ikonu
    [SerializeField] Image      slotFrame;       // çerçeve (opsiyonel)
    [SerializeField] TMP_Text   slotNameText;    // "Blink Dash"
    [SerializeField] TMP_Text   slotKeyText;     // "SHIFT"

    [Header("── Edinim Bildirimi (Orta) ──")]
    [SerializeField] GameObject notifPanel;
    [SerializeField] TMP_Text   notifText;
    [SerializeField] string     notifHeader      = "YENİ YETENEk EDİNİLDİ";
    [SerializeField] float      notifDuration    = 3f;   // toplam gösterim süresi
    [SerializeField] float      notifFadeDur     = 0.8f; // fade-out süresi

    void Awake()
    {
        Instance = this;
        if (slotPanel  != null) slotPanel.SetActive(false);
        if (notifPanel != null) notifPanel.SetActive(false);
    }

    /// <summary>
    /// Yetenek edinildiğinde çağır.
    /// </summary>
    public void ShowAbility(string abilityName, string keyHint, Sprite icon = null)
    {
        // — Kalıcı slot —
        if (slotPanel != null)
        {
            if (slotIcon != null)
            {
                if (icon != null) slotIcon.sprite = icon;
                slotIcon.enabled = true;
            }

            if (slotNameText != null) slotNameText.text = abilityName;
            if (slotKeyText  != null) slotKeyText.text  = keyHint;

            slotPanel.SetActive(true);
        }

        // — Geçici bildirim —
        if (notifPanel != null)
            StartCoroutine(ShowNotif(abilityName));
    }

    // ──────────────────────────────────────────
    IEnumerator SlideIn(GameObject panel)
    {
        RectTransform rt = panel.GetComponent<RectTransform>();
        if (rt == null) yield break;

        Vector2 target  = rt.anchoredPosition;
        Vector2 offscreen = target + new Vector2(-250f, 0f);

        float elapsed = 0f, dur = 0.35f;
        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            rt.anchoredPosition = Vector2.Lerp(offscreen, target, elapsed / dur);
            yield return null;
        }
        rt.anchoredPosition = target;
    }

    IEnumerator ShowNotif(string name)
    {
        if (notifText != null)
            notifText.text = notifHeader + "\n<b>" + name + "</b>";

        CanvasGroup cg = notifPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = notifPanel.AddComponent<CanvasGroup>();

        cg.alpha = 1f;
        notifPanel.SetActive(true);

        float waitTime = Mathf.Max(0f, notifDuration - notifFadeDur);
        yield return new WaitForSeconds(waitTime);

        float elapsed = 0f;
        while (elapsed < notifFadeDur)
        {
            elapsed  += Time.deltaTime;
            cg.alpha  = Mathf.Lerp(1f, 0f, elapsed / notifFadeDur);
            yield return null;
        }

        notifPanel.SetActive(false);
    }
}

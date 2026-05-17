using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    [Header("── Referanslar ──")]
    [SerializeField] BossController boss;
    [SerializeField] Image          fillImage;
    [SerializeField] Image          delayedFill;    // beyaz kayarak gerileyen ikinci bar
    [SerializeField] TMP_Text       bossNameText;
    [SerializeField] GameObject     barRoot;        // tüm panel (başta gizli)

    [Header("── Ayarlar ──")]
    [SerializeField] string bossName        = "???";
    [SerializeField] Color  phase1Color     = new Color(0.85f, 0.15f, 0.15f);
    [SerializeField] Color  phase2Color     = new Color(1f, 0.45f, 0f);
    [SerializeField] float  delayedSpeed    = 1.5f;  // beyaz barın erime hızı

    float targetFill    = 1f;
    float delayedFillVal = 1f;

    void Start()
    {
        if (barRoot != null) barRoot.SetActive(false);

        if (boss != null)
        {
            boss.OnActivated     += ShowBar;
            boss.OnHealthChanged += UpdateBar;
            boss.OnPhase2Entered += OnPhase2;
            boss.OnDeath         += OnBossDead;
        }

        if (bossNameText != null) bossNameText.text = bossName;
        if (fillImage    != null) fillImage.color   = phase1Color;
    }

    void ShowBar()
    {
        if (barRoot != null) barRoot.SetActive(true);
    }

    void UpdateBar(int current, int max)
    {
        ShowBar();
        targetFill = (float)current / max;
        if (fillImage    != null) fillImage.fillAmount    = targetFill;
        if (delayedFill  != null && delayedFillVal < targetFill) delayedFillVal = targetFill;
    }

    void Update()
    {
        // Beyaz bar yavaşça ana barın seviyesine iner
        if (delayedFill != null && delayedFillVal > targetFill)
        {
            delayedFillVal = Mathf.MoveTowards(
                delayedFillVal, targetFill, delayedSpeed * Time.deltaTime);
            delayedFill.fillAmount = delayedFillVal;
        }
    }

    void OnPhase2()
    {
        if (fillImage != null) fillImage.color = phase2Color;
    }

    void OnBossDead()
    {
        if (barRoot != null) barRoot.SetActive(false);
    }

    void OnDestroy()
    {
        if (boss != null)
        {
            boss.OnActivated     -= ShowBar;
            boss.OnHealthChanged -= UpdateBar;
            boss.OnPhase2Entered -= OnPhase2;
            boss.OnDeath         -= OnBossDead;
        }
    }
}

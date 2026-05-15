using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("── Can Barı ──")]
    [SerializeField] Image healthBarFill;
    [SerializeField] Text  healthText;

    [Header("── İlahi Ateş Barı ──")]
    [SerializeField] Image divineFireFill;

    [Header("── Referanslar ──")]
    [SerializeField] HealthController    healthController;
    [SerializeField] PlayerController   playerController;

    void Update()
    {
        if (healthController != null && healthBarFill != null)
        {
            healthBarFill.fillAmount = healthController.HealthPercent;
            if (healthText != null)
                healthText.text = healthController.CurrentHealth + " / " + healthController.MaxHealth;
        }

        if (playerController != null && divineFireFill != null)
            divineFireFill.fillAmount = playerController.DivineFire / playerController.DivineFireMax;
    }
}

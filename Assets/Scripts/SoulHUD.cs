using UnityEngine;
using TMPro;

public class SoulHUD : MonoBehaviour
{
    [SerializeField] TMP_Text soulText;

    void Start()
    {
        if (SoulCurrency.Instance != null)
        {
            SoulCurrency.Instance.OnSoulsChanged += UpdateText;
            UpdateText(SoulCurrency.Instance.CurrentSouls);
        }
    }

    void UpdateText(int souls)
    {
        if (soulText != null) soulText.text = souls.ToString();
    }

    void OnDestroy()
    {
        if (SoulCurrency.Instance != null)
            SoulCurrency.Instance.OnSoulsChanged -= UpdateText;
    }
}

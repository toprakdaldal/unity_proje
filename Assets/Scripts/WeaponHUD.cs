using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponHUD : MonoBehaviour
{
    [SerializeField] Image    iconImage;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text switchKeyText;

    void Start()
    {
        if (switchKeyText != null) switchKeyText.text = "[E]";

        if (WeaponController.Instance != null)
        {
            WeaponController.Instance.OnWeaponChanged += Refresh;
            Refresh();
        }
    }

    void Refresh()
    {
        var w = WeaponController.Instance?.Current;
        if (w == null) return;

        if (iconImage != null && w.icon != null) iconImage.sprite = w.icon;
        if (nameText  != null) nameText.text = w.weaponName;
    }

    void OnDestroy()
    {
        if (WeaponController.Instance != null)
            WeaponController.Instance.OnWeaponChanged -= Refresh;
    }
}

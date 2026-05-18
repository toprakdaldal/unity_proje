using UnityEngine;
using UnityEngine.UI;

public class RingHUD : MonoBehaviour
{
    [Header("── Slot Görselleri ──")]
    [SerializeField] Image[] slotIcons;     // 3 image
    [SerializeField] Sprite  emptySprite;   // boş slot ikonu (opsiyonel)

    void Start()
    {
        if (RingController.Instance != null)
        {
            RingController.Instance.OnRingsChanged += Refresh;
            Refresh();
        }
    }

    void Refresh()
    {
        if (RingController.Instance == null) return;
        var slots = RingController.Instance.equipped;
        for (int i = 0; i < slotIcons.Length && i < slots.Length; i++)
        {
            if (slotIcons[i] == null) continue;
            if (slots[i] != null && slots[i].icon != null)
            {
                slotIcons[i].sprite = slots[i].icon;
                slotIcons[i].color  = Color.white;
            }
            else
            {
                slotIcons[i].sprite = emptySprite;
                slotIcons[i].color  = new Color(1f, 1f, 1f, 0.3f);
            }
        }
    }

    void OnDestroy()
    {
        if (RingController.Instance != null)
            RingController.Instance.OnRingsChanged -= Refresh;
    }
}

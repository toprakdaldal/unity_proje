using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RebindSlotUI : MonoBehaviour
{
    [Header("── Hangi Eylem ──")]
    public InputAction action;

    [Header("── Görseller ──")]
    [SerializeField] TMP_Text actionLabel;
    [SerializeField] TMP_Text keyLabel;
    [SerializeField] Button   rebindButton;

    RebindMenu menu;

    void Start()
    {
        menu = GetComponentInParent<RebindMenu>();
        if (rebindButton != null) rebindButton.onClick.AddListener(OnRebindClick);
        InputBindings.OnBindingsChanged += Refresh;
        Refresh();
    }

    void OnDestroy()
    {
        InputBindings.OnBindingsChanged -= Refresh;
    }

    public void Refresh()
    {
        if (actionLabel != null) actionLabel.text = InputBindings.DisplayName(action);
        if (keyLabel    != null) keyLabel.text    = InputBindings.Get(action).ToString();
    }

    void OnRebindClick()
    {
        if (menu == null) return;
        if (keyLabel != null) keyLabel.text = "...";
        menu.BeginRebind(this);
    }

    public void Cancel()
    {
        Refresh();
    }
}

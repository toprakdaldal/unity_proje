using UnityEngine;
using UnityEngine.UI;

public class RebindMenu : MonoBehaviour
{
    [SerializeField] Button resetButton;

    RebindSlotUI waitingSlot;

    void Start()
    {
        if (resetButton != null) resetButton.onClick.AddListener(OnResetClick);
    }

    public void BeginRebind(RebindSlotUI slot)
    {
        waitingSlot = slot;
    }

    void Update()
    {
        if (waitingSlot == null) return;

        // Escape ile iptal
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            waitingSlot.Cancel();
            waitingSlot = null;
            return;
        }

        // Herhangi bir tuş basıldıysa yakala
        if (Input.anyKeyDown)
        {
            foreach (KeyCode kc in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (kc == KeyCode.Escape) continue;
                if (kc == KeyCode.Mouse0 || kc == KeyCode.Mouse1) continue; // sol/sağ tık çakışmasın
                if (Input.GetKeyDown(kc))
                {
                    InputBindings.Set(waitingSlot.action, kc);
                    waitingSlot = null;
                    return;
                }
            }
        }
    }

    void OnResetClick()
    {
        InputBindings.ResetToDefaults();
    }
}

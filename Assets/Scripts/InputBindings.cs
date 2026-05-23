using System.Collections.Generic;
using UnityEngine;

public enum InputAction
{
    Attack,        // Z
    DivineFire,    // X
    Fireball,      // C
    GhostStep,     // V
    Dash,          // LeftShift
    Interact       // E
}

public static class InputBindings
{
    static readonly Dictionary<InputAction, KeyCode> defaults = new Dictionary<InputAction, KeyCode>
    {
        { InputAction.Attack,     KeyCode.Z          },
        { InputAction.DivineFire, KeyCode.X          },
        { InputAction.Fireball,   KeyCode.C          },
        { InputAction.GhostStep,  KeyCode.V          },
        { InputAction.Dash,       KeyCode.LeftShift  },
        { InputAction.Interact,   KeyCode.E          }
    };

    static Dictionary<InputAction, KeyCode> bindings;
    static bool loaded = false;

    public static System.Action OnBindingsChanged;

    static void EnsureLoaded()
    {
        if (loaded) return;
        bindings = new Dictionary<InputAction, KeyCode>(defaults);

        foreach (var kvp in defaults)
        {
            string saved = PlayerPrefs.GetString(KeyName(kvp.Key), "");
            if (!string.IsNullOrEmpty(saved) && System.Enum.TryParse(saved, out KeyCode kc))
                bindings[kvp.Key] = kc;
        }
        loaded = true;
    }

    static string KeyName(InputAction a) => "bind_" + a;

    public static KeyCode Get(InputAction action)
    {
        EnsureLoaded();
        return bindings[action];
    }

    public static void Set(InputAction action, KeyCode key)
    {
        EnsureLoaded();
        bindings[action] = key;
        PlayerPrefs.SetString(KeyName(action), key.ToString());
        PlayerPrefs.Save();
        OnBindingsChanged?.Invoke();
    }

    public static void ResetToDefaults()
    {
        EnsureLoaded();
        foreach (var kvp in defaults)
        {
            bindings[kvp.Key] = kvp.Value;
            PlayerPrefs.DeleteKey(KeyName(kvp.Key));
        }
        OnBindingsChanged?.Invoke();
    }

    public static bool GetKeyDown(InputAction action) => Input.GetKeyDown(Get(action));
    public static bool GetKey    (InputAction action) => Input.GetKey    (Get(action));
    public static bool GetKeyUp  (InputAction action) => Input.GetKeyUp  (Get(action));

    public static string DisplayName(InputAction a)
    {
        switch (a)
        {
            case InputAction.Attack:     return "Saldırı";
            case InputAction.DivineFire: return "İlahi Ateş";
            case InputAction.Fireball:   return "Ateş Topu";
            case InputAction.GhostStep:  return "Hayalet Adım";
            case InputAction.Dash:       return "Dash";
            case InputAction.Interact:   return "Etkileşim";
            default: return a.ToString();
        }
    }
}

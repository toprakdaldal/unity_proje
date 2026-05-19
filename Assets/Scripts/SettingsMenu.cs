using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("── Ses Sliderları ──")]
    [SerializeField] Slider   masterSlider;
    [SerializeField] Slider   musicSlider;
    [SerializeField] Slider   sfxSlider;

    [Header("── Yüzde Etiketleri ──")]
    [SerializeField] TMP_Text masterLabel;
    [SerializeField] TMP_Text musicLabel;
    [SerializeField] TMP_Text sfxLabel;

    const string KEY_MASTER = "vol_master";
    const string KEY_MUSIC  = "vol_music";
    const string KEY_SFX    = "vol_sfx";

    void Start()
    {
        // Kayıtlı değerleri yükle
        float master = PlayerPrefs.GetFloat(KEY_MASTER, 1f);
        float music  = PlayerPrefs.GetFloat(KEY_MUSIC,  0.8f);
        float sfx    = PlayerPrefs.GetFloat(KEY_SFX,    1f);

        if (masterSlider != null) { masterSlider.value = master; masterSlider.onValueChanged.AddListener(OnMasterChanged); }
        if (musicSlider  != null) { musicSlider .value = music;  musicSlider .onValueChanged.AddListener(OnMusicChanged);  }
        if (sfxSlider    != null) { sfxSlider   .value = sfx;    sfxSlider   .onValueChanged.AddListener(OnSFXChanged);    }

        ApplyMaster(master);
        UpdateLabels();
    }

    void OnMasterChanged(float v)
    {
        PlayerPrefs.SetFloat(KEY_MASTER, v);
        ApplyMaster(v);
        UpdateLabels();
    }

    void OnMusicChanged(float v)
    {
        PlayerPrefs.SetFloat(KEY_MUSIC, v);
        UpdateLabels();
        // Müzik AudioSource bağlandığında burada uygulanacak
    }

    void OnSFXChanged(float v)
    {
        PlayerPrefs.SetFloat(KEY_SFX, v);
        UpdateLabels();
        // SFX mixer bağlandığında burada uygulanacak
    }

    void ApplyMaster(float v)
    {
        AudioListener.volume = v;
    }

    void UpdateLabels()
    {
        if (masterLabel != null && masterSlider != null) masterLabel.text = $"{Mathf.RoundToInt(masterSlider.value * 100)}%";
        if (musicLabel  != null && musicSlider  != null) musicLabel .text = $"{Mathf.RoundToInt(musicSlider .value * 100)}%";
        if (sfxLabel    != null && sfxSlider    != null) sfxLabel   .text = $"{Mathf.RoundToInt(sfxSlider   .value * 100)}%";
    }
}

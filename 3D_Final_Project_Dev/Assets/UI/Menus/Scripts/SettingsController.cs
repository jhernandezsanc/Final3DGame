using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    // ─── Panel References ─────────────────────────────────────────
    [Header("Panel References")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject pauseMenuPanel;

    // ─── Crosshair ────────────────────────────────────────────────
    [Header("Crosshair")]
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Toggle crosshairToggle;

    // ─── Audio ────────────────────────────────────────────────────
    [Header("Audio")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Audio Defaults")]
    [SerializeField][Range(0f, 1f)] private float defaultMasterVolume  = 1f;
    [SerializeField][Range(0f, 1f)] private float defaultMusicVolume   = 1f;
    [SerializeField][Range(0f, 1f)] private float defaultSFXVolume     = 1f;

    // ─── Display ──────────────────────────────────────────────────
    [Header("Display")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Image brightnessOverlay;

    [Header("Display Defaults")]
    [SerializeField][Range(0f, 1f)] private float defaultBrightness    = 1f;

    // ─── Gameplay ─────────────────────────────────────────────────
    [Header("Gameplay")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Toggle invertYToggle;

    [Header("Gameplay Defaults")]
    [SerializeField][Range(10f, 100f)] private float defaultSensitivity = 50f;
    [SerializeField] private bool defaultInvertY                         = false;
    [SerializeField] private bool defaultCrosshair                       = true;

    [Header("Sensitivity Slider Range")]
    [SerializeField] private float sensitivityMin = 10f;
    [SerializeField] private float sensitivityMax = 100f;

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    // ─── PlayerPrefs Keys ─────────────────────────────────────────
    private const string CROSSHAIR_PREF   = "CrosshairEnabled";
    private const string MASTER_VOL_PREF  = "MasterVolume";
    private const string MUSIC_VOL_PREF   = "MusicVolume";
    private const string SFX_VOL_PREF     = "SFXVolume";
    private const string BRIGHTNESS_PREF  = "Brightness";
    private const string SENSITIVITY_PREF = "Sensitivity";
    private const string INVERT_Y_PREF    = "InvertY";

    // ─── Statics (read by PlayerController) ───────────────────────
    public static float CurrentSensitivity { get; private set; } = 50f;
    public static float SFXVolume          { get; private set; } = 1f;
    public static bool  InvertY            { get; private set; } = false;

    // ─── Internal ─────────────────────────────────────────────────
    private bool _initialized  = false;
    private bool _initializing = false;

    // =============================================================
    // Lifecycle
    // =============================================================

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        ApplySliderRange(masterVolumeSlider, 0f, 1f);
        ApplySliderRange(sensitivitySlider, sensitivityMin, sensitivityMax);
        LoadAndApplyAll();
        RegisterListeners();
    }

    // =============================================================
    // Back Button
    // =============================================================

    public void OnBackClicked()
    {
        PlayerPrefs.Save();
        optionsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    // =============================================================
    // Initialization
    // =============================================================

    private void ApplySliderRange(Slider slider, float min, float max)
    {
        if (slider == null) return;
        slider.minValue = min;
        slider.maxValue = max;
    }

    private void LoadAndApplyAll()
    {
        _initializing = true;

        // Audio
        float masterVol = PlayerPrefs.GetFloat(MASTER_VOL_PREF, defaultMasterVolume);
        float musicVol  = PlayerPrefs.GetFloat(MUSIC_VOL_PREF,  defaultMusicVolume);
        float sfxVol    = PlayerPrefs.GetFloat(SFX_VOL_PREF,    defaultSFXVolume);

        SetSlider(masterVolumeSlider, masterVol);
        SetSlider(musicVolumeSlider,  musicVol);
        SetSlider(sfxVolumeSlider,    sfxVol);

        ApplyMasterVolume(masterVol);
        ApplyMusicVolume(musicVol);
        ApplySFXVolume(sfxVol);

        // Display
        float brightness = PlayerPrefs.GetFloat(BRIGHTNESS_PREF, defaultBrightness);

        SetSlider(brightnessSlider, brightness);
        ApplyBrightness(brightness);

        // Gameplay
        float sensitivity = PlayerPrefs.GetFloat(SENSITIVITY_PREF, defaultSensitivity);
        bool  invertY     = PlayerPrefs.GetInt(INVERT_Y_PREF, defaultInvertY ? 1 : 0) == 1;

        SetSlider(sensitivitySlider, sensitivity);
        SetToggle(invertYToggle,     invertY);

        ApplySensitivity(sensitivity);
        ApplyInvertY(invertY);

        // Crosshair
        bool crosshairOn = PlayerPrefs.GetInt(CROSSHAIR_PREF, defaultCrosshair ? 1 : 0) == 1;
        SetToggle(crosshairToggle, crosshairOn);
        SetCrosshair(crosshairOn);

        _initializing = false;
    }

    private void RegisterListeners()
    {
        masterVolumeSlider?.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        musicVolumeSlider?.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        sfxVolumeSlider?.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        brightnessSlider?.onValueChanged.RemoveListener(OnBrightnessChanged);
        sensitivitySlider?.onValueChanged.RemoveListener(OnSensitivityChanged);
        invertYToggle?.onValueChanged.RemoveListener(OnInvertYToggled);
        crosshairToggle?.onValueChanged.RemoveListener(OnCrosshairToggled);

        masterVolumeSlider?.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider?.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider?.onValueChanged.AddListener(OnSFXVolumeChanged);
        brightnessSlider?.onValueChanged.AddListener(OnBrightnessChanged);
        sensitivitySlider?.onValueChanged.AddListener(OnSensitivityChanged);
        invertYToggle?.onValueChanged.AddListener(OnInvertYToggled);
        crosshairToggle?.onValueChanged.AddListener(OnCrosshairToggled);
    }

    // =============================================================
    // Audio Handlers
    // =============================================================

    public void OnMasterVolumeChanged(float value)
    {
        if (_initializing) return;
        ApplyMasterVolume(value);
        PlayerPrefs.SetFloat(MASTER_VOL_PREF, value);
    }

    public void SyncMasterVolume(float value)
    {
        if (masterVolumeSlider == null) return;
        Debug.Log("[Settings] SyncMasterVolume called with value: " + value);
        masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        masterVolumeSlider.value = value;
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
    }

    public void OnMusicVolumeChanged(float value)
    {
        if (_initializing) return;
        ApplyMusicVolume(value);
        PlayerPrefs.SetFloat(MUSIC_VOL_PREF, value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        if (_initializing) return;
        ApplySFXVolume(value);
        PlayerPrefs.SetFloat(SFX_VOL_PREF, value);
    }

    private void ApplyMasterVolume(float value) => AudioListener.volume = value;

    private void ApplyMusicVolume(float value)
    {
        if (MusicHandler.musicInstance != null)
            MusicHandler.musicInstance.volume = value;
    }

    private void ApplySFXVolume(float value) => SFXVolume = value;

    // =============================================================
    // Display Handlers
    // =============================================================

    public void OnBrightnessChanged(float value)
    {
        if (_initializing) return;
        ApplyBrightness(value);
        PlayerPrefs.SetFloat(BRIGHTNESS_PREF, value);
    }

    private void ApplyBrightness(float value)
    {
        if (brightnessOverlay == null) return;
        float alpha = Mathf.Clamp01(1f - value);
        Color c = brightnessOverlay.color;
        c.a = alpha;
        brightnessOverlay.color = c;
    }

    // =============================================================
    // Gameplay Handlers
    // =============================================================

    public void OnSensitivityChanged(float value)
    {
        if (_initializing) return;
        ApplySensitivity(value);
        PlayerPrefs.SetFloat(SENSITIVITY_PREF, value);
    }

    public void OnInvertYToggled(bool isOn)
    {
        if (_initializing) return;
        ApplyInvertY(isOn);
        PlayerPrefs.SetInt(INVERT_Y_PREF, isOn ? 1 : 0);
    }

    private void ApplySensitivity(float value) => CurrentSensitivity = value;
    private void ApplyInvertY(bool isOn)        => InvertY = isOn;

    // =============================================================
    // Crosshair Handler
    // =============================================================

    public void OnCrosshairToggled(bool isOn)
    {
        if (_initializing) return;
        SetCrosshair(isOn);
        PlayerPrefs.SetInt(CROSSHAIR_PREF, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SetCrosshair(bool isOn)
    {
        if (crosshair != null)
            crosshair.SetActive(isOn);
        else
            PauseMenuController.Instance?.SetCrosshairVisible(isOn);
    }

    // =============================================================
    // Save / Reset
    // =============================================================

    public void SaveAllSettings()
    {
        PlayerPrefs.Save();
        Debug.Log("[Settings] All settings saved.");
    }

    public void ResetToDefaults()
    {
        PlayerPrefs.DeleteKey(MASTER_VOL_PREF);
        PlayerPrefs.DeleteKey(MUSIC_VOL_PREF);
        PlayerPrefs.DeleteKey(SFX_VOL_PREF);
        PlayerPrefs.DeleteKey(BRIGHTNESS_PREF);
        PlayerPrefs.DeleteKey(SENSITIVITY_PREF);
        PlayerPrefs.DeleteKey(INVERT_Y_PREF);
        PlayerPrefs.DeleteKey(CROSSHAIR_PREF);
        _initialized = false;
        Initialize();
        Debug.Log("[Settings] Reset to defaults.");
    }

    // =============================================================
    // Helpers
    // =============================================================

    private void SetSlider(Slider slider, float value)
    {
        if (slider == null) return;
        slider.value = value;
    }

    private void SetToggle(Toggle toggle, bool value)
    {
        if (toggle == null) return;
        toggle.isOn = value;
    }
}

public static class AudioSourceExtensions
{
    public static void SetVolumeIfExists(this AudioSource source, float volume)
    {
        if (source != null) source.volume = volume;
    }
}
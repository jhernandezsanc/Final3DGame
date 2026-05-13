using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject crosshairObject;
    [SerializeField] private Image menuBackdrop;

    [Header("Scene Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Level Complete Scenes")]
    [SerializeField] private Object currentLevelScene;
    [SerializeField] private Object levelSelectScene;

    [Header("Volume")]
    [SerializeField] private Slider volumeSlider;

    [Header("Audio")]
    [SerializeField] private AudioSource menuAudioSource;
    [SerializeField] private AudioClip buttonClickSound;
    [Range(0f, 1f)]
    [SerializeField] private float menuVolume = 1f;

    [Header("Save Toast")]
    [SerializeField] private CanvasGroup saveToast;
    [SerializeField] private float toastDuration = 2f;
    [SerializeField] private float fadeDuration  = 0.3f;

    [Header("Death Panel")]
    [SerializeField] private float deathPanelDuration = 3f;

    [Header("Backdrop")]
    [SerializeField][Range(0f, 1f)] private float backdropAlpha = 0.82f;

    public static PauseMenuController Instance { get; private set; }

    private bool _isPaused = false;
    private Coroutine _toastCoroutine;
    private Coroutine _deathCoroutine;

    private const string VOLUME_PREF = "MasterVolume";

    // ─────────────────────────────────────────────────────────────
    // Singleton
    // ─────────────────────────────────────────────────────────────

    void Awake()
    {
        Instance = this;
    }

    // ─────────────────────────────────────────────────────────────
    // Lifecycle
    // ─────────────────────────────────────────────────────────────

    void Start()
    {
        SetPauseState(false);

        if (menuAudioSource == null)
        {
            menuAudioSource = gameObject.AddComponent<AudioSource>();
            menuAudioSource.playOnAwake = false;
            menuAudioSource.spatialBlend = 0f;
        }

        InitVolumeSlider();

        if (saveToast != null)
        {
            saveToast.alpha = 0f;
            saveToast.gameObject.SetActive(false);
        }

        if (deathPanel != null)
            deathPanel.SetActive(false);

        SetBackdrop(false);

        if (crosshairObject != null)
            crosshairObject.SetActive(PlayerPrefs.GetInt("CrosshairEnabled", 1) == 1);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            HandleEscapePressed();

        if (Keyboard.current.tKey.wasPressedThisFrame && !_isPaused)
            OpenTutorial();
    }

    // ─────────────────────────────────────────────────────────────
    // Escape Handling
    // ─────────────────────────────────────────────────────────────

    private void HandleEscapePressed()
    {
        // Block ESC while death or level complete panels are showing
        if (deathPanel != null && deathPanel.activeSelf)
            return;

        if (levelCompletePanel != null && levelCompletePanel.activeSelf)
            return;

        if (tutorialPanel != null && tutorialPanel.activeSelf)
        {
            CloseTutorial();
            return;
        }

        if (optionsPanel != null && optionsPanel.activeSelf)
        {
            CloseOptions();
            return;
        }

        TogglePause();
    }

    // ─────────────────────────────────────────────────────────────
    // Backdrop
    // ─────────────────────────────────────────────────────────────

    private void SetBackdrop(bool visible)
    {
        if (menuBackdrop == null) return;
        Color c = menuBackdrop.color;
        c.a = visible ? backdropAlpha : 0f;
        menuBackdrop.color = c;
    }

    // ─────────────────────────────────────────────────────────────
    // Volume Slider
    // ─────────────────────────────────────────────────────────────

    private void InitVolumeSlider()
    {
        if (volumeSlider == null) return;

        float saved = PlayerPrefs.GetFloat(VOLUME_PREF, 1f);
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.value = saved;
        AudioListener.volume = saved;

        volumeSlider.onValueChanged.RemoveAllListeners();
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VOLUME_PREF, value);
        PlayerPrefs.Save();
    }

    // ─────────────────────────────────────────────────────────────
    // Audio
    // ─────────────────────────────────────────────────────────────

    private void PlayClick()
    {
        if (menuAudioSource == null || buttonClickSound == null) return;
        menuAudioSource.PlayOneShot(buttonClickSound, menuVolume);
    }

    // ─────────────────────────────────────────────────────────────
    // Pause State
    // ─────────────────────────────────────────────────────────────

    public void TogglePause()
    {
        SetPauseState(!_isPaused);
    }

    private void SetPauseState(bool paused)
    {
        _isPaused = paused;
        pauseMenuPanel.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;

        if (crosshairObject != null)
        {
            if (paused)
                crosshairObject.SetActive(false);
            else
                crosshairObject.SetActive(PlayerPrefs.GetInt("CrosshairEnabled", 1) == 1);
        }

        if (!paused && saveToast != null)
            StopToast();

        SetBackdrop(paused);
        PlayClick();
    }

    // ─────────────────────────────────────────────────────────────
    // Button Handlers
    // ─────────────────────────────────────────────────────────────

    public void OnResumeClicked()
    {
        PlayClick();
        SetPauseState(false);
    }

    public void OnSaveGameClicked()
    {
        PlayClick();
        SaveGame();
        ShowSaveToast();
    }

    public void OnLoadGameClicked()
    {
        PlayClick();

        if (!PlayerPrefs.HasKey("SavedScene"))
        {
            Debug.LogWarning("[PauseMenu] No save file found.");
            return;
        }

        LoadGame();
    }

    public void OnOptionsClicked()
    {
        PlayClick();
        if (optionsPanel == null) return;
        pauseMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        SetBackdrop(true);

        SettingsController sc = optionsPanel.GetComponent<SettingsController>();
        if (sc != null)
        {
            sc.Initialize();
            sc.SyncMasterVolume(AudioListener.volume);
        }
    }

    public void CloseOptions()
    {
        PlayClick();
        optionsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
        SetBackdrop(true);
    }

    public void OnQuitToMenuClicked()
    {
        PlayClick();
        StartCoroutine(QuitToMenuAfterSound());
    }

    // ─────────────────────────────────────────────────────────────
    // Back Button
    // ─────────────────────────────────────────────────────────────

    public void OnBackClicked()
    {
        PlayClick();

        if (tutorialPanel != null && tutorialPanel.activeSelf)
        {
            tutorialPanel.SetActive(false);
            pauseMenuPanel.SetActive(true);
            SetBackdrop(true);
            return;
        }

        if (optionsPanel != null && optionsPanel.activeSelf)
        {
            optionsPanel.SetActive(false);
            pauseMenuPanel.SetActive(true);
            SetBackdrop(true);
            return;
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Tutorial
    // ─────────────────────────────────────────────────────────────

    public void OpenTutorial()
    {
        if (tutorialPanel == null) return;
        PlayClick();
        pauseMenuPanel.SetActive(false);
        tutorialPanel.SetActive(true);
        SetBackdrop(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseTutorial()
    {
        if (tutorialPanel == null) return;
        PlayClick();
        tutorialPanel.SetActive(false);

        if (_isPaused)
        {
            pauseMenuPanel.SetActive(true);
            SetBackdrop(true);
        }
        else
        {
            SetBackdrop(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Death Panel
    // ─────────────────────────────────────────────────────────────

    public void ShowDeathPanel()
    {
        if (deathPanel == null) return;

        // Cancel any existing death panel coroutine
        if (_deathCoroutine != null)
            StopCoroutine(_deathCoroutine);

        _deathCoroutine = StartCoroutine(DeathPanelRoutine());
    }

    private IEnumerator DeathPanelRoutine()
    {
        deathPanel.SetActive(true);

        if (crosshairObject != null)
            crosshairObject.SetActive(false);

        yield return new WaitForSecondsRealtime(deathPanelDuration);

        deathPanel.SetActive(false);

        if (!_isPaused && crosshairObject != null)
            crosshairObject.SetActive(PlayerPrefs.GetInt("CrosshairEnabled", 1) == 1);

        _deathCoroutine = null;
    }

    // ─────────────────────────────────────────────────────────────
    // Level Complete
    // ─────────────────────────────────────────────────────────────

    public void ShowLevelComplete()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SetBackdrop(true);

        if (crosshairObject != null)
            crosshairObject.SetActive(false);

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);
    }

    public void OnRestartClicked()
    {
        PlayClick();
        Time.timeScale = 1f;

        #if UNITY_EDITOR
            if (currentLevelScene != null)
            {
                string path = UnityEditor.AssetDatabase.GetAssetPath(currentLevelScene);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
                SceneManager.LoadScene(sceneName);
                return;
            }
        #endif

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnLevelSelectClicked()
    {
        PlayClick();
        Time.timeScale = 1f;

        PlayerPrefs.SetInt("OpenLevelSelect", 1);
        PlayerPrefs.Save();

        #if UNITY_EDITOR
            if (levelSelectScene != null)
            {
                string path = UnityEditor.AssetDatabase.GetAssetPath(levelSelectScene);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
                SceneManager.LoadScene(sceneName);
                return;
            }
        #endif

        SceneManager.LoadScene(mainMenuSceneName);
    }

    // ─────────────────────────────────────────────────────────────
    // Save Toast
    // ─────────────────────────────────────────────────────────────

    private void ShowSaveToast()
    {
        if (saveToast == null) return;

        if (_toastCoroutine != null)
            StopCoroutine(_toastCoroutine);

        _toastCoroutine = StartCoroutine(ToastRoutine());
    }

    private void StopToast()
    {
        if (_toastCoroutine != null)
        {
            StopCoroutine(_toastCoroutine);
            _toastCoroutine = null;
        }

        saveToast.alpha = 0f;
        saveToast.gameObject.SetActive(false);
    }

    private IEnumerator ToastRoutine()
    {
        saveToast.gameObject.SetActive(true);
        saveToast.alpha = 0f;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            saveToast.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        saveToast.alpha = 1f;

        yield return new WaitForSecondsRealtime(toastDuration);

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            saveToast.alpha = Mathf.Clamp01(1f - (t / fadeDuration));
            yield return null;
        }

        saveToast.alpha = 0f;
        saveToast.gameObject.SetActive(false);
        _toastCoroutine = null;
    }

    // ─────────────────────────────────────────────────────────────
    // Crosshair
    // ─────────────────────────────────────────────────────────────

    public void SetCrosshairVisible(bool visible)
    {
        if (crosshairObject != null)
            crosshairObject.SetActive(visible);
    }

    // ─────────────────────────────────────────────────────────────
    // Save / Load
    // ─────────────────────────────────────────────────────────────

    private void SaveGame()
    {
        PlayerPrefs.SetString("SavedScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("SavedPosX", Camera.main.transform.position.x);
        PlayerPrefs.SetFloat("SavedPosY", Camera.main.transform.position.y);
        PlayerPrefs.SetFloat("SavedPosZ", Camera.main.transform.position.z);
        PlayerPrefs.Save();
        Debug.Log("[PauseMenu] Game saved.");
    }

    private void LoadGame()
    {
        Time.timeScale = 1f;
        string savedScene = PlayerPrefs.GetString("SavedScene");
        SceneManager.LoadScene(savedScene);
    }

    // ─────────────────────────────────────────────────────────────
    // Coroutines
    // ─────────────────────────────────────────────────────────────

    private IEnumerator QuitToMenuAfterSound()
    {
        float delay = buttonClickSound != null ? buttonClickSound.length : 0.15f;
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // ─────────────────────────────────────────────────────────────
    // Cleanup
    // ─────────────────────────────────────────────────────────────

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
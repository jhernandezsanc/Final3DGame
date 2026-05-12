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
    [SerializeField] private GameObject crosshairObject;
    [SerializeField] private Image menuBackdrop;

    [Header("Scene Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

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

    [Header("Backdrop")]
    [SerializeField][Range(0f, 1f)] private float backdropAlpha = 0.82f;

    private bool _isPaused = false;
    private Coroutine _toastCoroutine;

    private const string VOLUME_PREF = "MasterVolume";

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

        SetBackdrop(false);
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
        optionsPanel.GetComponent<SettingsController>()?.Initialize();
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
    // Level Complete
    // ─────────────────────────────────────────────────────────────

    public void ShowLevelComplete()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SetBackdrop(true);

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        StartCoroutine(LevelCompleteRoutine());
    }

    private IEnumerator LevelCompleteRoutine()
    {
        yield return new WaitForSecondsRealtime(10f);

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);

        SetBackdrop(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
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

    [Header("Scene Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Volume")]
    [SerializeField] private Slider volumeSlider;

    [Header("Audio")]
    [SerializeField] private AudioSource menuAudioSource;
    [SerializeField] private AudioClip buttonClickSound;
    [Range(0f, 1f)]
    [SerializeField] private float menuVolume = 1f;

    private bool _isPaused = false;

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
        // Close whatever is open, in priority order
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

        // Nothing else open — toggle the pause menu itself
        TogglePause();
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
        optionsPanel.GetComponent<SettingsController>()?.Initialize();
    }

    public void CloseOptions()
    {
        PlayClick();
        optionsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    public void OnQuitToMenuClicked()
    {
        PlayClick();
        StartCoroutine(QuitToMenuAfterSound());
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseTutorial()
    {
        if (tutorialPanel == null) return;
        PlayClick();
        tutorialPanel.SetActive(false);

        if (_isPaused)
            pauseMenuPanel.SetActive(true);
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
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

    private System.Collections.IEnumerator QuitToMenuAfterSound()
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
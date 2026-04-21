using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenuController : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject tutorialPanel;

    [Header("Tab Content")]
    [SerializeField] private GameObject menuTab;
    [SerializeField] private GameObject settingsTab;
    [SerializeField] private GameObject controlsTab;

    [Header("Scene Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Audio")]
    [SerializeField] private AudioSource menuAudioSource;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip resumeSound;
    [SerializeField] private AudioClip saveSound;
    [SerializeField] private AudioClip loadSound;
    [SerializeField] private AudioClip tabSwitchSound;
    [SerializeField] private AudioClip openMenuSound;
    [SerializeField] private AudioClip closeMenuSound;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip quitSound;
    [Range(0f, 1f)]
    [SerializeField] private float menuVolume = 1f;

    private bool _isPaused = false;

    // ─── Lifecycle ────────────────────────────────────────────────

    void Start()
    {
        SetPauseState(false);

        // If no AudioSource assigned, add one automatically
        if (menuAudioSource == null)
        {
            menuAudioSource = gameObject.AddComponent<AudioSource>();
            menuAudioSource.playOnAwake = false;
            menuAudioSource.spatialBlend = 0f; // 2D sound, no 3D falloff
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();

        if (Input.GetKeyDown(KeyCode.T) && !_isPaused)
            OpenTutorial();
    }

    // ─── Audio ────────────────────────────────────────────────────

    private void PlaySound(AudioClip clip)
    {
        if (menuAudioSource == null || clip == null) return;
        menuAudioSource.PlayOneShot(clip, menuVolume);
    }

    // ─── Pause State ──────────────────────────────────────────────

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

        if (paused)
        {
            PlaySound(openMenuSound);
            ShowTab(menuTab);
        }
        else
        {
            PlaySound(closeMenuSound);
        }
    }

    // ─── Button Handlers ──────────────────────────────────────────

    public void OnResumeClicked()
    {
        PlaySound(resumeSound != null ? resumeSound : clickSound);
        SetPauseState(false);
    }

    public void OnSaveGameClicked()
    {
        PlaySound(saveSound != null ? saveSound : clickSound);
        SaveGame();
    }

    public void OnLoadGameClicked()
    {
        if (!PlayerPrefs.HasKey("SavedScene"))
        {
            PlaySound(errorSound != null ? errorSound : clickSound);
            Debug.LogWarning("[PauseMenu] No save file found.");
            return;
        }

        PlaySound(loadSound != null ? loadSound : clickSound);
        LoadGame();
    }

    public void OnOptionsClicked()
    {
        PlaySound(tabSwitchSound != null ? tabSwitchSound : clickSound);
        ShowTab(settingsTab);
    }

    public void OnQuitToMenuClicked()
    {
        PlaySound(quitSound != null ? quitSound : clickSound);
        // Small delay so sound plays before scene unloads
        StartCoroutine(QuitToMenuAfterSound());
    }

    public void OnQuitToDesktopClicked()
    {
        PlaySound(quitSound != null ? quitSound : clickSound);
        StartCoroutine(QuitToDesktopAfterSound());
    }

    // ─── Tab Switching ────────────────────────────────────────────

    public void ShowMenuTab()
    {
        PlaySound(tabSwitchSound != null ? tabSwitchSound : clickSound);
        ShowTab(menuTab);
    }

    public void ShowSettingsTab()
    {
        PlaySound(tabSwitchSound != null ? tabSwitchSound : clickSound);
        ShowTab(settingsTab);
    }

    public void ShowControlsTab()
    {
        PlaySound(tabSwitchSound != null ? tabSwitchSound : clickSound);
        ShowTab(controlsTab);
    }

    private void ShowTab(GameObject targetTab)
    {
        menuTab?.SetActive(false);
        settingsTab?.SetActive(false);
        controlsTab?.SetActive(false);
        targetTab?.SetActive(true);
    }

    // ─── Tutorial ─────────────────────────────────────────────────

    public void OpenTutorial()
    {
        if (tutorialPanel == null) return;
        PlaySound(openMenuSound != null ? openMenuSound : clickSound);
        tutorialPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseTutorial()
    {
        if (tutorialPanel == null) return;
        PlaySound(closeMenuSound != null ? closeMenuSound : clickSound);
        tutorialPanel.SetActive(false);
        if (!_isPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // ─── Save / Load ──────────────────────────────────────────────

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

    // ─── Coroutines ───────────────────────────────────────────────

    private System.Collections.IEnumerator QuitToMenuAfterSound()
    {
        // Wait for clip length, fall back to 0.15s if no clip assigned
        float delay = quitSound != null ? quitSound.length : 0.15f;
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private System.Collections.IEnumerator QuitToDesktopAfterSound()
    {
        float delay = quitSound != null ? quitSound.length : 0.15f;
        yield return new WaitForSecondsRealtime(delay);
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // ─── Cleanup ──────────────────────────────────────────────────

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
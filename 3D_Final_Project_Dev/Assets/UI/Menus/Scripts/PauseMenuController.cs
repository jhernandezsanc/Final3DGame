using UnityEngine;
using UnityEngine.SceneManagement;
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

    private bool _isPaused = false;

    // ─── Lifecycle ────────────────────────────────────────────────

    void Start()
    {
        // Make sure menu is hidden on scene load
        SetPauseState(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();

        if (Input.GetKeyDown(KeyCode.T) && !_isPaused)
            OpenTutorial();
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
            ShowTab(menuTab); // Always open to main menu tab when pausing
    }

    // ─── Button Handlers ──────────────────────────────────────────

    public void OnResumeClicked()
    {
        SetPauseState(false);
    }

    public void OnSaveGameClicked()
    {
        SaveGame();
    }

    public void OnLoadGameClicked()
    {
        LoadGame();
    }

    public void OnOptionsClicked()
    {
        ShowTab(settingsTab);
    }

    public void OnQuitToMenuClicked()
    {
        // Unpause before scene transition or TimeScale stays 0 in next scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnQuitToDesktopClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // ─── Tab Switching ────────────────────────────────────────────

    public void ShowMenuTab()     => ShowTab(menuTab);
    public void ShowSettingsTab() => ShowTab(settingsTab);
    public void ShowControlsTab() => ShowTab(controlsTab);

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
        tutorialPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseTutorial()
    {
        if (tutorialPanel == null) return;
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
        // PlayerPrefs-based save — swap this out for your save system later
        PlayerPrefs.SetString("SavedScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("SavedPosX", Camera.main.transform.position.x);
        PlayerPrefs.SetFloat("SavedPosY", Camera.main.transform.position.y);
        PlayerPrefs.SetFloat("SavedPosZ", Camera.main.transform.position.z);
        PlayerPrefs.Save();
        Debug.Log("[PauseMenu] Game saved.");
    }

    private void LoadGame()
    {
        if (!PlayerPrefs.HasKey("SavedScene"))
        {
            Debug.LogWarning("[PauseMenu] No save file found.");
            return;
        }

        Time.timeScale = 1f;
        string savedScene = PlayerPrefs.GetString("SavedScene");
        SceneManager.LoadScene(savedScene);
    }

    // ─── Cleanup ──────────────────────────────────────────────────

    void OnDestroy()
    {
        // Safety net — always restore TimeScale if this object is destroyed
        // e.g. if scene changes while paused
        Time.timeScale = 1f;
    }
}
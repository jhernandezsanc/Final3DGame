using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject optionPanel;

    [Header("Scene References")]
    [SerializeField] private Object level1Scene;
    [SerializeField] private Object level2Scene;

    [Header("Audio")]
    [SerializeField] private AudioSource menuAudioSource;
    [SerializeField] private AudioClip buttonClickSound;
    [Range(0f, 1f)]
    [SerializeField] private float menuVolume = 1f;

    // ─────────────────────────────────────────────────────────────
    // Lifecycle
    // ─────────────────────────────────────────────────────────────

    void Start()
    {
        if (mainMenuPanel != null)    mainMenuPanel.SetActive(true);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        if (optionPanel != null)      optionPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (menuAudioSource == null)
        {
            menuAudioSource = gameObject.AddComponent<AudioSource>();
            menuAudioSource.playOnAwake = false;
            menuAudioSource.spatialBlend = 0f;
        }

        // Jump straight to level select if flagged from level complete
        if (PlayerPrefs.GetInt("OpenLevelSelect", 0) == 1)
        {
            PlayerPrefs.DeleteKey("OpenLevelSelect");
            PlayerPrefs.Save();
            mainMenuPanel.SetActive(false);
            levelSelectPanel.SetActive(true);
        }
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (optionPanel != null && optionPanel.activeSelf)
            {
                OnOptionsBackClicked();
                return;
            }

            if (levelSelectPanel != null && levelSelectPanel.activeSelf)
            {
                OnLevelSelectBackClicked();
                return;
            }
        }
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
    // Scene Loading
    // ─────────────────────────────────────────────────────────────

    private void LoadScene(Object sceneObject)
    {
        if (sceneObject == null)
        {
            Debug.LogWarning("[MainMenu] No scene assigned.");
            return;
        }

        #if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(sceneObject);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
            StartCoroutine(LoadAfterSound(sceneName));
        #else
            StartCoroutine(LoadAfterSound(sceneObject.name));
        #endif
    }

    private System.Collections.IEnumerator LoadAfterSound(string sceneName)
    {
        float delay = buttonClickSound != null ? buttonClickSound.length : 0.15f;
        yield return new WaitForSecondsRealtime(delay);
        SceneManager.LoadScene(sceneName);
    }

    // ─────────────────────────────────────────────────────────────
    // Main Menu Buttons
    // ─────────────────────────────────────────────────────────────

    public void OnStartGameClicked()
    {
        PlayClick();
        LoadScene(level1Scene);
    }

    public void OnLevelSelectClicked()
    {
        PlayClick();
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    public void OnOptionsClicked()
    {
        PlayClick();
        mainMenuPanel.SetActive(false);
        optionPanel.SetActive(true);

        SettingsController sc = optionPanel.GetComponent<SettingsController>();
        if (sc != null)
        {
            sc.Initialize();
            sc.SyncMasterVolume(AudioListener.volume);
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Level Select Buttons
    // ─────────────────────────────────────────────────────────────

    public void OnLevel1Clicked()
    {
        PlayClick();
        LoadScene(level1Scene);
    }

    public void OnLevel2Clicked()
    {
        PlayClick();
        LoadScene(level2Scene);
    }

    public void OnLevelSelectBackClicked()
    {
        PlayClick();
        levelSelectPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // ─────────────────────────────────────────────────────────────
    // Options Back Button
    // ─────────────────────────────────────────────────────────────

    public void OnOptionsBackClicked()
    {
        PlayClick();
        optionPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
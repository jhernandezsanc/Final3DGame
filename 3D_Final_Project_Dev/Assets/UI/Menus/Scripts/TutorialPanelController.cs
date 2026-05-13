using UnityEngine;
using UnityEngine.UI;

public class TutorialPanelController : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject pauseMenuPanel;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip backButtonSound;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    // ─────────────────────────────────────────────────────────────
    // Lifecycle
    // ─────────────────────────────────────────────────────────────

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
    }

    void Update()
    {
        // Allow ESC or T to also close tutorial while it's open
        if (tutorialPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.T))
                OnBackClicked();
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Button Handler
    // ─────────────────────────────────────────────────────────────

    public void OnBackClicked()
    {
        PlaySound();
        StartCoroutine(BackAfterSound());
    }

    // ─────────────────────────────────────────────────────────────
    // Transition
    // ─────────────────────────────────────────────────────────────

    private System.Collections.IEnumerator BackAfterSound()
    {
        // Wait for sound to finish before switching panels
        // Falls back to 0f if no clip assigned so there's no delay
        float delay = (backButtonSound != null) ? backButtonSound.length : 0f;
        yield return new WaitForSecondsRealtime(delay);

        tutorialPanel.SetActive(false);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
    }

    // ─────────────────────────────────────────────────────────────
    // Audio
    // ─────────────────────────────────────────────────────────────

    private void PlaySound()
    {
        if (audioSource == null || backButtonSound == null) return;
        audioSource.PlayOneShot(backButtonSound, SettingsController.SFXVolume);
    }
}
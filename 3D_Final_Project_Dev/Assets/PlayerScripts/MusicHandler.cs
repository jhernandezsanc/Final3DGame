using UnityEngine;
using System.Collections;

public class MusicHandler : MonoBehaviour
{
    public static MusicHandler musicInstance;
    [SerializeField] private AudioClip[] musicFiles;
    [SerializeField ]private AudioSource musicPlayer;

    public float volume = 1f;
    public float fadeInTime = 5f;
    public bool isPaused = false;
    private bool isFading;

    public int trackNumber = 0;
    private int trackNumberMax;
    private AudioClip currentTrack;
    void Awake()
    {
        if (musicInstance == null)
        {
            musicInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        trackNumberMax = musicFiles.Length;
        StartCoroutine(FadeIn());

    }

    IEnumerator FadeIn()
    {
        float currentTime = 0;
        musicPlayer.volume = 0f;
        while (currentTime < fadeInTime)
        {
            currentTime += Time.deltaTime;
            musicPlayer.volume = Mathf.Lerp(0, volume, currentTime / fadeInTime);
            //musicPlayer.volume = Mathf.MoveTowards(musicPlayer.volume, volume, 0.02f*Time.deltaTime);
            yield return null;
            isFading = true;
        }
        musicPlayer.volume = volume;
        isFading = false;
    }

    void Update()
    {

        if (!isFading) { musicPlayer.volume = volume; }

        if (!isPaused && !musicPlayer.isPlaying)
        {
            currentTrack = musicFiles[trackNumber]; //logic to cycle through tracks
            trackNumber++;
            if (trackNumber == trackNumberMax) { trackNumber = 0; }

            musicPlayer.clip = currentTrack;
            musicPlayer.Play();
        }
    }
}

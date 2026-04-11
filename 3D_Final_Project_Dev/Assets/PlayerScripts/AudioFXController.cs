using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioFXController : MonoBehaviour
{
    public static AudioFXController instance;

    [SerializeField] private AudioSource audioFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    //audioclips called default to 100% volume at player position

    // call using "AudioFXController.instance.PlaySoundFXClip();"
    // assign audioclips to scripts with "[SerializeField] private AudioClip <...name>;"
    public void PlayAudioFXClip(AudioClip audioClip, Transform spawnTransform = null, float volume = 1f)
    {
        //function default to play sound at player position
        if (spawnTransform == null) spawnTransform = transform;

        //spawn in game object at given location
        AudioSource audioSource = Instantiate(audioFXObject, spawnTransform.position, Quaternion.identity);

        //assign audioClip 
        audioSource.clip = audioClip;

        //assign volume
        audioSource.volume = volume;

        //play sound 
        audioSource.Play();

        //get length of sound FX clip 
        float clipLength = audioSource.clip.length;

        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLength);

    }

    // call using "AudioFXController.instance.PlayRandSoundFXClip();"
    // assign audioclips to scripts with "[SerializeField] private AudioClip[] <...name>;"
    public void PlayRandAudioFXClip(AudioClip[] audioClip, Transform spawnTransform = null, float volume = 1f)
    {
        //function defaults to play sound at player position
        if (spawnTransform == null) spawnTransform = transform;

        //assign random 
        int rand = Random.Range(0, audioClip.Length);

        //spawn in game object at given location
        AudioSource audioSource = Instantiate(audioFXObject, spawnTransform.position, Quaternion.identity);

        //assign audioClip 
        audioSource.clip = audioClip[rand];

        //assign volume
        audioSource.volume = volume;

        //play sound 
        audioSource.Play();

        //get length of sound FX clip 
        float clipLength = audioSource.clip.length;

        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLength);

    }

}

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ShotgunScript : MonoBehaviour
{
    [SerializeField] GameObject reticle;
    [SerializeField] private AudioClip[] shotgunSounds;
    public float shotgunVolume = 1f;

    HurtBoxToggles[] hurtBoxes;

    public float damageWindow = 0.05f;
    public float recoil = 5f;
    public Animator animator;
    public float animationSpeed = 1f;

    bool hasFired = false;
    public void Start()
    {
        hurtBoxes = GetComponentsInChildren<HurtBoxToggles>(true); //finds disabled objects on start

    }

    public void Update()
{
    if (Time.timeScale == 0f) return;
    
    if (Mouse.current.leftButton.wasPressedThisFrame && !hasFired)
    {
        Shoot();
    }
}

    public void Shoot()
    {
        hasFired = true;

        //passes through reticle position to determine recoil
        Vector3 reticleLocation = reticle.transform.position;
        RecoilController.Instance.Recoil(reticleLocation, recoil);

        foreach(HurtBoxToggles hurtBox in hurtBoxes)
        {
            hurtBox.ToggleHurtBoxOn();
        }
        AudioFXController.instance.PlayRandAudioFXClip(shotgunSounds, null, shotgunVolume);
        Invoke("ReLoad", damageWindow);
        StopAllCoroutines();
        StartCoroutine(PlayCycle());
    }

    public void ReLoad()
    {
        //hasFired = false;

        foreach(HurtBoxToggles hurtBox in hurtBoxes)
        {
            hurtBox.ToggleHurtBoxOff();
        }
    }

    IEnumerator PlayCycle()
    {
        animator.enabled = true;
        animator.speed = animationSpeed;
        yield return null;
        var state = animator.GetCurrentAnimatorStateInfo(0);
        float length = state.length / Mathf.Max(animator.speed, 0.0001f);

        const float endBuffer = 0.03f;
        yield return new WaitForSeconds(Mathf.Max(0f, length - endBuffer)); //working to prevent animation overlap

        animator.Play(0, -1, 0f); //resets animation
        animator.Update(0f);
        animator.enabled = false;
        hasFired = false; //ties reload to animation speed
    }
    
}

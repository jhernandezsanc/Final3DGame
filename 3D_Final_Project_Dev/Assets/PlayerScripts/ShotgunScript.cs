using UnityEngine;
using UnityEngine.InputSystem;

public class ShotgunScript : MonoBehaviour
{
    [SerializeField] GameObject reticle;

    HurtBoxToggles[] hurtBoxes;

    public float damageWindow = 0.05f;
    public float recoil = 5f;

    bool hasFired = false;
    public void Start()
    {
        hurtBoxes = GetComponentsInChildren<HurtBoxToggles>(true); //finds disabled objects on start

    }

    public void Update()
    {
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
        Invoke("ReLoad", damageWindow);
    }

    public void ReLoad()
    {
        hasFired = false;

        foreach(HurtBoxToggles hurtBox in hurtBoxes)
        {
            hurtBox.ToggleHurtBoxOff();
        }
    }
    
}

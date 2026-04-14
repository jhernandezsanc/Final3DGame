using UnityEngine;
using UnityEngine.InputSystem;

public class ShotgunScript : MonoBehaviour
{
    [SerializeField] GameObject reticle;
    public float recoil = 5f;
    public void Start()
    {
        
    }

    public void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 reticleLocation = reticle.transform.position;
            RecoilController.Instance.Recoil(reticleLocation, recoil);
        }
    }
}

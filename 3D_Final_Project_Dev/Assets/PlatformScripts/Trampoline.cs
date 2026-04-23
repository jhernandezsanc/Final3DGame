using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [Header("Bounciness")]
    public float bounceStrength = 500f; 
    
    // This helps the Recoil logic push the player "out" from the surface
    public float forceOffset = 1.0f; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Calculate a 'virtual' position behind the trampoline
            // This ensures the recoil direction is the trampoline's forward/up face
            Vector3 virtualForcePos = transform.position - (transform.up * forceOffset);

            // 2. Call your Singleton RecoilController
            if (RecoilController.Instance != null)
            {
                RecoilController.Instance.Recoil(virtualForcePos, bounceStrength);
            }
        }
    }

    // Visual aid to see where the "push" is coming from
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Vector3 virtualForcePos = transform.position - (transform.up * forceOffset);
        Gizmos.DrawSphere(virtualForcePos, 0.2f);
        Gizmos.DrawLine(virtualForcePos, transform.position);
    }
}
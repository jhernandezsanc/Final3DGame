using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float bounceStrength = 200f;

    private void OnTriggerEnter(Collider other)
    {
        // Make sure the player hit the trampoline
        if (!other.CompareTag("Player")) return;

        // Get the player's RecoilController
        RecoilController recoil = other.GetComponent<RecoilController>();
        if (recoil == null) return;

        /*
         * transform.up = the direction of the TOP face of the trampoline
         * This automatically works even if the trampoline is tilted/rotated
         */

        Vector3 launchDirection = transform.up;

        /*
         * Recoil(forcePos) pushes AWAY from forcePos:
         *
         * accelDir = playerPos - forcePos
         *
         * So to launch the player upward along transform.up,
         * we fake a force position slightly BELOW the trampoline.
         */

        Vector3 fakeForcePosition = other.transform.position - launchDirection;

        recoil.Recoil(fakeForcePosition, bounceStrength);
    }
}
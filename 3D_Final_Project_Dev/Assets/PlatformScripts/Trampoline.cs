using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float bounceStrength = 15f;

    private void OnTriggerEnter(Collider other)
    {
        // Only react to player
        if (!other.CompareTag("Player")) return;

        // Optional safety check: ensure it's coming from above
        if (other.transform.position.y < transform.position.y) return;

        Vector3 forcePos = transform.position;

        // Optional outward push based on contact direction
        Vector3 outward = other.transform.position - transform.position;
        outward.y = 0f;

        Vector3 adjustedForcePos = forcePos - outward * 0.2f;

        RecoilController.Instance.Recoil(adjustedForcePos, bounceStrength);
    }
}
using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    [Header("Chase Settings")]
    public Transform player;
    public float moveSpeed = 2f;
    public float stopDistance = 1.5f;

    [Header("Explosion Settings")]
    public float explodeDistance = 2f;
    public float explosionRadius = 5f;
    public float knockbackForce = 15f;
    public AudioClip explosionSound;

    private bool hasExploded = false;

    /* This will automatically find the player if not assigned in the inspector. Remove if assign the player manually.
    */
    private void Start() {
    if (player == null) {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        }
    }
    private void Update() {
        if (player == null || hasExploded) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // EXPLODE if close enough
        if (distance <= explodeDistance) {
            Explode();
            return;
        }

        // CHASE player if not close enough
        if (distance > stopDistance) {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Optional: face player
            direction.y = 0f;
            if (direction != Vector3.zero) {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    void Explode() {
        hasExploded = true;

        // Play explosion sound
        if (explosionSound != null) {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        // Check everything nearby
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits) {
            PlayerMovement playerMovement = hit.GetComponent<PlayerMovement>();

            if (playerMovement != null) {
                Vector3 pushDirection = playerMovement.transform.position - transform.position;
                pushDirection.y = 0.5f;

                playerMovement.ApplyKnockback(pushDirection, knockbackForce);
            }
        }

        Destroy(gameObject);
    }
}
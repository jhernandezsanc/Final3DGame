using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public float stopDistance = 1.5f;

    /* This will automatically find the player if not assigned in the inspector. You can remove this if you prefer to assign the player manually.
    private void Start() {
    if (player == null) {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        }
    }
    */
    private void Update() {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

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
}
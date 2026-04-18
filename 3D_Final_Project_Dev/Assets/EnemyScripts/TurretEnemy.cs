using UnityEngine;

public class TurretEnemy : MonoBehaviour
{
    public Transform player;
    public Transform firePoint;
    public GameObject rocketPrefab;

    public float shootRange = 15f;
    public float fireRate = 2f;

    private float fireTimer;

    private void Start() {
        if (player == null) {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void Update() {
        if (player == null) return;

        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;

        if (lookDir != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }

        float distance = Vector3.Distance(transform.position, player.position);

        fireTimer -= Time.deltaTime;

        if (distance <= shootRange && fireTimer <= 0f) {
            Shoot();
            fireTimer = fireRate;
        }
    }

    void Shoot() {
        if (rocketPrefab == null || firePoint == null) return;

        GameObject rocket = Instantiate(rocketPrefab, firePoint.position, firePoint.rotation);

        Rocket rocketScript = rocket.GetComponent<Rocket>();
        if (rocketScript != null) {
            rocketScript.SetTarget(player);
        }
    }
}
using UnityEngine;

public class TurretEnemy : MonoBehaviour
{
    public Transform player;
    public Transform turretHead;
    public Transform firePoint;
    public GameObject rocketPrefab;

    public float shootRange = 15f;
    public float fireRate = 2f;
    public float rocketForce = 20f;
    public float scanSpeed = 40f;
    public float turnSpeed = 5f;

    private float fireCooldown;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (turretHead == null)
        {
            Debug.LogError("TurretHead is NOT assigned!");
            return;
        }

        if (player == null) return;

        float distance = Vector3.Distance(turretHead.position, player.position);

        if (distance <= shootRange)
        {
            AimAtPlayer();

            if (fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = 1f / fireRate;
            }
        }
        else
        {
            ScanArea();
        }

        fireCooldown -= Time.deltaTime;
    }

    void AimAtPlayer()
    {
        Vector3 direction = player.position - turretHead.position;
        direction.y = 0f;

        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        turretHead.rotation = Quaternion.Slerp(
            turretHead.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    void ScanArea()
    {
        turretHead.Rotate(0f, scanSpeed * Time.deltaTime, 0f);
    }

    void Shoot()
    {
        GameObject rocket = Instantiate(
            rocketPrefab,
            firePoint.position,
            firePoint.rotation
        );

        Rigidbody rb = rocket.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * rocketForce;
        }
    }
}
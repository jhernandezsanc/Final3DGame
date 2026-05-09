using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.right;
    public float moveDistance = 5f;
    public float speed = 2f;

    [Header("Detection Settings")]
    public Vector3 detectionOffset = new Vector3(0, 0.5f, 0);
    public Vector3 detectionSize = new Vector3(10f, 0.5f, 10f);
    public ParticleSystem thrusterFX;

    private Vector3 startPos;
    private Vector3 lastPos;
    private Vector3 platformDelta;

    private bool playerOnPlatform;
    private Vector3 lastVelocity;
    private float speedValue;

    void Start()
    {
        startPos = transform.position;
        lastPos = transform.position;
    }

    void Update()
    {
        // 1. Move platform
        float cycle = Time.time * speed;

        float sine = Mathf.Sin(cycle);
        float t = (sine + 1f) * 0.5f;

        Vector3 targetPos = startPos + moveDirection.normalized * (t * moveDistance);
        transform.position = targetPos;

        // 2. Calculate delta movement
        platformDelta = transform.position - lastPos;
        lastPos = transform.position;

        // 3. Check if player is on platform
        playerOnPlatform = CheckForPlayer();

        // 4. Apply movement INTO MasterMovement BEFORE final move
        if (playerOnPlatform)
        {
            MasterMovement.Instance.frameDisplacement += platformDelta;
        }

        //thrusterFX scripts
        Vector3 velocity = (transform.position - lastPos) / Time.deltaTime;
        speedValue = velocity.magnitude;

        if (thrusterFX != null)
        {
            var emission = thrusterFX.emission;
            emission.rateOverTime = Mathf.Lerp(0f, 60f, speedValue / 3f);

            if (velocity.sqrMagnitude > 0.0001f)
            {
                thrusterFX.transform.rotation = Quaternion.LookRotation(-velocity.normalized);
            }
        }
        lastPos = transform.position;
    }

    bool CheckForPlayer()
    {
        Collider[] hits = Physics.OverlapBox(
            transform.position + detectionOffset,
            detectionSize / 2,
            Quaternion.identity
        );

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + detectionOffset, detectionSize);
    }
}
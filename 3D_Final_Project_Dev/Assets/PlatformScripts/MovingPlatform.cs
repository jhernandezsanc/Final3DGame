using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.right;
    public float moveDistance = 5f;
    public float speed = 2f;

    [Header("Detection Settings")]
    public Vector3 detectionOffset = new Vector3(0, 1.1f, 0);
    public Vector3 detectionSize = new Vector3(2f, 1f, 2f);

    private Vector3 startPos;
    private Vector3 lastPos;
    private Vector3 platformDelta;

    private bool playerOnPlatform;

    void Start()
    {
        startPos = transform.position;
        lastPos = transform.position;
    }

    void Update()
    {
        // 1. Move platform
        float movement = Mathf.PingPong(Time.time * speed, moveDistance);
        transform.position = startPos + moveDirection.normalized * movement;

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
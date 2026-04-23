using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.right;
    public float moveDistance = 5f;
    public float speed = 2f;

    [Header("Detection Settings")]
    public Vector3 detectionOffset = new Vector3(0, 1.1f, 0); // Positioned above the platform
    public Vector3 detectionSize = new Vector3(1f, 0.5f, 1f); 

    private Vector3 startPos;
    private Vector3 lastPos;
    private Vector3 platformDelta;
    private CharacterController playerController;

    void Start()
    {
        startPos = transform.position;
        lastPos = transform.position;
    }

    void Update()
    {
        // 1. Calculate Platform Movement
        float movement = Mathf.PingPong(Time.time * speed, moveDistance);
        transform.position = startPos + moveDirection.normalized * movement;

        // 2. Calculate the change in position (Delta)
        platformDelta = transform.position - lastPos;
        lastPos = transform.position;

        // 3. Find the player using their Tag
        FindPlayerViaTag();
    }

    void LateUpdate()
    {
        // 4. If the player is on us, move them by the delta
        if (playerController != null)
        {
            playerController.Move(platformDelta);
        }
    }

    void FindPlayerViaTag()
    {
        // This looks at every collider inside the box
        Collider[] hitColliders = Physics.OverlapBox(transform.position + detectionOffset, detectionSize / 2);
        
        bool foundPlayer = false;

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                playerController = hit.GetComponent<CharacterController>();
                foundPlayer = true;
                break; // Stop looking once we find the player
            }
        }

        if (!foundPlayer)
        {
            playerController = null;
        }
    }

    // This helps you see the detection zone in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + detectionOffset, detectionSize);
    }
}
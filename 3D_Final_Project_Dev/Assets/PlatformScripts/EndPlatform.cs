using UnityEngine;

public class EndPlatform : MonoBehaviour
{
    public float timeToWin = 3f;
    public Vector3 winRespawnPoint = new Vector3(0, 10, 0);

    [Header("Detection Settings")]
    public Vector3 detectionOffset = new Vector3(0, 1.1f, 0); 
    public Vector3 detectionSize = new Vector3(2f, 1f, 2f);

    private float timer = 0f;
    private PlayerMovement playerScript;

    void Update()
    {
        // 1. Check if the player is physically in the zone above this platform
        Collider[] hitColliders = Physics.OverlapBox(transform.position + detectionOffset, detectionSize / 2);
        
        bool playerIsOnMe = false;

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                // Cache the script so we don't have to look it up every frame
                if (playerScript == null) 
                    playerScript = hit.GetComponent<PlayerMovement>();
                
                playerIsOnMe = true;
                break;
            }
        }

        // 2. The Logic: Must be in the zone AND the script must report isGrounded = true
        if (playerIsOnMe && playerScript != null && playerScript.isGrounded)
        {
            timer += Time.deltaTime;

            if (timer >= timeToWin)
            {
                TeleportPlayer();
            }
        }
        else
        {
            // If they jump (isGrounded becomes false) or leave the zone, the timer resets
            timer = 0f;
        }
    }

    private void TeleportPlayer()
    {
        timer = 0f;
        CharacterController cc = playerScript.GetComponent<CharacterController>();
        
        if (cc != null)
        {
            cc.enabled = false;
            playerScript.transform.position = winRespawnPoint;
            cc.enabled = true;
        }
    }

    // Visual aid in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + detectionOffset, detectionSize);
    }
}
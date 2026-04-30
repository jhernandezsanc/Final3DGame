using UnityEngine;

public class EndPlatform : MonoBehaviour
{
    public float timeToWin = 3f;
    public Vector3 winRespawnPoint = new Vector3(0, 10, 0);

    [Header("Detection Settings")]
    public Vector3 detectionOffset = new Vector3(0, 1.1f, 0);
    public Vector3 detectionSize = new Vector3(2f, 1f, 2f);

    public float forgiveness = 0.2f;

    private float timer = 0f;
    private float lastSeenTime = -999f;

    private bool playerIsOnMe;

    void Update()
    {
        Collider[] hits = Physics.OverlapBox(
            transform.position + detectionOffset,
            detectionSize / 2
        );

        playerIsOnMe = false;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                playerIsOnMe = true;
                break;
            }
        }

        if (playerIsOnMe)
        {
            lastSeenTime = Time.time;
            timer += Time.deltaTime;

            if (timer >= timeToWin)
            {
                Debug.Log("WIN TRIGGERED");
                TeleportPlayer(hit: true);
            }
        }
        else
        {
            if (Time.time - lastSeenTime > forgiveness)
            {
                if (timer != 0f)
                    Debug.Log("TIMER RESET");

                timer = 0f;
            }
        }
    }

    private void TeleportPlayer(bool hit)
    {
        timer = 0f;

        CharacterController cc = FindObjectOfType<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;
            cc.transform.position = winRespawnPoint;
            cc.enabled = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + detectionOffset, detectionSize);
    }
}
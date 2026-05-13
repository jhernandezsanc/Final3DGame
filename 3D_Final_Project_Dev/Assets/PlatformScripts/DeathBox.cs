using UnityEngine;

public class DeathBox : MonoBehaviour
{
    public Vector3 respawnPoint = new Vector3(0, 10, 0);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController cc = other.GetComponent<CharacterController>();

            if (cc != null)
            {
                cc.enabled = false;
                other.transform.position = respawnPoint;
                cc.enabled = true;
            }

            // Trigger death panel on PauseMenuController
            if (PauseMenuController.Instance != null)
                PauseMenuController.Instance.ShowDeathPanel();
        }
    }
}
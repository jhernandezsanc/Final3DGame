using UnityEngine;

public class RecoilController : MonoBehaviour
{
    Vector3 speed = new Vector3(0f, 0f, 0f);
    public float decelPercent = 5f; 

    // Marks Singleton 
    public static RecoilController Instance;
    private CharacterController characterController;
    private PlayerMovement playerMovement;
    public void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        playerMovement = GetComponent<PlayerMovement>();
    }

    //finds distance between player and source of recoil, finds the direction, amplifies it by recoil strength
    public void Recoil(Vector3 forcePos, float recoilStrength = 1f)
    {
        if (playerMovement.isGrounded) //reduces recoil felt when player starts on ground
        {
            recoilStrength = recoilStrength / 10;
        }
        Vector3 playerPos = transform.position;
        Vector3 accelDir = playerPos - forcePos;
        speed += accelDir.normalized * recoilStrength;
    }


    private void Update()
    {

        if (speed.sqrMagnitude > 0.001f) //keeps from calling too much 
        {
            speed = Vector3.Lerp(speed, Vector3.zero, decelPercent * Time.deltaTime); //nifty little thang 
            // characterController.Move(speed * Time.deltaTime); //Old Logic
            MasterMovement.Instance.frameDisplacement += speed * Time.deltaTime;
            
        }

    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    public float speed = 12f;
    public float gravity = -20f;
    public float jumpHeight = 3f;

    public Transform groundCheck;

    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    [HideInInspector]public bool isGrounded;//made public for recoil but hidden in inspector
    bool isMoving; //bool for later use

    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
   
        //Ground check 
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //Resets defeault velocity 
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = 0f;
        float z = 0f;

        //keyboard inputs 
        if (Keyboard.current.wKey.isPressed) z += 1f;
        if (Keyboard.current.sKey.isPressed) z -= 1f;
        if (Keyboard.current.aKey.isPressed) x -= 1f;
        if (Keyboard.current.dKey.isPressed) x += 1f;

        //Move Vector 
        Vector3 move = transform.right * x + transform.forward * z;
        //Takes into account diagonal movement 
        if (move.magnitude > 1f) move.Normalize();

        controller.Move(move * speed * Time.deltaTime);

        //check if can jump 
        if(Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight*-2f*gravity);
        }

        //falling 
        velocity.y += gravity * Time.deltaTime;

        //jump 
        controller.Move(velocity * Time.deltaTime);

        if (lastPosition != gameObject.transform.position && isGrounded == true)
        {
            isMoving = true;
        }

        else
        {
            isMoving = false;
        }

        lastPosition = gameObject.transform.position;
    }
}

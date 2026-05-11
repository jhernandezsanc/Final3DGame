using UnityEngine;

public class MasterMovement : MonoBehaviour
{
    // This script congeals all inputs to player position into 
    // a single Vector3 and then calls Move. It resets itself 
    // so that all actual logic behind movement can be done 
    // locally in the apropriate script 

    public static MasterMovement Instance; 
    private CharacterController controller;
    public Vector3 frameDisplacement; 

    public void Awake() //singleton for access
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        controller = GetComponent<CharacterController>();
    }

    private void LateUpdate()
    {
        controller.Move(frameDisplacement);

        frameDisplacement = Vector3.zero;
    }
}

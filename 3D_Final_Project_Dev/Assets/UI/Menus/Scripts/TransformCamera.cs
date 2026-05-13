using UnityEngine;

public class TransformCamera : MonoBehaviour
{
    public float rotationSpeed = 10f; // Speed of rotation
    public GameObject cameraObject; // Reference to the camera object
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        cameraObject.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0); // Rotate the camera around the Y-axis
    }
}

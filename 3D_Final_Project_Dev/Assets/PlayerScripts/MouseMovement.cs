using UnityEngine;
using UnityEngine.InputSystem;

public class MouseMovement : MonoBehaviour
{
    public Transform cameraTransform;

    //adjusts player mouse sense
    public float xMouseSensativity = 100f;
    public float yMouseSensativity = 100f;

    float xRotation = 0f;
    float yRotation = 0f;

    //used to determine up and down look limits
    public float topClamp = -90f;
    public float bottomClamp = 90f;
    void Start()
    {
        //locks cursor to middle of screen and makes invisible
        Cursor.lockState = CursorLockMode.Locked; 
    }

    void Update()
    {
        if (Mouse.current != null) //just in case we ever add controller support
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            float sens = SettingsController.CurrentSensitivity;
            //grabs mouse inputs
            float mouseX = mouseDelta.x * sens * Time.deltaTime;
            float mouseY = mouseDelta.y * sens * Time.deltaTime;

            if (SettingsController.InvertY) mouseY = -mouseY;

            //rotations around axis
            xRotation -= mouseY;
            //halts view angle to cieling and floor
            xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

            yRotation += mouseX;

            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }
}

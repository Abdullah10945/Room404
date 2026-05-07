using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float mouseSensitivity = 200f;

    [Tooltip("Drag the Player Capsule here. Do NOT drag the Camera here.")]
    public Transform playerBody;

    private float xRotation = 0f;

    void Start()
    {
        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Get mouse input from Unity's Input Manager
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Calculate vertical rotation (up and down)
        xRotation -= mouseY;
        // Clamp it so the player can't snap their neck backward
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply vertical rotation to THIS object (which should be the Camera)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Apply horizontal rotation to the Player Body
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
        else
        {
            Debug.LogWarning("MouseLook script is missing the Player Body reference!");
        }
    }
}
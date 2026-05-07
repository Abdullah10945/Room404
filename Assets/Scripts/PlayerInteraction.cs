using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 3f;
    public KeyCode interactKey = KeyCode.E;

    // Optional: If you want to show a small dot in the center of the screen when looking at something
    // public GameObject crosshair; 

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // Draw a ray straight forward from the exact center of the camera
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        // Check if the ray hits anything within our interactRange
        if (Physics.Raycast(ray, out hit, interactRange))
        {
            // Check if the object we hit has a script that uses our IInteractable interface
            IInteractable interactableObject = hit.collider.GetComponent<IInteractable>();

            if (interactableObject != null)
            {
                // Here is where you would enable a crosshair highlight if you wanted one

                // Did the player press E?
                if (Input.GetKeyDown(interactKey))
                {
                    interactableObject.Interact();
                }
            }
        }
    }
}
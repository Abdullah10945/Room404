using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 3f;
    public KeyCode interactKey = KeyCode.E;

    [Header("UI Elements")]
    [Tooltip("Drag your 'Press E to Interact' UI Text GameObject here")]
    public GameObject interactUI;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (interactUI != null) interactUI.SetActive(false); // Hide UI at start
    }

    void Update()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        // Check if the ray hits anything within our interactRange
        if (Physics.Raycast(ray, out hit, interactRange))
        {
            // Check if the object we hit uses our IInteractable interface
            IInteractable interactableObject = hit.collider.GetComponent<IInteractable>();

            if (interactableObject != null)
            {
                // We are looking at something interactable! Show the text.
                if (interactUI != null) interactUI.SetActive(true);

                // Did the player press E?
                if (Input.GetKeyDown(interactKey))
                {
                    interactableObject.Interact();
                }
            }
            else
            {
                // We are looking at a normal wall. Hide the text.
                if (interactUI != null) interactUI.SetActive(false);
            }
        }
        else
        {
            // We aren't looking at anything. Hide the text.
            if (interactUI != null) interactUI.SetActive(false);
        }
    }
}
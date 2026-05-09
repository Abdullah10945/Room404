using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Anomaly_PortalDoor : MonoBehaviour, IInteractable
{
    [Header("Door Animation Settings")]
    public float smooth = 1.0f;
    public float doorOpenAngle = -90.0f; // Adjust to 90 if it swings the wrong way

    [Header("Audio")]
    public AudioClip knockSound;

    [Header("Teleportation Setup")]
    [Tooltip("Place an empty GameObject where you want the player to come out")]
    public Transform teleportDestination;

    private AudioSource audioSource;
    private bool isOpen = false;
    private float doorCloseAngle;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Remember starting rotation
        doorCloseAngle = transform.localEulerAngles.y;
    }

    void Update()
    {
        // Smoothly swing the door open
        if (isOpen)
        {
            Quaternion target = Quaternion.Euler(0, doorOpenAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * 5 * smooth);
        }
    }

    // This is called by PlayerInteract.cs when the player presses 'E'
    public void Interact()
    {
        if (!isOpen)
        {
            isOpen = true; // Starts the opening animation in Update()

            // Play the knock sound
            if (knockSound != null)
            {
                audioSource.PlayOneShot(knockSound);
            }
        }
    }

    // This triggers when the player walks THROUGH the open door
    private void OnTriggerEnter(Collider other)
    {
        // Only teleport if the door is actually open and it's the player walking through
        if (isOpen && other.CompareTag("Player"))
        {
            if (teleportDestination != null)
            {
                // Temporarily disable the CharacterController to allow manual repositioning
                CharacterController cc = other.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;

                // Teleport the player and match the rotation of the destination
                other.transform.position = teleportDestination.position;
                other.transform.rotation = teleportDestination.rotation;

                if (cc != null) cc.enabled = true;
            }

            // Set the stage as cleared so the stairs allow them to advance!
            GameManager.Instance.isCurrentPuzzleSolved = true;
            Debug.Log("Player walked through the portal! Puzzle 1 Solved.");
        }
    }
}
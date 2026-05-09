using UnityEngine;

// Put this script directly on the "Correct" door in Loop 1.
// Make sure it has a BoxCollider.
public class Anomaly_PortalDoor : MonoBehaviour, IInteractable
{
    [Header("Portal Settings")]
    [Tooltip("The duplicate hallway or void that appears when opened")]
    public GameObject portalVisuals;

    [Header("Audio")]
    public AudioSource heavyKnockSound;

    private bool hasBeenOpened = false;

    public void Interact()
    {
        if (!hasBeenOpened)
        {
            hasBeenOpened = true;

            // 1. Play the loud clang
            if (heavyKnockSound != null) heavyKnockSound.Play();

            // 2. Open the portal (e.g., enable the duplicate hallway mesh)
            if (portalVisuals != null) portalVisuals.SetActive(true);

            // 3. Solve the puzzle! Now the Treadmill will let them pass.
            GameManager.Instance.isCurrentPuzzleSolved = true;

            Debug.Log("Portal Opened! Puzzle 1 Solved.");
        }
    }
}
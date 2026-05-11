using UnityEngine;
using DoorScript; // This lets us talk to your custom Door class!

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Door))] // Ensures your Door.cs is attached to this same object
public class Anomaly_PortalDoor : MonoBehaviour, IInteractable
{
    [Header("Loop Logic")]
    [Tooltip("Which loop number should this puzzle be active on? (Loop 1 = Index 0)")]
    public int activeOnLoopIndex = 0;

    [Header("Door Links")]
    [Tooltip("Drag the SECOND door's Door.cs component here so they open together.")]
    public Door otherDoorToOpen;

    private Door thisDoor;
    private Collider doorCollider;
    private bool hasBeenOpened = false; // Tracks if we solved the puzzle this loop

    void Start()
    {
        thisDoor = GetComponent<Door>();
        doorCollider = GetComponent<Collider>();

        // Subscribe to GameManager events
        GameManager.Instance.OnLoopAdvanced += CheckLoopState;
        GameManager.Instance.OnLoopReset += CheckLoopState;

        CheckLoopState();
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLoopAdvanced -= CheckLoopState;
            GameManager.Instance.OnLoopReset -= CheckLoopState;
        }
    }

    private void CheckLoopState()
    {
        if (GameManager.Instance.currentLoop == activeOnLoopIndex || GameManager.Instance.currentLoop == 3)
        {
            // It is Loop 1! Turn this puzzle's interaction ON.
            doorCollider.enabled = true;
            hasBeenOpened = false;
        }
        else
        {
            // The loop is over. Disable interaction!
            doorCollider.enabled = false;
            hasBeenOpened = false;

            // Revert (Close) the doors if they were left open!
            // Since your OpenDoor() method toggles the state, we check if it's currently open before calling it.
            if (thisDoor.open)
            {
                thisDoor.OpenDoor();
            }

            if (otherDoorToOpen != null && otherDoorToOpen.open)
            {
                otherDoorToOpen.OpenDoor();
            }
        }
    }

    // Called by PlayerInteract.cs when the player presses 'E'
    public void Interact()
    {
        // Only allow interaction if we haven't opened it yet, and it's the correct loop
        if (!hasBeenOpened && (GameManager.Instance.currentLoop == activeOnLoopIndex || GameManager.Instance.currentLoop == 3))
        {
            hasBeenOpened = true;

            // Tell your custom Door script to open!
            if (!thisDoor.open) thisDoor.OpenDoor();

            // Tell the second Door script to open!
            if (otherDoorToOpen != null && !otherDoorToOpen.open)
            {
                otherDoorToOpen.OpenDoor();
            }

            // Mark the puzzle as solved so the stairs work.
            GameManager.Instance.isCurrentPuzzleSolved = true;
            Debug.Log("Both Portal Doors Triggered! Puzzle 1 Solved.");
        }
    }
}
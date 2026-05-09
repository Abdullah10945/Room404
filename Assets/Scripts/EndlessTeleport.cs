using UnityEngine;

public class EndlessTeleport : MonoBehaviour
{
    [Tooltip("The empty GameObject where the player will teleport to.")]
    public Transform teleportTarget;

    [Tooltip("Check this if this is the DOWN stairs trigger to advance the loop.")]
    public bool isDownTrigger = false;

    // A static boolean is shared across EVERY TreadmillTeleport script in the scene.
    // If one turns it false, they ALL turn false.
    public static bool canTeleport = true;

    private void OnTriggerEnter(Collider other)
    {
        // 1. If teleports are locked (because we just used one), ignore this completely!
        if (!canTeleport) return;

        if (other.CompareTag("Player"))
        {
            // 2. IMMEDIATELY lock all teleporters so we don't bounce back!
            canTeleport = false;

            // 3. Calculate offset for seamless movement
            Vector3 offset = other.transform.position - transform.position;

            // 4. Move the player
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            other.transform.position = teleportTarget.position + offset;

            if (cc != null) cc.enabled = true;

            // 5. Evaluate the game state (Only if they went DOWN)
            if (isDownTrigger)
            {
                if (GameManager.Instance.isCurrentPuzzleSolved)
                {
                    GameManager.Instance.AdvanceLoop();
                }
                else
                {
                    GameManager.Instance.ResetLoop();
                }
            }
            else
            {
                // Optional: Walking back up usually resets the loop as a punishment
                // GameManager.Instance.ResetLoop(); 
            }
        }
    }
}
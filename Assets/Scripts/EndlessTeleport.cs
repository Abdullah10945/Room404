using UnityEngine;

public class EndlessTeleport : MonoBehaviour
{
    [Tooltip("The empty GameObject where the player will teleport to.")]
    public Transform teleportTarget;

    [Tooltip("Check this if this is the DOWN stairs trigger to advance the loop.")]
    public bool isDownTrigger = true;

    [Tooltip("Slight upward bump to prevent falling through the floor.")]
    public float yOffsetBump = 0.1f;

    // A static boolean is shared across EVERY TreadmillTeleport script in the scene.
    // If one turns it false, they ALL turn false.
    public static bool canTeleport = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Print the current state as soon as the player touches the trigger
            Debug.Log($"[Treadmill] Trigger hit! canTeleport is currently: {canTeleport}");

            if (!canTeleport)
            {
                Debug.Log("[Treadmill] Teleport ignored because canTeleport was locked (false).");
                return;
            }

            if (teleportTarget == null)
            {
                Debug.LogError("[Treadmill] ERROR: Teleport Target is missing on " + gameObject.name);
                return;
            }

            // 2. IMMEDIATELY lock all teleporters so we don't bounce back!
            canTeleport = false;

            // Print the state change!
            Debug.Log($"[Treadmill] canTeleport CHANGED to: {canTeleport} (Locked)");

            // 3. Calculate offset for seamless movement
            Vector3 offset = other.transform.position - transform.position;

            // FIX: Force Y offset to 0 so they don't inherit negative gravity depth!
            offset.y = 0f;

            CharacterController cc = other.GetComponent<CharacterController>();

            // 4. Move the player with a safety net so the controller ALWAYS turns back on
            try
            {
                if (cc != null) cc.enabled = false;

                // Teleport the player using the flat offset + a tiny upward bump!
                other.transform.position = teleportTarget.position + offset + new Vector3(0f, yOffsetBump, 0f);

                // Force Unity to update the physics instantly
                Physics.SyncTransforms();
            }
            finally
            {
                if (cc != null) cc.enabled = true;
            }

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
        }
    }
}
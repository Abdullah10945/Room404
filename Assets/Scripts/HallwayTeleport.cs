using UnityEngine;

public class HallwayTeleport : MonoBehaviour
{
    [Tooltip("The empty GameObject where the player will teleport to.")]
    public Transform teleportTarget;

    // Shared static boolean with the stairs
    public static bool canTeleport = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!canTeleport) return;

        if (other.CompareTag("Player"))
        {
            // SAFETY CHECK: Prevents the script from breaking if you forgot to link the target
            if (teleportTarget == null)
            {
                Debug.LogError("Teleport Target is missing on " + gameObject.name + "!");
                return;
            }

            canTeleport = false;
            CharacterController cc = other.GetComponent<CharacterController>();

            // A try/finally block ensures that even if an error happens, the controller ALWAYS turns back on
            try
            {
                if (cc != null) cc.enabled = false;

                // 1. Calculate offset, but ZERO OUT the Y axis!
                // This prevents the player from accidentally being spawned inside the floor and launched upwards.
                Vector3 offset = other.transform.position - transform.position;
                offset.y = 0f;

                // 2. Teleport the player
                other.transform.position = teleportTarget.position + offset;

                // 3. Force Unity's physics engine to instantly update to the new location
                Physics.SyncTransforms();
            }
            finally
            {
                // Turn the controller back on securely
                if (cc != null) cc.enabled = true;
            }
        }
    }
}
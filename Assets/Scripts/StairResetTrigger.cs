using UnityEngine;

// Attach this to a large BoxCollider (Is Trigger = true) on your middle landing or hallway.
public class StairResetTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // The player has reached the safe "middle" area.
            // Unlock the teleporters so they can be used again!
            if (!EndlessTeleport.canTeleport)
            {
                EndlessTeleport.canTeleport = true;

                // Print the state change!
                Debug.Log($"[ResetTrigger] Player reached safe zone! canTeleport CHANGED to: {EndlessTeleport.canTeleport} (Unlocked)");
            }
        }
    }
}
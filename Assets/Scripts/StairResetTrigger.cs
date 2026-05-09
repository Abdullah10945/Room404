using UnityEngine;

// Attach this to a large BoxCollider (Is Trigger = true) on your middle landing.
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
                // Debug.Log("Stairs Reset! Player can now teleport again.");
            }
        }
    }
}
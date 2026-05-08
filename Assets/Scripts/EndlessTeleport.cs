using UnityEngine;

public class EndlessTeleport : MonoBehaviour
{
    [Tooltip("The empty GameObject where the player will teleport to.")]
    public Transform teleportTarget;

    [Tooltip("Check this if this is the DOWN stairs trigger to advance the loop.")]
    public bool isDownTrigger = true;

    // A static variable means ALL teleport triggers share this single timer.
    // This prevents the top and bottom triggers from bouncing the player back and forth.
    private static float nextTeleportTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        // 1. Safety Check: Are we on cooldown?
        if (Time.time < nextTeleportTime) return;

        if (other.CompareTag("Player"))
        {
            // 2. Set the cooldown so no other trigger can fire for 0.2 seconds
            nextTeleportTime = Time.time + 0.2f;

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
            // If they walked UP the stairs, we usually just reset them or do nothing, 
            // since walking UP isn't how you escape. You can adjust this based on your design!
            else
            {
                // Optional: Walking back up always resets the loop?
                // GameManager.Instance.ResetLoop(); 
            }
        }
    }
}
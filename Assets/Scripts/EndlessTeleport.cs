using UnityEngine;

public class EndlessTeleport : MonoBehaviour
{
    [Tooltip("The empty GameObject at the top of the stairs where the player will teleport to.")]
    public Transform teleportTarget;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the thing entering the trigger is the Player
        if (other.CompareTag("Player"))
        {
            // 1. Calculate the exact offset so the teleport is perfectly seamless
            // This prevents camera stuttering
            Vector3 offset = other.transform.position - transform.position;

            // 2. Temporarily disable the CharacterController (Unity prevents manual moving if this is on)
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // 3. Move the player!
            other.transform.position = teleportTarget.position + offset;

            // 4. Turn the controller back on
            if (cc != null) cc.enabled = true;

            // 5. Evaluate the game state
            if (GameManager.Instance.isCurrentPuzzleSolved)
            {
                GameManager.Instance.AdvanceLoop();
            }
            else
            {
                // The player tried to escape without solving the puzzle!
                GameManager.Instance.ResetLoop();
            }
        }
    }
}
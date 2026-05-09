using UnityEngine;

// Put this script on an Empty GameObject inside your Loop 2 Hallway Prefab.
public class Anomaly_EchoTrap : MonoBehaviour
{
    [Header("Echo Settings")]
    public Transform playerCamera;
    public AudioSource jumpScareSound;

    private bool hasFailed = false;

    void Start()
    {
        // By default, the puzzle is "solved" UNLESS they look back.
        // We set this to true instantly so they can use the stairs if they survive.
        GameManager.Instance.isCurrentPuzzleSolved = true;

        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;
    }

    void Update()
    {
        if (hasFailed) return;

        // Calculate if the player is looking backwards.
        // transform.forward should be pointing down the hall towards the stairs.
        // Dot product < 0 means they are looking more than 90 degrees away from forward.
        float lookAngle = Vector3.Dot(playerCamera.forward, transform.forward);

        if (lookAngle < 0)
        {
            hasFailed = true;

            // They looked back! Trigger the scare and reset the loop.
            if (jumpScareSound != null) jumpScareSound.Play();

            Debug.Log("Player looked back! Resetting Loop.");
            GameManager.Instance.ResetLoop();
        }
    }
}
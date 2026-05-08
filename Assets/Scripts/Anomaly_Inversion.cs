using UnityEngine;

// Put this script on an Empty GameObject inside your Loop 3 (Upside Down) Prefab.
public class Anomaly_Inversion : MonoBehaviour
{
    public Transform playerCamera;

    void Start()
    {
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;
    }

    void Update()
    {
        // For this puzzle, the player MUST be facing backwards when they hit the treadmill.
        // transform.forward is the direction to the stairs.
        float lookAngle = Vector3.Dot(playerCamera.forward, transform.forward);

        // If dot is less than -0.5, they are facing mostly backwards.
        if (lookAngle < -0.5f)
        {
            GameManager.Instance.isCurrentPuzzleSolved = true;
        }
        else
        {
            // If they turn around to look at the stairs normally, they un-solve it!
            GameManager.Instance.isCurrentPuzzleSolved = false;
        }
    }
}
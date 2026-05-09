using UnityEngine;

// Put this script on an Empty GameObject inside your Loop 4 Prefab.
public class Anomaly_MotionSensor : MonoBehaviour
{
    [Header("Sensor Settings")]
    public GameObject[] hallwayLights; // Drag all your point/spot lights here
    public float timeToDarkness = 3.0f;

    private Transform playerTransform;
    private Vector3 lastPosition;
    private float stationaryTimer = 0f;

    void Start()
    {
        // Find the player capsule
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            lastPosition = playerTransform.position;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Calculate how fast the player is moving
        float currentSpeed = (playerTransform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = playerTransform.position;

        if (currentSpeed > 0.1f)
        {
            // Player is moving! Reset timer, turn on lights, un-solve puzzle.
            stationaryTimer = 0f;
            SetLights(true);
            GameManager.Instance.isCurrentPuzzleSolved = false;
        }
        else
        {
            // Player is standing still. Start counting!
            stationaryTimer += Time.deltaTime;

            if (stationaryTimer >= timeToDarkness)
            {
                SetLights(false);
                GameManager.Instance.isCurrentPuzzleSolved = true; // Puzzle solved, they can enter the dark stairs!
            }
        }
    }

    private void SetLights(bool state)
    {
        foreach (GameObject light in hallwayLights)
        {
            if (light != null && light.activeSelf != state)
            {
                light.SetActive(state);
            }
        }
    }
}
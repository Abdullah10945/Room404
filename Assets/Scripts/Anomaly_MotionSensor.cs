using UnityEngine;

// Attach this to your Switchboard or Button model in the hallway.
[RequireComponent(typeof(Collider))]
public class Anomaly_MotionSensor : MonoBehaviour, IInteractable
{
    [Header("Loop Logic")]
    [Tooltip("Which loop number should this puzzle be active on? (Loop 2 = Index 1)")]
    public int activeOnLoopIndex = 1;

    [Header("Switch Settings")]
    public GameObject[] hallwayLights;

    [Tooltip("How long the lights stay on before shutting off again.")]
    public float lightOnDuration = 5.0f;

    [Header("Audio")]
    public AudioSource switchSound;

    private Collider switchCollider;
    private bool lightsAreOn = false;
    private float timer = 0f;

    void Start()
    {
        switchCollider = GetComponent<Collider>();

        // Subscribe to GameManager events to know when to turn this puzzle on/off
        GameManager.Instance.OnLoopAdvanced += CheckLoopState;
        GameManager.Instance.OnLoopReset += CheckLoopState;

        // Check state immediately to setup the initial lighting
        CheckLoopState();
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLoopAdvanced -= CheckLoopState;
            GameManager.Instance.OnLoopReset -= CheckLoopState;
        }
    }

    private void CheckLoopState()
    {
        if (GameManager.Instance.currentLoop == activeOnLoopIndex)
        {
            // It is Loop 2! Turn this puzzle ON.
            switchCollider.enabled = true;

            // Force the lights off instantly so the player is trapped in the dark
            SetLights(false);
            lightsAreOn = false;
            timer = 0f;
        }
        else
        {
            // Not this puzzle's turn. Act like a normal, non-interactable wall.
            switchCollider.enabled = false;

            // Leave the lights ON normally for all the other loops
            SetLights(true);
            lightsAreOn = true;
        }
    }

    void Update()
    {
        // Only run the countdown timer if the lights are on AND it's the active loop
        if (lightsAreOn && GameManager.Instance.currentLoop == activeOnLoopIndex)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                // Time's up! Lights go out.
                SetLights(false);
                lightsAreOn = false;

                // Puzzle is unsolved again. They didn't make it to the stairs in time!
                GameManager.Instance.isCurrentPuzzleSolved = false;
                Debug.Log("Time ran out. Lights are off. Puzzle unsolved.");
            }
        }
    }

    // Called by PlayerInteract.cs when the player presses 'E'
    public void Interact()
    {
        // Only allow interaction during Loop 2
        if (GameManager.Instance.currentLoop == activeOnLoopIndex)
        {
            // Play switch sound
            if (switchSound != null) switchSound.Play();

            // Turn lights on and reset the timer to 5 seconds
            SetLights(true);
            lightsAreOn = true;
            timer = lightOnDuration;

            // Puzzle is currently solved! Player must run to the stairs!
            GameManager.Instance.isCurrentPuzzleSolved = true;
            Debug.Log($"Switch flipped! Run to the stairs! You have {lightOnDuration} seconds.");
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
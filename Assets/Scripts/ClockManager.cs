using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ClockManager : MonoBehaviour
{
    [Header("Clock Hands (Transforms)")]
    public Transform hourHand;
    public Transform minuteHand;

    [Header("Rotation Settings")]
    [Tooltip("Which axis does the hand rotate around? Usually Vector3.forward (Z) or Vector3.up (Y) depending on your 3D model.")]
    public Vector3 rotationAxis = Vector3.forward;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Subscribe to the GameManager events! 
        // This tells the clock to listen for when the loop changes without constantly checking in Update()
        GameManager.Instance.OnLoopAdvanced += UpdateClockTime;
        GameManager.Instance.OnLoopReset += UpdateClockTime;

        // Set initial time to 1:01
        UpdateClockTime();
    }

    private void OnDestroy()
    {
        // Always unsubscribe from events when destroyed to prevent memory leaks
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLoopAdvanced -= UpdateClockTime;
            GameManager.Instance.OnLoopReset -= UpdateClockTime;
        }
    }

    private void UpdateClockTime()
    {
        // Get the current loop from the GameManager. 
        // Loop 0 = 1:01, Loop 1 = 2:02, etc.
        int currentLoop = GameManager.Instance.currentLoop;

        int targetHour = currentLoop + 1;
        int targetMinute = currentLoop + 1;

        // Calculate the degrees (360 degrees in a circle)
        // Hour hand: 30 degrees per hour (360 / 12)
        // Minute hand: 6 degrees per minute (360 / 60)
        float hourAngle = targetHour * 30f;
        float minuteAngle = targetMinute * 6f;

        // Apply the rotation. We use localRotation so it works no matter how the clock is placed on the wall.
        if (hourHand != null) hourHand.localRotation = Quaternion.Euler(rotationAxis * hourAngle);
        if (minuteHand != null) minuteHand.localRotation = Quaternion.Euler(rotationAxis * minuteAngle);

        // Play the KA-CHUNK sound!
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
}
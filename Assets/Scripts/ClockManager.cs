using UnityEngine;

[RequireComponent(typeof(Clock))] // Ensures your Clock.cs is on the same GameObject
[RequireComponent(typeof(AudioSource))]
public class ClockManager : MonoBehaviour
{
    [Tooltip("If true, manually ticks the seconds hand for atmosphere, without affecting minutes/hours.")]
    public bool tickSeconds = true;

    private Clock clockScript;
    private AudioSource audioSource;
    private float secondTimer = 0f;

    void Start()
    {
        clockScript = GetComponent<Clock>();
        audioSource = GetComponent<AudioSource>();

        // Force the built-in clock settings to exactly what you requested
        clockScript.realTime = false;
        clockScript.clockSpeed = 0f; // Stops automatic time progression

        // Subscribe to the GameManager events
        GameManager.Instance.OnLoopAdvanced += UpdateClockTime;
        GameManager.Instance.OnLoopReset += UpdateClockTime;

        // Set initial time to 1:01
        UpdateClockTime();
    }

    private void OnDestroy()
    {
        // Always unsubscribe from events when destroyed
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLoopAdvanced -= UpdateClockTime;
            GameManager.Instance.OnLoopReset -= UpdateClockTime;
        }
    }

    void Update()
    {
        // Manually tick the seconds for the creepy vibe. 
        // Because clockSpeed is 0, this will NEVER accidentally advance the minute hand!
        if (tickSeconds)
        {
            secondTimer += Time.deltaTime;
            if (secondTimer >= 1.0f)
            {
                secondTimer -= 1.0f;
                clockScript.seconds++;

                if (clockScript.seconds >= 60)
                {
                    clockScript.seconds = 0;
                }
            }
        }
    }

    private void UpdateClockTime()
    {
        int currentLoop = GameManager.Instance.currentLoop;

        // Loop 0 = 1:01, Loop 1 = 2:02, etc.
        clockScript.hour = currentLoop + 1;
        clockScript.minutes = currentLoop + 1;

        // Play the KA-CHUNK sound!
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
}
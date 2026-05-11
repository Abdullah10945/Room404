using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 4.0f;
    public float gravity = -15.0f;

    [Header("Footstep Audio")]
    public AudioSource footstepAudio;
    [Tooltip("Drag all 10 of your footstep audio clips here")]
    public AudioClip[] footstepClips;
    public AudioClip jumpScare;
    [Tooltip("How much time between each footstep sound?")]
    public float stepInterval = 0.5f;

    private CharacterController controller;
    private Vector3 velocity;
    private float stepTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Force the audio to be 2D so it plays directly in the player's ears everywhere!
        if (footstepAudio != null)
        {
            footstepAudio.spatialBlend = 0f; // 0 = 2D sound, 1 = 3D sound
            footstepAudio.loop = false;      // Prevents background looping bugs
        }
    }

    void Update()
    {
        // Get WASD input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Move relative to where the player is looking
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // --- FOOTSTEP LOGIC ---
        // Check if the player is actually moving AND on the ground
        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            // Count down the timer
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval; // Reset the timer for the next step
            }
        }
        else if (controller.velocity.magnitude <= 0.1f)
        {
            // Optional: If they stop moving, reset the timer to a tiny amount 
            // so the very next time they move, the first step plays almost instantly.
            stepTimer = 0.1f;
        }

        // Apply gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small constant downward force to stick to the ground
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void PlayFootstep()
    {
        // Safety check to make sure you added clips in the inspector
        if (footstepClips.Length == 0 || footstepAudio == null) return;

        // Pick a random clip from your 10 options
        int randomIndex = Random.Range(0, footstepClips.Length);

        // PlayOneShot allows the full sound to play out without being cut off,
        // even if another footstep starts playing!
        footstepAudio.PlayOneShot(footstepClips[randomIndex]);
    }

    public void PlayJumpScare()
    {
        if (jumpScare != null && footstepAudio != null)
        {
            footstepAudio.PlayOneShot(jumpScare);
        }
    }
}
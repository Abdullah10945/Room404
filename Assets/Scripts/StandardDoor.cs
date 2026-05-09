using UnityEngine;

// Attach this to all the "Fake" doors in the hallway
[RequireComponent(typeof(AudioSource))]
public class StandardDoor : MonoBehaviour, IInteractable
{
    [Header("Audio")]
    public AudioClip normalKnockSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void Interact()
    {
        // If they press E, just play the knock sound!
        if (normalKnockSound != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(normalKnockSound);
            Debug.Log("Knocked on a standard door.");
        }
    }
}
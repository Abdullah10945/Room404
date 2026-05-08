using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Attach to the Button object (not the Text child)
// Plays a click sound on pointer down, and a hover sound on pointer enter
[RequireComponent(typeof(Button))]
public class MenuButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverSound;    // Optional — subtle tick on hover
    public AudioClip clickSound;    // The main click sound

    [Range(0f, 1f)]
    public float hoverVolume = 0.3f;
    [Range(0f, 1f)]
    public float clickVolume = 0.7f;

    private AudioSource source;

    void Awake()
    {
        // Use existing AudioSource or create one
        source = GetComponent<AudioSource>();
        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
            source.PlayOneShot(hoverSound, hoverVolume);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null)
            source.PlayOneShot(clickSound, clickVolume);
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Anomaly_EchoTrap : MonoBehaviour
{
    [Header("Loop Logic")]
    [Tooltip("Which loop number should this puzzle be active on? (Loop 3 = Index 2)")]
    public int activeOnLoopIndex = 2;

    [Header("Triggers")]
    [Tooltip("Drag your empty Phantom Start Trigger GameObjects/Colliders here")]
    public Collider[] phantomTriggers;
    [Tooltip("Drag the collider at the END of the hallway that stops the chase")]
    public Collider endTrigger;
    [Tooltip("How long to wait after hitting the trigger before the phantom starts chasing")]
    public float startDelay = 2.0f;

    [Header("Player Tracking")]
    [Tooltip("Drag the Player capsule here")]
    public Transform player;
    [Tooltip("Drag the Main Camera here")]
    public Transform playerCamera;
    [Tooltip("Drag an empty GameObject that points EXACTLY down the hallway towards the stairs")]
    public Transform hallwayDirection;
    [Tooltip("Where does the player teleport when caught? (e.g., front of the clock)")]
    public Transform teleportStartPoint;

    [Header("Phantom Settings")]
    [Tooltip("An empty GameObject with an AudioSource to act as the phantom")]
    public AudioSource phantomAudioSource;
    public float safeDistance = 4.0f;
    [Tooltip("The constant speed at which the phantom catches up when you stop.")]
    public float catchUpSpeed = 3.0f;

    [Header("Effects")]
    [Tooltip("A UI Image covering the whole screen, set to black")]
    public Image blackoutPanel;
    [Tooltip("Optional: A loud sound to play when caught")]
    public AudioClip jumpScareSound;

    private CharacterController playerController;
    private bool isTrapActive = false;
    private bool isCaught = false;
    private bool isDelaying = false;
    private float currentDistance;
    private Vector3 initialPhantomPosition;

    // Tracks physical movement instead of buggy CharacterController velocity
    private Vector3 lastPlayerPos;

    void Start()
    {
        if (player != null)
        {
            playerController = player.GetComponent<CharacterController>();
            lastPlayerPos = player.position;
        }

        // Ensure the screen starts clear
        if (blackoutPanel != null)
        {
            blackoutPanel.color = new Color(0, 0, 0, 0);
            blackoutPanel.gameObject.SetActive(false);
        }

        // Store the original position of the phantom so we can snap it back later
        if (phantomAudioSource != null)
        {
            initialPhantomPosition = phantomAudioSource.transform.position;
            phantomAudioSource.dopplerLevel = 0f; // Prevents teleport audio glitching
        }

        // Setup Start Triggers
        foreach (Collider col in phantomTriggers)
        {
            if (col != null)
            {
                col.isTrigger = true;
                EchoTriggerForwarder forwarder = col.gameObject.AddComponent<EchoTriggerForwarder>();
                forwarder.mainScript = this;
            }
        }

        // Setup End Trigger
        if (endTrigger != null)
        {
            endTrigger.isTrigger = true;
            EchoEndForwarder endForwarder = endTrigger.gameObject.AddComponent<EchoEndForwarder>();
            endForwarder.mainScript = this;
        }

        // Subscribe to GameManager events
        GameManager.Instance.OnLoopAdvanced += CheckLoopState;
        GameManager.Instance.OnLoopReset += CheckLoopState;

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
        ResetTrapState();
    }

    private void SetStartTriggersEnabled(bool state)
    {
        foreach (Collider col in phantomTriggers)
        {
            if (col != null) col.enabled = state;
        }
    }

    public void ActivateTrap()
    {
        if (!isTrapActive && !isCaught && !isDelaying && GameManager.Instance.currentLoop == activeOnLoopIndex)
        {
            // Instantly disable ALL start triggers so the other side can't be triggered
            SetStartTriggersEnabled(false);
            StartCoroutine(DelayAndStartTrap());
        }
    }

    private IEnumerator DelayAndStartTrap()
    {
        isDelaying = true;

        yield return new WaitForSeconds(startDelay);

        if (GameManager.Instance.currentLoop == activeOnLoopIndex && !isCaught)
        {
            isTrapActive = true;
            isDelaying = false;
            currentDistance = safeDistance;

            if (phantomAudioSource != null)
            {
                phantomAudioSource.gameObject.SetActive(true);
                phantomAudioSource.Play();
            }

            // Enable the end trigger ONLY after the trap has officially started chasing
            if (endTrigger != null)
            {
                endTrigger.enabled = true;
            }

            Debug.Log("Echo Trap Started! Keep moving and don't look back!");
        }
    }

    public void EndTrap()
    {
        // Only trigger if the trap is actually actively chasing the player
        if (isTrapActive && GameManager.Instance.currentLoop == activeOnLoopIndex)
        {
            isTrapActive = false;

            if (phantomAudioSource != null)
            {
                phantomAudioSource.Stop();
                phantomAudioSource.gameObject.SetActive(false);
            }

            if (endTrigger != null) endTrigger.enabled = false;

            GameManager.Instance.isCurrentPuzzleSolved = true;
            Debug.Log("Player reached the end! Phantom stopped. Puzzle Solved.");
        }
    }

    private void ResetTrapState()
    {
        StopAllCoroutines();
        isTrapActive = false;
        isCaught = false;
        isDelaying = false;

        bool isCurrentLoop = (GameManager.Instance != null && GameManager.Instance.currentLoop == activeOnLoopIndex);

        if (phantomAudioSource != null)
        {
            phantomAudioSource.Stop();
            phantomAudioSource.transform.position = initialPhantomPosition;
            // Leave the phantom visually active if we are in Loop 3, so it's waiting for the player
            phantomAudioSource.gameObject.SetActive(isCurrentLoop);
        }

        SetStartTriggersEnabled(isCurrentLoop);
        if (endTrigger != null) endTrigger.enabled = false;
    }

    void Update()
    {
        if (player == null) return;

        // 1. Manually calculate physical speed (Ignoring Y gravity to prevent bugs)
        Vector3 currentFlatPos = new Vector3(player.position.x, 0, player.position.z);
        Vector3 lastFlatPos = new Vector3(lastPlayerPos.x, 0, lastPlayerPos.z);

        float playerSpeed = Vector3.Distance(currentFlatPos, lastFlatPos) / Time.deltaTime;

        // Always record the position for the next frame
        lastPlayerPos = player.position;

        // If the trap isn't actively chasing us, stop here.
        if (!isTrapActive || isCaught) return;

        // 2. Math check: Did the player look back?
        float lookDot = Vector3.Dot(playerCamera.forward, hallwayDirection.forward);
        if (lookDot < -0.1f)
        {
            Debug.Log("Player looked back! Caught!");
            StartCoroutine(CatchSequence());
            return;
        }

        // 3. Movement check: Uses our physical playerSpeed now!
        if (playerSpeed > 0.1f)
        {
            // Player is moving. Fall back to the safe distance at a CONSTANT predictable speed.
            // (We fall back slightly faster than the catchup speed so it resets cleanly)
            currentDistance = Mathf.MoveTowards(currentDistance, safeDistance, catchUpSpeed * 1.5f * Time.deltaTime);
        }
        else
        {
            // Player stopped! Phantom moves in at a strictly CONSTANT speed.
            currentDistance -= catchUpSpeed * Time.deltaTime;

            if (currentDistance <= 0.5f)
            {
                Debug.Log("Phantom caught up! Caught!");
                StartCoroutine(CatchSequence());
                return;
            }
        }

        // 4. Update Phantom Position. Because we use "currentDistance", 
        // the phantom flawlessly teleports exactly when the player hits the Treadmill!
        if (phantomAudioSource != null)
        {
            Vector3 phantomPos = player.position - hallwayDirection.forward * currentDistance;
            phantomAudioSource.transform.position = phantomPos;
        }
    }

    private IEnumerator CatchSequence()
    {
        isCaught = true;
        isTrapActive = false;

        if (phantomAudioSource != null) phantomAudioSource.Stop();
        if (jumpScareSound != null) AudioSource.PlayClipAtPoint(jumpScareSound, playerCamera.position);

        if (blackoutPanel != null)
        {
            blackoutPanel.gameObject.SetActive(true);
            blackoutPanel.color = Color.black;
        }

        playerController.enabled = false;
        player.position = teleportStartPoint.position;
        player.rotation = teleportStartPoint.rotation;

        Physics.SyncTransforms();
        playerController.enabled = true;

        yield return new WaitForSeconds(1.0f);

        if (blackoutPanel != null)
        {
            float fadeDuration = 1.0f;
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                blackoutPanel.color = new Color(0, 0, 0, 1f - (timer / fadeDuration));
                yield return null;
            }
            blackoutPanel.gameObject.SetActive(false);
        }

        ResetTrapState();
    }
}

// --- HELPER SCRIPTS ---
public class EchoTriggerForwarder : MonoBehaviour
{
    [HideInInspector] public Anomaly_EchoTrap mainScript;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && mainScript != null) mainScript.ActivateTrap();
    }
}

public class EchoEndForwarder : MonoBehaviour
{
    [HideInInspector] public Anomaly_EchoTrap mainScript;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && mainScript != null) mainScript.EndTrap();
    }
}
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement; // CRUCIAL: This line allows you to load scenes!

public class endScene : MonoBehaviour
{
    [Header("Game Logic")]
    public GameManager gameManager;

    [Header("UI References")]
    public CanvasGroup blackScreen;
    public CanvasGroup endingText;

    [Header("Timing Settings")]
    public float fadeSpeed = 0.5f;
    public float pauseBeforeText = 1.5f;
    public float timeBeforeMenu = 5.0f; // How long the text stays on screen

    [Header("Scene Management")]
    public string mainMenuSceneName = "MainMenu"; // Make sure this exactly matches your scene file name

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            if (gameManager.currentLoop == 3)
            {
                hasTriggered = true;
                StartCoroutine(EndSequence());
            }
        }
    }

    private IEnumerator EndSequence()
    {
        // 1. Fade Black Screen
        while (blackScreen.alpha < 1f)
        {
            blackScreen.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        blackScreen.alpha = 1f;

        // 2. Dramatic pause
        yield return new WaitForSeconds(pauseBeforeText);

        // 3. Fade Text
        while (endingText.alpha < 1f)
        {
            endingText.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        endingText.alpha = 1f;

        // 4. Wait for the player to read the message
        yield return new WaitForSeconds(timeBeforeMenu);

        // 5. Load the Main Menu!
        SceneManager.LoadScene(0);
    }
}
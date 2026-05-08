using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup fadeOverlay;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI taglineText;
    public CanvasGroup menuGroup;
    public Image vignette;               // Radial gradient image, black, stretched full screen

    [Header("Audio")]
    public AudioSource hum;
    public AudioClip humClip;

    [Header("Scene")]
    public string gameSceneName = "GameScene";

    [Header("Timing")]
    public float titleFadeInDuration = 2.5f;
    public float transitionFadeDuration = 1.2f;

    [Header("Glitch Settings")]
    public float glitchIntensity = 18f;        // How far the title shifts in pixels
    public float randomFlickerMinInterval = 8f;
    public float randomFlickerMaxInterval = 20f;

    private bool isTransitioning = false;
    private float[] introFlickerTimes = { 0.20f, 0.18f, 0.22f, 0.25f, 0.15f };
    private Vector3 titleOriginalPos;
    private bool menuReady = false;

    // Corrupted text variants shown during glitch
    private string[] corruptedVariants = {
        "R\u2593\u2593M 4\u25930",
        "R\u2592OM \u259340",
        "\u2593OOM4\u259304",
        "ROOM\u2592404",
        "R\u259300M404"
    };

    void Start()
    {
        SetAlpha(fadeOverlay, 1f);
        SetTMPAlpha(titleText, 0f);
        SetTMPAlpha(taglineText, 0f);
        SetAlpha(menuGroup, 0f);

        // Store title's original position for glitch reset
        titleOriginalPos = titleText.rectTransform.anchoredPosition;
        Debug.Log("Title original pos: " + titleOriginalPos);


        // Vignette starts visible but subtle
        if (vignette != null)
        {
            Color v = vignette.color;
            v.a = 0f;
            vignette.color = v;
        }

        if (hum != null && humClip != null)
        {
            hum.clip = humClip;
            hum.loop = true;
            hum.volume = 0f;
            hum.Play();
        }

        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        yield return StartCoroutine(FadeCanvasGroup(fadeOverlay, 1f, 0f, 1.5f));
        StartCoroutine(FadeAudio(hum, 0f, 0.6f, 2f));

        // Fade vignette in subtly
        if (vignette != null)
            StartCoroutine(FadeImageAlpha(vignette, 0f, 0.72f, 3f));

        yield return StartCoroutine(FadeTMPIn(titleText, titleFadeInDuration));

        // Intro flicker + glitch
        yield return StartCoroutine(FlickerAndGlitch(titleText, introFlickerTimes));

        yield return new WaitForSeconds(0.4f);
        yield return StartCoroutine(FadeTMPIn(taglineText, 1.2f));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeCanvasGroup(menuGroup, 0f, 1f, 1.0f));

        menuReady = true;
        StartCoroutine(RandomFlickerLoop());
    }

    // ── Flicker + Glitch ─────────────────────────────────────

    IEnumerator FlickerAndGlitch(TextMeshProUGUI t, float[] flickerTimes)
    {
        string originalText = t.text;

        foreach (float dur in flickerTimes)
        {
            // Randomly show corrupted text during some flickers
            if (Random.value > 0.2f)
                t.text = corruptedVariants[Random.Range(0, corruptedVariants.Length)];

            // Shift position (glitch offset)
            float offsetX = Random.Range(-glitchIntensity, glitchIntensity);
            float offsetY = Random.Range(-3f, 3f);
            t.rectTransform.anchoredPosition = titleOriginalPos + new Vector3(offsetX, offsetY, 0);

            SetTMPAlpha(t, 0f);
            yield return new WaitForSeconds(dur);

            t.text = originalText;
            t.rectTransform.anchoredPosition = titleOriginalPos;
            SetTMPAlpha(t, 1f);
            yield return new WaitForSeconds(dur * 1.5f);
        }

        // Snap back clean
        t.text = originalText;
        t.rectTransform.anchoredPosition = titleOriginalPos;
        SetTMPAlpha(t, 1f);
    }

    IEnumerator RandomFlickerLoop()
    {
        while (!isTransitioning)
        {
            float waitTime = Random.Range(randomFlickerMinInterval, randomFlickerMaxInterval);
            yield return new WaitForSeconds(waitTime);

            if (isTransitioning) yield break;

            // Quick random glitch burst — 2 to 4 flickers
            int burstCount = Random.Range(2, 5);
            float[] burstTimes = new float[burstCount];
            for (int i = 0; i < burstCount; i++)
                burstTimes[i] = Random.Range(0.03f, 0.1f);

            yield return StartCoroutine(FlickerAndGlitch(titleText, burstTimes));
        }
    }

    // ── Button Events ─────────────────────────────────────────

    public void OnStartPressed()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        StartCoroutine(TransitionToGame());
    }

    public void OnQuitPressed()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        StartCoroutine(TransitionToQuit());
    }

    IEnumerator TransitionToGame()
    {
        StartCoroutine(FadeAudio(hum, hum.volume, 0f, transitionFadeDuration));
        yield return StartCoroutine(FadeCanvasGroup(fadeOverlay, 0f, 1f, transitionFadeDuration));
        SceneManager.LoadScene(gameSceneName);
    }

    IEnumerator TransitionToQuit()
    {
        StartCoroutine(FadeAudio(hum, hum.volume, 0f, 0.8f));
        yield return StartCoroutine(FadeCanvasGroup(fadeOverlay, 0f, 1f, 0.8f));
        Application.Quit();
    }

    // ── Helpers ───────────────────────────────────────────────

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float t = 0f;
        cg.alpha = from;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        cg.alpha = to;
    }

    IEnumerator FadeTMPIn(TextMeshProUGUI tmp, float duration)
    {
        float t = 0f;
        Color c = tmp.color;
        c.a = 0f;
        tmp.color = c;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / duration);
            tmp.color = c;
            yield return null;
        }
        c.a = 1f;
        tmp.color = c;
    }

    IEnumerator FadeImageAlpha(Image img, float from, float to, float duration)
    {
        float t = 0f;
        Color c = img.color;
        c.a = from;
        img.color = c;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            img.color = c;
            yield return null;
        }
        c.a = to;
        img.color = c;
    }

    IEnumerator FadeAudio(AudioSource source, float from, float to, float duration)
    {
        if (source == null) yield break;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        source.volume = to;
    }

    void SetAlpha(CanvasGroup cg, float a) { cg.alpha = a; }

    void SetTMPAlpha(TextMeshProUGUI tmp, float a)
    {
        if (tmp == null) return;
        Color c = tmp.color;
        c.a = a;
        tmp.color = c;
    }
}
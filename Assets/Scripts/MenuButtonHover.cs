using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// Attach this to each button's TMP text child (Start, Quit)
public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI label;

    // Normal state — dim grey
    private Color normalColor = new Color(0.45f, 0.45f, 0.45f, 1f);
    // Hovered state — cold fluorescent white
    private Color hoverColor  = new Color(0.92f, 0.95f, 0.88f, 1f);

    private Coroutine currentTween;

    void Awake()
    {
        label = GetComponent<TextMeshProUGUI>();
        if (label != null)
            label.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentTween != null) StopCoroutine(currentTween);
        currentTween = StartCoroutine(TweenColor(hoverColor, 0.15f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentTween != null) StopCoroutine(currentTween);
        currentTween = StartCoroutine(TweenColor(normalColor, 0.25f));
    }

    IEnumerator TweenColor(Color target, float duration)
    {
        Color start = label.color;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            label.color = Color.Lerp(start, target, t / duration);
            yield return null;
        }
        label.color = target;
    }
}

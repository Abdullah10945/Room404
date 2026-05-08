using UnityEngine;
using UnityEngine.UI;

// Attach this to your Scanline Image object.
// It generates the scanline texture at runtime — no external image needed.
[RequireComponent(typeof(RawImage))]
public class ScanlineOverlay : MonoBehaviour
{
    [Range(0f, 1f)]
    public float opacity = 0.08f;          // How strong the scanlines are (keep low — 0.06-0.12)
    public int lineSpacing = 4;            // Pixels between each dark line (4 = dense, 8 = sparse)

    void Start()
    {
        RawImage img = GetComponent<RawImage>();
        img.texture = GenerateScanlineTex();
        img.color = new Color(0f, 0f, 0f, opacity);

        // Tile the texture vertically across the full screen
        img.uvRect = new Rect(0, 0, 1, Screen.height / (float)(lineSpacing * 2));
    }

    Texture2D GenerateScanlineTex()
    {
        // Creates a tiny texture: one dark line, one transparent line, tiled
        int height = lineSpacing * 2;
        Texture2D tex = new Texture2D(1, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Repeat;

        for (int y = 0; y < height; y++)
        {
            // Bottom half = opaque black line, top half = transparent
            Color c = (y < lineSpacing)
                ? new Color(0f, 0f, 0f, 1f)
                : new Color(0f, 0f, 0f, 0f);
            tex.SetPixel(0, y, c);
        }

        tex.Apply();
        return tex;
    }
}

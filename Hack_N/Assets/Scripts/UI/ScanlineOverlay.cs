using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class ScanlineOverlay : MonoBehaviour
{
    [Range(0f, 1f)] public float darkness = 0.25f;
    public int lineHeight = 4;
    public float scrollSpeed = 0.15f;

    RawImage img;
    RectTransform rt;

    void Awake()
    {
        img = GetComponent<RawImage>();
        rt = GetComponent<RectTransform>();
        img.raycastTarget = false;

        var tex = new Texture2D(1, lineHeight, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Repeat;
        tex.filterMode = FilterMode.Point;

        for (int y = 0; y < lineHeight; y++)
            tex.SetPixel(0, y, y == 0 ? new Color(0, 0, 0, darkness) : Color.clear);
        tex.Apply();

        img.texture = tex;
        img.color = Color.white;
    }

    void Update()
    {
        float repeats = rt.rect.height / lineHeight;
        float offset = (Time.time * scrollSpeed) % 1f;
        img.uvRect = new Rect(0f, offset, 1f, repeats);
    }
}
using UnityEngine;

namespace EvenMoreBalancedMortician.Scepter;

internal static class ScepterIcon
{
    private const float ScepterPurpleHue = 0.75f;

    public static Sprite TintedCopyOf(Sprite source)
    {
        if (!source)
            return source;

        var texture = ReadableCopyOf(source.texture);
        texture.SetPixels32(TintAll(texture.GetPixels32()));
        texture.Apply();

        var pivot = new Vector2(source.pivot.x / source.rect.width, source.pivot.y / source.rect.height);
        return Sprite.Create(texture, source.textureRect, pivot, source.pixelsPerUnit);
    }

    // The bundled texture isn't CPU-readable, so it is copied through the GPU first.
    private static Texture2D ReadableCopyOf(Texture source)
    {
        var renderTexture = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(source, renderTexture);

        var previouslyActive = RenderTexture.active;
        RenderTexture.active = renderTexture;

        var copy = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        copy.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);

        RenderTexture.active = previouslyActive;
        RenderTexture.ReleaseTemporary(renderTexture);

        return copy;
    }

    private static Color32[] TintAll(Color32[] pixels)
    {
        for (var i = 0; i < pixels.Length; i++)
            pixels[i] = Tint(pixels[i]);

        return pixels;
    }

    private static Color32 Tint(Color32 pixel)
    {
        Color.RGBToHSV(pixel, out _, out var saturation, out var value);

        Color32 tinted = Color.HSVToRGB(ScepterPurpleHue, saturation, value);
        tinted.a = pixel.a;

        return tinted;
    }
}

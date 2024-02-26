using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class Extensions
{
    public static Texture2D AspectRatioFitter(this Texture2D texture, Vector2 targetSize)
    {
        float targetAspectRatio = targetSize.x / targetSize.y;
        float sourceAspectRatio = (float)texture.width / texture.height;
        Vector2Int newSize;

        if (sourceAspectRatio < targetAspectRatio)
        {
            // thumbnail width
            newSize = new Vector2Int((int)targetSize.x, 0);
            newSize.y = (int)(newSize.x / sourceAspectRatio);
        }
        else
        {
            // thumbnail height
            newSize = new Vector2Int(0, (int)targetSize.y);
            newSize.x = (int)(newSize.y * sourceAspectRatio);
        }

        return texture.ResizeTexture2D(newSize);
    }

    public static Texture2D ResizeTexture2D(this Texture2D texture, Vector2Int targetSize)
    {
        RenderTexture rt = RenderTexture.GetTemporary(targetSize.x, targetSize.y);
        RenderTexture.active = rt;
        Graphics.Blit(texture, rt);
        Texture2D nTex = new Texture2D(targetSize.x, targetSize.y);
        nTex.ReadPixels(new Rect(0, 0, targetSize.x, targetSize.y), 0, 0);
        nTex.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return nTex;
    }

    public static Sprite GetSprite(this Texture2D tex)
    {
        if (tex == null)
        {
            return null;
        }
        Sprite s = Sprite.Create(
            tex,
            new Rect(0.0f, 0.0f, tex.width, tex.height),
            new Vector2(0.5f, 0.5f),
            100.0f,
            0,
            SpriteMeshType.FullRect);
        s.name = tex.name;
        return s;
    }

    public static void WriteTextureOnDisk(this Texture2D texture, string fileName)
    {
        byte[] textureBytes = texture.EncodeToPNG();
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(path, textureBytes);
        Debug.Log("Saved image: " + fileName + " to: " + path);
    }

    public static Texture2D LoadTextureFromDisk(string fileName)
    {
        var path = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(path)) {
            return null;
        }
        byte[] textureBytes = File.ReadAllBytes(Path.Combine(Application.persistentDataPath, fileName));
        Texture2D loadedTexture = new Texture2D(2, 2);
        loadedTexture.LoadImage(textureBytes);
        return loadedTexture;
    }

    public static string Sha256(this string randomString)
    {
        var crypt = new System.Security.Cryptography.SHA256Managed();
        var hash = new System.Text.StringBuilder();
        byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
        foreach (byte theByte in crypto)
        {
            hash.Append(theByte.ToString("x2"));
        }
        return hash.ToString();
    }

    public static Color GetDominantColor(this Sprite sprite)
    {
        Texture2D tex = sprite.texture;
        Color[] texColors = tex.GetPixels();
        Dictionary<Color, int> colorCounter = new Dictionary<Color, int>();
        foreach (Color c in texColors)
        {
            if (c.a <= 0.5 || c == Color.white || c == Color.black)
            {
                continue;
            }
            if (colorCounter.ContainsKey(c))
            {
                colorCounter[c]++;
            }
            else
            {
                colorCounter.Add(c, 1);
            }
        }

        Color dominantColor = Color.white;
        if (colorCounter.Count <= 0)
        {
            return dominantColor;
        }
        int maxCount = 0;
        foreach (var color in colorCounter)
        {
            if (color.Value > maxCount)
            {
                maxCount = color.Value;
                dominantColor = color.Key;
            }
        }
        Color.RGBToHSV(dominantColor, out float h, out float s, out float v);
        v = Mathf.Min(v, 0.8f);
        dominantColor = Color.HSVToRGB(h, s, v);
        return dominantColor;
    }
}

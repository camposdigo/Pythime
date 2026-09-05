using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Pythime
{
    public static class EmbeddedArtLoader
    {
        private static readonly Dictionary<string, Texture2D> TextureCache = new Dictionary<string, Texture2D>();

        public static bool HasAsset(string assetKey)
        {
            if (string.IsNullOrWhiteSpace(assetKey)) return false;
            var parts = Resources.LoadAll<TextAsset>("PythimeArt/Embedded/" + assetKey);
            return parts != null && parts.Length > 0;
        }

        public static Texture2D LoadTexture(string assetKey)
        {
            if (string.IsNullOrWhiteSpace(assetKey)) return null;

            Texture2D cached;
            if (TextureCache.TryGetValue(assetKey, out cached) && cached != null)
                return cached;

            var parts = Resources.LoadAll<TextAsset>("PythimeArt/Embedded/" + assetKey);
            if (parts == null || parts.Length == 0) return null;

            Array.Sort(parts, (a, b) => string.CompareOrdinal(a.name, b.name));
            var base64 = new StringBuilder(parts.Length * 20000);
            foreach (var part in parts)
            {
                if (part == null || string.IsNullOrWhiteSpace(part.text)) continue;
                base64.Append(part.text.Trim());
            }

            try
            {
                var bytes = Convert.FromBase64String(base64.ToString());
                var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false)
                {
                    name = assetKey,
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp
                };

                if (!ImageConversion.LoadImage(texture, bytes, false))
                {
                    UnityEngine.Object.Destroy(texture);
                    return null;
                }

                texture.filterMode = FilterMode.Point;
                texture.wrapMode = TextureWrapMode.Clamp;
                TextureCache[assetKey] = texture;
                return texture;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Pythime: failed to decode embedded art '" + assetKey + "': " + ex.Message);
                return null;
            }
        }
    }
}

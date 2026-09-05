using System.Collections.Generic;
using UnityEngine;

namespace Pythime
{
    public sealed class PlayerEyeStyleRuntime : MonoBehaviour
    {
        private readonly HashSet<Sprite> patchedSprites = new HashSet<Sprite>();
        private SpriteRenderer avatar;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (GameObject.Find("PythimePlayerEyeStyle") != null) return;
            var root = new GameObject("PythimePlayerEyeStyle");
            root.AddComponent<PlayerEyeStyleRuntime>();
        }

        private void LateUpdate()
        {
            if (avatar == null)
            {
                var player = GameObject.Find("Player");
                if (player == null) return;
                var child = player.transform.Find("Avatar");
                if (child == null) return;
                avatar = child.GetComponent<SpriteRenderer>();
            }

            if (avatar == null || avatar.sprite == null || avatar.sprite.texture == null) return;

            var sprite = avatar.sprite;
            if (patchedSprites.Contains(sprite)) return;

            MatchNpcFace(sprite.texture);
            patchedSprites.Add(sprite);
        }

        private static void MatchNpcFace(Texture2D texture)
        {
            if (texture.width < 20 || texture.height < 28) return;

            var front = IsBright(texture.GetPixel(8, 24)) || IsBright(texture.GetPixel(13, 24));
            var left = !front && IsBright(texture.GetPixel(6, 24));
            var right = !front && !left && IsBright(texture.GetPixel(15, 24));
            if (!front && !left && !right) return;

            var skin = texture.GetPixel(12, 22);
            var eye = new Color32(24, 27, 31, 255);
            var mouth = new Color32(151, 82, 72, 255);

            if (front)
            {
                Paint(texture, 8, 23, 3, 2, skin);
                Paint(texture, 13, 23, 3, 2, skin);
                texture.SetPixel(9, 25, skin);
                texture.SetPixel(14, 25, skin);
                texture.SetPixel(11, 20, skin);
                texture.SetPixel(12, 20, skin);

                texture.SetPixel(9, 24, eye);
                texture.SetPixel(15, 24, eye);
                texture.SetPixel(11, 21, mouth);
                texture.SetPixel(12, 21, mouth);
            }
            else if (left)
            {
                Paint(texture, 6, 23, 3, 2, skin);
                texture.SetPixel(6, 20, skin);
                texture.SetPixel(7, 24, eye);
            }
            else
            {
                Paint(texture, 15, 23, 3, 2, skin);
                texture.SetPixel(17, 20, skin);
                texture.SetPixel(16, 24, eye);
            }

            texture.Apply(false, false);
        }

        private static bool IsBright(Color color)
        {
            return color.r > 0.82f && color.g > 0.82f && color.b > 0.78f && color.a > 0.9f;
        }

        private static void Paint(Texture2D texture, int x, int y, int width, int height, Color color)
        {
            for (var py = y; py < y + height; py++)
            for (var px = x; px < x + width; px++)
                texture.SetPixel(px, py, color);
        }
    }
}

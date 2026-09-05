using System.Collections.Generic;
using UnityEngine;

namespace Pythime
{
    public sealed class PlayerEyeStyleRuntime : MonoBehaviour
    {
        private readonly HashSet<int> patchedSprites = new HashSet<int>();
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
            var id = sprite.GetInstanceID();
            if (patchedSprites.Contains(id)) return;

            SoftenEyes(sprite.texture);
            patchedSprites.Add(id);
        }

        private static void SoftenEyes(Texture2D texture)
        {
            if (texture.width < 20 || texture.height < 28) return;

            var eye = new Color32(48, 35, 30, 255);
            var changed = false;

            for (var y = 20; y <= Mathf.Min(texture.height - 1, 26); y++)
            {
                for (var x = 5; x <= Mathf.Min(texture.width - 1, 18); x++)
                {
                    var c = texture.GetPixel(x, y);
                    if (c.r > 0.88f && c.g > 0.88f && c.b > 0.84f && c.a > 0.9f)
                    {
                        texture.SetPixel(x, y, eye);
                        changed = true;
                    }
                }
            }

            if (changed) texture.Apply(false, false);
        }
    }
}

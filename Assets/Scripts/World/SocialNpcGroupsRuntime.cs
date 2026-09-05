using System.Collections;
using UnityEngine;

namespace Pythime
{
    public sealed class SocialNpcGroupsRuntime : MonoBehaviour
    {
        private static readonly Vector2[] GroupTiles =
        {
            new Vector2(28.8f, 35.0f), new Vector2(30.0f, 35.2f),
            new Vector2(34.2f, 35.0f), new Vector2(35.3f, 35.2f),
            new Vector2(31.2f, 32.7f), new Vector2(32.5f, 32.8f),
            new Vector2(31.7f, 36.4f), new Vector2(33.0f, 36.3f)
        };

        private static readonly string[] Names1956 = { "Clara", "Bento", "Ester", "Nelson", "Rita", "Cícero", "Irene", "Milton" };
        private static readonly string[] Names2026 = { "Lia", "Enzo", "Ayla", "Gui", "Nanda", "Leo", "Jade", "Sam" };
        private static readonly string[] Names2096 = { "Nox", "Eli-4", "Cira", "Oryn", "Zaya", "M0", "Sia", "Tao" };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (GameObject.Find("PythimeSocialNpcGroups") != null) return;
            var root = new GameObject("PythimeSocialNpcGroups");
            root.AddComponent<SocialNpcGroupsRuntime>();
        }

        private IEnumerator Start()
        {
            for (var i = 0; i < 180; i++)
            {
                if (FindEra(2026) != null) break;
                yield return null;
            }

            BuildEra(1956, Names1956);
            BuildEra(2026, Names2026);
            BuildEra(2096, Names2096);
        }

        private static Transform FindEra(int year)
        {
            var runtime = GameObject.Find("PythimeRuntime");
            if (runtime == null) return null;
            return runtime.transform.Find("Era_" + year);
        }

        private static void BuildEra(int year, string[] names)
        {
            var era = FindEra(year);
            if (era == null || era.Find("SocialNPCs") != null) return;

            var group = new GameObject("SocialNPCs");
            group.transform.SetParent(era);

            for (var i = 0; i < GroupTiles.Length; i++)
            {
                var npc = new GameObject("SocialNPC_" + names[i]);
                npc.transform.SetParent(group.transform);
                npc.transform.localPosition = StoryWorldFactory.TileToWorld(GroupTiles[i].x, GroupTiles[i].y);

                var shadowObject = new GameObject("Shadow");
                shadowObject.transform.SetParent(npc.transform);
                shadowObject.transform.localPosition = new Vector3(0f, 0.07f, 0f);
                var shadow = shadowObject.AddComponent<SpriteRenderer>();
                shadow.sprite = PixelArtFactory.CreateShadowSprite();
                shadow.color = new Color(1f, 1f, 1f, 0.62f);
                shadow.sortingOrder = 60;

                var visualObject = new GameObject("Visual");
                visualObject.transform.SetParent(npc.transform);
                var renderer = visualObject.AddComponent<SpriteRenderer>();
                renderer.sprite = SocialNpcSpriteFactory.Create(year, i);

                if (i == 0 || i == 4)
                {
                    var walker = npc.AddComponent<NpcWalker>();
                    walker.Initialize(renderer, visualObject.transform, 70 + i, year);
                }
                else
                {
                    var idle = npc.AddComponent<SocialNpcIdle>();
                    idle.Initialize(renderer, visualObject.transform, i);
                }

                var tag = npc.AddComponent<NpcNameTag>();
                tag.Initialize(names[i], year);
            }
        }
    }

    public sealed class SocialNpcIdle : MonoBehaviour
    {
        private SpriteRenderer rendererTarget;
        private Transform visual;
        private float phase;

        public void Initialize(SpriteRenderer rendererValue, Transform visualTransform, int index)
        {
            rendererTarget = rendererValue;
            visual = visualTransform;
            phase = index * 0.71f;
        }

        private void Update()
        {
            if (rendererTarget == null) return;
            rendererTarget.sortingOrder = 74 - Mathf.RoundToInt(transform.position.y * 3f);
            if (visual != null)
                visual.localPosition = new Vector3(0f, Mathf.Sin(Time.time * 2.2f + phase) * 0.012f, 0f);
        }
    }

    public static class SocialNpcSpriteFactory
    {
        private static readonly Color32[] Skins =
        {
            new Color32(244, 202, 166, 255), new Color32(220, 169, 129, 255), new Color32(190, 133, 91, 255),
            new Color32(139, 89, 59, 255), new Color32(82, 52, 41, 255)
        };

        private static readonly Color32[] HairColors =
        {
            new Color32(32, 28, 27, 255), new Color32(86, 48, 31, 255), new Color32(197, 151, 65, 255),
            new Color32(159, 70, 42, 255), new Color32(183, 184, 188, 255), new Color32(87, 53, 43, 255),
            new Color32(74, 85, 119, 255), new Color32(121, 77, 49, 255)
        };

        public static Sprite Create(int year, int variant)
        {
            const int width = 26;
            const int height = 32;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = "SocialNpc_" + year + "_" + variant
            };
            Clear(texture);

            var outline = new Color32(24, 27, 31, 255);
            var skin = Skins[(variant * 2 + year) % Skins.Length];
            var skinDark = Darken(skin, 0.14f);
            var hair = HairColors[(variant * 3 + year) % HairColors.Length];
            var shirt = Shirt(year, variant);
            var pants = Pants(year, variant);
            var shoe = variant % 3 == 0 ? new Color32(229, 227, 220, 255) : new Color32(43, 46, 53, 255);

            DrawLeg(texture, 8, 2, pants, shoe, outline);
            DrawLeg(texture, 14, 2, pants, shoe, outline);

            FillRect(texture, 8, 9, 10, 9, outline);
            FillRect(texture, 7, 11, 12, 5, outline);
            FillRect(texture, 9, 10, 8, 7, shirt);
            FillRect(texture, 8, 12, 10, 4, shirt);

            FillRect(texture, 5, 11, 3, 7, outline);
            FillRect(texture, 6, 13, 2, 3, shirt);
            texture.SetPixel(6, 12, skin);
            FillRect(texture, 18, 11, 3, 7, outline);
            FillRect(texture, 18, 13, 2, 3, Darken(shirt, 0.08f));
            texture.SetPixel(19, 12, skinDark);

            FillEllipse(texture, 13, 24, 8, 7, outline);
            FillEllipse(texture, 13, 24, 7, 6, skin);

            DrawHair(texture, hair, variant, outline);
            DrawFace(texture, variant, outline);
            DrawAccessory(texture, year, variant, outline);

            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.07f), 16f);
            sprite.name = texture.name;
            return sprite;
        }

        private static void DrawHair(Texture2D t, Color32 hair, int variant, Color32 outline)
        {
            switch (variant % 8)
            {
                case 0:
                    FillEllipse(t, 13, 29, 7, 3, outline);
                    FillEllipse(t, 13, 29, 6, 2, hair);
                    FillRect(t, 7, 27, 3, 2, hair);
                    break;
                case 1:
                    FillEllipse(t, 13, 29, 8, 3, outline);
                    FillEllipse(t, 13, 29, 7, 2, hair);
                    FillRect(t, 6, 27, 4, 3, hair);
                    FillRect(t, 16, 28, 4, 2, hair);
                    t.SetPixel(10, 31, hair);
                    t.SetPixel(17, 31, hair);
                    break;
                case 2:
                    FillEllipse(t, 8, 28, 4, 4, outline);
                    FillEllipse(t, 13, 30, 5, 3, outline);
                    FillEllipse(t, 18, 28, 4, 4, outline);
                    FillEllipse(t, 8, 28, 3, 3, hair);
                    FillEllipse(t, 13, 30, 4, 2, hair);
                    FillEllipse(t, 18, 28, 3, 3, hair);
                    break;
                case 3:
                    FillEllipse(t, 13, 28, 9, 6, outline);
                    FillEllipse(t, 13, 28, 8, 5, hair);
                    FillEllipse(t, 7, 25, 3, 3, hair);
                    FillEllipse(t, 19, 25, 3, 3, hair);
                    break;
                case 4:
                    FillEllipse(t, 13, 29, 7, 3, outline);
                    FillEllipse(t, 13, 29, 6, 2, hair);
                    FillRect(t, 6, 23, 3, 8, hair);
                    FillRect(t, 18, 23, 3, 8, hair);
                    break;
                case 5:
                    FillEllipse(t, 13, 29, 7, 3, outline);
                    FillEllipse(t, 13, 29, 6, 2, hair);
                    FillRect(t, 6, 24, 4, 6, hair);
                    FillRect(t, 17, 24, 4, 6, hair);
                    FillRect(t, 8, 23, 11, 2, hair);
                    break;
                case 6:
                    FillEllipse(t, 13, 29, 7, 3, outline);
                    FillEllipse(t, 13, 29, 6, 2, hair);
                    FillEllipse(t, 21, 25, 3, 4, outline);
                    FillEllipse(t, 21, 25, 2, 3, hair);
                    FillRect(t, 18, 27, 3, 2, hair);
                    break;
                default:
                    FillRect(t, 10, 27, 7, 4, outline);
                    FillRect(t, 11, 27, 5, 4, hair);
                    t.SetPixel(12, 31, hair);
                    t.SetPixel(13, 31, hair);
                    t.SetPixel(14, 31, hair);
                    break;
            }
        }

        private static void DrawFace(Texture2D t, int variant, Color32 outline)
        {
            var eye = variant % 4 == 0 ? new Color32(52, 41, 34, 255) : outline;
            t.SetPixel(10, 24, eye);
            t.SetPixel(16, 24, eye);
            if (variant % 3 == 1)
            {
                var mouth = new Color32(151, 82, 72, 255);
                t.SetPixel(12, 21, mouth);
                t.SetPixel(13, 21, mouth);
            }
        }

        private static void DrawAccessory(Texture2D t, int year, int variant, Color32 outline)
        {
            if (variant == 1)
            {
                FillRect(t, 7, 24, 12, 1, outline);
            }
            else if (variant == 2)
            {
                FillRect(t, 20, 12, 3, 5, outline);
                FillRect(t, 21, 13, 1, 3, new Color32(99, 68, 47, 255));
            }
            else if (variant == 5)
            {
                FillRect(t, 7, 29, 12, 2, outline);
                FillRect(t, 9, 30, 8, 1, year == 2096 ? new Color32(70, 230, 226, 255) : new Color32(120, 76, 48, 255));
            }
            else if (year == 2096 && variant % 2 == 0)
            {
                FillRect(t, 8, 12, 10, 1, new Color32(70, 230, 226, 255));
            }
        }

        private static Color32 Shirt(int year, int variant)
        {
            if (year == 1956)
            {
                var colors = new[]
                {
                    new Color32(167, 73, 58, 255), new Color32(215, 192, 139, 255), new Color32(75, 106, 92, 255),
                    new Color32(91, 98, 126, 255), new Color32(184, 142, 76, 255), new Color32(112, 78, 64, 255)
                };
                return colors[variant % colors.Length];
            }

            if (year == 2096)
            {
                var colors = new[]
                {
                    new Color32(68, 78, 128, 255), new Color32(48, 128, 126, 255), new Color32(119, 72, 154, 255),
                    new Color32(58, 92, 139, 255), new Color32(117, 55, 103, 255), new Color32(56, 66, 89, 255)
                };
                return colors[variant % colors.Length];
            }

            var modern = new[]
            {
                new Color32(58, 112, 179, 255), new Color32(195, 75, 66, 255), new Color32(67, 142, 91, 255),
                new Color32(137, 86, 165, 255), new Color32(219, 157, 57, 255), new Color32(51, 55, 62, 255),
                new Color32(76, 129, 143, 255), new Color32(151, 87, 102, 255)
            };
            return modern[variant % modern.Length];
        }

        private static Color32 Pants(int year, int variant)
        {
            if (year == 1956) return variant % 2 == 0 ? new Color32(107, 82, 63, 255) : new Color32(75, 80, 86, 255);
            if (year == 2096) return variant % 2 == 0 ? new Color32(48, 54, 73, 255) : new Color32(73, 61, 101, 255);
            return variant % 2 == 0 ? new Color32(52, 76, 119, 255) : new Color32(44, 48, 57, 255);
        }

        private static void DrawLeg(Texture2D t, int x, int y, Color32 pants, Color32 shoe, Color32 outline)
        {
            FillRect(t, x, y + 2, 4, 7, outline);
            FillRect(t, x + 1, y + 3, 2, 5, pants);
            FillRect(t, x - 1, y, 5, 3, outline);
            FillRect(t, x, y + 1, 4, 1, shoe);
        }

        private static void Clear(Texture2D texture)
        {
            var pixels = new Color32[texture.width * texture.height];
            for (var i = 0; i < pixels.Length; i++) pixels[i] = new Color32(0, 0, 0, 0);
            texture.SetPixels32(pixels);
        }

        private static void FillRect(Texture2D texture, int x, int y, int width, int height, Color32 color)
        {
            for (var py = y; py < y + height; py++)
            for (var px = x; px < x + width; px++)
                if (px >= 0 && py >= 0 && px < texture.width && py < texture.height)
                    texture.SetPixel(px, py, color);
        }

        private static void FillEllipse(Texture2D texture, int cx, int cy, int rx, int ry, Color32 color)
        {
            for (var y = cy - ry; y <= cy + ry; y++)
            for (var x = cx - rx; x <= cx + rx; x++)
            {
                var nx = (x - cx) / (float)rx;
                var ny = (y - cy) / (float)ry;
                if (nx * nx + ny * ny > 1f) continue;
                if (x >= 0 && y >= 0 && x < texture.width && y < texture.height)
                    texture.SetPixel(x, y, color);
            }
        }

        private static Color32 Darken(Color32 color, float amount)
        {
            return new Color32((byte)(color.r * (1f - amount)), (byte)(color.g * (1f - amount)), (byte)(color.b * (1f - amount)), color.a);
        }
    }
}

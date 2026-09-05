using System.Collections;
using UnityEngine;

namespace Pythime
{
    public sealed class NpcVisualVariationRuntime : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (GameObject.Find("PythimeNpcVisualVariation") != null) return;
            var root = new GameObject("PythimeNpcVisualVariation");
            root.AddComponent<NpcVisualVariationRuntime>();
        }

        private IEnumerator Start()
        {
            for (var frame = 0; frame < 180; frame++)
            {
                var a = ApplyEra(1956);
                var b = ApplyEra(2026);
                var c = ApplyEra(2096);
                if (a && b && c) yield break;
                yield return null;
            }
        }

        private static bool ApplyEra(int year)
        {
            var runtime = GameObject.Find("PythimeRuntime");
            if (runtime == null) return false;
            var era = runtime.transform.Find("Era_" + year);
            if (era == null) return false;
            var group = era.Find("NPCs");
            if (group == null || group.childCount == 0) return false;

            for (var i = 0; i < group.childCount; i++)
            {
                var npc = group.GetChild(i);
                var visual = npc.Find("Visual");
                if (visual == null) continue;
                var renderer = visual.GetComponent<SpriteRenderer>();
                if (renderer == null) continue;
                renderer.sprite = BuildNpc(year, i);
            }

            return true;
        }

        private static Sprite BuildNpc(int year, int variant)
        {
            const int width = 26;
            const int height = 32;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = "NpcVaried_" + year + "_" + variant
            };
            Clear(texture);

            var outline = new Color32(24, 27, 31, 255);
            var skin = Skin(variant);
            var skinDark = Darken(skin, 0.15f);
            var hair = HairColor(year, variant);
            var shirt = Shirt(year, variant);
            var pants = Pants(year, variant);
            var shoes = variant % 4 == 0 ? new Color32(225, 223, 215, 255) : new Color32(43, 46, 52, 255);

            DrawLeg(texture, 7, 2, pants, shoes, outline);
            DrawLeg(texture, 14, 2, pants, shoes, outline);

            FillRect(texture, 7, 9, 12, 9, outline);
            FillRect(texture, 8, 10, 10, 7, shirt);
            FillRect(texture, 6, 11, 2, 6, outline);
            FillRect(texture, 7, 12, 1, 4, shirt);
            texture.SetPixel(7, 11, skin);
            FillRect(texture, 19, 11, 2, 6, outline);
            FillRect(texture, 19, 12, 1, 4, Darken(shirt, 0.08f));
            texture.SetPixel(19, 11, skinDark);

            FillEllipse(texture, 13, 24, 8, 7, outline);
            FillEllipse(texture, 13, 24, 7, 6, skin);

            DrawHair(texture, hair, variant, outline);
            DrawFace(texture, variant, outline);
            DrawEraDetail(texture, year, variant, outline);

            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.07f), 16f);
            sprite.name = texture.name;
            return sprite;
        }

        private static void DrawHair(Texture2D t, Color32 hair, int variant, Color32 outline)
        {
            switch (variant % 12)
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
                    t.SetPixel(9, 31, hair);
                    t.SetPixel(17, 31, hair);
                    break;
                case 2:
                    FillEllipse(t, 13, 29, 7, 3, outline);
                    FillEllipse(t, 13, 29, 6, 2, hair);
                    FillRect(t, 7, 25, 3, 5, hair);
                    FillRect(t, 17, 27, 2, 3, Darken(hair, 0.16f));
                    break;
                case 3:
                    FillEllipse(t, 8, 28, 4, 4, outline);
                    FillEllipse(t, 13, 30, 5, 3, outline);
                    FillEllipse(t, 18, 28, 4, 4, outline);
                    FillEllipse(t, 8, 28, 3, 3, hair);
                    FillEllipse(t, 13, 30, 4, 2, hair);
                    FillEllipse(t, 18, 28, 3, 3, hair);
                    break;
                case 4:
                    FillEllipse(t, 13, 28, 9, 6, outline);
                    FillEllipse(t, 13, 28, 8, 5, hair);
                    FillEllipse(t, 7, 25, 3, 3, hair);
                    FillEllipse(t, 19, 25, 3, 3, hair);
                    break;
                case 5:
                    FillEllipse(t, 13, 29, 7, 3, outline);
                    FillEllipse(t, 13, 29, 6, 2, hair);
                    FillRect(t, 6, 23, 3, 8, hair);
                    FillRect(t, 18, 23, 3, 8, hair);
                    break;
                case 6:
                    FillEllipse(t, 13, 29, 7, 3, outline);
                    FillEllipse(t, 13, 29, 6, 2, hair);
                    FillRect(t, 6, 24, 4, 6, hair);
                    FillRect(t, 17, 24, 4, 6, hair);
                    FillRect(t, 8, 23, 11, 2, hair);
                    break;
                case 7:
                    FillEllipse(t, 13, 29, 7, 3, outline);
                    FillEllipse(t, 13, 29, 6, 2, hair);
                    FillEllipse(t, 21, 25, 3, 4, outline);
                    FillEllipse(t, 21, 25, 2, 3, hair);
                    FillRect(t, 18, 27, 3, 2, hair);
                    break;
                case 8:
                    FillEllipse(t, 13, 29, 7, 3, outline);
                    FillEllipse(t, 13, 29, 6, 2, hair);
                    FillEllipse(t, 13, 31, 4, 2, outline);
                    FillEllipse(t, 13, 31, 3, 1, hair);
                    break;
                case 9:
                    FillRect(t, 10, 27, 7, 4, outline);
                    FillRect(t, 11, 27, 5, 4, hair);
                    t.SetPixel(12, 31, hair);
                    t.SetPixel(13, 31, hair);
                    t.SetPixel(14, 31, hair);
                    break;
                case 10:
                    FillEllipse(t, 13, 29, 7, 3, outline);
                    FillEllipse(t, 13, 29, 6, 2, hair);
                    FillRect(t, 7, 26, 11, 3, hair);
                    FillRect(t, 7, 24, 4, 3, hair);
                    break;
                default:
                    FillEllipse(t, 8, 29, 4, 3, outline);
                    FillEllipse(t, 13, 30, 5, 3, outline);
                    FillEllipse(t, 18, 29, 4, 3, outline);
                    FillEllipse(t, 8, 29, 3, 2, hair);
                    FillEllipse(t, 13, 30, 4, 2, hair);
                    FillEllipse(t, 18, 29, 3, 2, hair);
                    FillRect(t, 6, 25, 3, 4, hair);
                    break;
            }
        }

        private static void DrawFace(Texture2D t, int variant, Color32 outline)
        {
            var eye = variant % 5 == 0 ? new Color32(52, 41, 34, 255) : outline;
            t.SetPixel(10, 24, eye);
            t.SetPixel(16, 24, eye);
            if (variant % 3 == 0)
            {
                t.SetPixel(11, 21, new Color32(151, 82, 72, 255));
                t.SetPixel(12, 21, new Color32(151, 82, 72, 255));
            }
        }

        private static void DrawEraDetail(Texture2D t, int year, int variant, Color32 outline)
        {
            if (year == 1956)
            {
                if (variant % 3 == 0)
                {
                    FillRect(t, 7, 29, 12, 2, outline);
                    FillRect(t, 9, 30, 8, 1, new Color32(96, 67, 47, 255));
                }
                else if (variant % 3 == 1)
                {
                    FillRect(t, 8, 15, 10, 1, new Color32(222, 200, 153, 255));
                }
            }
            else if (year == 2026)
            {
                if (variant % 4 == 1)
                {
                    FillRect(t, 20, 12, 3, 5, outline);
                    FillRect(t, 21, 13, 1, 3, new Color32(89, 63, 45, 255));
                }
                else if (variant % 4 == 2)
                {
                    FillRect(t, 8, 24, 10, 1, new Color32(52, 57, 65, 255));
                }
            }
            else
            {
                var cyan = new Color32(70, 230, 226, 255);
                if (variant % 2 == 0)
                {
                    FillRect(t, 8, 11, 10, 1, cyan);
                    t.SetPixel(20, 14, cyan);
                }
                else
                {
                    FillRect(t, 8, 22, 10, 1, cyan);
                }
            }
        }

        private static Color32 Skin(int variant)
        {
            var skins = new[]
            {
                new Color32(244, 202, 166, 255), new Color32(220, 169, 129, 255), new Color32(190, 133, 91, 255),
                new Color32(158, 105, 72, 255), new Color32(122, 78, 55, 255), new Color32(82, 52, 41, 255)
            };
            return skins[(variant * 5 + 1) % skins.Length];
        }

        private static Color32 HairColor(int year, int variant)
        {
            var colors = new[]
            {
                new Color32(31, 27, 25, 255), new Color32(73, 42, 27, 255), new Color32(113, 68, 39, 255),
                new Color32(205, 159, 72, 255), new Color32(157, 66, 39, 255), new Color32(173, 174, 177, 255),
                new Color32(67, 54, 46, 255), new Color32(53, 77, 129, 255), new Color32(132, 66, 118, 255),
                new Color32(35, 93, 85, 255)
            };
            var offset = year == 1956 ? 0 : year == 2026 ? 2 : 4;
            return colors[(variant * 3 + offset) % colors.Length];
        }

        private static Color32 Shirt(int year, int variant)
        {
            if (year == 1956)
            {
                var colors = new[] { C(174, 76, 59), C(215, 191, 135), C(73, 104, 93), C(87, 95, 126), C(188, 145, 72), C(110, 74, 62) };
                return colors[variant % colors.Length];
            }
            if (year == 2096)
            {
                var colors = new[] { C(69, 77, 127), C(47, 128, 126), C(119, 72, 154), C(58, 92, 139), C(117, 55, 103), C(67, 104, 112) };
                return colors[variant % colors.Length];
            }
            var modern = new[] { C(58, 112, 179), C(195, 75, 66), C(67, 142, 91), C(137, 86, 165), C(219, 157, 57), C(51, 55, 62), C(68, 136, 149) };
            return modern[variant % modern.Length];
        }

        private static Color32 Pants(int year, int variant)
        {
            if (year == 1956) return variant % 3 == 0 ? C(107, 82, 63) : variant % 3 == 1 ? C(75, 80, 86) : C(118, 102, 78);
            if (year == 2096) return variant % 3 == 0 ? C(48, 54, 73) : variant % 3 == 1 ? C(73, 61, 101) : C(45, 77, 83);
            return variant % 3 == 0 ? C(52, 76, 119) : variant % 3 == 1 ? C(44, 48, 57) : C(101, 80, 60);
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

        private static void FillRect(Texture2D t, int x, int y, int width, int height, Color32 color)
        {
            for (var py = y; py < y + height; py++)
            for (var px = x; px < x + width; px++)
                if (px >= 0 && py >= 0 && px < t.width && py < t.height) t.SetPixel(px, py, color);
        }

        private static void FillEllipse(Texture2D t, int centerX, int centerY, int radiusX, int radiusY, Color32 color)
        {
            if (radiusX <= 0 || radiusY <= 0) return;
            for (var y = centerY - radiusY; y <= centerY + radiusY; y++)
            for (var x = centerX - radiusX; x <= centerX + radiusX; x++)
            {
                var nx = (x - centerX) / (float)radiusX;
                var ny = (y - centerY) / (float)radiusY;
                if (nx * nx + ny * ny <= 1f && x >= 0 && y >= 0 && x < t.width && y < t.height) t.SetPixel(x, y, color);
            }
        }

        private static Color32 Darken(Color32 color, float amount)
        {
            return new Color32((byte)(color.r * (1f - amount)), (byte)(color.g * (1f - amount)), (byte)(color.b * (1f - amount)), color.a);
        }

        private static Color32 C(byte r, byte g, byte b)
        {
            return new Color32(r, g, b, 255);
        }
    }
}

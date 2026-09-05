using System.Collections;
using UnityEngine;

namespace Pythime
{
    public sealed class AreaPolishRuntime : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (GameObject.Find("PythimeAreaPolish") != null) return;
            var root = new GameObject("PythimeAreaPolish");
            root.AddComponent<AreaPolishRuntime>();
        }

        private IEnumerator Start()
        {
            GameObject player = null;
            for (var i = 0; i < 90; i++)
            {
                player = GameObject.Find("Player");
                if (player != null && GameObject.Find("Era_2026") != null) break;
                yield return null;
            }

            if (player != null && Vector2.Distance(player.transform.position, StoryWorldFactory.StartPoint) < 0.8f)
                player.transform.position = StoryWorldFactory.TileToWorld(14f, 10f);

            DecorateEra(1956);
            DecorateEra(2026);
            DecorateEra(2096);
        }

        private static void DecorateEra(int year)
        {
            var era = GameObject.Find("Era_" + year);
            if (era == null || era.transform.Find("AreaPolish") != null) return;

            var root = new GameObject("AreaPolish");
            root.transform.SetParent(era.transform);

            if (year == 1956)
            {
                Create(root.transform, "CafeTable", StoryWorldFactory.TileToWorld(14f, 11f), BuildCafeTable(new Color32(147, 92, 54, 255)), 14);
                Create(root.transform, "NewspaperStand", StoryWorldFactory.TileToWorld(16f, 10f), BuildKiosk(new Color32(181, 74, 52, 255), new Color32(238, 217, 166, 255)), 14);
                Create(root.transform, "StreetBench", StoryWorldFactory.TileToWorld(13f, 14f), BuildBench(new Color32(120, 79, 45, 255)), 13);
                Create(root.transform, "WoodBarrier", StoryWorldFactory.TileToWorld(18f, 9f), BuildBarrier(new Color32(183, 126, 65, 255)), 13);
                Create(root.transform, "FlowerPatch", StoryWorldFactory.TileToWorld(11f, 10f), BuildPlanter(new Color32(124, 84, 52, 255), new Color32(70, 133, 69, 255)), 12);
            }
            else if (year == 2026)
            {
                Create(root.transform, "CafeTable", StoryWorldFactory.TileToWorld(14f, 11f), BuildCafeTable(new Color32(91, 98, 103, 255)), 14);
                Create(root.transform, "InfoKiosk", StoryWorldFactory.TileToWorld(16f, 10f), BuildKiosk(new Color32(45, 111, 157, 255), new Color32(211, 235, 239, 255)), 14);
                Create(root.transform, "StreetBench", StoryWorldFactory.TileToWorld(13f, 14f), BuildBench(new Color32(91, 101, 106, 255)), 13);
                Create(root.transform, "BikeRack", StoryWorldFactory.TileToWorld(18f, 9f), BuildBikeRack(), 13);
                Create(root.transform, "Planter", StoryWorldFactory.TileToWorld(11f, 10f), BuildPlanter(new Color32(150, 154, 151, 255), new Color32(44, 128, 69, 255)), 12);
            }
            else
            {
                Create(root.transform, "HoloTable", StoryWorldFactory.TileToWorld(14f, 11f), BuildCafeTable(new Color32(80, 232, 224, 255)), 14);
                Create(root.transform, "HoloKiosk", StoryWorldFactory.TileToWorld(16f, 10f), BuildKiosk(new Color32(79, 61, 127, 255), new Color32(80, 232, 224, 255)), 14);
                Create(root.transform, "EnergyBench", StoryWorldFactory.TileToWorld(13f, 14f), BuildBench(new Color32(80, 232, 224, 255)), 13);
                Create(root.transform, "ChargeRack", StoryWorldFactory.TileToWorld(18f, 9f), BuildBikeRack(), 13);
                Create(root.transform, "SyntheticPlanter", StoryWorldFactory.TileToWorld(11f, 10f), BuildPlanter(new Color32(61, 66, 82, 255), new Color32(75, 195, 153, 255)), 12);
            }

            Create(root.transform, "TrashBinA", StoryWorldFactory.TileToWorld(20f, 17f), BuildBin(year), 12);
            Create(root.transform, "TrashBinB", StoryWorldFactory.TileToWorld(43f, 18f), BuildBin(year), 12);
            Create(root.transform, "CornerBench", StoryWorldFactory.TileToWorld(48f, 25f), BuildBench(year == 1956 ? new Color32(120, 79, 45, 255) : new Color32(84, 92, 101, 255)), 13);
        }

        private static void Create(Transform parent, string name, Vector2 position, Sprite sprite, int sortingOrder)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.localPosition = position;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
        }

        private static Sprite BuildCafeTable(Color32 accent)
        {
            var t = NewTexture(32, 26, "CafeTable");
            var outline = new Color32(28, 31, 34, 255);
            var shadow = new Color32(19, 22, 25, 75);
            FillEllipse(t, 16, 5, 11, 3, shadow);
            FillRect(t, 14, 5, 4, 11, outline);
            FillRect(t, 15, 6, 2, 10, Darken(accent, 0.25f));
            FillEllipse(t, 16, 17, 12, 6, outline);
            FillEllipse(t, 16, 17, 10, 4, accent);
            FillRect(t, 11, 19, 10, 1, Lighten(accent, 0.18f));
            return Finish(t, 0.5f, 0.08f);
        }

        private static Sprite BuildKiosk(Color32 body, Color32 screen)
        {
            var t = NewTexture(30, 42, "StreetKiosk");
            var outline = new Color32(27, 30, 34, 255);
            FillEllipse(t, 15, 4, 10, 3, new Color32(15, 18, 22, 70));
            FillRect(t, 6, 6, 18, 31, outline);
            FillRect(t, 8, 8, 14, 27, body);
            FillRect(t, 9, 23, 12, 9, outline);
            FillRect(t, 10, 24, 10, 7, screen);
            FillRect(t, 11, 26, 8, 1, Lighten(screen, 0.22f));
            FillRect(t, 10, 11, 10, 4, Darken(body, 0.18f));
            return Finish(t, 0.5f, 0.04f);
        }

        private static Sprite BuildBench(Color32 seat)
        {
            var t = NewTexture(42, 24, "StreetBench");
            var outline = new Color32(31, 34, 36, 255);
            FillEllipse(t, 21, 4, 17, 3, new Color32(18, 20, 22, 65));
            FillRect(t, 5, 8, 32, 6, outline);
            FillRect(t, 7, 10, 28, 3, seat);
            FillRect(t, 8, 14, 26, 6, outline);
            FillRect(t, 9, 15, 24, 3, Lighten(seat, 0.08f));
            FillRect(t, 8, 4, 3, 5, outline);
            FillRect(t, 31, 4, 3, 5, outline);
            return Finish(t, 0.5f, 0.06f);
        }

        private static Sprite BuildBarrier(Color32 wood)
        {
            var t = NewTexture(38, 24, "Barrier");
            var outline = new Color32(38, 35, 31, 255);
            FillRect(t, 5, 5, 4, 15, outline);
            FillRect(t, 29, 5, 4, 15, outline);
            FillRect(t, 3, 12, 32, 6, outline);
            FillRect(t, 5, 14, 28, 2, wood);
            FillRect(t, 7, 6, 2, 6, wood);
            FillRect(t, 29, 6, 2, 6, wood);
            return Finish(t, 0.5f, 0.05f);
        }

        private static Sprite BuildBikeRack()
        {
            var t = NewTexture(38, 22, "BikeRack");
            var metal = new Color32(89, 99, 107, 255);
            var dark = new Color32(32, 36, 40, 255);
            FillEllipse(t, 19, 3, 15, 2, new Color32(17, 20, 23, 60));
            for (var i = 0; i < 4; i++)
            {
                var x = 5 + i * 8;
                FillRect(t, x, 5, 2, 11, dark);
                FillRect(t, x + 1, 6, 1, 9, metal);
                FillRect(t, x, 14, 7, 2, dark);
            }
            return Finish(t, 0.5f, 0.05f);
        }

        private static Sprite BuildPlanter(Color32 pot, Color32 leaves)
        {
            var t = NewTexture(28, 32, "Planter");
            var outline = new Color32(30, 34, 34, 255);
            FillEllipse(t, 14, 4, 10, 3, new Color32(16, 20, 20, 65));
            FillRect(t, 6, 5, 16, 10, outline);
            FillRect(t, 8, 7, 12, 6, pot);
            FillEllipse(t, 14, 20, 9, 10, outline);
            FillEllipse(t, 14, 21, 8, 9, leaves);
            FillEllipse(t, 9, 18, 5, 6, leaves);
            FillEllipse(t, 19, 18, 5, 6, Darken(leaves, 0.08f));
            t.SetPixel(12, 26, Lighten(leaves, 0.18f));
            t.SetPixel(17, 23, Lighten(leaves, 0.18f));
            return Finish(t, 0.5f, 0.05f);
        }

        private static Sprite BuildBin(int year)
        {
            var body = year == 1956
                ? new Color32(111, 91, 69, 255)
                : year == 2096 ? new Color32(70, 75, 93, 255) : new Color32(72, 88, 92, 255);
            var accent = year == 2096 ? new Color32(80, 232, 224, 255) : Lighten(body, 0.2f);
            var t = NewTexture(18, 24, "StreetBin");
            var outline = new Color32(28, 31, 34, 255);
            FillEllipse(t, 9, 3, 6, 2, new Color32(16, 18, 21, 65));
            FillRect(t, 3, 5, 12, 15, outline);
            FillRect(t, 5, 7, 8, 11, body);
            FillRect(t, 4, 19, 10, 3, outline);
            FillRect(t, 6, 19, 6, 1, accent);
            return Finish(t, 0.5f, 0.06f);
        }

        private static Texture2D NewTexture(int width, int height, string name)
        {
            var t = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = name
            };
            var pixels = new Color32[width * height];
            var clear = new Color32(0, 0, 0, 0);
            for (var i = 0; i < pixels.Length; i++) pixels[i] = clear;
            t.SetPixels32(pixels);
            return t;
        }

        private static Sprite Finish(Texture2D texture, float pivotX, float pivotY)
        {
            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(pivotX, pivotY), 16f);
            sprite.name = texture.name;
            return sprite;
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
            if (rx <= 0 || ry <= 0) return;
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

        private static Color32 Lighten(Color32 color, float amount)
        {
            return new Color32(
                (byte)Mathf.Clamp(color.r + (255 - color.r) * amount, 0f, 255f),
                (byte)Mathf.Clamp(color.g + (255 - color.g) * amount, 0f, 255f),
                (byte)Mathf.Clamp(color.b + (255 - color.b) * amount, 0f, 255f),
                color.a);
        }
    }
}

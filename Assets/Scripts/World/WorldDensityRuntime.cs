using System.Collections;
using UnityEngine;

namespace Pythime
{
    public sealed class WorldDensityRuntime : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (OfficialEraMapRuntime.IsAvailable) return;
            if (GameObject.Find("PythimeWorldDensity") != null) return;
            var root = new GameObject("PythimeWorldDensity");
            root.AddComponent<WorldDensityRuntime>();
        }

        private IEnumerator Start()
        {
            GameObject player = null;
            for (var i = 0; i < 180; i++)
            {
                player = GameObject.Find("Player");
                if (player != null && FindEra(2026) != null) break;
                yield return null;
            }

            DecorateEra(1956);
            DecorateEra(2026);
            DecorateEra(2096);

            for (var i = 0; i < 18; i++) yield return null;

            if (player != null)
            {
                player.transform.position = StoryWorldFactory.TileToWorld(30f, 34f);
                var body = player.GetComponent<Rigidbody2D>();
                if (body != null) body.linearVelocity = Vector2.zero;
            }
        }

        private static GameObject FindEra(int year)
        {
            var runtime = GameObject.Find("PythimeRuntime");
            if (runtime == null) return null;
            var child = runtime.transform.Find("Era_" + year);
            return child != null ? child.gameObject : null;
        }

        private static void DecorateEra(int year)
        {
            var era = FindEra(year);
            if (era == null || era.transform.Find("ReadableHub") != null) return;

            var root = new GameObject("ReadableHub");
            root.transform.SetParent(era.transform);

            Create(root.transform, "CentralPlaza", StoryWorldFactory.ClockPlazaPoint, BuildPlaza(year), -18);

            var benchColor = year == 1956
                ? new Color32(125, 79, 47, 255)
                : year == 2096 ? new Color32(73, 220, 218, 255) : new Color32(79, 88, 96, 255);
            var planterColor = year == 1956
                ? new Color32(151, 97, 61, 255)
                : year == 2096 ? new Color32(67, 74, 92, 255) : new Color32(137, 143, 146, 255);
            var leaves = year == 2096
                ? new Color32(78, 203, 156, 255)
                : new Color32(64, 137, 75, 255);

            Create(root.transform, "BenchWest", StoryWorldFactory.TileToWorld(28f, 34f), BuildBench(benchColor), 14);
            Create(root.transform, "BenchEast", StoryWorldFactory.TileToWorld(36f, 34f), BuildBench(benchColor), 14);
            Create(root.transform, "PlanterNW", StoryWorldFactory.TileToWorld(29f, 36f), BuildPlanter(planterColor, leaves), 13);
            Create(root.transform, "PlanterNE", StoryWorldFactory.TileToWorld(35f, 36f), BuildPlanter(planterColor, leaves), 13);
            Create(root.transform, "PlanterSW", StoryWorldFactory.TileToWorld(29f, 32f), BuildPlanter(planterColor, leaves), 13);
            Create(root.transform, "PlanterSE", StoryWorldFactory.TileToWorld(35f, 32f), BuildPlanter(planterColor, leaves), 13);

            Create(root.transform, "LampA", StoryWorldFactory.TileToWorld(27.4f, 36.4f), BuildLamp(year), 15);
            Create(root.transform, "LampB", StoryWorldFactory.TileToWorld(36.6f, 36.4f), BuildLamp(year), 15);
            Create(root.transform, "LampC", StoryWorldFactory.TileToWorld(27.4f, 31.8f), BuildLamp(year), 15);
            Create(root.transform, "LampD", StoryWorldFactory.TileToWorld(36.6f, 31.8f), BuildLamp(year), 15);

            Create(root.transform, "WorkshopDirection", StoryWorldFactory.TileToWorld(36.8f, 35.1f), BuildDirectionSign(year), 16);
            Create(root.transform, "NoticeBoard", StoryWorldFactory.TileToWorld(32f, 37f), BuildNoticeBoard(year), 15);

            if (year == 1956)
            {
                Create(root.transform, "NewsCart", StoryWorldFactory.TileToWorld(31f, 32.1f), BuildStall(new Color32(174, 75, 56, 255), new Color32(233, 209, 154, 255)), 15);
                Create(root.transform, "CafeCart", StoryWorldFactory.TileToWorld(33.4f, 32.1f), BuildStall(new Color32(119, 81, 54, 255), new Color32(225, 193, 132, 255)), 15);
            }
            else if (year == 2026)
            {
                Create(root.transform, "CoffeeCart", StoryWorldFactory.TileToWorld(31f, 32.1f), BuildStall(new Color32(50, 108, 147, 255), new Color32(220, 234, 236, 255)), 15);
                Create(root.transform, "InfoStand", StoryWorldFactory.TileToWorld(33.4f, 32.1f), BuildStall(new Color32(56, 62, 70, 255), new Color32(74, 191, 213, 255)), 15);
            }
            else
            {
                Create(root.transform, "HoloVendor", StoryWorldFactory.TileToWorld(31f, 32.1f), BuildStall(new Color32(78, 62, 126, 255), new Color32(76, 229, 226, 255)), 15);
                Create(root.transform, "EnergyStand", StoryWorldFactory.TileToWorld(33.4f, 32.1f), BuildStall(new Color32(53, 72, 100, 255), new Color32(101, 236, 204, 255)), 15);
            }
        }

        private static void Create(Transform parent, string name, Vector2 position, Sprite sprite, int order)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.localPosition = position;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = order;
        }

        private static Sprite BuildPlaza(int year)
        {
            const int width = 160;
            const int height = 96;
            var t = NewTexture(width, height, "ReadablePlaza_" + year);

            var baseColor = year == 1956
                ? new Color32(190, 167, 123, 255)
                : year == 2096 ? new Color32(48, 58, 76, 255) : new Color32(169, 174, 173, 255);
            var alt = year == 1956
                ? new Color32(205, 183, 138, 255)
                : year == 2096 ? new Color32(60, 73, 92, 255) : new Color32(184, 189, 187, 255);
            var edge = year == 2096
                ? new Color32(70, 222, 220, 255)
                : new Color32(80, 84, 83, 255);

            FillRect(t, 0, 0, width, height, baseColor);
            for (var ty = 0; ty < height; ty += 16)
            for (var tx = 0; tx < width; tx += 16)
            {
                if (((tx / 16) + (ty / 16)) % 2 == 0)
                    FillRect(t, tx + 1, ty + 1, 14, 14, alt);
            }

            FillRect(t, 0, 0, width, 3, edge);
            FillRect(t, 0, height - 3, width, 3, edge);
            FillRect(t, 0, 0, 3, height, edge);
            FillRect(t, width - 3, 0, 3, height, edge);

            FillEllipse(t, width / 2, height / 2, 28, 20, Darken(baseColor, 0.08f));
            FillEllipse(t, width / 2, height / 2, 23, 16, alt);

            if (year == 2096)
            {
                FillRect(t, 11, 10, width - 22, 2, edge);
                FillRect(t, 11, height - 12, width - 22, 2, edge);
                for (var x = 18; x < width - 18; x += 28) FillRect(t, x, 8, 10, 2, edge);
            }

            return Finish(t, 0.5f, 0.5f);
        }

        private static Sprite BuildBench(Color32 color)
        {
            var t = NewTexture(38, 22, "HubBench");
            var outline = new Color32(28, 31, 34, 255);
            FillEllipse(t, 19, 3, 15, 2, new Color32(15, 17, 20, 65));
            FillRect(t, 4, 7, 30, 6, outline);
            FillRect(t, 6, 9, 26, 3, color);
            FillRect(t, 7, 13, 24, 5, outline);
            FillRect(t, 8, 14, 22, 2, Lighten(color, 0.12f));
            FillRect(t, 7, 3, 3, 5, outline);
            FillRect(t, 28, 3, 3, 5, outline);
            return Finish(t, 0.5f, 0.08f);
        }

        private static Sprite BuildPlanter(Color32 pot, Color32 leaves)
        {
            var t = NewTexture(24, 30, "HubPlanter");
            var outline = new Color32(28, 31, 34, 255);
            FillEllipse(t, 12, 3, 8, 2, new Color32(16, 18, 20, 65));
            FillRect(t, 5, 5, 14, 9, outline);
            FillRect(t, 7, 7, 10, 5, pot);
            FillEllipse(t, 12, 20, 8, 9, outline);
            FillEllipse(t, 12, 20, 7, 8, leaves);
            FillEllipse(t, 7, 18, 4, 5, leaves);
            FillEllipse(t, 17, 18, 4, 5, Darken(leaves, 0.08f));
            return Finish(t, 0.5f, 0.06f);
        }

        private static Sprite BuildLamp(int year)
        {
            var t = NewTexture(18, 44, "HubLamp");
            var outline = new Color32(27, 30, 34, 255);
            var metal = year == 1956 ? new Color32(81, 67, 54, 255) : new Color32(72, 80, 88, 255);
            var light = year == 2096 ? new Color32(75, 231, 228, 255) : new Color32(247, 221, 148, 255);
            FillEllipse(t, 9, 3, 5, 2, new Color32(12, 15, 18, 60));
            FillRect(t, 7, 5, 4, 28, outline);
            FillRect(t, 8, 6, 2, 27, metal);
            FillRect(t, 3, 31, 12, 8, outline);
            FillRect(t, 5, 33, 8, 4, light);
            if (year == 2096) FillRect(t, 6, 40, 6, 2, light);
            return Finish(t, 0.5f, 0.05f);
        }

        private static Sprite BuildDirectionSign(int year)
        {
            var t = NewTexture(52, 34, "WorkshopSign");
            var outline = new Color32(25, 28, 32, 255);
            var body = year == 1956 ? new Color32(137, 88, 52, 255) : year == 2096 ? new Color32(70, 61, 117, 255) : new Color32(47, 101, 136, 255);
            var accent = year == 2096 ? new Color32(72, 230, 226, 255) : new Color32(234, 225, 197, 255);
            FillRect(t, 4, 10, 38, 17, outline);
            FillRect(t, 6, 12, 34, 13, body);
            FillRect(t, 39, 14, 9, 9, outline);
            FillRect(t, 40, 16, 6, 5, accent);
            FillRect(t, 44, 14, 4, 9, accent);
            FillRect(t, 24, 7, 4, 5, outline);
            FillRect(t, 25, 0, 2, 8, outline);
            FillRect(t, 11, 17, 18, 2, accent);
            return Finish(t, 0.5f, 0.02f);
        }

        private static Sprite BuildNoticeBoard(int year)
        {
            var t = NewTexture(48, 38, "HubNoticeBoard");
            var outline = new Color32(27, 30, 33, 255);
            var frame = year == 1956 ? new Color32(122, 79, 48, 255) : new Color32(67, 78, 87, 255);
            var paper = year == 2096 ? new Color32(80, 226, 221, 255) : new Color32(224, 216, 191, 255);
            FillRect(t, 5, 9, 38, 23, outline);
            FillRect(t, 7, 11, 34, 19, frame);
            FillRect(t, 10, 14, 8, 10, paper);
            FillRect(t, 21, 13, 7, 12, paper);
            FillRect(t, 31, 15, 7, 9, paper);
            FillRect(t, 10, 4, 3, 6, outline);
            FillRect(t, 35, 4, 3, 6, outline);
            return Finish(t, 0.5f, 0.05f);
        }

        private static Sprite BuildStall(Color32 body, Color32 accent)
        {
            var t = NewTexture(42, 42, "HubStall");
            var outline = new Color32(26, 29, 33, 255);
            FillRect(t, 5, 8, 32, 22, outline);
            FillRect(t, 7, 10, 28, 18, body);
            FillRect(t, 3, 28, 36, 8, outline);
            for (var x = 5; x < 37; x += 8) FillRect(t, x, 30, 5, 4, accent);
            FillRect(t, 8, 16, 26, 3, accent);
            FillRect(t, 9, 4, 3, 6, outline);
            FillRect(t, 30, 4, 3, 6, outline);
            return Finish(t, 0.5f, 0.04f);
        }

        private static Texture2D NewTexture(int width, int height, string name)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = name
            };
            var pixels = new Color32[width * height];
            for (var i = 0; i < pixels.Length; i++) pixels[i] = new Color32(0, 0, 0, 0);
            texture.SetPixels32(pixels);
            return texture;
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

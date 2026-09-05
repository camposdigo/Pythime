using System.Collections;
using UnityEngine;

namespace Pythime
{
    [DefaultExecutionOrder(950)]
    public sealed class WorldDepthController : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (GameObject.Find("PythimeWorldDepth") != null) return;
            var root = new GameObject("PythimeWorldDepth");
            root.AddComponent<WorldDepthController>();
        }

        private IEnumerator Start()
        {
            GameObject runtime = null;
            for (var i = 0; i < 30; i++)
            {
                runtime = GameObject.Find("PythimeRuntime");
                if (runtime != null) break;
                yield return null;
            }

            if (runtime == null) yield break;

            AddPlayerSorting();
            BuildEra(runtime.transform, 1956);
            BuildEra(runtime.transform, 2026);
            BuildEra(runtime.transform, 2096);
        }

        private static void AddPlayerSorting()
        {
            var player = GameObject.Find("Player");
            if (player == null) return;
            var avatar = player.transform.Find("Avatar");
            if (avatar == null) return;
            if (avatar.GetComponent<YSortSprite>() == null)
            {
                var sorter = avatar.gameObject.AddComponent<YSortSprite>();
                sorter.Configure(350);
            }
        }

        private static void BuildEra(Transform runtime, int year)
        {
            var era = runtime.Find($"Era_{year}");
            if (era == null || era.Find("DepthProps") != null) return;

            var root = new GameObject("DepthProps");
            root.transform.SetParent(era);

            var treePoints = new[]
            {
                new Vector2(5f, 35f), new Vector2(12f, 35f), new Vector2(15f, 26f),
                new Vector2(48f, 34f), new Vector2(50f, 17f), new Vector2(14f, 17f),
                new Vector2(48f, 8f), new Vector2(30f, 7f)
            };

            foreach (var point in treePoints)
                CreateProp(root.transform, $"Tree_{point.x}_{point.y}", StoryWorldFactory.TileToWorld(point.x, point.y), CreateTreeSprite(year), true, new Vector2(0.54f, 0.42f), new Vector2(0f, 0.22f));

            var benchPoints = new[]
            {
                new Vector2(27f, 31f), new Vector2(36f, 31f),
                new Vector2(27f, 37f), new Vector2(36f, 37f),
                new Vector2(6f, 27f), new Vector2(11f, 27f)
            };
            foreach (var point in benchPoints)
                CreateProp(root.transform, $"Bench_{point.x}_{point.y}", StoryWorldFactory.TileToWorld(point.x, point.y), CreateBenchSprite(year), true, new Vector2(0.82f, 0.26f), new Vector2(0f, 0.12f));

            var binPoints = new[]
            {
                new Vector2(22f, 22f), new Vector2(42f, 22f), new Vector2(22f, 25f), new Vector2(42f, 25f)
            };
            foreach (var point in binPoints)
                CreateProp(root.transform, $"Bin_{point.x}_{point.y}", StoryWorldFactory.TileToWorld(point.x, point.y), CreateBinSprite(year), false, Vector2.zero, Vector2.zero);

            var signPoints = new[]
            {
                new Vector2(8f, 20f), new Vector2(55f, 20f), new Vector2(32f, 28f)
            };
            foreach (var point in signPoints)
                CreateProp(root.transform, $"Sign_{point.x}_{point.y}", StoryWorldFactory.TileToWorld(point.x, point.y), CreateSignSprite(year), false, Vector2.zero, Vector2.zero);

            if (year == 2026)
            {
                CreateProp(root.transform, "HydrantWest", StoryWorldFactory.TileToWorld(18f, 21f), CreateHydrantSprite(), false, Vector2.zero, Vector2.zero);
                CreateProp(root.transform, "HydrantEast", StoryWorldFactory.TileToWorld(46f, 21f), CreateHydrantSprite(), false, Vector2.zero, Vector2.zero);
            }

            if (year == 2096)
            {
                var holoPoints = new[]
                {
                    new Vector2(24f, 19f), new Vector2(40f, 19f), new Vector2(24f, 26f), new Vector2(40f, 26f)
                };
                foreach (var point in holoPoints)
                {
                    var holo = CreateProp(root.transform, $"Holo_{point.x}_{point.y}", StoryWorldFactory.TileToWorld(point.x, point.y), CreateHoloSprite(), false, Vector2.zero, Vector2.zero);
                    holo.AddComponent<TemporalVehiclePulse>();
                }
            }
        }

        private static GameObject CreateProp(Transform parent, string name, Vector2 position, Sprite sprite, bool collider, Vector2 colliderSize, Vector2 colliderOffset)
        {
            var shadow = new GameObject(name + "_Shadow");
            shadow.transform.SetParent(parent);
            shadow.transform.localPosition = position + new Vector2(0f, -0.06f);
            var shadowRenderer = shadow.AddComponent<SpriteRenderer>();
            shadowRenderer.sprite = CreateShadowSprite(sprite.bounds.size.x);
            shadowRenderer.color = new Color(0f, 0f, 0f, 0.23f);
            var shadowSort = shadow.AddComponent<YSortSprite>();
            shadowSort.Configure(180);

            var prop = new GameObject(name);
            prop.transform.SetParent(parent);
            prop.transform.localPosition = position;
            var renderer = prop.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            var sorter = prop.AddComponent<YSortSprite>();
            sorter.Configure(250);

            if (collider)
            {
                var box = prop.AddComponent<BoxCollider2D>();
                box.size = colliderSize;
                box.offset = colliderOffset;
            }

            return prop;
        }

        private static Sprite CreateTreeSprite(int year)
        {
            var texture = NewTexture(30, 42, $"DepthTree_{year}");
            var outline = new Color32(25, 29, 31, 255);
            var trunk = year == 2096 ? new Color32(81, 70, 92, 255) : new Color32(102, 66, 38, 255);
            var leaves = year == 1956 ? new Color32(73, 137, 64, 255) : year == 2096 ? new Color32(42, 145, 125, 255) : new Color32(51, 145, 77, 255);
            var light = year == 2096 ? new Color32(76, 222, 196, 255) : new Color32(85, 173, 91, 255);

            FillRect(texture, 12, 2, 6, 18, outline);
            FillRect(texture, 13, 3, 4, 17, trunk);
            FillRect(texture, 5, 15, 20, 18, outline);
            FillRect(texture, 3, 20, 24, 13, outline);
            FillRect(texture, 7, 17, 16, 14, leaves);
            FillRect(texture, 5, 22, 20, 9, leaves);
            FillRect(texture, 10, 29, 11, 7, leaves);
            FillRect(texture, 8, 25, 6, 3, light);
            FillRect(texture, 17, 20, 5, 3, light);
            if (year == 2096) FillRect(texture, 13, 31, 4, 2, new Color32(89, 240, 222, 255));
            texture.Apply(false, false);
            return MakeSprite(texture, new Vector2(0.5f, 0.08f));
        }

        private static Sprite CreateBenchSprite(int year)
        {
            var texture = NewTexture(24, 14, $"Bench_{year}");
            var outline = new Color32(27, 30, 34, 255);
            var seat = year == 2096 ? new Color32(69, 74, 92, 255) : year == 1956 ? new Color32(128, 83, 48, 255) : new Color32(98, 73, 52, 255);
            var highlight = year == 2096 ? new Color32(72, 214, 221, 255) : new Color32(151, 112, 71, 255);
            FillRect(texture, 2, 5, 20, 6, outline);
            FillRect(texture, 3, 6, 18, 4, seat);
            FillRect(texture, 4, 9, 16, 1, highlight);
            FillRect(texture, 4, 2, 3, 4, outline);
            FillRect(texture, 17, 2, 3, 4, outline);
            texture.Apply(false, false);
            return MakeSprite(texture, new Vector2(0.5f, 0.15f));
        }

        private static Sprite CreateBinSprite(int year)
        {
            var texture = NewTexture(14, 18, $"Bin_{year}");
            var outline = new Color32(27, 30, 34, 255);
            var body = year == 1956 ? new Color32(92, 91, 72, 255) : year == 2096 ? new Color32(54, 73, 88, 255) : new Color32(61, 94, 75, 255);
            FillRect(texture, 3, 2, 8, 13, outline);
            FillRect(texture, 4, 3, 6, 11, body);
            FillRect(texture, 2, 14, 10, 2, outline);
            if (year == 2096) FillRect(texture, 5, 9, 4, 2, new Color32(76, 226, 232, 255));
            texture.Apply(false, false);
            return MakeSprite(texture, new Vector2(0.5f, 0.1f));
        }

        private static Sprite CreateSignSprite(int year)
        {
            var texture = NewTexture(18, 26, $"Sign_{year}");
            var outline = new Color32(27, 30, 34, 255);
            var board = year == 1956 ? new Color32(198, 169, 99, 255) : year == 2096 ? new Color32(47, 92, 113, 255) : new Color32(56, 116, 151, 255);
            FillRect(texture, 7, 2, 4, 14, outline);
            FillRect(texture, 8, 3, 2, 13, new Color32(87, 88, 87, 255));
            FillRect(texture, 2, 14, 14, 9, outline);
            FillRect(texture, 3, 15, 12, 7, board);
            FillRect(texture, 5, 18, 8, 1, year == 2096 ? new Color32(82, 231, 235, 255) : new Color32(233, 239, 235, 255));
            texture.Apply(false, false);
            return MakeSprite(texture, new Vector2(0.5f, 0.08f));
        }

        private static Sprite CreateHydrantSprite()
        {
            var texture = NewTexture(14, 18, "Hydrant");
            var outline = new Color32(28, 31, 35, 255);
            var red = new Color32(201, 56, 48, 255);
            FillRect(texture, 4, 3, 6, 11, outline);
            FillRect(texture, 5, 4, 4, 9, red);
            FillRect(texture, 2, 7, 10, 4, outline);
            FillRect(texture, 3, 8, 8, 2, red);
            FillRect(texture, 3, 13, 8, 3, outline);
            FillRect(texture, 4, 14, 6, 1, new Color32(237, 93, 70, 255));
            texture.Apply(false, false);
            return MakeSprite(texture, new Vector2(0.5f, 0.08f));
        }

        private static Sprite CreateHoloSprite()
        {
            var texture = NewTexture(18, 30, "HoloPost");
            var dark = new Color32(28, 31, 42, 255);
            var cyan = new Color32(72, 224, 235, 255);
            FillRect(texture, 7, 2, 4, 18, dark);
            FillRect(texture, 5, 19, 8, 8, new Color32(49, 100, 122, 180));
            FillRect(texture, 6, 20, 6, 6, new Color32(75, 224, 236, 125));
            FillRect(texture, 8, 5, 2, 14, cyan);
            texture.Apply(false, false);
            return MakeSprite(texture, new Vector2(0.5f, 0.08f));
        }

        private static Sprite CreateShadowSprite(float worldWidth)
        {
            var width = Mathf.Clamp(Mathf.RoundToInt(worldWidth * 16f), 12, 42);
            var texture = NewTexture(width, 8, "PropShadow");
            var shadow = new Color32(255, 255, 255, 255);
            FillRect(texture, 3, 2, Mathf.Max(1, width - 6), 4, shadow);
            FillRect(texture, 1, 3, Mathf.Max(1, width - 2), 2, shadow);
            texture.Apply(false, false);
            return MakeSprite(texture, new Vector2(0.5f, 0.5f));
        }

        private static Texture2D NewTexture(int width, int height, string name)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.name = name;
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            var clear = new Color32(0, 0, 0, 0);
            var pixels = new Color32[width * height];
            for (var i = 0; i < pixels.Length; i++) pixels[i] = clear;
            texture.SetPixels32(pixels);
            return texture;
        }

        private static Sprite MakeSprite(Texture2D texture, Vector2 pivot)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot, 16f, 0, SpriteMeshType.FullRect);
        }

        private static void FillRect(Texture2D texture, int x, int y, int width, int height, Color32 color)
        {
            for (var py = y; py < y + height; py++)
            for (var px = x; px < x + width; px++)
            {
                if (px < 0 || py < 0 || px >= texture.width || py >= texture.height) continue;
                texture.SetPixel(px, py, color);
            }
        }
    }

    public sealed class YSortSprite : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private int offset;

        public void Configure(int sortingOffset)
        {
            offset = sortingOffset;
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            if (spriteRenderer == null) return;
            spriteRenderer.sortingOrder = offset - Mathf.RoundToInt(transform.position.y * 10f);
        }
    }
}

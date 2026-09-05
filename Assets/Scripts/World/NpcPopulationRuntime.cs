using System.Collections;
using UnityEngine;

namespace Pythime
{
    public sealed class NpcPopulationRuntime : MonoBehaviour
    {
        private static readonly Vector2[] SpawnTiles =
        {
            new Vector2(14f, 23f), new Vector2(24f, 23f), new Vector2(33f, 24f), new Vector2(49f, 23f),
            new Vector2(31f, 34f), new Vector2(46f, 34f), new Vector2(14f, 10f), new Vector2(49f, 10f),
            new Vector2(29f, 15f), new Vector2(35f, 15f)
        };

        private static readonly string[] Names1956 =
        {
            "Mabel", "Arthur", "Rosa", "Walter", "Evelyn", "Otávio", "Nina", "Jorge", "Lídia", "Augusto"
        };

        private static readonly string[] Names2026 =
        {
            "Maya", "Caio", "Nina", "Theo", "Luna", "Ravi", "Bia", "Noah", "Yasmin", "Davi"
        };

        private static readonly string[] Names2096 =
        {
            "Ari-7", "Lio", "Nova", "Soren", "Mika", "Z3N", "Iris", "Kairo", "Vega", "Aya"
        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (GameObject.Find("PythimeNpcPopulation") != null) return;
            var root = new GameObject("PythimeNpcPopulation");
            root.AddComponent<NpcPopulationRuntime>();
        }

        private IEnumerator Start()
        {
            for (var i = 0; i < 120; i++)
            {
                if (GameObject.Find("Era_1956") != null && GameObject.Find("Era_2026") != null && GameObject.Find("Era_2096") != null)
                    break;
                yield return null;
            }

            PopulateEra(1956, Names1956, 7);
            PopulateEra(2026, Names2026, 10);
            PopulateEra(2096, Names2096, 8);
        }

        private static void PopulateEra(int year, string[] names, int count)
        {
            var era = GameObject.Find("Era_" + year);
            if (era == null || era.transform.Find("NPCs") != null) return;

            var group = new GameObject("NPCs");
            group.transform.SetParent(era.transform);

            count = Mathf.Min(count, SpawnTiles.Length);
            for (var i = 0; i < count; i++)
            {
                var npc = new GameObject("NPC_" + names[i]);
                npc.transform.SetParent(group.transform);
                npc.transform.localPosition = StoryWorldFactory.TileToWorld(SpawnTiles[i].x, SpawnTiles[i].y);

                var shadowObject = new GameObject("Shadow");
                shadowObject.transform.SetParent(npc.transform);
                shadowObject.transform.localPosition = new Vector3(0f, 0.08f, 0f);
                var shadow = shadowObject.AddComponent<SpriteRenderer>();
                shadow.sprite = PixelArtFactory.CreateShadowSprite();
                shadow.color = new Color(1f, 1f, 1f, 0.72f);

                var visualObject = new GameObject("Visual");
                visualObject.transform.SetParent(npc.transform);
                visualObject.transform.localPosition = Vector3.zero;
                var renderer = visualObject.AddComponent<SpriteRenderer>();
                renderer.sprite = NpcSpriteFactory.Create(year, i);

                var walker = npc.AddComponent<NpcWalker>();
                walker.Initialize(renderer, visualObject.transform, i, year);

                var tag = npc.AddComponent<NpcNameTag>();
                tag.Initialize(names[i], year);
            }
        }
    }

    public sealed class NpcWalker : MonoBehaviour
    {
        private SpriteRenderer rendererTarget;
        private Transform visual;
        private Vector2 home;
        private Vector2 target;
        private float wait;
        private float phase;
        private int seed;
        private float speed;

        public void Initialize(SpriteRenderer rendererValue, Transform visualTransform, int index, int year)
        {
            rendererTarget = rendererValue;
            visual = visualTransform;
            seed = index * 31 + year;
            home = transform.position;
            target = home;
            phase = index * 0.67f;
            speed = year == 1956 ? 0.62f : year == 2096 ? 0.82f : 0.72f;
            wait = 0.4f + (index % 4) * 0.3f;
        }

        private void Update()
        {
            if (rendererTarget == null) return;

            rendererTarget.sortingOrder = 72 - Mathf.RoundToInt(transform.position.y * 3f);

            if (wait > 0f)
            {
                wait -= Time.deltaTime;
                if (visual != null) visual.localPosition = new Vector3(0f, Mathf.Sin(Time.time * 2f + phase) * 0.012f, 0f);
                if (wait <= 0f) PickTarget();
                return;
            }

            var before = (Vector2)transform.position;
            var next = Vector2.MoveTowards(before, target, speed * Time.deltaTime);
            transform.position = next;
            var delta = next - before;

            if (delta.x > 0.002f) rendererTarget.flipX = false;
            else if (delta.x < -0.002f) rendererTarget.flipX = true;

            if (visual != null)
                visual.localPosition = new Vector3(0f, Mathf.Abs(Mathf.Sin(Time.time * 8f + phase)) * 0.035f, 0f);

            if (Vector2.Distance(next, target) < 0.03f)
            {
                wait = 1.1f + Mathf.Abs(Mathf.Sin(seed * 0.37f + Time.time)) * 2.1f;
                target = next;
            }
        }

        private void PickTarget()
        {
            seed = unchecked(seed * 1103515245 + 12345);
            var x = ((seed >> 8) & 255) / 255f * 2f - 1f;
            seed = unchecked(seed * 1103515245 + 12345);
            var y = ((seed >> 8) & 255) / 255f * 2f - 1f;
            var offset = new Vector2(x, y);
            if (offset.sqrMagnitude > 1f) offset.Normalize();
            target = home + offset * 1.15f;
        }
    }

    public sealed class NpcNameTag : MonoBehaviour
    {
        private string displayName;
        private int year;
        private Transform player;
        private GUIStyle style;
        private Texture2D background;

        public void Initialize(string value, int eraYear)
        {
            displayName = value;
            year = eraYear;
        }

        private void OnGUI()
        {
            if (Camera.main == null || string.IsNullOrEmpty(displayName)) return;
            if (player == null)
            {
                var playerObject = GameObject.Find("Player");
                if (playerObject != null) player = playerObject.transform;
            }
            if (player == null || Vector2.Distance(player.position, transform.position) > 2.25f) return;

            if (style == null)
            {
                background = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                background.SetPixel(0, 0, new Color(0.06f, 0.075f, 0.10f, 0.88f));
                background.Apply(false, true);
                style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                style.normal.textColor = year == 2096 ? new Color(0.42f, 0.95f, 0.95f) : Color.white;
            }

            var screen = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f, 1.65f, 0f));
            if (screen.z <= 0f) return;
            var rect = new Rect(screen.x - 43f, Screen.height - screen.y - 12f, 86f, 24f);
            GUI.DrawTexture(rect, background);
            GUI.Label(rect, displayName, style);
        }
    }

    public static class NpcSpriteFactory
    {
        private static readonly Color32[] Skins =
        {
            new Color32(244, 202, 166, 255), new Color32(220, 169, 129, 255), new Color32(190, 133, 91, 255),
            new Color32(139, 89, 59, 255), new Color32(82, 52, 41, 255)
        };

        private static readonly Color32[] Hairs =
        {
            new Color32(35, 29, 27, 255), new Color32(91, 50, 30, 255), new Color32(211, 166, 72, 255),
            new Color32(161, 72, 41, 255), new Color32(83, 91, 111, 255), new Color32(184, 186, 191, 255)
        };

        public static Sprite Create(int year, int variant)
        {
            const int width = 22;
            const int height = 30;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = "Npc_" + year + "_" + variant
            };

            var pixels = new Color32[width * height];
            var clear = new Color32(0, 0, 0, 0);
            for (var i = 0; i < pixels.Length; i++) pixels[i] = clear;
            texture.SetPixels32(pixels);

            var outline = new Color32(24, 27, 31, 255);
            var skin = Skins[variant % Skins.Length];
            var hair = Hairs[(variant * 2 + year) % Hairs.Length];
            var shirt = ShirtColor(year, variant);
            var pants = PantsColor(year, variant);
            var shoe = variant % 3 == 0 ? new Color32(230, 228, 219, 255) : new Color32(47, 49, 55, 255);

            DrawLeg(texture, 6, 2, pants, shoe, outline);
            DrawLeg(texture, 12, 2, pants, shoe, outline);
            FillRect(texture, 6, 8, 10, 8, outline);
            FillRect(texture, 5, 10, 12, 4, outline);
            FillRect(texture, 7, 9, 8, 6, shirt);
            FillRect(texture, 6, 11, 10, 3, shirt);

            FillRect(texture, 3, 10, 3, 6, outline);
            FillRect(texture, 4, 11, 2, 4, shirt);
            texture.SetPixel(4, 10, skin);
            FillRect(texture, 16, 10, 3, 6, outline);
            FillRect(texture, 16, 11, 2, 4, shirt);
            texture.SetPixel(17, 10, skin);

            FillEllipse(texture, 11, 22, 8, 7, outline);
            FillEllipse(texture, 11, 22, 7, 6, skin);
            DrawHair(texture, hair, variant, outline);
            DrawEyes(texture, variant, outline);
            DrawEraAccessory(texture, year, variant, outline);

            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.07f), 16f);
            sprite.name = texture.name;
            return sprite;
        }

        private static Color32 ShirtColor(int year, int variant)
        {
            if (year == 1956)
            {
                var colors = new[]
                {
                    new Color32(174, 76, 59, 255), new Color32(215, 191, 135, 255), new Color32(73, 104, 93, 255),
                    new Color32(87, 95, 126, 255), new Color32(188, 145, 72, 255)
                };
                return colors[variant % colors.Length];
            }
            if (year == 2096)
            {
                var colors = new[]
                {
                    new Color32(69, 77, 127, 255), new Color32(47, 128, 126, 255), new Color32(119, 72, 154, 255),
                    new Color32(58, 92, 139, 255), new Color32(117, 55, 103, 255)
                };
                return colors[variant % colors.Length];
            }

            var modern = new[]
            {
                new Color32(58, 112, 179, 255), new Color32(195, 75, 66, 255), new Color32(67, 142, 91, 255),
                new Color32(137, 86, 165, 255), new Color32(219, 157, 57, 255), new Color32(51, 55, 62, 255)
            };
            return modern[variant % modern.Length];
        }

        private static Color32 PantsColor(int year, int variant)
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

        private static void DrawHair(Texture2D t, Color32 hair, int variant, Color32 outline)
        {
            FillEllipse(t, 11, 26, 8, 4, outline);
            FillEllipse(t, 11, 26, 7, 3, hair);

            switch (variant % 5)
            {
                case 0:
                    FillRect(t, 5, 23, 3, 4, hair);
                    break;
                case 1:
                    FillEllipse(t, 6, 24, 4, 4, hair);
                    FillEllipse(t, 15, 24, 4, 4, hair);
                    break;
                case 2:
                    FillRect(t, 5, 22, 3, 6, hair);
                    FillRect(t, 14, 23, 3, 5, hair);
                    break;
                case 3:
                    FillRect(t, 8, 27, 8, 2, hair);
                    texturePixelSafe(t, 16, 28, hair);
                    break;
                default:
                    FillEllipse(t, 8, 27, 4, 3, hair);
                    FillEllipse(t, 14, 27, 4, 3, hair);
                    break;
            }
        }

        private static void DrawEyes(Texture2D t, int variant, Color32 outline)
        {
            var white = new Color32(241, 240, 234, 255);
            FillRect(t, 7, 21, 3, 2, outline);
            FillRect(t, 12, 21, 3, 2, outline);
            t.SetPixel(7, 22, white);
            t.SetPixel(12, 22, white);
            t.SetPixel(9, 21, outline);
            t.SetPixel(14, 21, outline);

            if (variant % 4 == 0)
            {
                FillRect(t, 6, 21, 10, 1, new Color32(49, 54, 62, 255));
                t.SetPixel(10, 21, new Color32(120, 174, 193, 255));
                t.SetPixel(11, 21, new Color32(120, 174, 193, 255));
            }
        }

        private static void DrawEraAccessory(Texture2D t, int year, int variant, Color32 outline)
        {
            if (year == 1956 && variant % 3 == 0)
            {
                var hat = new Color32(91, 68, 51, 255);
                FillRect(t, 5, 27, 12, 2, outline);
                FillRect(t, 7, 28, 8, 1, hat);
            }
            else if (year == 2026 && variant % 3 == 1)
            {
                var bag = new Color32(91, 63, 44, 255);
                FillRect(t, 17, 11, 3, 5, outline);
                FillRect(t, 18, 12, 2, 3, bag);
            }
            else if (year == 2096)
            {
                var cyan = new Color32(69, 230, 225, 255);
                if (variant % 2 == 0)
                {
                    FillRect(t, 7, 10, 8, 1, cyan);
                    t.SetPixel(18, 12, cyan);
                }
                else
                {
                    FillRect(t, 6, 20, 10, 1, cyan);
                }
            }
        }

        private static void texturePixelSafe(Texture2D t, int x, int y, Color32 color)
        {
            if (x >= 0 && y >= 0 && x < t.width && y < t.height) t.SetPixel(x, y, color);
        }

        private static void FillRect(Texture2D t, int x, int y, int width, int height, Color32 color)
        {
            for (var py = y; py < y + height; py++)
            for (var px = x; px < x + width; px++)
                if (px >= 0 && py >= 0 && px < t.width && py < t.height)
                    t.SetPixel(px, py, color);
        }

        private static void FillEllipse(Texture2D t, int centerX, int centerY, int radiusX, int radiusY, Color32 color)
        {
            for (var y = centerY - radiusY; y <= centerY + radiusY; y++)
            for (var x = centerX - radiusX; x <= centerX + radiusX; x++)
            {
                var nx = (x - centerX) / (float)radiusX;
                var ny = (y - centerY) / (float)radiusY;
                if (nx * nx + ny * ny <= 1f && x >= 0 && y >= 0 && x < t.width && y < t.height)
                    t.SetPixel(x, y, color);
            }
        }
    }
}

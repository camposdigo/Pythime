using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pythime
{
    public sealed class WorkshopInteriorRuntime : MonoBehaviour
    {
        private static readonly Vector2 ExteriorDoor = StoryWorldFactory.WorkshopPoint + new Vector2(0f, -0.55f);
        private static readonly Vector2 OutsideReturn = StoryWorldFactory.WorkshopPoint + new Vector2(0f, -1.35f);
        private static readonly Vector2 InteriorCenter = new Vector2(44f, 0f);
        private static readonly Vector2 InteriorSpawn = InteriorCenter + new Vector2(0f, -2.65f);
        private static readonly Vector2 InteriorExit = InteriorCenter + new Vector2(0f, -3.15f);

        private readonly Dictionary<int, SpriteRenderer> roofs = new Dictionary<int, SpriteRenderer>();
        private Transform player;
        private Transform tock;
        private bool inside;
        private GUIStyle promptStyle;
        private GUIStyle titleStyle;
        private Texture2D panel;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (GameObject.Find("PythimeWorkshopInterior") != null) return;
            var root = new GameObject("PythimeWorkshopInterior");
            root.AddComponent<WorkshopInteriorRuntime>();
        }

        private IEnumerator Start()
        {
            for (var frame = 0; frame < 180; frame++)
            {
                var runtime = GameObject.Find("PythimeRuntime");
                var playerObject = GameObject.Find("Player");
                if (runtime != null && playerObject != null && runtime.transform.Find("Era_2026") != null)
                {
                    player = playerObject.transform;
                    var tockObject = GameObject.Find("Tock");
                    if (tockObject != null) tock = tockObject.transform;
                    BuildEraInterior(runtime.transform, 1956);
                    BuildEraInterior(runtime.transform, 2026);
                    BuildEraInterior(runtime.transform, 2096);
                    yield break;
                }
                yield return null;
            }
        }

        private void Update()
        {
            if (player == null)
            {
                var playerObject = GameObject.Find("Player");
                if (playerObject != null) player = playerObject.transform;
                if (player == null) return;
            }

            if (tock == null)
            {
                var tockObject = GameObject.Find("Tock");
                if (tockObject != null) tock = tockObject.transform;
            }

            UpdateRoofFade();

            var keyboard = Keyboard.current;
            if (keyboard == null || !keyboard.fKey.wasPressedThisFrame) return;

            if (!inside && Vector2.Distance(player.position, ExteriorDoor) <= 1.35f)
            {
                EnterWorkshop();
            }
            else if (inside && Vector2.Distance(player.position, InteriorExit) <= 1.45f)
            {
                ExitWorkshop();
            }
        }

        private void EnterWorkshop()
        {
            inside = true;
            MovePlayer(InteriorSpawn);
            SetRoofAlpha(0.08f);
        }

        private void ExitWorkshop()
        {
            inside = false;
            MovePlayer(OutsideReturn);
            SetRoofAlpha(1f);
        }

        private void MovePlayer(Vector2 target)
        {
            var controller = player.GetComponent<PlayerController>();
            if (controller != null) controller.StopImmediately();
            player.position = target;
            if (tock != null) tock.position = target + new Vector2(0.8f, 0.7f);
        }

        private void UpdateRoofFade()
        {
            var target = inside ? 0.08f : 1f;
            foreach (var pair in roofs)
            {
                var renderer = pair.Value;
                if (renderer == null) continue;
                var color = renderer.color;
                color.a = Mathf.MoveTowards(color.a, target, Time.deltaTime * 4.5f);
                renderer.color = color;
            }
        }

        private void SetRoofAlpha(float alpha)
        {
            foreach (var pair in roofs)
            {
                if (pair.Value == null) continue;
                var color = pair.Value.color;
                color.a = alpha;
                pair.Value.color = color;
            }
        }

        private void OnGUI()
        {
            if (player == null) return;
            BuildStyles();

            var nearExterior = !inside && Vector2.Distance(player.position, ExteriorDoor) <= 1.75f;
            var nearExit = inside && Vector2.Distance(player.position, InteriorExit) <= 1.75f;
            if (nearExterior || nearExit)
            {
                var width = 310f;
                var rect = new Rect((Screen.width - width) * 0.5f, Screen.height - 116f, width, 42f);
                GUI.DrawTexture(rect, panel);
                GUI.Label(rect, nearExterior ? "F  ENTRAR NA OFICINA TEMPORAL" : "F  SAIR PARA A RUA", promptStyle);
            }

            if (!inside) return;
            var timeline = TimeTravelManager.Instance;
            var year = timeline != null ? timeline.CurrentYear : 2026;
            var titleRect = new Rect((Screen.width - 410f) * 0.5f, 92f, 410f, 54f);
            GUI.DrawTexture(titleRect, panel);
            GUI.Label(new Rect(titleRect.x, titleRect.y + 3f, titleRect.width, 25f), "OFICINA TEMPORAL  •  " + year, titleStyle);
            GUI.Label(new Rect(titleRect.x, titleRect.y + 27f, titleRect.width, 20f), "Q / E muda a oficina no tempo", promptStyle);
        }

        private void BuildStyles()
        {
            if (promptStyle != null) return;
            panel = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            panel.SetPixel(0, 0, new Color(0.045f, 0.055f, 0.075f, 0.94f));
            panel.Apply(false, true);

            promptStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                fontStyle = FontStyle.Bold
            };
            promptStyle.normal.textColor = new Color(1f, 0.82f, 0.28f);

            titleStyle = new GUIStyle(promptStyle)
            {
                fontSize = 17
            };
            titleStyle.normal.textColor = Color.white;
        }

        private void BuildEraInterior(Transform runtime, int year)
        {
            var era = runtime.Find("Era_" + year);
            if (era == null || era.Find("WorkshopInterior") != null) return;

            var root = new GameObject("WorkshopInterior");
            root.transform.SetParent(era);
            root.transform.localPosition = InteriorCenter;

            CreateSprite(root.transform, "Floor", Vector2.zero, BuildRoomSprite(year), 24);
            CreateSprite(root.transform, "Decor", Vector2.zero, BuildDecorSprite(year), 36);

            var roof = CreateSprite(root.transform, "Roof", Vector2.zero, BuildRoofSprite(year), 95);
            roofs[year] = roof;

            AddRoomBounds(root.transform);
            CreateExteriorDoorMarker(era, year);
        }

        private static SpriteRenderer CreateSprite(Transform parent, string name, Vector2 localPosition, Sprite sprite, int sortingOrder)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.localPosition = localPosition;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        private static void CreateExteriorDoorMarker(Transform era, int year)
        {
            if (era.Find("WorkshopDoorMarker") != null) return;
            var marker = new GameObject("WorkshopDoorMarker");
            marker.transform.SetParent(era);
            marker.transform.localPosition = ExteriorDoor;
            var renderer = marker.AddComponent<SpriteRenderer>();
            renderer.sprite = BuildDoorMarker(year);
            renderer.sortingOrder = 48;
        }

        private static void AddRoomBounds(Transform root)
        {
            AddWall(root, new Vector2(-6.35f, 0f), new Vector2(0.35f, 8.4f));
            AddWall(root, new Vector2(6.35f, 0f), new Vector2(0.35f, 8.4f));
            AddWall(root, new Vector2(0f, 4.15f), new Vector2(12.7f, 0.35f));
            AddWall(root, new Vector2(0f, -4.15f), new Vector2(12.7f, 0.35f));
        }

        private static void AddWall(Transform parent, Vector2 localPosition, Vector2 size)
        {
            var wall = new GameObject("InteriorWall");
            wall.transform.SetParent(parent);
            wall.transform.localPosition = localPosition;
            var collider = wall.AddComponent<BoxCollider2D>();
            collider.size = size;
        }

        private static Sprite BuildRoomSprite(int year)
        {
            const int width = 208;
            const int height = 144;
            var t = NewTexture(width, height, "WorkshopFloor_" + year);
            var wall = year == 1956 ? new Color32(107, 72, 50, 255) : year == 2096 ? new Color32(58, 55, 88, 255) : new Color32(80, 88, 94, 255);
            var floorA = year == 1956 ? new Color32(185, 151, 105, 255) : year == 2096 ? new Color32(48, 56, 75, 255) : new Color32(151, 158, 157, 255);
            var floorB = year == 1956 ? new Color32(164, 129, 88, 255) : year == 2096 ? new Color32(55, 66, 86, 255) : new Color32(132, 140, 141, 255);
            var outline = new Color32(27, 30, 34, 255);

            FillRect(t, 0, 0, width, height, outline);
            FillRect(t, 8, 8, width - 16, height - 16, floorA);
            FillRect(t, 8, height - 24, width - 16, 16, wall);
            FillRect(t, 8, 8, 12, height - 16, wall);
            FillRect(t, width - 20, 8, 12, height - 16, wall);

            for (var y = 20; y < height - 25; y += 16)
            for (var x = 20; x < width - 20; x += 16)
                if (((x + y) / 16) % 2 == 0) FillRect(t, x, y, 16, 16, floorB);

            var accent = year == 1956 ? new Color32(222, 178, 87, 255) : year == 2096 ? new Color32(73, 230, 226, 255) : new Color32(64, 179, 205, 255);
            FillRect(t, 28, height - 29, width - 56, 3, accent);
            FillRect(t, 88, 8, 32, 5, outline);
            FillRect(t, 94, 9, 20, 3, accent);
            return Finish(t, 0.5f, 0.5f);
        }

        private static Sprite BuildDecorSprite(int year)
        {
            const int width = 208;
            const int height = 144;
            var t = NewTexture(width, height, "WorkshopDecor_" + year);
            var outline = new Color32(26, 29, 33, 255);
            var wood = new Color32(117, 76, 48, 255);
            var metal = new Color32(78, 88, 96, 255);
            var cyan = new Color32(72, 228, 232, 255);
            var gold = new Color32(221, 173, 75, 255);
            var purple = new Color32(103, 75, 148, 255);

            var bench = year == 1956 ? wood : year == 2096 ? purple : metal;
            var accent = year == 1956 ? gold : cyan;

            DrawWorkbench(t, 22, 94, bench, accent, outline, year);
            DrawWorkbench(t, 132, 94, bench, accent, outline, year);
            DrawShelf(t, 26, 36, bench, accent, outline, year);
            DrawConsole(t, 145, 38, bench, accent, outline, year);
            DrawChronoCore(t, 92, 52, accent, outline, year);
            DrawChair(t, 61, 41, bench, outline);
            DrawChair(t, 123, 42, bench, outline);

            if (year == 1956)
            {
                DrawCrate(t, 49, 20, wood, outline);
                DrawCrate(t, 147, 18, Darken(wood, 0.12f), outline);
                FillRect(t, 73, 105, 48, 28, new Color32(224, 213, 178, 255));
                for (var i = 0; i < 4; i++) FillRect(t, 78, 111 + i * 5, 36, 1, new Color32(92, 79, 64, 255));
            }
            else if (year == 2026)
            {
                DrawMonitorBank(t, 69, 106, cyan, outline);
                FillRect(t, 88, 20, 32, 8, outline);
                FillRect(t, 91, 22, 26, 4, new Color32(190, 215, 219, 255));
            }
            else
            {
                DrawMonitorBank(t, 65, 105, cyan, outline);
                FillRect(t, 72, 18, 64, 3, cyan);
                FillRect(t, 78, 23, 52, 2, new Color32(177, 96, 219, 210));
                FillRect(t, 39, 73, 20, 2, cyan);
                FillRect(t, 149, 72, 22, 2, cyan);
                DrawCrack(t, 28, 120, outline);
                DrawCrack(t, 174, 111, outline);
            }

            return Finish(t, 0.5f, 0.5f);
        }

        private static Sprite BuildRoofSprite(int year)
        {
            const int width = 208;
            const int height = 144;
            var t = NewTexture(width, height, "WorkshopRoof_" + year);
            var dark = year == 1956 ? new Color32(74, 54, 41, 255) : year == 2096 ? new Color32(35, 34, 58, 255) : new Color32(49, 55, 60, 255);
            var mid = year == 1956 ? new Color32(124, 86, 57, 255) : year == 2096 ? new Color32(75, 61, 112, 255) : new Color32(82, 94, 103, 255);
            var accent = year == 1956 ? new Color32(221, 173, 75, 255) : new Color32(71, 225, 230, 255);
            FillRect(t, 0, 0, width, height, dark);
            FillRect(t, 8, 8, width - 16, height - 16, mid);
            FillRect(t, 18, 18, width - 36, 12, dark);
            FillRect(t, 30, 22, width - 60, 4, accent);
            FillRect(t, 80, 0, 48, 16, new Color32(24, 27, 31, 255));
            FillRect(t, 92, 5, 24, 5, accent);
            return Finish(t, 0.5f, 0.5f);
        }

        private static Sprite BuildDoorMarker(int year)
        {
            var t = NewTexture(24, 30, "WorkshopDoorMarker_" + year);
            var outline = new Color32(25, 28, 31, 255);
            var body = year == 1956 ? new Color32(139, 89, 56, 255) : year == 2096 ? new Color32(73, 61, 112, 255) : new Color32(62, 105, 128, 255);
            var accent = year == 1956 ? new Color32(230, 184, 85, 255) : new Color32(72, 230, 232, 255);
            FillRect(t, 3, 3, 18, 24, outline);
            FillRect(t, 5, 5, 14, 20, body);
            FillRect(t, 7, 20, 10, 3, accent);
            t.SetPixel(16, 13, accent);
            return Finish(t, 0.5f, 0.08f);
        }

        private static void DrawWorkbench(Texture2D t, int x, int y, Color32 body, Color32 accent, Color32 outline, int year)
        {
            FillRect(t, x, y, 54, 8, outline);
            FillRect(t, x + 2, y + 2, 50, 4, body);
            FillRect(t, x + 5, y - 22, 5, 22, outline);
            FillRect(t, x + 44, y - 22, 5, 22, outline);
            FillRect(t, x + 13, y + 8, 28, 14, outline);
            FillRect(t, x + 15, y + 10, 24, 10, year == 1956 ? new Color32(216, 203, 169, 255) : new Color32(42, 57, 65, 255));
            FillRect(t, x + 18, y + 13, 18, 2, accent);
        }

        private static void DrawShelf(Texture2D t, int x, int y, Color32 body, Color32 accent, Color32 outline, int year)
        {
            FillRect(t, x, y, 40, 48, outline);
            FillRect(t, x + 3, y + 3, 34, 42, body);
            for (var i = 0; i < 3; i++) FillRect(t, x + 4, y + 12 + i * 12, 32, 3, outline);
            for (var i = 0; i < 5; i++)
            {
                var color = year == 1956 ? new Color32(173, 128, 72, 255) : i % 2 == 0 ? accent : new Color32(135, 145, 151, 255);
                FillRect(t, x + 7 + i * 5, y + 6, 3, 7, color);
            }
        }

        private static void DrawConsole(Texture2D t, int x, int y, Color32 body, Color32 accent, Color32 outline, int year)
        {
            FillRect(t, x, y, 38, 42, outline);
            FillRect(t, x + 3, y + 3, 32, 36, body);
            FillRect(t, x + 7, y + 20, 24, 13, outline);
            FillRect(t, x + 9, y + 22, 20, 9, year == 1956 ? new Color32(213, 202, 165, 255) : new Color32(36, 64, 72, 255));
            FillRect(t, x + 12, y + 25, 14, 2, accent);
            for (var i = 0; i < 4; i++) t.SetPixel(x + 9 + i * 6, y + 10, accent);
        }

        private static void DrawChronoCore(Texture2D t, int x, int y, Color32 accent, Color32 outline, int year)
        {
            FillEllipse(t, x + 14, y + 18, 16, 18, outline);
            FillEllipse(t, x + 14, y + 18, 13, 15, year == 1956 ? new Color32(122, 86, 55, 255) : new Color32(48, 60, 72, 255));
            FillEllipse(t, x + 14, y + 18, 7, 9, accent);
            FillEllipse(t, x + 14, y + 18, 3, 4, new Color32(230, 248, 245, 255));
            FillRect(t, x + 3, y - 6, 22, 8, outline);
            FillRect(t, x + 6, y - 4, 16, 4, Darken(accent, 0.18f));
        }

        private static void DrawChair(Texture2D t, int x, int y, Color32 body, Color32 outline)
        {
            FillRect(t, x, y, 16, 6, outline);
            FillRect(t, x + 2, y + 2, 12, 3, body);
            FillRect(t, x + 2, y + 6, 3, 12, outline);
            FillRect(t, x + 11, y + 6, 3, 12, outline);
        }

        private static void DrawCrate(Texture2D t, int x, int y, Color32 wood, Color32 outline)
        {
            FillRect(t, x, y, 24, 20, outline);
            FillRect(t, x + 2, y + 2, 20, 16, wood);
            FillRect(t, x + 5, y + 4, 2, 12, Darken(wood, 0.18f));
            FillRect(t, x + 17, y + 4, 2, 12, Darken(wood, 0.18f));
        }

        private static void DrawMonitorBank(Texture2D t, int x, int y, Color32 accent, Color32 outline)
        {
            for (var i = 0; i < 3; i++)
            {
                FillRect(t, x + i * 24, y, 20, 16, outline);
                FillRect(t, x + 3 + i * 24, y + 3, 14, 10, new Color32(36, 60, 69, 255));
                FillRect(t, x + 5 + i * 24, y + 6, 10, 2, accent);
            }
        }

        private static void DrawCrack(Texture2D t, int x, int y, Color32 color)
        {
            t.SetPixel(x, y, color);
            t.SetPixel(x + 1, y - 1, color);
            t.SetPixel(x + 2, y - 2, color);
            t.SetPixel(x + 2, y - 3, color);
            t.SetPixel(x + 3, y - 4, color);
            t.SetPixel(x + 4, y - 3, color);
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
            var clear = new Color32(0, 0, 0, 0);
            for (var i = 0; i < pixels.Length; i++) pixels[i] = clear;
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

        private static void FillEllipse(Texture2D texture, int centerX, int centerY, int radiusX, int radiusY, Color32 color)
        {
            if (radiusX <= 0 || radiusY <= 0) return;
            for (var y = centerY - radiusY; y <= centerY + radiusY; y++)
            for (var x = centerX - radiusX; x <= centerX + radiusX; x++)
            {
                var nx = (x - centerX) / (float)radiusX;
                var ny = (y - centerY) / (float)radiusY;
                if (nx * nx + ny * ny > 1f) continue;
                if (x >= 0 && y >= 0 && x < texture.width && y < texture.height)
                    texture.SetPixel(x, y, color);
            }
        }

        private static Color32 Darken(Color32 color, float amount)
        {
            return new Color32(
                (byte)(color.r * (1f - amount)),
                (byte)(color.g * (1f - amount)),
                (byte)(color.b * (1f - amount)),
                color.a);
        }
    }
}

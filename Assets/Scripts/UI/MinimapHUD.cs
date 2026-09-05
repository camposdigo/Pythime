using UnityEngine;
using UnityEngine.InputSystem;

namespace Pythime
{
    public sealed class MinimapHUD : MonoBehaviour
    {
        private Transform player;
        private StoryDirector story;
        private bool visible = true;
        private GUIStyle titleStyle;
        private GUIStyle smallStyle;
        private GUIStyle mapLabelStyle;
        private Texture2D panel;
        private Texture2D mapBackground;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (GameObject.Find("PythimeMinimap") != null) return;
            var go = new GameObject("PythimeMinimap");
            go.AddComponent<MinimapHUD>();
        }

        private void OnEnable()
        {
            visible = true;
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
                visible = !visible;

            if (player == null)
            {
                var playerObject = GameObject.Find("Player");
                if (playerObject != null) player = playerObject.transform;
            }

            if (story == null)
            {
                var runtime = GameObject.Find("PythimeRuntime");
                if (runtime != null) story = runtime.GetComponent<StoryDirector>();
            }
        }

        private void OnGUI()
        {
            if (!visible || player == null) return;
            BuildStyles();
            GUI.depth = -30;

            var scale = Mathf.Clamp(Screen.height / 1440f, 0.9f, 1.3f);
            var width = 294f * scale;
            var height = 220f * scale;
            var x = Screen.width - width - 18f * scale;
            var y = 194f * scale;
            var outer = new Rect(x, y, width, height);
            var inner = new Rect(x + 11f * scale, y + 38f * scale, width - 22f * scale, height - 58f * scale);

            GUI.DrawTexture(outer, panel);
            DrawHeader(new Rect(x + 12f * scale, y + 8f * scale, width - 24f * scale, 26f * scale), scale);
            DrawMap(inner, scale);
            DrawLegend(new Rect(x + 12f * scale, y + height - 18f * scale, width - 24f * scale, 14f * scale), scale);
        }

        private void DrawHeader(Rect rect, float scale)
        {
            titleStyle.fontSize = Mathf.RoundToInt(14f * scale);
            smallStyle.fontSize = Mathf.RoundToInt(10f * scale);
            GUI.Label(new Rect(rect.x, rect.y, 160f * scale, rect.height), "MAPA DE PYTHIME", titleStyle);
            GUI.Label(new Rect(rect.x + rect.width - 82f * scale, rect.y, 82f * scale, rect.height), "M ocultar", smallStyle);
        }

        private void DrawMap(Rect rect, float scale)
        {
            var timeline = TimeTravelManager.Instance;
            var currentYear = timeline != null ? timeline.CurrentYear : 2026;
            var eraTint = TimeShiftPresentation.EraTint(currentYear);

            var old = GUI.color;
            GUI.color = new Color(eraTint.r * 0.24f + 0.12f, eraTint.g * 0.24f + 0.14f, eraTint.b * 0.24f + 0.15f, 1f);
            GUI.DrawTexture(rect, mapBackground);
            GUI.color = old;

            DrawRoad(rect, new Rect(0.41f, 0f, 0.17f, 1f));
            DrawRoad(rect, new Rect(0f, 0.42f, 1f, 0.18f));
            DrawRoad(rect, new Rect(0f, 0.18f, 1f, 0.07f));

            DrawLandmark(rect, StoryWorldFactory.WorkshopPoint, new Color(0.76f, 0.78f, 0.82f), 6f * scale, false);
            DrawLandmark(rect, StoryWorldFactory.ClockPlazaPoint, new Color(0.42f, 0.70f, 0.80f), 6f * scale, false);
            DrawLandmark(rect, StoryWorldFactory.VehiclePoint, new Color(0.35f, 0.91f, 1f), 7f * scale, false);

            mapLabelStyle.fontSize = Mathf.RoundToInt(8f * scale);
            DrawMapLabel(rect, StoryWorldFactory.WorkshopPoint, "OFICINA", new Vector2(-32f, -15f));
            DrawMapLabel(rect, StoryWorldFactory.ClockPlazaPoint, "PRAÇA", new Vector2(-23f, 8f));

            if (story != null && story.HasObjectiveTarget)
            {
                var pulse = 0.75f + Mathf.Sin(Time.time * 5f) * 0.25f;
                DrawLandmark(rect, story.ObjectiveTarget, new Color(1f, 0.78f, 0.18f, pulse), 10f * scale, true);
            }

            DrawLandmark(rect, player.position, new Color(0.96f, 0.98f, 1f), 9f * scale, true);

            GUI.color = eraTint;
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 3f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height - 3f, rect.width, 3f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, 3f, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x + rect.width - 3f, rect.y, 3f, rect.height), Texture2D.whiteTexture);
            GUI.color = old;
        }

        private static void DrawRoad(Rect map, Rect normalized)
        {
            var old = GUI.color;
            GUI.color = new Color(0.20f, 0.22f, 0.25f, 0.95f);
            GUI.DrawTexture(new Rect(
                map.x + normalized.x * map.width,
                map.y + normalized.y * map.height,
                normalized.width * map.width,
                normalized.height * map.height), Texture2D.whiteTexture);
            GUI.color = old;
        }

        private static void DrawLandmark(Rect map, Vector2 worldPosition, Color color, float size, bool diamond)
        {
            var point = WorldToMap(map, worldPosition);
            var old = GUI.color;
            GUI.color = color;

            if (diamond)
            {
                var oldMatrix = GUI.matrix;
                GUIUtility.RotateAroundPivot(45f, point);
                GUI.DrawTexture(new Rect(point.x - size * 0.5f, point.y - size * 0.5f, size, size), Texture2D.whiteTexture);
                GUI.matrix = oldMatrix;
            }
            else
            {
                GUI.DrawTexture(new Rect(point.x - size * 0.5f, point.y - size * 0.5f, size, size), Texture2D.whiteTexture);
            }

            GUI.color = old;
        }

        private void DrawMapLabel(Rect map, Vector2 worldPosition, string text, Vector2 offset)
        {
            var point = WorldToMap(map, worldPosition);
            GUI.Label(new Rect(point.x + offset.x, point.y + offset.y, 64f, 14f), text, mapLabelStyle);
        }

        private static Vector2 WorldToMap(Rect map, Vector2 world)
        {
            var normalizedX = Mathf.Clamp01((world.x + StoryWorldFactory.MapWidthTiles * 0.5f) / StoryWorldFactory.MapWidthTiles);
            var normalizedY = Mathf.Clamp01((world.y + StoryWorldFactory.MapHeightTiles * 0.5f) / StoryWorldFactory.MapHeightTiles);
            return new Vector2(
                map.x + normalizedX * map.width,
                map.y + (1f - normalizedY) * map.height);
        }

        private void DrawLegend(Rect rect, float scale)
        {
            smallStyle.fontSize = Mathf.RoundToInt(9f * scale);
            GUI.Label(rect, "◆ você     ◆ objetivo     ■ locais", smallStyle);
        }

        private void BuildStyles()
        {
            if (titleStyle != null) return;

            panel = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            panel.SetPixel(0, 0, new Color(0.035f, 0.045f, 0.065f, 0.97f));
            panel.Apply(false, true);

            mapBackground = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            mapBackground.SetPixel(0, 0, Color.white);
            mapBackground.Apply(false, true);

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            titleStyle.normal.textColor = Color.white;

            smallStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleRight
            };
            smallStyle.normal.textColor = new Color(0.71f, 0.77f, 0.82f);

            mapLabelStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            mapLabelStyle.normal.textColor = new Color(0.88f, 0.92f, 0.95f);
        }
    }
}

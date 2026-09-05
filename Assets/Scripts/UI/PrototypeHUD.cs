using UnityEngine;

namespace Pythime
{
    public sealed class PrototypeHUD : MonoBehaviour
    {
        private StoryDirector story;
        private Transform player;

        private GUIStyle brandStyle;
        private GUIStyle smallStyle;
        private GUIStyle timelineYearStyle;
        private GUIStyle timelineEraStyle;
        private GUIStyle missionHeaderStyle;
        private GUIStyle missionStepStyle;
        private GUIStyle missionTextStyle;
        private GUIStyle targetStyle;
        private GUIStyle hintStyle;
        private GUIStyle centerStyle;

        private Texture2D darkPanel;
        private Texture2D darkerPanel;
        private float viewWidth;
        private float viewHeight;

        public void Initialize(StoryDirector storyDirector, Transform playerTransform)
        {
            story = storyDirector;
            player = playerTransform;
        }

        private void BuildStyles()
        {
            if (brandStyle != null) return;

            darkPanel = MakeTexture(new Color32(16, 20, 26, 238));
            darkerPanel = MakeTexture(new Color32(10, 13, 18, 248));

            brandStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 25,
                fontStyle = FontStyle.Bold
            };
            brandStyle.normal.textColor = new Color(0.94f, 0.97f, 1f);

            smallStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12
            };
            smallStyle.normal.textColor = new Color(0.70f, 0.77f, 0.84f);

            timelineYearStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            timelineYearStyle.normal.textColor = Color.white;

            timelineEraStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            timelineEraStyle.normal.textColor = new Color(0.86f, 0.89f, 0.92f);

            missionHeaderStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };
            missionHeaderStyle.normal.textColor = new Color(1f, 0.79f, 0.24f);

            missionStepStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold
            };
            missionStepStyle.normal.textColor = new Color(0.56f, 0.91f, 1f);

            missionTextStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                wordWrap = true
            };
            missionTextStyle.normal.textColor = Color.white;

            targetStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };
            targetStyle.normal.textColor = new Color(0.90f, 0.93f, 0.96f);

            hintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            hintStyle.normal.textColor = new Color(1f, 0.86f, 0.34f);

            centerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            centerStyle.normal.textColor = Color.white;
        }

        private void OnGUI()
        {
            BuildStyles();
            var timeline = TimeTravelManager.Instance;
            if (timeline == null) return;

            var scale = Mathf.Clamp(Screen.height / 900f, 1f, 1.65f);
            var previousMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1f));
            viewWidth = Screen.width / scale;
            viewHeight = Screen.height / scale;

            DrawBrand(story != null ? story.LocationName : "Pythime");
            DrawTimeline(timeline.CurrentYear);
            if (story != null) DrawMission(story, timeline.CurrentYear);
            DrawContextHint(story);

            GUI.matrix = previousMatrix;
        }

        private void DrawBrand(string location)
        {
            GUI.DrawTexture(new Rect(18, 18, 238, 78), darkPanel);
            GUI.Label(new Rect(34, 25, 190, 31), "PYTHIME", brandStyle);
            GUI.Label(new Rect(34, 56, 205, 20), location.ToUpperInvariant(), smallStyle);
            GUI.Label(new Rect(34, 76, 205, 18), "TAB personagem   •   P terminal", smallStyle);
        }

        private void DrawTimeline(int currentYear)
        {
            const float badgeWidth = 118f;
            const float gap = 7f;
            var totalWidth = badgeWidth * 3f + gap * 2f;
            var x = (viewWidth - totalWidth) * 0.5f;
            const float y = 18f;

            DrawEraBadge(new Rect(x, y, badgeWidth, 60f), 1956, "PASSADO", currentYear == 1956);
            DrawEraBadge(new Rect(x + badgeWidth + gap, y, badgeWidth, 60f), 2026, "PRESENTE", currentYear == 2026);
            DrawEraBadge(new Rect(x + (badgeWidth + gap) * 2f, y, badgeWidth, 60f), 2096, "FUTURO", currentYear == 2096);

            var helper = new GUIStyle(smallStyle) { alignment = TextAnchor.MiddleCenter };
            GUI.Label(new Rect(x, y + 61f, totalWidth, 20f), "Q  ◀ MUDAR ÉPOCA ▶  E", helper);
        }

        private void DrawEraBadge(Rect rect, int year, string era, bool active)
        {
            var previous = GUI.color;
            var tint = TimeShiftPresentation.EraTint(year);
            GUI.color = active
                ? new Color(tint.r, tint.g, tint.b, 0.96f)
                : new Color(0.12f, 0.14f, 0.18f, 0.92f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = previous;

            GUI.Label(new Rect(rect.x, rect.y + 6, rect.width, 26), year.ToString(), timelineYearStyle);
            GUI.Label(new Rect(rect.x, rect.y + 33, rect.width, 17), era, timelineEraStyle);

            if (active)
            {
                GUI.color = tint;
                GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height - 4f, rect.width, 4f), Texture2D.whiteTexture);
                GUI.color = previous;
            }
        }

        private void DrawMission(StoryDirector director, int currentYear)
        {
            var width = Mathf.Min(455f, viewWidth * 0.40f);
            var rect = new Rect(viewWidth - width - 18f, 18f, width, 174f);
            GUI.DrawTexture(rect, darkerPanel);

            GUI.Label(new Rect(rect.x + 18, rect.y + 12, rect.width - 36, 20), "MISSÃO  •  " + director.MissionTitle, missionHeaderStyle);
            GUI.Label(new Rect(rect.x + 18, rect.y + 35, rect.width - 36, 21), director.ObjectiveStep, missionStepStyle);
            GUI.Label(new Rect(rect.x + 18, rect.y + 59, rect.width - 36, 50), director.Objective, missionTextStyle);

            if (!director.HasObjectiveTarget) return;

            var correctYear = director.RequiredYear == 0 || director.RequiredYear == currentYear;
            var targetLine = "ALVO: " + director.ObjectiveTargetName;
            if (director.DistanceToObjective > 0.1f)
                targetLine += "   •   " + Mathf.RoundToInt(director.DistanceToObjective) + "m";
            GUI.Label(new Rect(rect.x + 18, rect.y + 113, rect.width - 74, 20), targetLine, targetStyle);

            var yearText = director.RequiredYear == 0
                ? string.Empty
                : correctYear ? "✓ ÉPOCA CERTA: " + currentYear : "⚠ VÁ PARA: " + director.RequiredYear;
            var previous = missionStepStyle.normal.textColor;
            missionStepStyle.normal.textColor = correctYear
                ? new Color(0.45f, 0.94f, 0.58f)
                : new Color(1f, 0.48f, 0.24f);
            GUI.Label(new Rect(rect.x + 18, rect.y + 138, rect.width - 74, 21), yearText, missionStepStyle);
            missionStepStyle.normal.textColor = previous;

            DrawDirectionArrow(new Vector2(rect.x + rect.width - 42, rect.y + 136), director.ObjectiveTarget);
        }

        private void DrawDirectionArrow(Vector2 center, Vector2 target)
        {
            if (player == null) return;
            var direction = target - (Vector2)player.position;
            if (direction.sqrMagnitude < 0.01f) return;

            var angle = -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            var oldMatrix = GUI.matrix;
            GUIUtility.RotateAroundPivot(angle, center);
            GUI.Label(new Rect(center.x - 19, center.y - 19, 38, 38), "▲", centerStyle);
            GUI.matrix = oldMatrix;
        }

        private void DrawContextHint(StoryDirector director)
        {
            if (director == null || string.IsNullOrWhiteSpace(director.ContextHint)) return;

            var width = Mathf.Min(650f, viewWidth - 80f);
            var x = (viewWidth - width) * 0.5f;
            var y = director.DialogueOpen ? viewHeight - 214f : viewHeight - 68f;
            GUI.DrawTexture(new Rect(x, y, width, 42f), darkPanel);
            GUI.Label(new Rect(x + 12, y + 6, width - 24, 30f), director.ContextHint, hintStyle);
        }

        private static Texture2D MakeTexture(Color32 color)
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, color);
            texture.Apply(false, true);
            return texture;
        }
    }
}

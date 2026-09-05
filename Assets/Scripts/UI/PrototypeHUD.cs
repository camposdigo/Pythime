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

        public void Initialize(StoryDirector storyDirector, Transform playerTransform)
        {
            story = storyDirector;
            player = playerTransform;
        }

        private void BuildStyles()
        {
            if (brandStyle != null) return;

            darkPanel = MakeTexture(new Color32(16, 20, 26, 235));
            darkerPanel = MakeTexture(new Color32(10, 13, 18, 245));

            brandStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold
            };
            brandStyle.normal.textColor = new Color(0.94f, 0.97f, 1f);

            smallStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11
            };
            smallStyle.normal.textColor = new Color(0.65f, 0.72f, 0.79f);

            timelineYearStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            timelineYearStyle.normal.textColor = Color.white;

            timelineEraStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 9,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            timelineEraStyle.normal.textColor = new Color(0.86f, 0.89f, 0.92f);

            missionHeaderStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold
            };
            missionHeaderStyle.normal.textColor = new Color(1f, 0.79f, 0.24f);

            missionStepStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };
            missionStepStyle.normal.textColor = new Color(0.56f, 0.91f, 1f);

            missionTextStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                wordWrap = true
            };
            missionTextStyle.normal.textColor = Color.white;

            targetStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold
            };
            targetStyle.normal.textColor = new Color(0.90f, 0.93f, 0.96f);

            hintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            hintStyle.normal.textColor = new Color(1f, 0.86f, 0.34f);

            centerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
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

            DrawBrand(story != null ? story.LocationName : "Pythime");
            DrawTimeline(timeline.CurrentYear);
            if (story != null) DrawMission(story, timeline.CurrentYear);
            DrawContextHint(story);
        }

        private void DrawBrand(string location)
        {
            GUI.DrawTexture(new Rect(18, 18, 216, 72), darkPanel);
            GUI.Label(new Rect(32, 25, 170, 30), "PYTHIME", brandStyle);
            GUI.Label(new Rect(32, 56, 185, 20), location.ToUpperInvariant(), smallStyle);
            GUI.Label(new Rect(32, 76, 185, 16), "TAB personagem   •   P terminal", smallStyle);
        }

        private void DrawTimeline(int currentYear)
        {
            const float badgeWidth = 112f;
            const float gap = 6f;
            var totalWidth = badgeWidth * 3f + gap * 2f;
            var x = (Screen.width - totalWidth) * 0.5f;
            var y = 18f;

            DrawEraBadge(new Rect(x, y, badgeWidth, 56f), 1956, "PASSADO", currentYear == 1956);
            DrawEraBadge(new Rect(x + badgeWidth + gap, y, badgeWidth, 56f), 2026, "PRESENTE", currentYear == 2026);
            DrawEraBadge(new Rect(x + (badgeWidth + gap) * 2f, y, badgeWidth, 56f), 2096, "FUTURO", currentYear == 2096);

            GUI.Label(new Rect(x, y + 57f, totalWidth, 18f), "Q  ◀ mudar época ▶  E", smallStyle);
        }

        private void DrawEraBadge(Rect rect, int year, string era, bool active)
        {
            var previous = GUI.color;
            var tint = TimeShiftPresentation.EraTint(year);
            GUI.color = active
                ? new Color(tint.r, tint.g, tint.b, 0.92f)
                : new Color(0.12f, 0.14f, 0.18f, 0.88f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = previous;

            GUI.Label(new Rect(rect.x, rect.y + 6, rect.width, 25), year.ToString(), timelineYearStyle);
            GUI.Label(new Rect(rect.x, rect.y + 31, rect.width, 16), era, timelineEraStyle);

            if (active)
            {
                GUI.color = tint;
                GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height - 3f, rect.width, 3f), Texture2D.whiteTexture);
                GUI.color = previous;
            }
        }

        private void DrawMission(StoryDirector director, int currentYear)
        {
            var width = Mathf.Min(420f, Screen.width * 0.38f);
            var rect = new Rect(Screen.width - width - 18f, 18f, width, 164f);
            GUI.DrawTexture(rect, darkerPanel);

            GUI.Label(new Rect(rect.x + 18, rect.y + 12, rect.width - 36, 20), "MISSÃO  •  " + director.MissionTitle, missionHeaderStyle);
            GUI.Label(new Rect(rect.x + 18, rect.y + 34, rect.width - 36, 20), director.ObjectiveStep, missionStepStyle);
            GUI.Label(new Rect(rect.x + 18, rect.y + 56, rect.width - 36, 48), director.Objective, missionTextStyle);

            if (!director.HasObjectiveTarget) return;

            var correctYear = director.RequiredYear == 0 || director.RequiredYear == currentYear;
            var targetLine = director.ObjectiveTargetName;
            if (director.DistanceToObjective > 0.1f)
                targetLine += "   •   " + Mathf.RoundToInt(director.DistanceToObjective) + "m";
            GUI.Label(new Rect(rect.x + 18, rect.y + 108, rect.width - 72, 20), targetLine, targetStyle);

            var yearText = director.RequiredYear == 0
                ? string.Empty
                : correctYear ? "ÉPOCA CERTA: " + currentYear : "VÁ PARA: " + director.RequiredYear;
            var previous = missionStepStyle.normal.textColor;
            missionStepStyle.normal.textColor = correctYear
                ? new Color(0.45f, 0.94f, 0.58f)
                : new Color(1f, 0.48f, 0.24f);
            GUI.Label(new Rect(rect.x + 18, rect.y + 130, rect.width - 72, 20), yearText, missionStepStyle);
            missionStepStyle.normal.textColor = previous;

            DrawDirectionArrow(new Vector2(rect.x + rect.width - 40, rect.y + 126), director.ObjectiveTarget);
        }

        private void DrawDirectionArrow(Vector2 center, Vector2 target)
        {
            if (player == null) return;
            var direction = target - (Vector2)player.position;
            if (direction.sqrMagnitude < 0.01f) return;

            var angle = -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            var oldMatrix = GUI.matrix;
            GUIUtility.RotateAroundPivot(angle, center);
            GUI.Label(new Rect(center.x - 18, center.y - 18, 36, 36), "▲", centerStyle);
            GUI.matrix = oldMatrix;
        }

        private void DrawContextHint(StoryDirector director)
        {
            if (director == null || string.IsNullOrWhiteSpace(director.ContextHint)) return;

            var width = Mathf.Min(620f, Screen.width - 80f);
            var x = (Screen.width - width) * 0.5f;
            var y = director.DialogueOpen ? Screen.height - 205f : Screen.height - 62f;
            GUI.DrawTexture(new Rect(x, y, width, 38f), darkPanel);
            GUI.Label(new Rect(x + 12, y + 5, width - 24, 28f), director.ContextHint, hintStyle);
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

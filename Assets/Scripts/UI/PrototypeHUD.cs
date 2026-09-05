using UnityEngine;

namespace Pythime
{
    public sealed class PrototypeHUD : MonoBehaviour
    {
        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;
        private GUIStyle yearStyle;
        private GUIStyle hintStyle;
        private Texture2D panelTexture;
        private Texture2D badgeTexture;

        private void BuildStyles()
        {
            if (panelStyle != null) return;

            panelTexture = MakeTexture(new Color32(18, 22, 28, 225));
            badgeTexture = MakeTexture(new Color32(37, 46, 57, 245));

            panelStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(14, 14, 12, 12)
            };
            panelStyle.normal.background = panelTexture;

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                fontStyle = FontStyle.Bold
            };
            titleStyle.normal.textColor = new Color(0.93f, 0.97f, 1f);

            bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                wordWrap = true
            };
            bodyStyle.normal.textColor = new Color(0.75f, 0.81f, 0.86f);

            yearStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            yearStyle.normal.background = badgeTexture;
            yearStyle.normal.textColor = new Color(0.34f, 0.91f, 1f);

            hintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };
            hintStyle.normal.textColor = new Color(1f, 0.82f, 0.32f);
        }

        private void OnGUI()
        {
            BuildStyles();
            var timeline = TimeTravelManager.Instance;
            if (timeline == null) return;

            var story = FindFirstObjectByType<StoryDirector>();
            var dialogueOpen = story != null && story.DialogueOpen;

            GUI.Box(new Rect(18, 18, 288, 106), GUIContent.none, panelStyle);
            GUI.Label(new Rect(34, 28, 150, 30), "PYTHIME", titleStyle);
            GUI.Box(new Rect(207, 27, 80, 30), timeline.CurrentYear.ToString(), yearStyle);
            GUI.Label(new Rect(34, 62, 245, 48), "WASD mover  •  Q/E tempo\nP PyTerminal  •  C roupa  •  H cabelo", bodyStyle);

            if (!dialogueOpen && timeline.CurrentYear == 1956 && !timeline.SeedPlanted)
            {
                GUI.Box(new Rect(18, Screen.height - 68, 355, 44), GUIContent.none, panelStyle);
                GUI.Label(new Rect(34, Screen.height - 57, 325, 22), "T  interagir com o canteiro temporal", hintStyle);
            }
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

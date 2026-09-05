using UnityEngine;

namespace Pythime
{
    public sealed class PrototypeHUD : MonoBehaviour
    {
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;
        private GUIStyle badgeStyle;

        private void BuildStyles()
        {
            if (titleStyle != null) return;

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };
            titleStyle.normal.textColor = Color.white;

            bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                wordWrap = true
            };
            bodyStyle.normal.textColor = new Color(0.90f, 0.93f, 0.96f);

            badgeStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            badgeStyle.normal.textColor = Color.white;
        }

        private void OnGUI()
        {
            BuildStyles();
            var timeline = TimeTravelManager.Instance;
            if (timeline == null) return;

            GUI.Box(new Rect(18, 18, 260, 94), string.Empty);
            GUI.Label(new Rect(32, 26, 130, 28), "PYTHIME", titleStyle);
            GUI.Box(new Rect(174, 24, 86, 30), timeline.CurrentYear.ToString(), badgeStyle);
            GUI.Label(new Rect(32, 60, 220, 42),
                "WASD mover   Q/E tempo   P PyTerminal\nC roupa   H cabelo   T ação temporal   F interagir", bodyStyle);
        }
    }
}

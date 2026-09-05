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
                fontSize = 24,
                fontStyle = FontStyle.Bold
            };
            titleStyle.normal.textColor = Color.white;

            bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                wordWrap = true
            };
            bodyStyle.normal.textColor = new Color(0.95f, 0.95f, 0.95f);

            badgeStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 18,
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

            GUI.Box(new Rect(18, 18, 330, 132), string.Empty);
            GUI.Label(new Rect(32, 28, 190, 34), "PYTHIME", titleStyle);
            GUI.Box(new Rect(238, 28, 92, 34), timeline.CurrentYear.ToString(), badgeStyle);
            GUI.Label(new Rect(32, 68, 300, 72),
                "WASD/Setas  mover\nQ / E  viajar no tempo   P  PyTerminal\nC  roupa   H  cabelo   T  plantar em 1956", bodyStyle);

            if (timeline.CurrentYear == 1956)
            {
                GUI.Box(new Rect(18, Screen.height - 76, 430, 52), string.Empty);
                GUI.Label(new Rect(30, Screen.height - 67, 405, 36),
                    timeline.SeedPlanted
                        ? "A muda existe nesta linha do tempo. Viaje ao futuro."
                        : "Encontre o canteiro marrom ao norte e pressione T.", bodyStyle);
            }
        }
    }
}

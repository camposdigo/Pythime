using UnityEngine;

namespace Pythime
{
    public sealed class TimeShiftPresentation : MonoBehaviour
    {
        private TimeTravelManager timeline;
        private float timer;
        private int year;
        private bool initialized;

        private GUIStyle yearStyle;
        private GUIStyle eraStyle;
        private GUIStyle descriptionStyle;
        private Texture2D panelTexture;

        public void Initialize(TimeTravelManager manager)
        {
            if (initialized) return;
            initialized = true;
            timeline = manager;
            if (timeline == null) return;

            timeline.EraChanged += OnEraChanged;
            Show(timeline.CurrentYear, 1.35f);
        }

        private void OnDestroy()
        {
            if (timeline != null)
                timeline.EraChanged -= OnEraChanged;
        }

        private void OnEraChanged(int newYear)
        {
            Show(newYear, 1.05f);
        }

        private void Show(int newYear, float duration)
        {
            year = newYear;
            timer = duration;
        }

        private void Update()
        {
            if (timer > 0f)
                timer = Mathf.Max(0f, timer - Time.unscaledDeltaTime);
        }

        private void BuildStyles()
        {
            if (yearStyle != null) return;

            panelTexture = MakeTexture(new Color32(13, 16, 22, 235));

            yearStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 52,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            yearStyle.normal.textColor = Color.white;

            eraStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            descriptionStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter
            };
            descriptionStyle.normal.textColor = new Color(0.80f, 0.84f, 0.89f);
        }

        private void OnGUI()
        {
            if (timer <= 0f) return;
            BuildStyles();

            var normalized = Mathf.Clamp01(timer / 1.05f);
            var fade = Mathf.Min(1f, normalized * 2.2f);
            var tint = EraTint(year);

            var previousColor = GUI.color;
            GUI.color = new Color(tint.r, tint.g, tint.b, 0.12f * fade);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = previousColor;

            var width = Mathf.Min(540f, Screen.width - 60f);
            var height = 154f;
            var x = (Screen.width - width) * 0.5f;
            var y = Screen.height * 0.17f;

            GUI.color = new Color(1f, 1f, 1f, fade);
            GUI.DrawTexture(new Rect(x, y, width, height), panelTexture);

            eraStyle.normal.textColor = tint;
            GUI.Label(new Rect(x + 20, y + 16, width - 40, 28), EraName(year), eraStyle);
            GUI.Label(new Rect(x + 20, y + 40, width - 40, 66), year.ToString(), yearStyle);
            GUI.Label(new Rect(x + 24, y + 108, width - 48, 28), EraDescription(year), descriptionStyle);
            GUI.color = previousColor;
        }

        public static Color EraTint(int targetYear)
        {
            if (targetYear == 1956) return new Color(1f, 0.73f, 0.35f);
            if (targetYear == 2096) return new Color(0.30f, 0.94f, 0.96f);
            return new Color(0.42f, 0.76f, 1f);
        }

        public static string EraName(int targetYear)
        {
            if (targetYear == 1956) return "PASSADO";
            if (targetYear == 2096) return "FUTURO";
            return "PRESENTE";
        }

        private static string EraDescription(int targetYear)
        {
            if (targetYear == 1956) return "Pythime ainda está crescendo — terrenos vazios, obras e tecnologia analógica.";
            if (targetYear == 2096) return "Pythime foi reconstruída — neon, estruturas sintéticas e anomalias temporais.";
            return "A Pythime que você conhece — ponto de referência para comparar as mudanças.";
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

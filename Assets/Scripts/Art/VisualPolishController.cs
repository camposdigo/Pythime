using System.Collections;
using UnityEngine;

namespace Pythime
{
    [DefaultExecutionOrder(900)]
    public sealed class VisualPolishController : MonoBehaviour
    {
        private float transitionAlpha;
        private int transitionYear;
        private GUIStyle yearStyle;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (OfficialEraMapRuntime.IsAvailable) return;
            if (GameObject.Find("PythimeVisualPolish") != null) return;
            var go = new GameObject("PythimeVisualPolish");
            go.AddComponent<VisualPolishController>();
        }

        private IEnumerator Start()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;

            for (var i = 0; i < 20 && GameObject.Find("PythimeRuntime") == null; i++)
                yield return null;

            var runtime = GameObject.Find("PythimeRuntime");
            if (runtime == null) yield break;

            var player = GameObject.Find("Player");
            if (player != null)
            {
                player.transform.position = StoryWorldFactory.TileToWorld(22f, 19f);
                var body = player.GetComponent<Rigidbody2D>();
                if (body != null) body.linearVelocity = Vector2.zero;
            }

            if (Camera.main != null)
            {
                Camera.main.orthographic = true;
                Camera.main.orthographicSize = 5.7f;
                if (Camera.main.GetComponent<PixelSnapCamera>() == null)
                    Camera.main.gameObject.AddComponent<PixelSnapCamera>();
            }

            var transforms = runtime.GetComponentsInChildren<Transform>(true);
            for (var i = 0; i < transforms.Length; i++)
            {
                var current = transforms[i];
                if (!current.name.StartsWith("TemporalVehicle_")) continue;
                if (current.Find("TemporalGlow") != null) continue;

                var split = current.name.Split('_');
                var year = 2026;
                if (split.Length > 1) int.TryParse(split[split.Length - 1], out year);

                var glow = new GameObject("TemporalGlow");
                glow.transform.SetParent(current);
                glow.transform.localPosition = new Vector3(0f, 0.12f, 0f);
                var renderer = glow.AddComponent<SpriteRenderer>();
                renderer.sprite = StoryWorldFactory.CreateVehicleGlowSprite(year);
                renderer.sortingOrder = 10;
                renderer.color = new Color(1f, 1f, 1f, 0.45f);
                glow.AddComponent<TemporalVehiclePulse>();
            }

            if (TimeTravelManager.Instance != null)
            {
                TimeTravelManager.Instance.EraChanged += OnEraChanged;
                transitionYear = TimeTravelManager.Instance.CurrentYear;
            }
        }

        private void OnDestroy()
        {
            if (TimeTravelManager.Instance != null)
                TimeTravelManager.Instance.EraChanged -= OnEraChanged;
        }

        private void OnEraChanged(int year)
        {
            transitionYear = year;
            transitionAlpha = 0.9f;
        }

        private void Update()
        {
            if (transitionAlpha > 0f)
                transitionAlpha = Mathf.MoveTowards(transitionAlpha, 0f, Time.unscaledDeltaTime * 1.5f);
        }

        private void OnGUI()
        {
            if (transitionAlpha <= 0.001f) return;

            var old = GUI.color;
            GUI.color = new Color(0.20f, 0.88f, 1f, transitionAlpha * 0.28f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = old;

            if (yearStyle == null)
            {
                yearStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 36,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                yearStyle.normal.textColor = Color.white;
            }

            var textColor = yearStyle.normal.textColor;
            yearStyle.normal.textColor = new Color(textColor.r, textColor.g, textColor.b, Mathf.Clamp01(transitionAlpha * 1.5f));
            GUI.Label(new Rect(0f, Screen.height * 0.40f, Screen.width, 70f), transitionYear.ToString(), yearStyle);
        }
    }
}

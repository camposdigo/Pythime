using UnityEngine;

namespace Pythime
{
    public sealed class ObjectiveBeacon : MonoBehaviour
    {
        private Transform player;
        private StoryDirector story;
        private GameObject marker;
        private SpriteRenderer markerRenderer;
        private Vector3 baseScale = Vector3.one;

        public void Initialize(Transform playerTransform, StoryDirector storyDirector)
        {
            player = playerTransform;
            story = storyDirector;
            BuildMarker();
        }

        private void BuildMarker()
        {
            marker = new GameObject("ObjectiveBeaconMarker");
            marker.transform.SetParent(transform);
            markerRenderer = marker.AddComponent<SpriteRenderer>();
            markerRenderer.sprite = BuildBeaconSprite();
            markerRenderer.sortingOrder = 500;
            baseScale = new Vector3(1.15f, 1.15f, 1f);
            marker.transform.localScale = baseScale;
            marker.SetActive(false);
        }

        private void Update()
        {
            if (story == null || marker == null)
                return;

            var show = story.HasObjectiveTarget && !story.ChapterComplete;
            if (marker.activeSelf != show)
                marker.SetActive(show);
            if (!show) return;

            var time = Time.time;
            var bob = Mathf.Sin(time * 3.4f) * 0.12f;
            marker.transform.position = new Vector3(story.ObjectiveTarget.x, story.ObjectiveTarget.y + 1.25f + bob, 0f);

            var pulse = 1f + Mathf.Sin(time * 5f) * 0.08f;
            marker.transform.localScale = baseScale * pulse;

            var timeline = TimeTravelManager.Instance;
            var correctYear = story.RequiredYear == 0 || timeline == null || timeline.CurrentYear == story.RequiredYear;
            markerRenderer.color = correctYear
                ? new Color(1f, 0.83f, 0.20f, 1f)
                : new Color(1f, 0.45f, 0.20f, 0.78f);
        }

        private static Sprite BuildBeaconSprite()
        {
            const int width = 18;
            const int height = 26;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = "ObjectiveBeacon"
            };

            var clear = new Color32(0, 0, 0, 0);
            var outline = new Color32(34, 31, 27, 255);
            var gold = new Color32(255, 204, 49, 255);
            var shine = new Color32(255, 240, 156, 255);
            var pixels = new Color32[width * height];
            for (var i = 0; i < pixels.Length; i++) pixels[i] = clear;
            texture.SetPixels32(pixels);

            FillRect(texture, 7, 1, 4, 7, outline);
            FillRect(texture, 8, 2, 2, 5, gold);

            FillRect(texture, 5, 8, 8, 2, outline);
            FillRect(texture, 3, 10, 12, 8, outline);
            FillRect(texture, 5, 18, 8, 4, outline);
            FillRect(texture, 6, 10, 6, 9, gold);
            FillRect(texture, 7, 11, 4, 4, shine);
            FillRect(texture, 8, 16, 2, 2, outline);

            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0f), 16f);
            sprite.name = "ObjectiveBeacon";
            return sprite;
        }

        private static void FillRect(Texture2D texture, int x, int y, int width, int height, Color32 color)
        {
            for (var py = y; py < y + height; py++)
            for (var px = x; px < x + width; px++)
                if (px >= 0 && py >= 0 && px < texture.width && py < texture.height)
                    texture.SetPixel(px, py, color);
        }
    }
}

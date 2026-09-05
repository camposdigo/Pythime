using System.Collections;
using System.Reflection;
using UnityEngine;

namespace Pythime
{
    [DefaultExecutionOrder(-1800)]
    public sealed class OfficialEraMapRuntime : MonoBehaviour
    {
        private const string RootName = "PythimeOfficialMaps";
        private static readonly Vector2 InstituteDoor = StoryWorldFactory.TileToWorld(32f, 21.2f);
        private static readonly Vector2 FountainPoint = StoryWorldFactory.TileToWorld(32f, 15.7f);
        private static readonly Vector2 FutureAnomaly = StoryWorldFactory.TileToWorld(32f, 27.5f);
        private static readonly int[] Years = { 1956, 2026, 2096 };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (GameObject.Find(RootName) != null) return;
            var root = new GameObject(RootName);
            root.AddComponent<OfficialEraMapRuntime>();
        }

        private IEnumerator Start()
        {
            for (var i = 0; i < 180; i++)
            {
                if (GameObject.Find("PythimeRuntime") != null && GameObject.Find("Era_2026") != null)
                    break;
                yield return null;
            }

            ApplyOfficialMaps();
            DisableOldProceduralPolish();
            DisableOldBuildingColliders();
            MovePlayerToOfficialStart();
            PatchStoryTargets();
        }

        private static void ApplyOfficialMaps()
        {
            foreach (var year in Years)
            {
                var texture = Resources.Load<Texture2D>("OfficialMaps/city_" + year);
                var mapRenderer = FindMapRenderer(year);
                if (texture == null || mapRenderer == null) continue;

                texture.filterMode = FilterMode.Point;
                texture.wrapMode = TextureWrapMode.Clamp;

                var ppu = texture.width / (float)StoryWorldFactory.MapWidthTiles;
                var sprite = Sprite.Create(
                    texture,
                    new Rect(0f, 0f, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f),
                    ppu,
                    0,
                    SpriteMeshType.FullRect);
                sprite.name = "OfficialCity_" + year;

                mapRenderer.sprite = sprite;
                mapRenderer.color = Color.white;
                mapRenderer.sortingOrder = -60;
                mapRenderer.transform.localPosition = Vector3.zero;
                mapRenderer.transform.localScale = Vector3.one;
            }
        }

        private static SpriteRenderer FindMapRenderer(int year)
        {
            var map = GameObject.Find("PythimeCity_" + year);
            return map != null ? map.GetComponent<SpriteRenderer>() : null;
        }

        private static void DisableOldProceduralPolish()
        {
            DisableRoot("PythimeAreaPolish");
            DisableRoot("PythimeWorldDepth");
            DisableRoot("PythimeKenneyTilemapOverlay");

            foreach (var year in Years)
            {
                var era = GameObject.Find("Era_" + year);
                if (era == null) continue;

                for (var i = 0; i < era.transform.childCount; i++)
                {
                    var child = era.transform.GetChild(i).gameObject;
                    var name = child.name;
                    if (name.Contains("AreaPolish") || name.Contains("Kenney") || name.Contains("Depth") || name.Contains("StreetProp"))
                        child.SetActive(false);
                }
            }
        }

        private static void DisableRoot(string name)
        {
            var go = GameObject.Find(name);
            if (go != null) go.SetActive(false);
        }

        private static void DisableOldBuildingColliders()
        {
            foreach (var year in Years)
            {
                var era = GameObject.Find("Era_" + year);
                if (era == null) continue;

                var colliders = era.GetComponentsInChildren<BoxCollider2D>(true);
                foreach (var collider in colliders)
                {
                    if (collider == null) continue;
                    var n = collider.gameObject.name;
                    if (n.StartsWith("BuildingCollider_"))
                        collider.gameObject.SetActive(false);
                }
            }
        }

        private static void MovePlayerToOfficialStart()
        {
            var player = GameObject.Find("Player");
            if (player == null) return;

            player.transform.position = StoryWorldFactory.TileToWorld(32f, 18.3f);
            var body = player.GetComponent<Rigidbody2D>();
            if (body != null) body.linearVelocity = Vector2.zero;
        }

        private static void PatchStoryTargets()
        {
            var runtime = GameObject.Find("PythimeRuntime");
            if (runtime == null) return;

            var story = runtime.GetComponent<StoryDirector>();
            if (story == null) return;

            SetPrivateVector(story, "workshopPoint", InstituteDoor);
            SetPrivateVector(story, "soilPoint", FountainPoint);
            SetPrivateVector(story, "monolithPoint", FutureAnomaly);

            if (story.HasObjectiveTarget && story.ChapterStage <= 1)
                SetPrivateVector(story, "objectiveTarget", InstituteDoor);
        }

        private static void SetPrivateVector(StoryDirector story, string fieldName, Vector2 value)
        {
            var field = typeof(StoryDirector).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null) field.SetValue(story, value);
        }
    }
}

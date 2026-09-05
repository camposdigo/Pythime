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
        private static bool officialMapsAvailable;

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

            officialMapsAvailable = HasOfficialMaps();
            if (!officialMapsAvailable) yield break;

            ApplyOfficialMaps();
            DisableOldProceduralPolish();
            DisableOldBuildingColliders();
            DisableRuntimeObject("PythimeAreaPolish");
            DisableRuntimeObject("PythimeWorldDepth");
            DisableRuntimeObject("PythimeWorldDensity");
            DisableRuntimeObject("PythimeKenneyTilemapOverlay");
            DisableRuntimeObject("PythimeNpcPopulation");
            DisableRuntimeObject("PythimeNpcVisualVariation");
            DisableRuntimeObject("PythimeSocialNpcGroups");
            DisableRuntimeObject("PythimeWorkshopInterior");
            BuildOfficialMapCollisions();
            MovePlayerToOfficialStart();
            PatchStoryTargets();
            TuneCameraForOfficialMap();
        }

        private static bool HasOfficialMaps()
        {
            return Resources.Load<Texture2D>("OfficialMaps/city_1956") != null
                || Resources.Load<Texture2D>("OfficialMaps/city_2026") != null
                || Resources.Load<Texture2D>("OfficialMaps/city_2096") != null;
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
            foreach (var year in Years)
            {
                var era = GameObject.Find("Era_" + year);
                if (era == null) continue;

                for (var i = 0; i < era.transform.childCount; i++)
                {
                    var child = era.transform.GetChild(i).gameObject;
                    var name = child.name;
                    if (name.Contains("AreaPolish") || name.Contains("Kenney") || name.Contains("Depth") || name.Contains("StreetProp") || name.Contains("TemporalVehicle") || name.Contains("ImpossibleMonolith") || name.Contains("TemporalSoil"))
                        child.SetActive(false);
                }
            }
        }

        private static void DisableRuntimeObject(string name)
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

        private static void BuildOfficialMapCollisions()
        {
            foreach (var year in Years)
            {
                var era = GameObject.Find("Era_" + year);
                if (era == null) continue;
                if (era.transform.Find("OfficialMapCollision") != null) continue;

                var root = new GameObject("OfficialMapCollision");
                root.transform.SetParent(era.transform);
                root.transform.localPosition = Vector3.zero;

                AddCollider(root.transform, "NorthBuildings", 0f, 17.7f, 62f, 7.4f);
                AddCollider(root.transform, "SouthBuildings", 0f, -20.0f, 62f, 5.0f);
                AddCollider(root.transform, "WestBuildings", -27.9f, 0.2f, 7.6f, 32f);
                AddCollider(root.transform, "EastBuildings", 27.9f, 0.2f, 7.6f, 32f);

                AddCollider(root.transform, "CityHallBody", 0f, 7.9f, 10.8f, 7.0f);
                AddCollider(root.transform, "CityHallSteps", 0f, 3.9f, 5.2f, 1.1f);
                AddCollider(root.transform, "Fountain", 0f, -7.8f, 3.1f, 2.6f);
                AddCollider(root.transform, "ParkNorthLeft", -8.0f, -4.0f, 3.0f, 2.2f);
                AddCollider(root.transform, "ParkNorthRight", 8.0f, -4.0f, 3.0f, 2.2f);
                AddCollider(root.transform, "ParkSouthLeft", -8.4f, -11.2f, 4.6f, 2.2f);
                AddCollider(root.transform, "ParkSouthRight", 8.4f, -11.2f, 4.6f, 2.2f);

                AddCollider(root.transform, "MapTop", 0f, 23.45f, 64f, 0.7f);
                AddCollider(root.transform, "MapBottom", 0f, -23.45f, 64f, 0.7f);
                AddCollider(root.transform, "MapLeft", -32.35f, 0f, 0.7f, 46f);
                AddCollider(root.transform, "MapRight", 32.35f, 0f, 0.7f, 46f);
            }
        }

        private static void AddCollider(Transform parent, string name, float x, float y, float width, float height)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.localPosition = new Vector3(x, y, 0f);
            var box = go.AddComponent<BoxCollider2D>();
            box.size = new Vector2(width, height);
        }

        private static void MovePlayerToOfficialStart()
        {
            var player = GameObject.Find("Player");
            if (player == null) return;

            player.transform.position = StoryWorldFactory.TileToWorld(32f, 13.0f);
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

        private static void TuneCameraForOfficialMap()
        {
            var camera = Camera.main;
            if (camera == null) return;
            camera.orthographicSize = 8.2f;
        }

        private static void SetPrivateVector(StoryDirector story, string fieldName, Vector2 value)
        {
            var field = typeof(StoryDirector).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null) field.SetValue(story, value);
        }
    }
}

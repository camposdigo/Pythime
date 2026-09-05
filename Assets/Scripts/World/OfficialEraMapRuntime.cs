using System.Collections.Generic;
using UnityEngine;

namespace Pythime
{
    public sealed class OfficialEraMapRuntime : MonoBehaviour
    {
        public const float MapSize = 64f;
        public static readonly Rect MapBounds = new Rect(-32f, -32f, MapSize, MapSize);
        public static Vector2 SpawnPoint => MapPoint(.5f, .55f);
        public static Vector2 WorkshopPoint => MapPoint(.5f, .505f);
        public static Vector2 SoilPoint => MapPoint(.5f, .555f);
        public static Vector2 AnomalyPoint => MapPoint(.5f, .73f);
        private static bool? available;
        private readonly List<Sprite> ownedSprites = new List<Sprite>();
        private TimeTravelManager timeline;
        private Transform player;

        public static bool IsAvailable
        {
            get
            {
                if (!available.HasValue)
                    available = GetMapTexture(1956) != null && GetMapTexture(2026) != null && GetMapTexture(2096) != null;
                return available.Value;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetAvailability() => available = null;

        public static Texture2D GetMapTexture(int year)
        {
            var path = "OfficialMaps/city_" + year;
            var sprite = Resources.Load<Sprite>(path);
            return sprite != null ? sprite.texture : Resources.Load<Texture2D>(path);
        }

        // Normalized source coordinates measured from the top-left of the official artwork.
        public static Vector2 MapPoint(float x, float y) => new Vector2((x - .5f) * MapSize, (.5f - y) * MapSize);

        public void BuildEra(Transform era, int year)
        {
            var texture = GetMapTexture(year);
            var map = new GameObject("PythimeCity_" + year);
            map.transform.SetParent(era, false);
            var renderer = map.AddComponent<SpriteRenderer>();
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), texture.width / MapSize, 0, SpriteMeshType.FullRect);
            sprite.name = "OfficialCity_" + year;
            ownedSprites.Add(sprite);
            renderer.sprite = sprite;
            renderer.sortingOrder = -60;
            var collisionRoot = new GameObject("PythimeOfficialMapColliders");
            collisionRoot.transform.SetParent(era, false);
            var root = collisionRoot.transform;
            Box(root, "MapTop", 0, -.015f, 1, 0);
            Box(root, "MapBottom", 0, 1, 1, 1.015f);
            Box(root, "MapLeft", -.015f, 0, 0, 1);
            Box(root, "MapRight", 1, 0, 1.015f, 1);
            if (year == 1956) BuildPastColliders(root);
            else BuildModernColliders(root);
        }

        private static void BuildModernColliders(Transform root)
        {
            Box(root, "NorthBuildingsWest", .010f, .006f, .421f, .151f);
            Box(root, "NorthBuildingsEast", .574f, .007f, .733f, .152f);
            Box(root, "NorthBuildingsFarEast", .816f, .006f, .965f, .151f);
            Box(root, "WestBuildingsNorth", .024f, .253f, .157f, .506f);
            Box(root, "WestBuildingsSouth", .030f, .516f, .160f, .674f);
            Box(root, "EastBuildingsNorth", .822f, .246f, .976f, .397f);
            Box(root, "EastBuildingsMiddle", .841f, .400f, .971f, .552f);
            Box(root, "EastBuildingsSouth", .828f, .555f, .969f, .671f);
            Box(root, "SouthBuildingsWest", .019f, .827f, .124f, .978f);
            Box(root, "SouthBuildingsMiddle", .190f, .842f, .409f, .978f);
            Box(root, "SouthBuildingsEast", .597f, .847f, .731f, .978f);
            Box(root, "SouthBuildingsFarEast", .830f, .827f, .970f, .978f);
            Box(root, "CentralBuilding", .351f, .235f, .649f, .474f);
            Box(root, "CentralTower", .458f, .180f, .540f, .237f);
            Box(root, "CentralHedgeLeft", .325f, .337f, .350f, .492f);
            Box(root, "CentralHedgeRight", .650f, .337f, .675f, .492f);
            Box(root, "CentralHedgeSouthLeft", .348f, .471f, .423f, .495f);
            Box(root, "CentralHedgeSouthRight", .575f, .471f, .655f, .495f);
            Box(root, "Fountain", .454f, .611f, .546f, .697f);
            // C-shaped gardens preserve the inner paths around the fountain.
            Box(root, "PlanterNorthLeft", .321f, .532f, .463f, .610f);
            Box(root, "PlanterNorthRight", .535f, .532f, .677f, .610f);
            Box(root, "PlanterWest", .318f, .610f, .388f, .708f);
            Box(root, "PlanterEast", .609f, .610f, .678f, .708f);
            Box(root, "PlanterSouthLeft", .320f, .705f, .467f, .735f);
            Box(root, "PlanterSouthRight", .575f, .705f, .677f, .735f);
            Tree(root, "NorthEast", .780f, .102f);
            Tree(root, "WestNorth", .181f, .346f);
            Tree(root, "WestMiddle", .171f, .450f);
            Tree(root, "SouthWest", .151f, .907f);
            Tree(root, "SouthEast", .773f, .906f);
            Lamp(root, "NorthWest", .434f, .149f);
            Lamp(root, "NorthEast", .563f, .149f);
            Lamp(root, "HallWest", .332f, .311f);
            Lamp(root, "HallEast", .665f, .311f);
            Lamp(root, "PlazaWest", .296f, .452f);
            Lamp(root, "PlazaEast", .702f, .452f);
            Lamp(root, "SouthWest", .419f, .856f);
            Lamp(root, "SouthEast", .578f, .856f);
        }

        private static void BuildPastColliders(Transform root)
        {
            Box(root, "NorthBuildingsWest", .061f, .013f, .216f, .183f);
            Box(root, "NorthBuildingsMiddle", .341f, .012f, .493f, .184f);
            Box(root, "NorthBuildingsEast", .503f, .012f, .670f, .177f);
            Box(root, "NorthCinema", .713f, 0, .938f, .219f);
            Box(root, "WestBarber", .017f, .240f, .177f, .388f);
            Box(root, "WestDiner", .013f, .440f, .220f, .574f);
            Box(root, "EastBooks", .799f, .258f, .986f, .422f);
            Box(root, "EastGarage", .773f, .461f, .991f, .611f);
            Box(root, "CentralBuilding", .362f, .295f, .628f, .486f);
            Box(root, "CentralTower", .445f, .248f, .544f, .296f);
            Box(root, "Fountain_Memorial", .446f, .576f, .545f, .645f);
            Box(root, "PlanterNorthLeft", .335f, .410f, .441f, .497f);
            Box(root, "PlanterNorthRight", .555f, .410f, .652f, .497f);
            Box(root, "PlanterSouthLeft", .326f, .673f, .438f, .699f);
            Box(root, "PlanterSouthRight", .558f, .673f, .663f, .699f);
            Box(root, "SouthPlanter", .375f, .929f, .587f, .962f);
            Box(root, "MallSign", .068f, .864f, .213f, .948f);
            Box(root, "WelcomeSign", .794f, .839f, .912f, .930f);
            Tree(root, "PlazaNorthLeft", .393f, .493f);
            Tree(root, "PlazaNorthRight", .596f, .493f);
            Tree(root, "PlazaSouthLeft", .368f, .638f);
            Tree(root, "PlazaSouthRight", .623f, .638f);
            Tree(root, "SouthEastUpper", .622f, .841f);
            Tree(root, "SouthEastLower", .623f, .926f);
            Tree(root, "NorthWest", .026f, .078f);
            Tree(root, "NorthWestLower", .025f, .151f);
            Tree(root, "NorthEast", .977f, .078f);
            Tree(root, "NorthEastLower", .981f, .179f);
            Tree(root, "WestMiddle", .018f, .613f);
            Tree(root, "WestSouth", .024f, .752f);
            Tree(root, "WestBottom", .027f, .852f);
            Tree(root, "EastMiddle", .985f, .640f);
            Tree(root, "EastBottom", .982f, .971f);
            Lamp(root, "NorthWest", .326f, .196f);
            Lamp(root, "HallWest", .337f, .364f);
            Lamp(root, "HallEast", .652f, .364f);
            Lamp(root, "PlazaMiddleLeft", .446f, .555f);
            Lamp(root, "PlazaMiddleRight", .546f, .555f);
            Lamp(root, "PlazaSouthLeft", .332f, .695f);
            Lamp(root, "PlazaSouthRight", .655f, .695f);
            Lamp(root, "SouthWest", .330f, .797f);
            Box(root, "BenchLeft", .378f, .526f, .421f, .552f);
            Box(root, "BenchRight", .573f, .526f, .614f, .552f);
        }

        private static void Tree(Transform root, string name, float x, float y) => Box(root, "Tree_" + name, x - .017f, y - .027f, x + .017f, y + .008f);
        private static void Lamp(Transform root, string name, float x, float y) => Box(root, "Lamp_" + name, x - .008f, y - .008f, x + .008f, y + .008f);
        private static void Box(Transform root, string name, float left, float top, float right, float bottom)
        {
            var go = new GameObject("Collider_" + name);
            go.transform.SetParent(root, false);
            go.transform.localPosition = MapPoint((left + right) * .5f, (top + bottom) * .5f);
            go.AddComponent<BoxCollider2D>().size = new Vector2((right - left) * MapSize, (bottom - top) * MapSize);
        }

        public void Initialize(TimeTravelManager manager, Transform playerTransform)
        {
            timeline = manager;
            player = playerTransform;
            timeline.EraChanged += EnsureWalkablePosition;
            var camera = Camera.main;
            camera.orthographicSize = 8.2f;
            camera.GetComponent<CameraFollow>().SetMapBounds(MapBounds);
            var snap = camera.GetComponent<PixelSnapCamera>();
            if (snap != null) snap.enabled = false;
            EnsureWalkablePosition(timeline.CurrentYear);
        }

        private void EnsureWalkablePosition(int year)
        {
            Physics2D.SyncTransforms();
            Vector2 origin = player.position;
            if (IsWalkable(origin)) return;
            // Different eras have different footprints: only relocate if the destination is blocked.
            for (float radius = .5f; radius <= MapSize; radius += .5f)
                for (int step = 0; step < 32; step++)
                {
                    float angle = step * Mathf.PI / 16f;
                    Vector2 candidate = origin + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                    if (!IsWalkable(candidate)) continue;
                    player.position = candidate;
                    player.GetComponent<Rigidbody2D>().position = candidate;
                    player.GetComponent<PlayerController>().StopImmediately();
                    return;
                }
            Debug.LogError("Pythime: não foi encontrada posição livre no mapa oficial.");
        }

        public static bool IsWalkable(Vector2 position)
        {
            if (!new Rect(-31.5f, -31.5f, 63f, 63f).Contains(position)) return false;
            foreach (var collider in Physics2D.OverlapBoxAll(position + new Vector2(0, .16f), new Vector2(.5f, .4f), 0))
                if (!collider.isTrigger && collider.transform.parent != null && collider.transform.parent.name == "PythimeOfficialMapColliders") return false;
            return true;
        }

        private void OnDestroy()
        {
            if (timeline != null) timeline.EraChanged -= EnsureWalkablePosition;
            foreach (var sprite in ownedSprites) if (sprite != null) Destroy(sprite);
        }
    }
}

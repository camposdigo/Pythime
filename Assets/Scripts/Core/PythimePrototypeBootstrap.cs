using UnityEngine;

namespace Pythime
{
    public static class PythimePrototypeBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Build()
        {
            if (GameObject.Find("PythimeRuntime") != null) return;

            var runtime = new GameObject("PythimeRuntime");

            var timelineObject = new GameObject("TimeTravelManager");
            timelineObject.transform.SetParent(runtime.transform);
            var timeline = timelineObject.AddComponent<TimeTravelManager>();

            var world1956 = BuildWorld(runtime.transform, 1956);
            var world2026 = BuildWorld(runtime.transform, 2026);
            var world2096 = BuildWorld(runtime.transform, 2096);

            timeline.RegisterEra(1956, world1956);
            timeline.RegisterEra(2026, world2026);
            timeline.RegisterEra(2096, world2096);

            var player = BuildPlayer(runtime.transform);
            SetupCamera(player.transform);
            BuildTock(runtime.transform, player.transform);

            var soilPoint = StoryWorldFactory.SoilPoint;
            CreateSoilPatch(world1956.transform, soilPoint);

            var seedling = BuildTemporalTree(world1956.transform, "Seedling_1956", soilPoint, 1956, false);
            var tree2026 = BuildTemporalTree(world2026.transform, "Tree_2026", soilPoint, 2026, true);
            var tree2096 = BuildTemporalTree(world2096.transform, "Tree_2096", soilPoint, 2096, true);

            seedling.SetActive(false);
            tree2026.SetActive(false);
            tree2096.SetActive(false);

            timeline.ConfigureTemporalSeed(player.transform, soilPoint, seedling, tree2026, tree2096);
            timeline.SetInitialYear(2026);

            var story = runtime.AddComponent<StoryDirector>();
            story.Initialize(player.transform, StoryWorldFactory.WorkshopPoint, soilPoint, StoryWorldFactory.MonolithPoint);

            runtime.AddComponent<PrototypeHUD>();
            runtime.AddComponent<PythonPuzzleConsole>();
        }

        private static GameObject BuildWorld(Transform parent, int year)
        {
            var root = new GameObject($"Era_{year}");
            root.transform.SetParent(parent);

            var map = new GameObject($"PythimeCity_{year}");
            map.transform.SetParent(root.transform);
            var mapRenderer = map.AddComponent<SpriteRenderer>();
            mapRenderer.sprite = StoryWorldFactory.CreateTownMap(year);
            mapRenderer.sortingOrder = -50;

            foreach (var rect in StoryWorldFactory.BuildingRects)
                AddBuildingCollider(root.transform, rect);

            BuildTemporalVehicle(root.transform, year);

            if (year == 2096)
                BuildMonolith(root.transform, year);

            AddMapBounds(root.transform);
            return root;
        }

        private static GameObject BuildPlayer(Transform parent)
        {
            var player = new GameObject("Player");
            player.transform.SetParent(parent);
            player.transform.position = StoryWorldFactory.StartPoint;

            var body = player.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;

            var collider = player.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.55f, 0.58f);
            collider.offset = new Vector2(0f, 0.28f);

            player.AddComponent<PlayerController>();

            var shadowObject = new GameObject("Shadow");
            shadowObject.transform.SetParent(player.transform);
            shadowObject.transform.localPosition = new Vector3(0f, 0.08f, 0f);
            var shadow = shadowObject.AddComponent<SpriteRenderer>();
            shadow.sprite = PixelArtFactory.CreateShadowSprite();
            shadow.sortingOrder = 18;

            var avatarObject = new GameObject("Avatar");
            avatarObject.transform.SetParent(player.transform);
            avatarObject.transform.localPosition = Vector3.zero;
            var avatar = avatarObject.AddComponent<SpriteRenderer>();
            avatar.sortingOrder = 20;

            var visual = player.AddComponent<PlayerVisual>();
            visual.Initialize(avatar);
            return player;
        }

        private static void BuildTock(Transform parent, Transform player)
        {
            var tock = new GameObject("Tock");
            tock.transform.SetParent(parent);
            tock.transform.position = player.position + new Vector3(0.8f, 0.8f, 0f);
            var renderer = tock.AddComponent<SpriteRenderer>();
            renderer.sprite = StoryWorldFactory.CreateTockSprite();
            renderer.sortingOrder = 30;
            var follower = tock.AddComponent<TockFollower>();
            follower.Initialize(player);
        }

        private static void BuildTemporalVehicle(Transform parent, int year)
        {
            var vehicle = new GameObject($"TemporalVehicle_{year}");
            vehicle.transform.SetParent(parent);
            vehicle.transform.localPosition = StoryWorldFactory.VehiclePoint;
            var renderer = vehicle.AddComponent<SpriteRenderer>();
            renderer.sprite = StoryWorldFactory.CreateTemporalVehicleSprite(year);
            renderer.sortingOrder = 12;

            var collider = vehicle.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(3.2f, 1.4f);
            collider.offset = new Vector2(0f, 0.45f);
        }

        private static void BuildMonolith(Transform parent, int year)
        {
            var monolith = new GameObject("ImpossibleMonolith");
            monolith.transform.SetParent(parent);
            monolith.transform.localPosition = StoryWorldFactory.MonolithPoint;
            var renderer = monolith.AddComponent<SpriteRenderer>();
            renderer.sprite = StoryWorldFactory.CreateMonolithSprite(year);
            renderer.sortingOrder = 15;

            var collider = monolith.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.8f, 1.35f);
            collider.offset = new Vector2(0f, 0.65f);
        }

        private static void SetupCamera(Transform player)
        {
            var camera = Camera.main;
            if (camera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
            }

            camera.orthographic = true;
            camera.orthographicSize = 6.3f;
            camera.backgroundColor = new Color32(24, 27, 31, 255);
            camera.transform.position = new Vector3(player.position.x, player.position.y, -10f);

            var follow = camera.GetComponent<CameraFollow>();
            if (follow == null) follow = camera.gameObject.AddComponent<CameraFollow>();
            follow.Initialize(player);
        }

        private static GameObject BuildTemporalTree(Transform parent, string name, Vector2 position, int year, bool grown)
        {
            var tree = new GameObject(name);
            tree.transform.SetParent(parent);
            tree.transform.localPosition = position;
            var renderer = tree.AddComponent<SpriteRenderer>();
            renderer.sprite = PixelArtFactory.CreateTreeSprite(year, grown);
            renderer.sortingOrder = 13;

            if (grown)
            {
                var collider = tree.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(0.55f, 0.45f);
                collider.offset = new Vector2(0f, 0.25f);
            }

            return tree;
        }

        private static void CreateSoilPatch(Transform parent, Vector2 position)
        {
            var texture = new Texture2D(24, 20, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = "TemporalSoil"
            };

            var clear = new Color32(0, 0, 0, 0);
            var dirt = new Color32(105, 67, 38, 255);
            var dirtLight = new Color32(151, 102, 58, 255);
            var edge = new Color32(56, 48, 39, 255);
            var pixels = new Color32[texture.width * texture.height];
            for (var i = 0; i < pixels.Length; i++) pixels[i] = clear;
            texture.SetPixels32(pixels);

            for (var y = 3; y < 17; y++)
            for (var x = 2; x < 22; x++)
            {
                var border = x == 2 || x == 21 || y == 3 || y == 16;
                texture.SetPixel(x, y, border ? edge : ((x + y) % 5 == 0 ? dirtLight : dirt));
            }
            texture.Apply(false, false);

            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 16f);
            var patch = new GameObject("TemporalSoilPatch");
            patch.transform.SetParent(parent);
            patch.transform.localPosition = position;
            var renderer = patch.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 3;
        }

        private static void AddBuildingCollider(Transform parent, RectInt rect)
        {
            var colliderObject = new GameObject($"BuildingCollider_{rect.x}_{rect.y}");
            colliderObject.transform.SetParent(parent);
            var lowerLeftX = -StoryWorldFactory.MapWidthTiles / 2f;
            var lowerLeftY = -StoryWorldFactory.MapHeightTiles / 2f;
            colliderObject.transform.localPosition = new Vector3(
                lowerLeftX + rect.x + rect.width / 2f,
                lowerLeftY + rect.y + rect.height / 2f,
                0f);

            var collider = colliderObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(rect.width - 0.1f, rect.height - 0.1f);
        }

        private static void AddMapBounds(Transform parent)
        {
            var halfW = StoryWorldFactory.MapWidthTiles / 2f;
            var halfH = StoryWorldFactory.MapHeightTiles / 2f;
            AddWall(parent, new Vector2(0f, halfH + 0.25f), new Vector2(StoryWorldFactory.MapWidthTiles + 1f, 0.5f));
            AddWall(parent, new Vector2(0f, -halfH - 0.25f), new Vector2(StoryWorldFactory.MapWidthTiles + 1f, 0.5f));
            AddWall(parent, new Vector2(halfW + 0.25f, 0f), new Vector2(0.5f, StoryWorldFactory.MapHeightTiles + 1f));
            AddWall(parent, new Vector2(-halfW - 0.25f, 0f), new Vector2(0.5f, StoryWorldFactory.MapHeightTiles + 1f));
        }

        private static void AddWall(Transform parent, Vector2 position, Vector2 size)
        {
            var wall = new GameObject("MapBoundary");
            wall.transform.SetParent(parent);
            wall.transform.localPosition = position;
            var collider = wall.AddComponent<BoxCollider2D>();
            collider.size = size;
        }
    }
}

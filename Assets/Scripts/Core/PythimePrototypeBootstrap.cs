using UnityEngine;

namespace Pythime
{
    public static class PythimePrototypeBootstrap
    {
        private static Sprite whiteSprite;

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

            var patchPosition = new Vector2(-3f, 3f);
            CreateBox(world1956.transform, "TemporalSoilPatch", patchPosition, new Vector2(1.3f, 0.8f),
                new Color(0.42f, 0.24f, 0.12f), 2, false);

            var seedling = BuildTree(world1956.transform, "Seedling_1956", patchPosition + new Vector2(0f, 0.35f), 0.28f,
                new Color(0.28f, 0.62f, 0.23f));
            var tree2026 = BuildTree(world2026.transform, "Tree_2026", patchPosition, 1.0f,
                new Color(0.20f, 0.58f, 0.28f));
            var tree2096 = BuildTree(world2096.transform, "Tree_2096", patchPosition, 1.75f,
                new Color(0.25f, 0.86f, 0.62f));

            seedling.SetActive(false);
            tree2026.SetActive(false);
            tree2096.SetActive(false);

            timeline.ConfigureTemporalSeed(player.transform, patchPosition, seedling, tree2026, tree2096);
            timeline.SetInitialYear(2026);

            runtime.AddComponent<PrototypeHUD>();
            runtime.AddComponent<PythonPuzzleConsole>();
        }

        private static GameObject BuildWorld(Transform parent, int year)
        {
            var root = new GameObject($"Era_{year}");
            root.transform.SetParent(parent);

            var palette = GetPalette(year);
            CreateBox(root.transform, "Ground", Vector2.zero, new Vector2(24f, 16f), palette.ground, -20, false);
            CreateBox(root.transform, "MainRoad", new Vector2(0f, 0f), new Vector2(24f, 3.1f), palette.road, -10, false);
            CreateBox(root.transform, "NorthWalk", new Vector2(0f, 2.0f), new Vector2(24f, 0.85f), palette.walk, -9, false);
            CreateBox(root.transform, "SouthWalk", new Vector2(0f, -2.0f), new Vector2(24f, 0.85f), palette.walk, -9, false);

            BuildBuilding(root.transform, new Vector2(-7.3f, 5.0f), new Vector2(4.4f, 3.2f), palette.buildingA, year, "NorthWest");
            BuildBuilding(root.transform, new Vector2(6.8f, 5.1f), new Vector2(5.2f, 3.0f), palette.buildingB, year, "NorthEast");
            BuildBuilding(root.transform, new Vector2(-7.0f, -5.0f), new Vector2(4.8f, 3.2f), palette.buildingB, year, "SouthWest");
            BuildBuilding(root.transform, new Vector2(7.1f, -5.0f), new Vector2(4.5f, 3.2f), palette.buildingA, year, "SouthEast");

            CreateTemporalVehicle(root.transform, year, new Vector2(4.2f, -0.2f), palette.accent);
            CreateStreetDetails(root.transform, year, palette);
            return root;
        }

        private static GameObject BuildPlayer(Transform parent)
        {
            var player = new GameObject("Player");
            player.transform.SetParent(parent);
            player.transform.position = new Vector3(0f, -3.2f, 0f);

            var body = player.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;

            var collider = player.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.62f, 0.92f);

            player.AddComponent<PlayerController>();
            var visual = player.AddComponent<PlayerVisual>();

            CreateBox(player.transform, "Shadow", new Vector2(0f, -0.48f), new Vector2(0.72f, 0.20f),
                new Color(0f, 0f, 0f, 0.22f), 18, false);
            var bodyRenderer = CreateBox(player.transform, "Body", new Vector2(0f, -0.02f), new Vector2(0.62f, 0.72f),
                Color.white, 20, false).GetComponent<SpriteRenderer>();
            CreateBox(player.transform, "Head", new Vector2(0f, 0.52f), new Vector2(0.54f, 0.48f),
                new Color(0.88f, 0.67f, 0.52f), 21, false);
            var hairRenderer = CreateBox(player.transform, "Hair", new Vector2(0f, 0.72f), new Vector2(0.58f, 0.20f),
                new Color(0.12f, 0.08f, 0.05f), 22, false).GetComponent<SpriteRenderer>();
            var accentRenderer = CreateBox(player.transform, "TemporalPack", new Vector2(-0.30f, -0.02f), new Vector2(0.18f, 0.48f),
                new Color(0.44f, 0.80f, 1f), 19, false).GetComponent<SpriteRenderer>();

            visual.Initialize(bodyRenderer, hairRenderer, accentRenderer);
            return player;
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
            camera.transform.position = new Vector3(player.position.x, player.position.y, -10f);

            var follow = camera.GetComponent<CameraFollow>();
            if (follow == null) follow = camera.gameObject.AddComponent<CameraFollow>();
            follow.Initialize(player);
        }

        private static void BuildBuilding(Transform parent, Vector2 position, Vector2 size, Color color, int year, string label)
        {
            var building = CreateBox(parent, $"Building_{label}_{year}", position, size, color, 3, true);
            var roofColor = Color.Lerp(color, year == 2096 ? new Color(0.25f, 0.90f, 1f) : Color.black, 0.22f);
            CreateBox(building.transform, "RoofLine", new Vector2(0f, size.y * 0.34f), new Vector2(size.x * 0.88f, 0.22f), roofColor, 4, false);

            var windowCount = Mathf.Max(2, Mathf.FloorToInt(size.x / 1.4f));
            for (var i = 0; i < windowCount; i++)
            {
                var x = -size.x * 0.34f + i * (size.x * 0.68f / Mathf.Max(1, windowCount - 1));
                var windowColor = year == 2096
                    ? new Color(0.24f, 0.90f, 0.98f)
                    : new Color(0.85f, 0.91f, 0.84f);
                CreateBox(building.transform, $"Window_{i}", new Vector2(x, 0f), new Vector2(0.48f, 0.62f), windowColor, 5, false);
            }
        }

        private static void CreateTemporalVehicle(Transform parent, int year, Vector2 position, Color accent)
        {
            var vehicle = new GameObject($"TemporalVehicle_{year}");
            vehicle.transform.SetParent(parent);
            vehicle.transform.localPosition = position;

            if (year == 1956)
            {
                CreateBox(vehicle.transform, "ClockworkBody", Vector2.zero, new Vector2(1.9f, 0.75f), new Color(0.42f, 0.22f, 0.10f), 6, true);
                CreateBox(vehicle.transform, "Cabin", new Vector2(0.25f, 0.48f), new Vector2(0.92f, 0.45f), new Color(0.74f, 0.55f, 0.28f), 7, false);
                CreateBox(vehicle.transform, "TimeCore", new Vector2(-0.58f, 0.05f), new Vector2(0.22f, 0.42f), accent, 8, false);
            }
            else if (year == 2026)
            {
                CreateBox(vehicle.transform, "RetroCoupe", Vector2.zero, new Vector2(2.25f, 0.72f), new Color(0.33f, 0.36f, 0.39f), 6, true);
                CreateBox(vehicle.transform, "Cabin", new Vector2(0.18f, 0.48f), new Vector2(1.05f, 0.46f), new Color(0.18f, 0.31f, 0.39f), 7, false);
                CreateBox(vehicle.transform, "TemporalRails", new Vector2(0f, -0.30f), new Vector2(1.75f, 0.10f), accent, 8, false);
            }
            else
            {
                CreateBox(vehicle.transform, "HoverPod", Vector2.zero, new Vector2(2.0f, 0.68f), new Color(0.21f, 0.18f, 0.38f), 6, true);
                CreateBox(vehicle.transform, "Glass", new Vector2(0.15f, 0.46f), new Vector2(1.05f, 0.48f), new Color(0.22f, 0.82f, 0.92f), 7, false);
                CreateBox(vehicle.transform, "HoverGlow", new Vector2(0f, -0.42f), new Vector2(1.65f, 0.16f), accent, 8, false);
            }
        }

        private static void CreateStreetDetails(Transform parent, int year, Palette palette)
        {
            var positions = new[]
            {
                new Vector2(-9.6f, 2.8f), new Vector2(9.5f, 2.8f),
                new Vector2(-9.3f, -2.8f), new Vector2(9.2f, -2.8f)
            };

            foreach (var position in positions)
            {
                CreateBox(parent, $"StreetPost_{position.x:0}_{position.y:0}", position, new Vector2(0.16f, 1.15f), palette.post, 2, false);
                CreateBox(parent, "StreetLamp", position + new Vector2(0f, 0.65f), new Vector2(0.42f, 0.22f), palette.accent, 3, false);
            }

            if (year == 2096)
            {
                CreateBox(parent, "FutureMonolith", new Vector2(-8.7f, 0f), new Vector2(0.62f, 2.2f),
                    new Color(0.035f, 0.04f, 0.065f), 5, true);
                CreateBox(parent, "MonolithSignal", new Vector2(-8.7f, 0.82f), new Vector2(0.30f, 0.08f),
                    new Color(0.20f, 0.95f, 0.90f), 6, false);
            }
        }

        private static GameObject BuildTree(Transform parent, string name, Vector2 position, float scale, Color leaves)
        {
            var tree = new GameObject(name);
            tree.transform.SetParent(parent);
            tree.transform.localPosition = position;
            CreateBox(tree.transform, "Trunk", new Vector2(0f, 0.35f * scale), new Vector2(0.34f * scale, 1.2f * scale),
                new Color(0.40f, 0.23f, 0.10f), 7, true);
            CreateBox(tree.transform, "Crown", new Vector2(0f, 1.2f * scale), new Vector2(1.35f * scale, 1.25f * scale),
                leaves, 8, false);
            return tree;
        }

        private static GameObject CreateBox(Transform parent, string name, Vector2 position, Vector2 size, Color color, int order, bool collider)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = new Vector3(position.x, position.y, 0f);
            obj.transform.localScale = new Vector3(size.x, size.y, 1f);

            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = WhiteSprite;
            renderer.color = color;
            renderer.sortingOrder = order;

            if (collider) obj.AddComponent<BoxCollider2D>();
            return obj;
        }

        private static Sprite WhiteSprite
        {
            get
            {
                if (whiteSprite != null) return whiteSprite;

                var texture = new Texture2D(1, 1)
                {
                    name = "PythimeRuntimeWhite",
                    filterMode = FilterMode.Point
                };
                texture.SetPixel(0, 0, Color.white);
                texture.Apply();
                whiteSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
                return whiteSprite;
            }
        }

        private static Palette GetPalette(int year)
        {
            return year switch
            {
                1956 => new Palette(
                    new Color(0.63f, 0.72f, 0.46f), new Color(0.25f, 0.24f, 0.21f), new Color(0.78f, 0.69f, 0.54f),
                    new Color(0.73f, 0.45f, 0.28f), new Color(0.88f, 0.75f, 0.49f), new Color(0.18f, 0.58f, 0.62f), new Color(0.30f, 0.22f, 0.16f)),
                2026 => new Palette(
                    new Color(0.47f, 0.68f, 0.48f), new Color(0.22f, 0.25f, 0.28f), new Color(0.68f, 0.69f, 0.66f),
                    new Color(0.80f, 0.48f, 0.33f), new Color(0.31f, 0.55f, 0.67f), new Color(0.18f, 0.73f, 0.91f), new Color(0.20f, 0.23f, 0.26f)),
                _ => new Palette(
                    new Color(0.16f, 0.25f, 0.25f), new Color(0.09f, 0.09f, 0.14f), new Color(0.18f, 0.18f, 0.25f),
                    new Color(0.25f, 0.18f, 0.38f), new Color(0.15f, 0.33f, 0.43f), new Color(0.20f, 0.92f, 0.88f), new Color(0.12f, 0.15f, 0.22f))
            };
        }

        private readonly struct Palette
        {
            public readonly Color ground;
            public readonly Color road;
            public readonly Color walk;
            public readonly Color buildingA;
            public readonly Color buildingB;
            public readonly Color accent;
            public readonly Color post;

            public Palette(Color ground, Color road, Color walk, Color buildingA, Color buildingB, Color accent, Color post)
            {
                this.ground = ground;
                this.road = road;
                this.walk = walk;
                this.buildingA = buildingA;
                this.buildingB = buildingB;
                this.accent = accent;
                this.post = post;
            }
        }
    }
}

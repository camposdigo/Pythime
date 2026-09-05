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

            var patchPosition = new Vector2(-5.5f, 4.25f);
            CreateSoilPatch(world1956.transform, patchPosition);

            var seedling = BuildTemporalTree(world1956.transform, "Seedling_1956", patchPosition, 1956, false);
            var tree2026 = BuildTemporalTree(world2026.transform, "Tree_2026", patchPosition, 2026, true);
            var tree2096 = BuildTemporalTree(world2096.transform, "Tree_2096", patchPosition, 2096, true);

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

            if (!TryBuildKenneyWorld(root.transform, year))
            {
                var map = new GameObject($"PixelTown_{year}");
                map.transform.SetParent(root.transform);
                var mapRenderer = map.AddComponent<SpriteRenderer>();
                mapRenderer.sprite = PixelArtFactory.CreateTownMap(year);
                mapRenderer.sortingOrder = -20;

                AddBuildingCollider(root.transform, 2, 15, 8, 6);
                AddBuildingCollider(root.transform, 12, 15, 7, 6);
                AddBuildingCollider(root.transform, 2, 1, 8, 5);
                AddBuildingCollider(root.transform, 12, 1, 7, 5);
                AddBuildingCollider(root.transform, 27, 15, 4, 6);
                AddBuildingCollider(root.transform, 27, 1, 4, 5);
            }

            AddMapBounds(root.transform);
            return root;
        }

        private static bool TryBuildKenneyWorld(Transform parent, int year)
        {
            var texture = Resources.Load<Texture2D>("PythimeArt/KenneyRPGUrban/Sample");
            if (texture == null || texture.width < 32 || texture.height < 32) return false;

            var cropBottom = Mathf.Min(24, Mathf.Max(0, texture.height / 18));
            var rect = new Rect(0, cropBottom, texture.width, texture.height - cropBottom);
            var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 16f, 0, SpriteMeshType.FullRect);
            sprite.name = $"KenneyUrbanReference_{year}";

            var map = new GameObject($"CC0UrbanTown_{year}");
            map.transform.SetParent(parent);
            var renderer = map.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = -20;

            var size = sprite.bounds.size;
            map.transform.localScale = new Vector3(
                PixelArtFactory.MapWidthTiles / Mathf.Max(0.01f, size.x),
                PixelArtFactory.MapHeightTiles / Mathf.Max(0.01f, size.y),
                1f);

            renderer.color = year switch
            {
                1956 => new Color(1f, 0.90f, 0.77f, 1f),
                2096 => new Color(0.72f, 0.89f, 1f, 1f),
                _ => Color.white
            };

            return true;
        }

        private static GameObject BuildPlayer(Transform parent)
        {
            var player = new GameObject("Player");
            player.transform.SetParent(parent);
            player.transform.position = new Vector3(-5f, -0.7f, 0f);

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
            camera.orthographicSize = 5.4f;
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
            renderer.sortingOrder = 12;

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
            var texture = new Texture2D(16, 16, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = "TemporalSoil"
            };

            var clear = new Color32(0, 0, 0, 0);
            var dirt = new Color32(105, 67, 38, 255);
            var dirtLight = new Color32(137, 91, 50, 255);
            var pixels = new Color32[256];
            for (var i = 0; i < pixels.Length; i++) pixels[i] = clear;
            texture.SetPixels32(pixels);

            for (var y = 4; y < 12; y++)
            for (var x = 2; x < 14; x++)
                texture.SetPixel(x, y, ((x + y) % 4 == 0) ? dirtLight : dirt);

            texture.SetPixel(1, 7, dirt);
            texture.SetPixel(14, 8, dirt);
            texture.Apply(false, false);

            var sprite = Sprite.Create(texture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16f);
            var patch = new GameObject("TemporalSoilPatch");
            patch.transform.SetParent(parent);
            patch.transform.localPosition = position;
            var renderer = patch.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 2;
        }

        private static void AddBuildingCollider(Transform parent, int tx, int ty, int tw, int th)
        {
            var colliderObject = new GameObject($"BuildingCollider_{tx}_{ty}");
            colliderObject.transform.SetParent(parent);

            var lowerLeftX = -PixelArtFactory.MapWidthTiles / 2f;
            var lowerLeftY = -PixelArtFactory.MapHeightTiles / 2f;
            colliderObject.transform.localPosition = new Vector3(
                lowerLeftX + tx + tw / 2f,
                lowerLeftY + ty + th / 2f,
                0f);

            var collider = colliderObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(tw - 0.1f, th - 0.1f);
        }

        private static void AddMapBounds(Transform parent)
        {
            var halfW = PixelArtFactory.MapWidthTiles / 2f;
            var halfH = PixelArtFactory.MapHeightTiles / 2f;
            AddWall(parent, new Vector2(0f, halfH + 0.25f), new Vector2(PixelArtFactory.MapWidthTiles + 1f, 0.5f));
            AddWall(parent, new Vector2(0f, -halfH - 0.25f), new Vector2(PixelArtFactory.MapWidthTiles + 1f, 0.5f));
            AddWall(parent, new Vector2(halfW + 0.25f, 0f), new Vector2(0.5f, PixelArtFactory.MapHeightTiles + 1f));
            AddWall(parent, new Vector2(-halfW - 0.25f, 0f), new Vector2(0.5f, PixelArtFactory.MapHeightTiles + 1f));
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

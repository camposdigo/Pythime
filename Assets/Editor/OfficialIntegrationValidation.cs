using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Pythime.EditorTools
{
    // Runs in a disposable batch project; never changes the user's open scene.
    public static class OfficialIntegrationValidation
    {
        private const string Pending = "Pythime.OfficialValidation.Pending";
        private static IEnumerator checks;
        private static int lastFrame;
        private static int assertions;
        private static double startedAt;
        private static readonly List<string> errors = new List<string>();

        public static void RunBatch()
        {
            if (!Application.isBatchMode) throw new InvalidOperationException("RunBatch requires a disposable batch project.");
            Application.runInBackground = true;
            OfficialAssetPostprocessor.Reimport();
            SessionState.SetBool(Pending, true);
            EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
            EditorApplication.EnterPlaymode();
        }

        [InitializeOnLoadMethod]
        private static void Subscribe()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state != PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool(Pending, false)) return;
                Application.logMessageReceived += OnLog;
                checks = RunChecks();
                startedAt = EditorApplication.timeSinceStartup;
                lastFrame = -1;
                EditorApplication.update += Tick;
            };
        }

        private static void OnLog(string message, string stack, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) errors.Add(message);
        }

        private static void Tick()
        {
            if (EditorApplication.timeSinceStartup - startedAt > 180)
            {
                Finish(1, "FAIL: Play validation timed out at frame " + Time.frameCount);
                return;
            }
            if (!EditorApplication.isPlaying || lastFrame == Time.frameCount) return;
            lastFrame = Time.frameCount;
            try
            {
                if (checks.MoveNext()) return;
                Check(errors.Count == 0, string.Join("\n", errors));
                Finish(0, $"PASS: {assertions} assertions; official sprites, input, eras, collisions, reachability and camera bounds.");
            }
            catch (Exception ex) { Finish(1, "FAIL: " + ex); }
        }

        private static void Finish(int code, string report)
        {
            EditorApplication.update -= Tick;
            Application.logMessageReceived -= OnLog;
            SessionState.SetBool(Pending, false);
            Directory.CreateDirectory("Logs");
            File.WriteAllText("Logs/official-validation.txt", report);
            Debug.Log(report);
            EditorApplication.Exit(code);
        }

        private static void Check(bool condition, string message)
        {
            assertions++;
            if (!condition) throw new InvalidOperationException(message);
        }

        private static IEnumerator RunChecks()
        {
            Application.runInBackground = true;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            Time.captureDeltaTime = 1f / 60f;
            Debug.Log("Official validation: Play checks started.");
            for (int i = 0; i < 10; i++) yield return null;
            Check(OfficialEraMapRuntime.IsAvailable, "Official maps unavailable.");
            var runtime = GameObject.Find("PythimeRuntime");
            var player = GameObject.Find("Player");
            var animator = player.GetComponent<OfficialPlayerAnimator>();
            var renderer = player.transform.Find("Avatar").GetComponent<SpriteRenderer>();
            var controller = player.GetComponent<PlayerController>();
            var body = player.GetComponent<Rigidbody2D>();
            var timeline = TimeTravelManager.Instance;
            Check(animator != null && animator.UsingOfficialSheet, "Official player not initialized.");
            Check(player.GetComponent<PlayerVisual>() == null, "Procedural player still attached.");
            Check(renderer.bounds.size.y > 1.7f && renderer.bounds.size.y < 2.2f, "Character scale.");
            Check(Vector2.Distance(player.transform.position, OfficialEraMapRuntime.SpawnPoint) < .01f, "Spawn was moved by another script.");
            foreach (string name in new[] { "PythimeAreaPolish", "PythimeWorldDepth", "PythimeWorldDensity", "PythimeKenneyTilemapOverlay", "PythimeNpcPopulation", "PythimeNpcVisualVariation", "PythimeSocialNpcGroups", "PythimeWorkshopInterior", "PythimeVisualPolish" })
                Check(GameObject.Find(name) == null, "Procedural overlay active: " + name);

            var texture = renderer.sprite.texture;
            Check(texture.GetPixel(0, 0).a == 0f, "White background was not removed.");
            foreach (int year in new[] { 1956, 2026, 2096 })
            {
                var source = OfficialEraMapRuntime.GetMapTexture(year);
                OfficialPngValidation.ValidateFile($"Assets/Resources/OfficialMaps/city_{year}.png");
                Check(source.width == source.height, "Official collision layout requires square maps.");
                var importer = (TextureImporter)AssetImporter.GetAtPath($"Assets/Resources/OfficialMaps/city_{year}.png");
                Check(importer.spriteImportMode == SpriteImportMode.Single && !importer.mipmapEnabled && !importer.isReadable
                    && importer.maxTextureSize == 4096 && importer.textureCompression == TextureImporterCompression.Uncompressed, "Map import settings.");
                timeline.TravelToYear(year, false);
                yield return null;
                var era = runtime.transform.Find("Era_" + year);
                Check(era.Find("PythimeCity_" + year).GetComponent<SpriteRenderer>().sprite.name == "OfficialCity_" + year, "Inactive era missed integration.");
                Check(era.Find("PythimeOfficialMapColliders").childCount > 30, "Colliders absent.");
                Check(OfficialEraMapRuntime.IsWalkable(OfficialEraMapRuntime.SpawnPoint), "Blocked spawn in " + year);
                Check(!OfficialEraMapRuntime.IsWalkable(OfficialEraMapRuntime.MapPoint(.5f, .4f)), "Central building unblocked.");
                Check(!OfficialEraMapRuntime.IsWalkable(OfficialEraMapRuntime.MapPoint(.5f, year == 1956 ? .615f : .66f)), "Fountain/memorial unblocked.");
                Check(!OfficialEraMapRuntime.IsWalkable(new Vector2(33, 0)), "Map edge unblocked.");
                CheckReachability(year);
                body.position = OfficialEraMapRuntime.MapPoint(.5f, .4f);
                player.transform.position = body.position;
                timeline.TravelToYear(year, false);
                Check(OfficialEraMapRuntime.IsWalkable(body.position), "Era swap left player inside a building.");
            }
            var playerImporter = (TextureImporter)AssetImporter.GetAtPath(OfficialPlayerAnimator.AssetPath);
            Check(playerImporter.isReadable && playerImporter.spriteImportMode == SpriteImportMode.Single
                && playerImporter.textureCompression == TextureImporterCompression.Uncompressed && playerImporter.filterMode == FilterMode.Point, "Player import settings.");
            var corrupted = File.ReadAllBytes(OfficialPlayerAnimator.AssetPath);
            corrupted[45] ^= 1;
            bool rejected = false;
            try { OfficialPngValidation.Validate(corrupted); } catch (InvalidDataException) { rejected = true; }
            Check(rejected, "Corrupt PNG accepted.");

            runtime.GetComponent<StoryDirector>().SkipDialogue();
            timeline.TravelToYear(2026, false);
            var keyboard = InputSystem.AddDevice<Keyboard>();
            var keys = new[] { Key.S, Key.W, Key.A, Key.D };
            var directions = new[] { Vector2.down, Vector2.up, Vector2.left, Vector2.right };
            for (int direction = 0; direction < 4; direction++)
            {
                body.position = OfficialEraMapRuntime.SpawnPoint;
                player.transform.position = body.position;
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(keys[direction]));
                yield return null;
                yield return null;
                Check(controller.MoveInput == directions[direction], "MoveInput direction " + direction);
                Check(renderer.sprite.name.StartsWith("Marty_r" + (direction + 1)), "Walk direction row " + direction);
                var names = new HashSet<string>();
                for (int frame = 0; frame < 4; frame++)
                {
                    animator.ApplyFrame(directions[direction], .115f);
                    names.Add(renderer.sprite.name);
                }
                Check(names.Count == 4, "Missing walking frames.");
                InputSystem.QueueStateEvent(keyboard, new KeyboardState());
                yield return null;
                yield return null;
                Check(renderer.sprite.name.StartsWith("Marty_r0"), "Idle row.");
                Check(renderer.flipX == (direction == 3), "Idle facing.");
            }
            // Exercise the real input -> controller -> Rigidbody2D path into the central building.
            body.position = OfficialEraMapRuntime.MapPoint(.5f, .515f);
            player.transform.position = body.position;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.W));
            for (int i = 0; i < 100; i++) yield return null;
            Check(body.position.y > .8f && body.position.y < 1.6f, "Player did not reach/stop at central building: " + body.position);
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Q));
            yield return null;
            yield return null;
            Check(timeline.CurrentYear == 1956, "Q did not switch to 1956.");
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.E));
            yield return null;
            yield return null;
            Check(timeline.CurrentYear == 2026, "E did not switch to 2026.");
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return null;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.E));
            yield return null;
            yield return null;
            Check(timeline.CurrentYear == 2096, "E did not switch to 2096.");
            InputSystem.RemoveDevice(keyboard);

            var camera = Camera.main;
            foreach (float aspect in new[] { 16f / 9f, 21f / 9f, 4f / 3f })
            {
                camera.aspect = aspect;
                foreach (var point in new[] { new Vector2(-31, -31), new Vector2(31, 31) })
                {
                    player.transform.position = point;
                    body.position = point;
                    for (int i = 0; i < 60; i++) yield return null;
                    float x = camera.orthographicSize * camera.aspect, y = camera.orthographicSize;
                    Check(camera.transform.position.x - x >= -32.001f && camera.transform.position.x + x <= 32.001f
                        && camera.transform.position.y - y >= -32.001f && camera.transform.position.y + y <= 32.001f, "Camera outside map.");
                }
            }
            camera.ResetAspect();
            body.position = OfficialEraMapRuntime.SpawnPoint;
            player.transform.position = body.position;
            timeline.TravelToYear(2026, false);
            for (int i = 0; i < 100; i++) yield return null;
            ScreenCapture.CaptureScreenshot("Logs/official-play-1080.png");
            for (int i = 0; i < 10; i++) yield return null;
        }

        private static void CheckReachability(int year)
        {
            const int size = 127;
            var open = new bool[size, size];
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++) open[x, y] = OfficialEraMapRuntime.IsWalkable(new Vector2(x * .5f - 31.5f, y * .5f - 31.5f));
            Vector2Int Cell(Vector2 p) => new Vector2Int(Mathf.RoundToInt((p.x + 31.5f) * 2), Mathf.RoundToInt((p.y + 31.5f) * 2));
            var visited = new bool[size, size];
            var queue = new Queue<Vector2Int>();
            var start = Cell(OfficialEraMapRuntime.SpawnPoint);
            queue.Enqueue(start);
            visited[start.x, start.y] = true;
            while (queue.Count > 0)
            {
                var p = queue.Dequeue();
                foreach (var d in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                {
                    var q = p + d;
                    if (q.x < 0 || q.y < 0 || q.x >= size || q.y >= size || visited[q.x, q.y] || !open[q.x, q.y]) continue;
                    visited[q.x, q.y] = true;
                    queue.Enqueue(q);
                }
            }
            foreach (var destination in new[] { OfficialEraMapRuntime.MapPoint(.44f, .22f), OfficialEraMapRuntime.MapPoint(.28f, .65f),
                OfficialEraMapRuntime.MapPoint(.71f, .65f), OfficialEraMapRuntime.MapPoint(.5f, .75f), OfficialEraMapRuntime.WorkshopPoint, OfficialEraMapRuntime.SoilPoint })
            {
                var cell = Cell(destination);
                Check(visited[cell.x, cell.y], "Route blocked in " + year + " to " + destination);
            }
        }
    }
}

using System.Collections;
using UnityEngine;

namespace Pythime
{
    [DefaultExecutionOrder(-1700)]
    public sealed class OfficialMapCleanupRuntime : MonoBehaviour
    {
        private static readonly int[] Years = { 1956, 2026, 2096 };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (GameObject.Find("PythimeOfficialMapCleanup") != null) return;
            var root = new GameObject("PythimeOfficialMapCleanup");
            root.AddComponent<OfficialMapCleanupRuntime>();
        }

        private IEnumerator Start()
        {
            for (var i = 0; i < 240; i++)
            {
                if (GameObject.Find("PythimeRuntime") != null && GameObject.Find("PythimeCity_2026") != null)
                    break;
                yield return null;
            }

            if (!HasAnyOfficialMap()) yield break;

            for (var i = 0; i < 4; i++)
            {
                CleanupGeneratedLayers();
                CleanupEraChildren();
                CleanupProceduralCharacters();
                PositionPlayerAndCamera();
                yield return null;
            }
        }

        private static bool HasAnyOfficialMap()
        {
            for (var i = 0; i < Years.Length; i++)
                if (Resources.Load<Texture2D>("OfficialMaps/city_" + Years[i]) != null)
                    return true;
            return false;
        }

        private static void CleanupGeneratedLayers()
        {
            DisableRoot("PythimeAreaPolish");
            DisableRoot("PythimeWorldDepth");
            DisableRoot("PythimeWorldDensity");
            DisableRoot("PythimeKenneyTilemapOverlay");
            DisableRoot("PythimeNpcPopulation");
            DisableRoot("PythimeNpcVisualVariation");
            DisableRoot("PythimeSocialNpcGroups");
            DisableRoot("PythimeWorkshopInterior");
        }

        private static void CleanupEraChildren()
        {
            for (var y = 0; y < Years.Length; y++)
            {
                var era = GameObject.Find("Era_" + Years[y]);
                if (era == null) continue;

                for (var i = 0; i < era.transform.childCount; i++)
                {
                    var child = era.transform.GetChild(i).gameObject;
                    var n = child.name;
                    if (n.StartsWith("PythimeCity_")) continue;
                    if (n.StartsWith("TemporalVehicle_")) continue;
                    if (n.StartsWith("ImpossibleMonolith")) continue;
                    if (n.StartsWith("Tree_") || n.StartsWith("Seedling_") || n.StartsWith("TemporalSoil")) continue;

                    if (n.Contains("AreaPolish") || n.Contains("Density") || n.Contains("Npc") || n.Contains("NPC") ||
                        n.Contains("Social") || n.Contains("WorkshopInterior") || n.Contains("Interior") ||
                        n.Contains("Depth") || n.Contains("Kenney") || n.Contains("Street") || n.Contains("Bench") ||
                        n.Contains("Kiosk") || n.Contains("Planter") || n.Contains("Bin"))
                    {
                        child.SetActive(false);
                    }
                }
            }
        }

        private static void CleanupProceduralCharacters()
        {
            var renderers = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
            for (var i = 0; i < renderers.Length; i++)
            {
                var go = renderers[i].gameObject;
                if (go == null) continue;
                var n = go.name;
                if (n == "Avatar" || n == "Shadow" || n == "Tock") continue;
                if (n.Contains("Npc") || n.Contains("NPC") || n.Contains("Group"))
                    go.SetActive(false);
            }
        }

        private static void PositionPlayerAndCamera()
        {
            var player = GameObject.Find("Player");
            if (player != null)
            {
                player.transform.position = StoryWorldFactory.TileToWorld(32f, 18.5f);
                var body = player.GetComponent<Rigidbody2D>();
                if (body != null) body.linearVelocity = Vector2.zero;
            }

            var camera = Camera.main;
            if (camera != null)
            {
                camera.orthographicSize = 8.2f;
                if (player != null)
                    camera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
            }
        }

        private static void DisableRoot(string name)
        {
            var go = GameObject.Find(name);
            if (go != null) go.SetActive(false);
        }
    }
}

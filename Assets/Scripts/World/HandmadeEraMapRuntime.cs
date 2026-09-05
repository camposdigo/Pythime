using System.Collections;
using UnityEngine;

namespace Pythime
{
    public sealed class HandmadeEraMapRuntime : MonoBehaviour
    {
        private const float MapWorldSize = 54f;
        private static readonly Vector3 MapOffset = new Vector3(3f, 0f, 0f);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (!EmbeddedArtLoader.HasAsset("map_1956") ||
                !EmbeddedArtLoader.HasAsset("map_2026") ||
                !EmbeddedArtLoader.HasAsset("map_2096"))
                return;

            if (GameObject.Find("PythimeHandmadeEraMaps") != null) return;
            var root = new GameObject("PythimeHandmadeEraMaps");
            root.AddComponent<HandmadeEraMapRuntime>();
        }

        private IEnumerator Start()
        {
            for (var frame = 0; frame < 240; frame++)
            {
                ApplyEra(1956);
                ApplyEra(2026);
                ApplyEra(2096);

                if (frame == 30)
                {
                    var camera = Camera.main;
                    if (camera != null && camera.orthographic)
                        camera.orthographicSize = 6.8f;
                }

                yield return null;
            }
        }

        private static void ApplyEra(int year)
        {
            var era = GameObject.Find("Era_" + year);
            if (era == null) return;

            var mapObject = era.transform.Find("PythimeCity_" + year);
            if (mapObject != null)
            {
                var renderer = mapObject.GetComponent<SpriteRenderer>();
                if (renderer != null && (renderer.sprite == null || !renderer.sprite.name.StartsWith("HandmadeMap_")))
                {
                    var texture = EmbeddedArtLoader.LoadTexture("map_" + year);
                    if (texture != null)
                    {
                        texture.filterMode = FilterMode.Point;
                        var ppu = texture.width / MapWorldSize;
                        var sprite = Sprite.Create(
                            texture,
                            new Rect(0, 0, texture.width, texture.height),
                            new Vector2(0.5f, 0.5f),
                            ppu,
                            0,
                            SpriteMeshType.FullRect);
                        sprite.name = "HandmadeMap_" + year;
                        renderer.sprite = sprite;
                        renderer.color = Color.white;
                        renderer.sortingOrder = -100;
                        mapObject.localPosition = MapOffset;
                        mapObject.localScale = Vector3.one;
                    }
                }
            }

            CleanupProceduralGeometry(era.transform, year);
        }

        private static void CleanupProceduralGeometry(Transform era, int year)
        {
            var all = era.GetComponentsInChildren<Transform>(true);
            foreach (var item in all)
            {
                if (item == null || item == era) continue;
                var name = item.name;

                if (name.StartsWith("BuildingCollider_") ||
                    name.StartsWith(year + "_") ||
                    name == "AreaPolish" ||
                    name.Contains("KenneyGrid") ||
                    name.Contains("CC0_StreetProps") ||
                    name.Contains("WorldDepth") ||
                    name.Contains("WorldDensity") ||
                    name.Contains("DensityProps"))
                {
                    item.gameObject.SetActive(false);
                }
            }
        }
    }
}

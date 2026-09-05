using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pythime
{
    [DefaultExecutionOrder(980)]
    public sealed class KenneyTilemapOverlay : MonoBehaviour
    {
        private static readonly Dictionary<int, Tile> TileCache = new Dictionary<int, Tile>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (GameObject.Find("PythimeKenneyTilemaps") != null) return;
            var root = new GameObject("PythimeKenneyTilemaps");
            root.AddComponent<KenneyTilemapOverlay>();
        }

        private IEnumerator Start()
        {
            for (var i = 0; i < 40; i++)
            {
                if (GameObject.Find("Era_2026") != null) break;
                yield return null;
            }

            if (LoadTile(178) == null)
            {
                Debug.Log("Pythime: pack Kenney ainda não disponível no runtime; usando arte procedural de fallback.");
                yield break;
            }

            BuildEra(1956);
            BuildEra(2026);
            BuildEra(2096);
        }

        private static void BuildEra(int year)
        {
            var era = GameObject.Find("Era_" + year);
            if (era == null || era.transform.Find("KenneyGrid") != null) return;

            var gridObject = new GameObject("KenneyGrid");
            gridObject.transform.SetParent(era.transform);
            gridObject.transform.localPosition = Vector3.zero;
            var grid = gridObject.AddComponent<Grid>();
            grid.cellSize = Vector3.one;

            var propsObject = new GameObject("CC0_StreetProps");
            propsObject.transform.SetParent(gridObject.transform);
            var tilemap = propsObject.AddComponent<Tilemap>();
            var renderer = propsObject.AddComponent<TilemapRenderer>();
            renderer.sortingOrder = 7;

            tilemap.color = year == 1956
                ? new Color(1f, 0.90f, 0.78f, 1f)
                : year == 2096
                    ? new Color(0.78f, 0.96f, 1f, 1f)
                    : Color.white;

            var greenTreeA = year == 1956 ? 259 : 178;
            var greenTreeB = year == 2096 ? 182 : 179;
            var lamp = 138;
            var streetProp = 145;
            var carA = year == 1956 ? 371 : year == 2096 ? 376 : 367;
            var carB = year == 1956 ? 372 : year == 2096 ? 377 : 368;

            Place(tilemap, greenTreeA, 5, 35);
            Place(tilemap, greenTreeB, 13, 35);
            Place(tilemap, greenTreeA, 49, 35);
            Place(tilemap, greenTreeB, 50, 16);
            Place(tilemap, greenTreeA, 14, 17);
            Place(tilemap, greenTreeB, 47, 8);

            Place(tilemap, lamp, 17, 24);
            Place(tilemap, lamp, 37, 24);
            Place(tilemap, lamp, 17, 19);
            Place(tilemap, lamp, 47, 19);
            Place(tilemap, streetProp, 30, 24);
            Place(tilemap, streetProp, 34, 19);

            Place(tilemap, carA, 20, 21);
            Place(tilemap, carB, 43, 21);
            Place(tilemap, carA, 29, 31);

            if (year == 2096)
            {
                Place(tilemap, 381, 45, 30);
                Place(tilemap, 382, 47, 30);
            }
        }

        private static void Place(Tilemap map, int tileIndex, int tileX, int tileY)
        {
            var tile = LoadTile(tileIndex);
            if (tile == null) return;
            var world = StoryWorldFactory.TileToWorld(tileX, tileY);
            var cell = new Vector3Int(Mathf.RoundToInt(world.x), Mathf.RoundToInt(world.y), 0);
            map.SetTile(cell, tile);
        }

        private static Tile LoadTile(int index)
        {
            Tile cached;
            if (TileCache.TryGetValue(index, out cached)) return cached;

            var path = "PythimeArt/KenneyRPGUrban/Tiles/tile_" + index.ToString("0000");
            var sprite = Resources.Load<Sprite>(path);
            if (sprite == null) return null;

            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.colliderType = Tile.ColliderType.None;
            TileCache[index] = tile;
            return tile;
        }
    }
}

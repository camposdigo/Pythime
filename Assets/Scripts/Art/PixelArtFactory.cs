using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pythime
{
    public static class PixelArtFactory
    {
        public const int TileSize = 16;
        public const int MapWidthTiles = 32;
        public const int MapHeightTiles = 22;

        private static readonly Dictionary<int, Sprite> mapCache = new();
        private static readonly Dictionary<string, Sprite> propCache = new();

        public static Sprite CreateTownMap(int year)
        {
            if (mapCache.TryGetValue(year, out var cached)) return cached;

            var width = MapWidthTiles * TileSize;
            var height = MapHeightTiles * TileSize;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = $"PythimeTown_{year}",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            var palette = PaletteFor(year);
            Fill(texture, palette.grass);

            var random = new System.Random(year);
            for (var i = 0; i < 520; i++)
            {
                var x = random.Next(0, width);
                var y = random.Next(0, height);
                if (random.NextDouble() > 0.52)
                    texture.SetPixel(x, y, palette.grassDetail);
            }

            DrawRoads(texture, palette);
            DrawBuildings(texture, palette, year);
            DrawTrees(texture, palette, year);
            DrawStreetDetails(texture, palette, year);
            DrawTemporalVehicle(texture, palette, year);

            texture.Apply(false, false);

            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, width, height),
                new Vector2(0.5f, 0.5f),
                TileSize,
                0,
                SpriteMeshType.FullRect);
            sprite.name = $"Town_{year}";
            mapCache[year] = sprite;
            return sprite;
        }

        public static Sprite CreateTreeSprite(int year, bool grown)
        {
            var key = $"tree_{year}_{grown}";
            if (propCache.TryGetValue(key, out var cached)) return cached;

            var palette = PaletteFor(year);
            var texture = NewTexture(grown ? 32 : 16, grown ? 40 : 24, key);
            Clear(texture);

            var trunk = year >= 2096 ? new Color32(76, 70, 84, 255) : new Color32(99, 62, 34, 255);
            var leaves = palette.tree;
            var darkLeaves = Darken(leaves, 0.24f);

            if (grown)
            {
                FillRect(texture, 13, 2, 6, 18, trunk);
                FillRect(texture, 5, 17, 22, 13, darkLeaves);
                FillRect(texture, 2, 21, 28, 11, leaves);
                FillRect(texture, 8, 30, 16, 6, leaves);
                OutlineAlpha(texture, new Color32(31, 35, 38, 255));
            }
            else
            {
                FillRect(texture, 7, 2, 3, 9, trunk);
                FillRect(texture, 3, 9, 10, 8, leaves);
                FillRect(texture, 5, 16, 6, 4, leaves);
                OutlineAlpha(texture, new Color32(31, 35, 38, 255));
            }

            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.08f), TileSize);
            sprite.name = key;
            propCache[key] = sprite;
            return sprite;
        }

        public static Sprite CreateShadowSprite()
        {
            const string key = "shadow";
            if (propCache.TryGetValue(key, out var cached)) return cached;

            var texture = NewTexture(14, 6, key);
            Clear(texture);
            var shadow = new Color32(18, 22, 24, 92);
            FillRect(texture, 3, 1, 8, 4, shadow);
            FillRect(texture, 1, 2, 12, 2, shadow);
            texture.Apply(false, false);

            var sprite = Sprite.Create(texture, new Rect(0, 0, 14, 6), new Vector2(0.5f, 0.5f), TileSize);
            propCache[key] = sprite;
            return sprite;
        }

        private static void DrawRoads(Texture2D texture, Palette palette)
        {
            DrawTileRect(texture, 0, 8, MapWidthTiles, 5, palette.road);
            DrawTileRect(texture, 21, 0, 5, MapHeightTiles, palette.road);

            DrawTileRect(texture, 0, 7, MapWidthTiles, 1, palette.sidewalk);
            DrawTileRect(texture, 0, 13, MapWidthTiles, 1, palette.sidewalk);
            DrawTileRect(texture, 20, 0, 1, MapHeightTiles, palette.sidewalk);
            DrawTileRect(texture, 26, 0, 1, MapHeightTiles, palette.sidewalk);

            DrawTileGridLines(texture, 0, 7, MapWidthTiles, 1, palette.sidewalkLine);
            DrawTileGridLines(texture, 0, 13, MapWidthTiles, 1, palette.sidewalkLine);
            DrawTileGridLines(texture, 20, 0, 1, MapHeightTiles, palette.sidewalkLine);
            DrawTileGridLines(texture, 26, 0, 1, MapHeightTiles, palette.sidewalkLine);

            for (var x = 1; x < MapWidthTiles; x += 3)
                FillRect(texture, x * TileSize + 3, 10 * TileSize + 7, 10, 2, palette.roadMark);

            for (var y = 1; y < MapHeightTiles; y += 3)
                FillRect(texture, 23 * TileSize + 7, y * TileSize + 3, 2, 10, palette.roadMark);

            for (var i = 0; i < 5; i++)
            {
                FillRect(texture, 19 * TileSize + i * 8, 8 * TileSize + 2, 4, 12, palette.crosswalk);
                FillRect(texture, 26 * TileSize + 2, 7 * TileSize + i * 8, 12, 4, palette.crosswalk);
            }
        }

        private static void DrawBuildings(Texture2D texture, Palette palette, int year)
        {
            DrawBuilding(texture, 2, 15, 8, 6, palette.buildingA, palette, year, false);
            DrawBuilding(texture, 12, 15, 7, 6, palette.buildingB, palette, year, true);
            DrawBuilding(texture, 2, 1, 8, 5, palette.buildingB, palette, year, true);
            DrawBuilding(texture, 12, 1, 7, 5, palette.buildingA, palette, year, false);
            DrawBuilding(texture, 27, 15, 4, 6, palette.buildingB, palette, year, false);
            DrawBuilding(texture, 27, 1, 4, 5, palette.buildingA, palette, year, true);
        }

        private static void DrawBuilding(Texture2D texture, int tx, int ty, int tw, int th, Color32 wall, Palette palette, int year, bool alt)
        {
            var dark = Darken(wall, 0.28f);
            var roof = alt ? palette.roofB : palette.roofA;

            DrawTileRect(texture, tx, ty, tw, th, dark);
            FillRect(texture, tx * TileSize + 3, ty * TileSize + 3, tw * TileSize - 6, th * TileSize - 6, wall);
            FillRect(texture, tx * TileSize + 2, (ty + th - 2) * TileSize, tw * TileSize - 4, TileSize + 7, roof);

            for (var x = tx + 1; x < tx + tw - 1; x += 2)
            {
                var wx = x * TileSize + 4;
                var wy = ty * TileSize + 10;
                FillRect(texture, wx - 1, wy - 1, 10, 10, palette.outline);
                FillRect(texture, wx, wy, 8, 8, palette.window);
                FillRect(texture, wx + 1, wy + 5, 6, 2, palette.windowShine);
            }

            var doorX = (tx + tw / 2) * TileSize + 3;
            FillRect(texture, doorX - 1, ty * TileSize + 1, 12, 16, palette.outline);
            FillRect(texture, doorX, ty * TileSize + 2, 10, 14, palette.door);
            texture.SetPixel(doorX + 7, ty * TileSize + 8, palette.accent);

            if (year == 2096)
            {
                FillRect(texture, tx * TileSize + 5, (ty + th) * TileSize - 5, tw * TileSize - 10, 2, palette.accent);
            }
        }

        private static void DrawTrees(Texture2D texture, Palette palette, int year)
        {
            var points = new[]
            {
                new Vector2Int(1, 18), new Vector2Int(10, 18), new Vector2Int(19, 18),
                new Vector2Int(1, 4), new Vector2Int(10, 4), new Vector2Int(19, 4),
                new Vector2Int(28, 11), new Vector2Int(30, 11)
            };

            foreach (var p in points)
            {
                var x = p.x * TileSize + 4;
                var y = p.y * TileSize + 2;
                FillRect(texture, x + 5, y, 3, 9, palette.trunk);
                FillRect(texture, x + 1, y + 7, 11, 8, Darken(palette.tree, 0.25f));
                FillRect(texture, x, y + 10, 13, 7, palette.tree);
                DrawRectOutline(texture, x, y + 9, 13, 8, palette.outline);
            }
        }

        private static void DrawStreetDetails(Texture2D texture, Palette palette, int year)
        {
            var lamps = new[]
            {
                new Vector2Int(5, 7), new Vector2Int(15, 7), new Vector2Int(29, 7),
                new Vector2Int(5, 14), new Vector2Int(15, 14), new Vector2Int(29, 14)
            };

            foreach (var p in lamps)
            {
                var x = p.x * TileSize + 7;
                var y = p.y * TileSize + 2;
                FillRect(texture, x, y, 2, 11, palette.post);
                FillRect(texture, x - 3, y + 9, 8, 4, palette.outline);
                FillRect(texture, x - 2, y + 10, 6, 2, palette.lamp);
            }

            if (year == 2096)
            {
                var x = 29 * TileSize + 4;
                var y = 17 * TileSize;
                FillRect(texture, x, y, 8, 31, palette.outline);
                FillRect(texture, x + 2, y + 2, 4, 27, new Color32(20, 22, 31, 255));
                FillRect(texture, x + 2, y + 23, 4, 2, palette.accent);
            }
        }

        private static void DrawTemporalVehicle(Texture2D texture, Palette palette, int year)
        {
            var x = 14 * TileSize;
            var y = 9 * TileSize;
            var outline = palette.outline;

            if (year == 1956)
            {
                FillRect(texture, x, y, 32, 13, outline);
                FillRect(texture, x + 2, y + 2, 28, 9, new Color32(129, 77, 45, 255));
                FillRect(texture, x + 9, y + 10, 14, 7, outline);
                FillRect(texture, x + 11, y + 11, 10, 5, new Color32(213, 177, 108, 255));
                FillRect(texture, x + 2, y + 4, 3, 3, palette.accent);
            }
            else if (year == 2026)
            {
                FillRect(texture, x, y, 36, 12, outline);
                FillRect(texture, x + 2, y + 2, 32, 8, new Color32(74, 79, 86, 255));
                FillRect(texture, x + 10, y + 9, 16, 7, outline);
                FillRect(texture, x + 12, y + 10, 12, 5, new Color32(78, 131, 155, 255));
                FillRect(texture, x + 5, y + 1, 24, 2, palette.accent);
            }
            else
            {
                FillRect(texture, x + 2, y + 3, 34, 10, outline);
                FillRect(texture, x + 4, y + 5, 30, 6, new Color32(50, 43, 78, 255));
                FillRect(texture, x + 11, y + 10, 16, 7, outline);
                FillRect(texture, x + 13, y + 11, 12, 5, new Color32(57, 202, 222, 255));
                FillRect(texture, x + 6, y, 24, 2, palette.accent);
            }
        }

        private static Palette PaletteFor(int year)
        {
            if (year == 1956)
            {
                return new Palette(
                    new Color32(116, 155, 89, 255), new Color32(101, 141, 77, 255),
                    new Color32(56, 58, 55, 255), new Color32(181, 173, 150, 255), new Color32(151, 144, 124, 255),
                    new Color32(230, 225, 205, 255), new Color32(218, 120, 71, 255), new Color32(225, 176, 94, 255),
                    new Color32(174, 91, 55, 255), new Color32(207, 146, 68, 255), new Color32(217, 238, 226, 255),
                    new Color32(242, 250, 245, 255), new Color32(92, 58, 39, 255), new Color32(40, 45, 43, 255),
                    new Color32(72, 65, 55, 255), new Color32(255, 216, 100, 255), new Color32(47, 143, 153, 255),
                    new Color32(49, 116, 61, 255), new Color32(92, 61, 34, 255));
            }

            if (year == 2026)
            {
                return new Palette(
                    new Color32(100, 163, 96, 255), new Color32(82, 145, 80, 255),
                    new Color32(52, 57, 63, 255), new Color32(186, 190, 184, 255), new Color32(155, 161, 157, 255),
                    new Color32(235, 237, 232, 255), new Color32(219, 108, 64, 255), new Color32(78, 138, 166, 255),
                    new Color32(164, 74, 50, 255), new Color32(52, 104, 132, 255), new Color32(211, 235, 239, 255),
                    new Color32(240, 250, 252, 255), new Color32(76, 74, 70, 255), new Color32(34, 39, 44, 255),
                    new Color32(56, 61, 65, 255), new Color32(255, 226, 116, 255), new Color32(57, 188, 222, 255),
                    new Color32(52, 133, 70, 255), new Color32(90, 63, 38, 255));
            }

            return new Palette(
                new Color32(48, 83, 78, 255), new Color32(38, 70, 68, 255),
                new Color32(24, 25, 35, 255), new Color32(69, 73, 89, 255), new Color32(53, 57, 71, 255),
                new Color32(118, 128, 145, 255), new Color32(70, 55, 101, 255), new Color32(43, 91, 111, 255),
                new Color32(45, 34, 68, 255), new Color32(34, 73, 91, 255), new Color32(73, 202, 216, 255),
                new Color32(180, 247, 249, 255), new Color32(47, 53, 65, 255), new Color32(18, 20, 29, 255),
                new Color32(42, 47, 60, 255), new Color32(125, 246, 235, 255), new Color32(54, 234, 228, 255),
                new Color32(50, 153, 115, 255), new Color32(73, 65, 62, 255));
        }

        private static Texture2D NewTexture(int width, int height, string name)
        {
            return new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = name,
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
        }

        private static void Clear(Texture2D texture)
        {
            var clear = new Color32(0, 0, 0, 0);
            var colors = new Color32[texture.width * texture.height];
            Array.Fill(colors, clear);
            texture.SetPixels32(colors);
        }

        private static void Fill(Texture2D texture, Color32 color)
        {
            var colors = new Color32[texture.width * texture.height];
            Array.Fill(colors, color);
            texture.SetPixels32(colors);
        }

        private static void DrawTileRect(Texture2D texture, int tx, int ty, int tw, int th, Color32 color)
        {
            FillRect(texture, tx * TileSize, ty * TileSize, tw * TileSize, th * TileSize, color);
        }

        private static void DrawTileGridLines(Texture2D texture, int tx, int ty, int tw, int th, Color32 color)
        {
            for (var x = tx; x <= tx + tw; x++)
                FillRect(texture, x * TileSize, ty * TileSize, 1, th * TileSize, color);
            for (var y = ty; y <= ty + th; y++)
                FillRect(texture, tx * TileSize, y * TileSize, tw * TileSize, 1, color);
        }

        private static void FillRect(Texture2D texture, int x, int y, int width, int height, Color32 color)
        {
            for (var py = Mathf.Max(0, y); py < Mathf.Min(texture.height, y + height); py++)
            {
                for (var px = Mathf.Max(0, x); px < Mathf.Min(texture.width, x + width); px++)
                    texture.SetPixel(px, py, color);
            }
        }

        private static void DrawRectOutline(Texture2D texture, int x, int y, int width, int height, Color32 color)
        {
            FillRect(texture, x, y, width, 1, color);
            FillRect(texture, x, y + height - 1, width, 1, color);
            FillRect(texture, x, y, 1, height, color);
            FillRect(texture, x + width - 1, y, 1, height, color);
        }

        private static void OutlineAlpha(Texture2D texture, Color32 outline)
        {
            var copy = texture.GetPixels32();
            for (var y = 0; y < texture.height; y++)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    var index = y * texture.width + x;
                    if (copy[index].a != 0) continue;

                    var hasNeighbor = false;
                    for (var oy = -1; oy <= 1 && !hasNeighbor; oy++)
                    for (var ox = -1; ox <= 1; ox++)
                    {
                        if (ox == 0 && oy == 0) continue;
                        var nx = x + ox;
                        var ny = y + oy;
                        if (nx < 0 || ny < 0 || nx >= texture.width || ny >= texture.height) continue;
                        if (copy[ny * texture.width + nx].a > 0)
                        {
                            hasNeighbor = true;
                            break;
                        }
                    }

                    if (hasNeighbor) texture.SetPixel(x, y, outline);
                }
            }
        }

        private static Color32 Darken(Color32 color, float amount)
        {
            return new Color32(
                (byte)(color.r * (1f - amount)),
                (byte)(color.g * (1f - amount)),
                (byte)(color.b * (1f - amount)),
                color.a);
        }

        private readonly struct Palette
        {
            public readonly Color32 grass;
            public readonly Color32 grassDetail;
            public readonly Color32 road;
            public readonly Color32 sidewalk;
            public readonly Color32 sidewalkLine;
            public readonly Color32 crosswalk;
            public readonly Color32 buildingA;
            public readonly Color32 buildingB;
            public readonly Color32 roofA;
            public readonly Color32 roofB;
            public readonly Color32 window;
            public readonly Color32 windowShine;
            public readonly Color32 door;
            public readonly Color32 outline;
            public readonly Color32 post;
            public readonly Color32 lamp;
            public readonly Color32 accent;
            public readonly Color32 tree;
            public readonly Color32 trunk;
            public readonly Color32 roadMark;

            public Palette(
                Color32 grass, Color32 grassDetail, Color32 road, Color32 sidewalk, Color32 sidewalkLine,
                Color32 crosswalk, Color32 buildingA, Color32 buildingB, Color32 roofA, Color32 roofB,
                Color32 window, Color32 windowShine, Color32 door, Color32 outline, Color32 post,
                Color32 lamp, Color32 accent, Color32 tree, Color32 trunk)
            {
                this.grass = grass;
                this.grassDetail = grassDetail;
                this.road = road;
                this.sidewalk = sidewalk;
                this.sidewalkLine = sidewalkLine;
                this.crosswalk = crosswalk;
                this.buildingA = buildingA;
                this.buildingB = buildingB;
                this.roofA = roofA;
                this.roofB = roofB;
                this.window = window;
                this.windowShine = windowShine;
                this.door = door;
                this.outline = outline;
                this.post = post;
                this.lamp = lamp;
                this.accent = accent;
                this.tree = tree;
                this.trunk = trunk;
                roadMark = new Color32(225, 225, 206, 255);
            }
        }
    }
}

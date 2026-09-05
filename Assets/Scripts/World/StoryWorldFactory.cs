using System.Collections.Generic;
using UnityEngine;

namespace Pythime
{
    public static class StoryWorldFactory
    {
        public const int TileSize = 16;
        public const int MapWidthTiles = 64;
        public const int MapHeightTiles = 46;

        public static readonly Vector2 StartPoint = TileToWorld(7f, 6f);
        public static readonly Vector2 WorkshopPoint = TileToWorld(56f, 36f);
        public static readonly Vector2 SoilPoint = TileToWorld(8f, 32f);
        public static readonly Vector2 MonolithPoint = TileToWorld(45f, 34f);
        public static readonly Vector2 VehiclePoint = TileToWorld(22f, 23f);
        public static readonly Vector2 ClockPlazaPoint = TileToWorld(32f, 34f);

        private static readonly RectInt[] buildingRects =
        {
            new RectInt(2, 38, 10, 6), new RectInt(18, 38, 8, 6), new RectInt(38, 38, 8, 6), new RectInt(53, 37, 8, 7),
            new RectInt(18, 27, 8, 5), new RectInt(38, 27, 8, 5), new RectInt(53, 27, 8, 6),
            new RectInt(2, 12, 10, 6), new RectInt(18, 12, 8, 6), new RectInt(38, 12, 8, 6), new RectInt(53, 11, 8, 7),
            new RectInt(2, 2, 10, 7), new RectInt(18, 2, 8, 7), new RectInt(38, 2, 9, 7), new RectInt(53, 2, 8, 7)
        };

        private static readonly Dictionary<int, Sprite> mapCache = new Dictionary<int, Sprite>();
        private static readonly Dictionary<string, Sprite> propCache = new Dictionary<string, Sprite>();

        public static IReadOnlyList<RectInt> BuildingRects => buildingRects;

        public static Vector2 TileToWorld(float tileX, float tileY)
        {
            return new Vector2(tileX - MapWidthTiles / 2f, tileY - MapHeightTiles / 2f);
        }

        public static Sprite CreateTownMap(int year)
        {
            Sprite cached;
            if (mapCache.TryGetValue(year, out cached)) return cached;

            var width = MapWidthTiles * TileSize;
            var height = MapHeightTiles * TileSize;
            var texture = NewTexture(width, height, "PythimeCity_" + year);
            var palette = new Palette(year);

            Fill(texture, palette.grass);
            AddGroundTexture(texture, palette, year);
            DrawRoadNetwork(texture, palette);
            DrawBuildings(texture, palette, year);
            DrawClockPlaza(texture, palette, year);
            DrawPark(texture, palette, year);
            DrawStation(texture, palette, year);
            DrawStreetProps(texture, palette, year);
            DrawParkedCars(texture, palette, year);
            DrawEraDetails(texture, palette, year);

            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), TileSize, 0, SpriteMeshType.FullRect);
            sprite.name = "PythimeCity_" + year;
            mapCache[year] = sprite;
            return sprite;
        }

        public static Sprite CreateTemporalVehicleSprite(int year)
        {
            var key = "timecar_" + year;
            Sprite cached;
            if (propCache.TryGetValue(key, out cached)) return cached;

            var texture = NewTexture(96, 64, key);
            Clear(texture);
            var outline = new Color32(24, 27, 32, 255);
            var cyan = new Color32(72, 225, 245, 255);

            if (year == 2026)
            {
                var steel = new Color32(143, 151, 158, 255);
                var steelLight = new Color32(200, 205, 208, 255);
                var steelDark = new Color32(73, 82, 91, 255);
                var glass = new Color32(31, 52, 63, 255);
                var red = new Color32(220, 67, 58, 255);

                FillRect(texture, 8, 18, 80, 25, outline);
                FillRect(texture, 12, 21, 72, 19, steel);
                FillRect(texture, 17, 17, 62, 5, steelDark);
                FillRect(texture, 22, 40, 52, 13, outline);
                FillRect(texture, 26, 41, 44, 10, glass);
                FillRect(texture, 30, 43, 36, 2, new Color32(57, 92, 105, 255));

                FillRect(texture, 12, 23, 7, 14, steelLight);
                FillRect(texture, 77, 23, 7, 14, steelLight);
                FillRect(texture, 17, 38, 62, 3, steelLight);

                FillRect(texture, 4, 20, 10, 18, outline);
                FillRect(texture, 82, 20, 10, 18, outline);
                FillRect(texture, 6, 23, 6, 12, new Color32(39, 42, 47, 255));
                FillRect(texture, 84, 23, 6, 12, new Color32(39, 42, 47, 255));

                FillLine(texture, 26, 40, 15, 24, steelDark);
                FillLine(texture, 70, 40, 81, 24, steelDark);
                FillLine(texture, 48, 41, 48, 22, steelDark);

                FillRect(texture, 20, 20, 56, 2, cyan);
                FillRect(texture, 28, 16, 40, 2, cyan);
                FillRect(texture, 38, 13, 20, 4, outline);
                FillRect(texture, 40, 14, 16, 2, cyan);

                FillRect(texture, 13, 27, 6, 4, new Color32(248, 232, 178, 255));
                FillRect(texture, 77, 27, 6, 4, new Color32(248, 232, 178, 255));
                FillRect(texture, 18, 36, 8, 3, red);
                FillRect(texture, 70, 36, 8, 3, red);

                FillRect(texture, 29, 52, 10, 4, outline);
                FillRect(texture, 57, 52, 10, 4, outline);
                FillRect(texture, 31, 53, 6, 2, cyan);
                FillRect(texture, 59, 53, 6, 2, cyan);
            }
            else if (year == 1956)
            {
                var bronze = new Color32(131, 78, 48, 255);
                var brass = new Color32(218, 169, 82, 255);
                var glass = new Color32(72, 102, 103, 255);

                FillRect(texture, 10, 20, 76, 23, outline);
                FillRect(texture, 14, 23, 68, 17, bronze);
                FillRect(texture, 27, 40, 42, 12, outline);
                FillRect(texture, 31, 41, 34, 9, glass);
                FillRect(texture, 5, 22, 11, 16, outline);
                FillRect(texture, 80, 22, 11, 16, outline);
                FillRect(texture, 7, 25, 7, 10, new Color32(47, 46, 42, 255));
                FillRect(texture, 82, 25, 7, 10, new Color32(47, 46, 42, 255));
                FillRect(texture, 20, 21, 56, 2, brass);
                FillRect(texture, 39, 15, 18, 6, outline);
                FillRect(texture, 42, 17, 12, 3, brass);
                FillRect(texture, 27, 52, 12, 3, brass);
                FillRect(texture, 57, 52, 12, 3, brass);
                FillRect(texture, 17, 27, 5, 4, new Color32(247, 222, 150, 255));
                FillRect(texture, 74, 27, 5, 4, new Color32(247, 222, 150, 255));
            }
            else
            {
                var body = new Color32(63, 53, 99, 255);
                var bodyLight = new Color32(101, 88, 145, 255);
                var glass = new Color32(48, 188, 211, 255);

                FillRect(texture, 9, 23, 78, 20, outline);
                FillRect(texture, 14, 26, 68, 14, body);
                FillRect(texture, 25, 40, 46, 13, outline);
                FillRect(texture, 29, 41, 38, 10, glass);
                FillRect(texture, 17, 21, 62, 2, cyan);
                FillRect(texture, 22, 17, 52, 5, outline);
                FillRect(texture, 26, 18, 44, 3, cyan);
                FillRect(texture, 17, 41, 8, 4, bodyLight);
                FillRect(texture, 71, 41, 8, 4, bodyLight);
                FillRect(texture, 15, 53, 16, 3, cyan);
                FillRect(texture, 65, 53, 16, 3, cyan);
            }

            OutlineAlpha(texture, outline);
            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.38f), TileSize);
            sprite.name = key;
            propCache[key] = sprite;
            return sprite;
        }

        public static Sprite CreateVehicleGlowSprite(int year)
        {
            var key = "vehicle_glow_" + year;
            Sprite cached;
            if (propCache.TryGetValue(key, out cached)) return cached;

            var texture = NewTexture(80, 30, key);
            Clear(texture);
            var color = year == 1956 ? new Color32(245, 189, 76, 70) : new Color32(63, 220, 246, 75);
            for (var y = 3; y < 26; y++)
            {
                for (var x = 5; x < 75; x++)
                {
                    var dx = (x - 40f) / 35f;
                    var dy = (y - 14f) / 11f;
                    var d = dx * dx + dy * dy;
                    if (d <= 1f)
                    {
                        var a = (byte)Mathf.Clamp(color.a * (1f - d), 0f, 255f);
                        texture.SetPixel(x, y, new Color32(color.r, color.g, color.b, a));
                    }
                }
            }
            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), TileSize);
            propCache[key] = sprite;
            return sprite;
        }

        public static Sprite CreateTockSprite()
        {
            const string key = "tock_companion";
            Sprite cached;
            if (propCache.TryGetValue(key, out cached)) return cached;

            var texture = NewTexture(20, 24, key);
            Clear(texture);
            var outline = new Color32(25, 28, 33, 255);
            var shell = new Color32(232, 224, 198, 255);
            var shellDark = new Color32(174, 165, 144, 255);
            var cyan = new Color32(67, 224, 239, 255);
            FillRect(texture, 4, 6, 12, 12, outline);
            FillRect(texture, 5, 7, 10, 10, shell);
            FillRect(texture, 5, 7, 2, 10, shellDark);
            FillRect(texture, 7, 18, 6, 3, outline);
            FillRect(texture, 8, 19, 4, 1, cyan);
            FillRect(texture, 7, 12, 2, 2, cyan);
            FillRect(texture, 11, 12, 2, 2, cyan);
            FillRect(texture, 8, 4, 4, 3, outline);
            FillRect(texture, 9, 3, 2, 2, cyan);
            FillRect(texture, 3, 9, 2, 5, outline);
            FillRect(texture, 15, 9, 2, 5, outline);
            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, 20, 24), new Vector2(0.5f, 0.22f), TileSize);
            propCache[key] = sprite;
            return sprite;
        }

        public static Sprite CreateMonolithSprite(int year)
        {
            var key = "monolith_" + year;
            Sprite cached;
            if (propCache.TryGetValue(key, out cached)) return cached;
            var texture = NewTexture(24, 52, key);
            Clear(texture);
            var outline = new Color32(10, 12, 17, 255);
            var core = new Color32(26, 28, 38, 255);
            var glow = new Color32(77, 234, 226, 255);
            FillRect(texture, 4, 2, 16, 47, outline);
            FillRect(texture, 7, 5, 10, 41, core);
            FillRect(texture, 9, 37, 6, 2, glow);
            FillRect(texture, 9, 25, 6, 1, glow);
            FillRect(texture, 9, 14, 6, 1, glow);
            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, 24, 52), new Vector2(0.5f, 0.08f), TileSize);
            propCache[key] = sprite;
            return sprite;
        }

        private static void AddGroundTexture(Texture2D texture, Palette p, int year)
        {
            var random = new System.Random(year * 31);
            for (var i = 0; i < 2800; i++)
            {
                var x = random.Next(0, texture.width);
                var y = random.Next(0, texture.height);
                if (random.NextDouble() > 0.38)
                    texture.SetPixel(x, y, p.grassDetail);
            }
        }

        private static void DrawRoadNetwork(Texture2D texture, Palette p)
        {
            DrawTileRect(texture, 0, 20, MapWidthTiles, 6, p.road);
            DrawTileRect(texture, 29, 0, 6, MapHeightTiles, p.road);
            DrawTileRect(texture, 0, 34, 29, 4, p.road);
            DrawTileRect(texture, 35, 34, 29, 4, p.road);
            DrawTileRect(texture, 13, 0, 4, 20, p.road);
            DrawTileRect(texture, 48, 26, 4, 20, p.road);

            DrawSidewalk(texture, 0, 19, MapWidthTiles, 1, p);
            DrawSidewalk(texture, 0, 26, MapWidthTiles, 1, p);
            DrawSidewalk(texture, 28, 0, 1, MapHeightTiles, p);
            DrawSidewalk(texture, 35, 0, 1, MapHeightTiles, p);
            DrawSidewalk(texture, 0, 33, 29, 1, p);
            DrawSidewalk(texture, 35, 33, 29, 1, p);
            DrawSidewalk(texture, 0, 38, 29, 1, p);
            DrawSidewalk(texture, 35, 38, 29, 1, p);

            for (var x = 1; x < MapWidthTiles - 1; x += 4)
                FillRect(texture, x * TileSize + 3, 23 * TileSize + 7, 11, 2, p.roadMark);
            for (var y = 1; y < MapHeightTiles - 1; y += 4)
                FillRect(texture, 32 * TileSize + 7, y * TileSize + 3, 2, 11, p.roadMark);

            for (var i = 0; i < 6; i++)
            {
                FillRect(texture, 27 * TileSize + i * 10, 20 * TileSize + 3, 5, 12, p.roadMark);
                FillRect(texture, 35 * TileSize + 3, 18 * TileSize + i * 10, 12, 5, p.roadMark);
            }
        }

        private static void DrawBuildings(Texture2D texture, Palette p, int year)
        {
            for (var i = 0; i < buildingRects.Length; i++)
            {
                var r = buildingRects[i];
                var wall = i % 2 == 0 ? p.buildingA : p.buildingB;
                DrawBuilding(texture, r.x, r.y, r.width, r.height, wall, p, year, i);
            }
            DrawWorkshopSign(texture, p, year);
            DrawMuseumSign(texture, p);
        }

        private static void DrawBuilding(Texture2D texture, int tx, int ty, int tw, int th, Color32 wall, Palette p, int year, int index)
        {
            var sx = tx * TileSize;
            var sy = ty * TileSize;
            FillRect(texture, sx + 5, sy - 5, tw * TileSize, th * TileSize, new Color32(22, 25, 27, 70));
            DrawTileRect(texture, tx, ty, tw, th, p.outline);
            FillRect(texture, sx + 4, sy + 4, tw * TileSize - 8, th * TileSize - 8, wall);
            var roof = index % 3 == 0 ? p.roofA : p.roofB;
            FillRect(texture, sx + 2, (ty + th - 2) * TileSize, tw * TileSize - 4, TileSize + 8, roof);
            FillRect(texture, sx + 5, (ty + th - 1) * TileSize + 2, tw * TileSize - 10, 2, Lighten(roof, 0.15f));

            for (var x = tx + 1; x < tx + tw - 1; x += 2)
            {
                var wx = x * TileSize + 4;
                var wy = ty * TileSize + 10;
                FillRect(texture, wx - 1, wy - 1, 10, 10, p.outline);
                FillRect(texture, wx, wy, 8, 8, p.window);
                FillRect(texture, wx + 1, wy + 5, 6, 2, p.windowShine);
            }

            var doorX = (tx + tw / 2) * TileSize + 3;
            FillRect(texture, doorX - 1, sy + 1, 12, 17, p.outline);
            FillRect(texture, doorX, sy + 2, 10, 15, p.door);
            texture.SetPixel(doorX + 7, sy + 9, p.accent);

            if (tw >= 8)
            {
                FillRect(texture, sx + 10, (ty + th) * TileSize - 9, 18, 6, p.outline);
                FillRect(texture, sx + 12, (ty + th) * TileSize - 8, 14, 4, p.sidewalkAlt);
            }

            if (year == 2096)
                FillRect(texture, sx + 6, (ty + th) * TileSize - 5, tw * TileSize - 12, 2, p.accent);
        }

        private static void DrawWorkshopSign(Texture2D texture, Palette p, int year)
        {
            var x = 54 * TileSize;
            var y = 40 * TileSize;
            FillRect(texture, x, y, 82, 15, p.outline);
            FillRect(texture, x + 2, y + 2, 78, 11, year == 2096 ? p.accent : p.roofA);
            FillRect(texture, x + 8, y + 5, 8, 5, p.windowShine);
            FillRect(texture, x + 22, y + 5, 8, 5, p.windowShine);
            FillRect(texture, x + 36, y + 5, 8, 5, p.windowShine);
        }

        private static void DrawMuseumSign(Texture2D texture, Palette p)
        {
            var x = 39 * TileSize;
            var y = 42 * TileSize;
            FillRect(texture, x, y, 54, 12, p.outline);
            FillRect(texture, x + 2, y + 2, 50, 8, p.roofB);
            FillRect(texture, x + 8, y + 4, 5, 4, p.windowShine);
        }

        private static void DrawClockPlaza(Texture2D texture, Palette p, int year)
        {
            DrawTileRect(texture, 25, 29, 14, 11, p.plaza);
            for (var x = 25; x < 39; x++)
            {
                for (var y = 29; y < 40; y++)
                {
                    if ((x + y) % 2 == 0)
                        FillRect(texture, x * TileSize + 1, y * TileSize + 1, TileSize - 2, TileSize - 2, p.plazaAlt);
                }
            }

            var cx = 32 * TileSize;
            var cy = 34 * TileSize;
            FillRect(texture, cx - 15, cy - 15, 30, 30, p.outline);
            FillRect(texture, cx - 12, cy - 12, 24, 24, p.water);
            FillRect(texture, cx - 2, cy - 2, 4, 22, p.outline);
            FillRect(texture, cx - 5, cy + 18, 10, 7, p.clock);
            FillRect(texture, cx - 9, cy - 4, 18, 2, Lighten(p.water, 0.18f));
            if (year == 2096) FillRect(texture, cx - 10, cy - 10, 20, 2, p.accent);
        }

        private static void DrawPark(Texture2D texture, Palette p, int year)
        {
            FillRect(texture, 2 * TileSize, 27 * TileSize, 11 * TileSize, 10 * TileSize, Lighten(p.grass, 0.04f));
            for (var x = 3; x < 12; x += 2)
                FillRect(texture, x * TileSize, 30 * TileSize, 12, 2, p.sidewalkAlt);

            var trees = new[]
            {
                new Vector2Int(3, 29), new Vector2Int(6, 36), new Vector2Int(11, 29), new Vector2Int(10, 35),
                new Vector2Int(20, 31), new Vector2Int(42, 30), new Vector2Int(58, 30), new Vector2Int(7, 16),
                new Vector2Int(57, 16), new Vector2Int(42, 7)
            };
            for (var i = 0; i < trees.Length; i++) DrawTree(texture, trees[i].x, trees[i].y, p, year);

            for (var i = 0; i < 3; i++)
            {
                var bx = (4 + i * 3) * TileSize;
                var by = 28 * TileSize;
                FillRect(texture, bx, by, 13, 5, p.outline);
                FillRect(texture, bx + 1, by + 1, 11, 2, p.bench);
            }
        }

        private static void DrawStation(Texture2D texture, Palette p, int year)
        {
            var x = 2 * TileSize;
            var y = 21 * TileSize;
            FillRect(texture, x, y, 126, 5, p.outline);
            for (var i = 0; i < 7; i++) FillRect(texture, x + 8 + i * 17, y + 5, 3, 20, p.post);
            FillRect(texture, x + 4, y + 25, 118, 6, p.roofB);
            FillRect(texture, x + 10, y + 12, 28, 8, p.outline);
            FillRect(texture, x + 12, y + 14, 24, 4, p.window);
            if (year == 2096) FillRect(texture, x + 8, y + 28, 110, 2, p.accent);
        }

        private static void DrawStreetProps(Texture2D texture, Palette p, int year)
        {
            var lamps = new[]
            {
                new Vector2Int(6, 19), new Vector2Int(21, 19), new Vector2Int(43, 19), new Vector2Int(58, 19),
                new Vector2Int(6, 26), new Vector2Int(21, 26), new Vector2Int(43, 26), new Vector2Int(58, 26),
                new Vector2Int(28, 7), new Vector2Int(35, 7), new Vector2Int(28, 42), new Vector2Int(35, 42)
            };

            for (var i = 0; i < lamps.Length; i++)
            {
                var x = lamps[i].x * TileSize + 7;
                var y = lamps[i].y * TileSize + 2;
                FillRect(texture, x, y, 2, 12, p.post);
                FillRect(texture, x - 3, y + 10, 8, 4, p.outline);
                FillRect(texture, x - 2, y + 11, 6, 2, year == 2096 ? p.accent : p.lamp);
            }

            for (var x = 39; x < 48; x += 3)
            {
                FillRect(texture, x * TileSize + 3, 18 * TileSize + 8, 10, 4, p.outline);
                FillRect(texture, x * TileSize + 4, 18 * TileSize + 9, 8, 2, p.bench);
            }
        }

        private static void DrawParkedCars(Texture2D texture, Palette p, int year)
        {
            var carA = year == 1956 ? new Color32(188, 91, 57, 255) : year == 2096 ? new Color32(63, 130, 152, 255) : new Color32(210, 72, 60, 255);
            var carB = year == 1956 ? new Color32(224, 183, 96, 255) : year == 2096 ? new Color32(100, 82, 151, 255) : new Color32(63, 128, 178, 255);
            DrawSmallCar(texture, 8 * TileSize, 22 * TileSize + 3, carA, p);
            DrawSmallCar(texture, 43 * TileSize, 22 * TileSize + 3, carB, p);
            DrawSmallCar(texture, 55 * TileSize, 22 * TileSize + 3, carA, p);
        }

        private static void DrawSmallCar(Texture2D texture, int x, int y, Color32 body, Palette p)
        {
            FillRect(texture, x, y, 25, 12, p.outline);
            FillRect(texture, x + 2, y + 2, 21, 8, body);
            FillRect(texture, x + 6, y + 8, 13, 5, p.outline);
            FillRect(texture, x + 8, y + 9, 9, 3, p.window);
            FillRect(texture, x + 3, y - 2, 5, 3, p.outline);
            FillRect(texture, x + 17, y - 2, 5, 3, p.outline);
        }

        private static void DrawEraDetails(Texture2D texture, Palette p, int year)
        {
            if (year == 1956)
            {
                FillRect(texture, 50 * TileSize, 30 * TileSize, 4, 31, new Color32(94, 70, 48, 255));
                FillRect(texture, 49 * TileSize + 8, 31 * TileSize + 12, 24, 5, new Color32(224, 183, 102, 255));
            }
            else if (year == 2096)
            {
                for (var y = 3; y < 44; y += 7)
                    FillRect(texture, 34 * TileSize + 2, y * TileSize, 2, 18, p.accent);
                FillRect(texture, 46 * TileSize, 33 * TileSize, 20, 3, p.accent);
            }
        }

        private static void DrawTree(Texture2D texture, int tx, int ty, Palette p, int year)
        {
            var x = tx * TileSize + 2;
            var y = ty * TileSize + 1;
            var trunk = year == 2096 ? new Color32(84, 72, 96, 255) : p.trunk;
            FillRect(texture, x + 6, y, 4, 11, trunk);
            FillRect(texture, x + 2, y + 8, 13, 10, Darken(p.tree, 0.22f));
            FillRect(texture, x, y + 12, 17, 8, p.tree);
            DrawRectOutline(texture, x, y + 11, 17, 9, p.outline);
            FillRect(texture, x + 4, y + 16, 3, 2, Lighten(p.tree, 0.16f));
        }

        private static void DrawSidewalk(Texture2D texture, int tx, int ty, int tw, int th, Palette p)
        {
            DrawTileRect(texture, tx, ty, tw, th, p.sidewalk);
            for (var x = tx; x < tx + tw; x++)
            {
                for (var y = ty; y < ty + th; y++)
                {
                    if ((x + y) % 2 == 0)
                        FillRect(texture, x * TileSize + 1, y * TileSize + 1, TileSize - 2, TileSize - 2, p.sidewalkAlt);
                }
            }
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
            var pixels = new Color32[texture.width * texture.height];
            var clear = new Color32(0, 0, 0, 0);
            for (var i = 0; i < pixels.Length; i++) pixels[i] = clear;
            texture.SetPixels32(pixels);
        }

        private static void Fill(Texture2D texture, Color32 color)
        {
            var pixels = new Color32[texture.width * texture.height];
            for (var i = 0; i < pixels.Length; i++) pixels[i] = color;
            texture.SetPixels32(pixels);
        }

        private static void DrawTileRect(Texture2D texture, int tx, int ty, int tw, int th, Color32 color)
        {
            FillRect(texture, tx * TileSize, ty * TileSize, tw * TileSize, th * TileSize, color);
        }

        private static void FillRect(Texture2D texture, int x, int y, int width, int height, Color32 color)
        {
            var x0 = Mathf.Clamp(x, 0, texture.width);
            var y0 = Mathf.Clamp(y, 0, texture.height);
            var x1 = Mathf.Clamp(x + width, 0, texture.width);
            var y1 = Mathf.Clamp(y + height, 0, texture.height);
            for (var py = y0; py < y1; py++)
                for (var px = x0; px < x1; px++)
                    texture.SetPixel(px, py, color);
        }

        private static void FillLine(Texture2D texture, int x0, int y0, int x1, int y1, Color32 color)
        {
            var dx = Mathf.Abs(x1 - x0);
            var sx = x0 < x1 ? 1 : -1;
            var dy = -Mathf.Abs(y1 - y0);
            var sy = y0 < y1 ? 1 : -1;
            var err = dx + dy;
            while (true)
            {
                if (x0 >= 0 && x0 < texture.width && y0 >= 0 && y0 < texture.height)
                    texture.SetPixel(x0, y0, color);
                if (x0 == x1 && y0 == y1) break;
                var e2 = 2 * err;
                if (e2 >= dy) { err += dy; x0 += sx; }
                if (e2 <= dx) { err += dx; y0 += sy; }
            }
        }

        private static void DrawRectOutline(Texture2D texture, int x, int y, int width, int height, Color32 color)
        {
            FillRect(texture, x, y, width, 1, color);
            FillRect(texture, x, y + height - 1, width, 1, color);
            FillRect(texture, x, y, 1, height, color);
            FillRect(texture, x + width - 1, y, 1, height, color);
        }

        private static void OutlineAlpha(Texture2D texture, Color32 color)
        {
            var source = texture.GetPixels32();
            var result = texture.GetPixels32();
            for (var y = 0; y < texture.height; y++)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    var i = y * texture.width + x;
                    if (source[i].a != 0) continue;
                    var neighbor = false;
                    for (var oy = -1; oy <= 1 && !neighbor; oy++)
                    {
                        for (var ox = -1; ox <= 1; ox++)
                        {
                            if (ox == 0 && oy == 0) continue;
                            var nx = x + ox;
                            var ny = y + oy;
                            if (nx < 0 || ny < 0 || nx >= texture.width || ny >= texture.height) continue;
                            if (source[ny * texture.width + nx].a != 0) { neighbor = true; break; }
                        }
                    }
                    if (neighbor) result[i] = color;
                }
            }
            texture.SetPixels32(result);
        }

        private static Color32 Darken(Color32 color, float amount)
        {
            return new Color32((byte)(color.r * (1f - amount)), (byte)(color.g * (1f - amount)), (byte)(color.b * (1f - amount)), color.a);
        }

        private static Color32 Lighten(Color32 color, float amount)
        {
            return new Color32(
                (byte)Mathf.Clamp(color.r + (255 - color.r) * amount, 0f, 255f),
                (byte)Mathf.Clamp(color.g + (255 - color.g) * amount, 0f, 255f),
                (byte)Mathf.Clamp(color.b + (255 - color.b) * amount, 0f, 255f),
                color.a);
        }

        private readonly struct Palette
        {
            public readonly Color32 grass;
            public readonly Color32 grassDetail;
            public readonly Color32 road;
            public readonly Color32 roadMark;
            public readonly Color32 sidewalk;
            public readonly Color32 sidewalkAlt;
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
            public readonly Color32 plaza;
            public readonly Color32 plazaAlt;
            public readonly Color32 water;
            public readonly Color32 bench;
            public readonly Color32 clock;

            public Palette(int year)
            {
                if (year == 1956)
                {
                    grass = new Color32(113, 157, 89, 255); grassDetail = new Color32(93, 141, 78, 255);
                    road = new Color32(57, 57, 54, 255); roadMark = new Color32(229, 216, 176, 255);
                    sidewalk = new Color32(188, 174, 144, 255); sidewalkAlt = new Color32(168, 154, 128, 255);
                    buildingA = new Color32(213, 106, 65, 255); buildingB = new Color32(221, 172, 91, 255);
                    roofA = new Color32(171, 86, 53, 255); roofB = new Color32(196, 135, 67, 255);
                    window = new Color32(215, 234, 224, 255); windowShine = new Color32(242, 249, 242, 255);
                    door = new Color32(92, 57, 38, 255); outline = new Color32(39, 44, 42, 255);
                    post = new Color32(75, 67, 55, 255); lamp = new Color32(255, 220, 111, 255);
                    accent = new Color32(47, 145, 153, 255); tree = new Color32(48, 117, 60, 255);
                    trunk = new Color32(93, 62, 35, 255); plaza = new Color32(207, 196, 168, 255);
                    plazaAlt = new Color32(194, 181, 151, 255); water = new Color32(119, 178, 188, 255);
                    bench = new Color32(105, 78, 48, 255); clock = new Color32(230, 207, 121, 255);
                }
                else if (year == 2096)
                {
                    grass = new Color32(49, 78, 73, 255); grassDetail = new Color32(41, 67, 63, 255);
                    road = new Color32(24, 25, 35, 255); roadMark = new Color32(75, 160, 168, 255);
                    sidewalk = new Color32(70, 74, 83, 255); sidewalkAlt = new Color32(60, 65, 74, 255);
                    buildingA = new Color32(81, 56, 110, 255); buildingB = new Color32(45, 83, 103, 255);
                    roofA = new Color32(53, 42, 75, 255); roofB = new Color32(37, 69, 92, 255);
                    window = new Color32(69, 173, 190, 255); windowShine = new Color32(113, 229, 237, 255);
                    door = new Color32(72, 59, 82, 255); outline = new Color32(19, 22, 29, 255);
                    post = new Color32(55, 60, 70, 255); lamp = new Color32(80, 232, 224, 255);
                    accent = new Color32(80, 232, 224, 255); tree = new Color32(44, 124, 104, 255);
                    trunk = new Color32(76, 69, 88, 255); plaza = new Color32(75, 78, 90, 255);
                    plazaAlt = new Color32(65, 68, 80, 255); water = new Color32(48, 121, 132, 255);
                    bench = new Color32(54, 59, 70, 255); clock = new Color32(80, 232, 224, 255);
                }
                else
                {
                    grass = new Color32(96, 163, 95, 255); grassDetail = new Color32(80, 145, 80, 255);
                    road = new Color32(53, 58, 64, 255); roadMark = new Color32(226, 220, 188, 255);
                    sidewalk = new Color32(187, 190, 185, 255); sidewalkAlt = new Color32(158, 163, 159, 255);
                    buildingA = new Color32(216, 107, 65, 255); buildingB = new Color32(77, 137, 165, 255);
                    roofA = new Color32(164, 74, 50, 255); roofB = new Color32(51, 103, 131, 255);
                    window = new Color32(211, 235, 239, 255); windowShine = new Color32(241, 250, 252, 255);
                    door = new Color32(75, 72, 67, 255); outline = new Color32(31, 35, 39, 255);
                    post = new Color32(67, 73, 78, 255); lamp = new Color32(249, 219, 116, 255);
                    accent = new Color32(46, 177, 207, 255); tree = new Color32(43, 128, 69, 255);
                    trunk = new Color32(88, 60, 36, 255); plaza = new Color32(194, 199, 195, 255);
                    plazaAlt = new Color32(176, 183, 179, 255); water = new Color32(94, 154, 174, 255);
                    bench = new Color32(78, 84, 88, 255); clock = new Color32(239, 217, 126, 255);
                }
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace Pythime
{
    public static class StoryWorldFactory
    {
        public const int TileSize = 16;
        public const int MapWidthTiles = 56;
        public const int MapHeightTiles = 40;

        public static readonly Vector2 StartPoint = TileToWorld(6f, 5f);
        public static readonly Vector2 WorkshopPoint = TileToWorld(49f, 33f);
        public static readonly Vector2 SoilPoint = TileToWorld(8f, 30f);
        public static readonly Vector2 MonolithPoint = TileToWorld(45f, 29f);
        public static readonly Vector2 VehiclePoint = TileToWorld(19f, 20.5f);
        public static readonly Vector2 ClockPlazaPoint = TileToWorld(28f, 30f);

        private static readonly RectInt[] buildingRects =
        {
            new(2, 32, 8, 6), new(13, 32, 8, 6), new(35, 32, 8, 6), new(47, 31, 7, 7),
            new(2, 24, 8, 5), new(14, 24, 7, 5), new(35, 24, 7, 5), new(48, 23, 6, 6),
            new(2, 10, 9, 6), new(14, 10, 8, 6), new(34, 10, 9, 6), new(47, 9, 7, 7),
            new(3, 2, 9, 6), new(16, 2, 7, 6), new(34, 2, 9, 6), new(47, 1, 7, 7)
        };

        private static readonly Dictionary<int, Sprite> mapCache = new();
        private static readonly Dictionary<string, Sprite> propCache = new();

        public static IReadOnlyList<RectInt> BuildingRects => buildingRects;

        public static Vector2 TileToWorld(float tileX, float tileY)
        {
            return new Vector2(tileX - MapWidthTiles / 2f, tileY - MapHeightTiles / 2f);
        }

        public static Sprite CreateTownMap(int year)
        {
            if (mapCache.TryGetValue(year, out var cached)) return cached;

            var width = MapWidthTiles * TileSize;
            var height = MapHeightTiles * TileSize;
            var texture = NewTexture(width, height, $"PythimeChapterOne_{year}");
            var p = PaletteFor(year);

            Fill(texture, p.grass);
            AddGrassNoise(texture, p, year);
            DrawRoadNetwork(texture, p);
            DrawBuildings(texture, p, year);
            DrawClockPlaza(texture, p, year);
            DrawPark(texture, p, year);
            DrawStation(texture, p, year);
            DrawStreetProps(texture, p, year);
            DrawEraDetails(texture, p, year);

            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), TileSize, 0, SpriteMeshType.FullRect);
            sprite.name = $"PythimeChapterOne_{year}";
            mapCache[year] = sprite;
            return sprite;
        }

        public static Sprite CreateTemporalVehicleSprite(int year)
        {
            var key = $"timecar_{year}";
            if (propCache.TryGetValue(key, out var cached)) return cached;

            var texture = NewTexture(72, 48, key);
            Clear(texture);
            var outline = new Color32(28, 31, 36, 255);
            var cyan = new Color32(79, 221, 246, 255);

            if (year == 2026)
            {
                var steel = new Color32(151, 158, 163, 255);
                var steelLight = new Color32(192, 198, 201, 255);
                var steelDark = new Color32(92, 101, 109, 255);
                var glass = new Color32(37, 58, 68, 255);

                FillRect(texture, 8, 14, 56, 18, outline);
                FillRect(texture, 11, 16, 50, 14, steel);
                FillRect(texture, 16, 12, 40, 5, outline);
                FillRect(texture, 19, 10, 34, 4, steelDark);

                FillRect(texture, 19, 29, 34, 10, outline);
                FillRect(texture, 22, 30, 28, 7, glass);
                FillRect(texture, 26, 31, 20, 2, new Color32(59, 95, 108, 255));

                FillRect(texture, 12, 17, 5, 10, steelLight);
                FillRect(texture, 55, 17, 5, 10, steelLight);
                FillRect(texture, 13, 27, 46, 2, steelLight);

                FillRect(texture, 5, 16, 7, 12, outline);
                FillRect(texture, 60, 16, 7, 12, outline);
                FillRect(texture, 6, 18, 5, 8, new Color32(45, 48, 53, 255));
                FillRect(texture, 61, 18, 5, 8, new Color32(45, 48, 53, 255));

                FillRect(texture, 13, 38, 12, 3, outline);
                FillRect(texture, 47, 38, 12, 3, outline);
                FillRect(texture, 14, 39, 10, 1, cyan);
                FillRect(texture, 48, 39, 10, 1, cyan);

                FillRect(texture, 30, 13, 12, 3, outline);
                FillRect(texture, 31, 14, 10, 1, cyan);
                FillRect(texture, 14, 15, 44, 2, steelLight);
                FillRect(texture, 17, 18, 2, 9, steelDark);
                FillRect(texture, 53, 18, 2, 9, steelDark);

                FillRect(texture, 20, 18, 1, 9, new Color32(63, 68, 73, 255));
                FillRect(texture, 51, 18, 1, 9, new Color32(63, 68, 73, 255));
                FillRect(texture, 11, 20, 5, 3, new Color32(250, 232, 176, 255));
                FillRect(texture, 56, 20, 5, 3, new Color32(250, 232, 176, 255));
            }
            else if (year == 1956)
            {
                var bronze = new Color32(132, 78, 49, 255);
                var brass = new Color32(219, 170, 83, 255);
                var glass = new Color32(77, 102, 103, 255);
                FillRect(texture, 9, 15, 54, 18, outline);
                FillRect(texture, 12, 17, 48, 14, bronze);
                FillRect(texture, 19, 29, 32, 9, outline);
                FillRect(texture, 22, 30, 26, 6, glass);
                FillRect(texture, 6, 18, 7, 10, outline);
                FillRect(texture, 59, 18, 7, 10, outline);
                FillRect(texture, 7, 20, 5, 6, new Color32(48, 48, 44, 255));
                FillRect(texture, 60, 20, 5, 6, new Color32(48, 48, 44, 255));
                FillRect(texture, 28, 12, 16, 5, outline);
                FillRect(texture, 30, 13, 12, 3, brass);
                FillRect(texture, 16, 16, 40, 2, brass);
                FillRect(texture, 14, 38, 9, 3, brass);
                FillRect(texture, 49, 38, 9, 3, brass);
            }
            else
            {
                var body = new Color32(67, 56, 103, 255);
                var bodyLight = new Color32(101, 89, 143, 255);
                var glass = new Color32(50, 181, 204, 255);
                FillRect(texture, 8, 17, 56, 15, outline);
                FillRect(texture, 12, 19, 48, 11, body);
                FillRect(texture, 20, 29, 32, 10, outline);
                FillRect(texture, 23, 30, 26, 7, glass);
                FillRect(texture, 15, 16, 42, 2, cyan);
                FillRect(texture, 18, 11, 36, 5, outline);
                FillRect(texture, 21, 12, 30, 3, cyan);
                FillRect(texture, 13, 31, 7, 3, bodyLight);
                FillRect(texture, 52, 31, 7, 3, bodyLight);
                FillRect(texture, 11, 38, 13, 3, cyan);
                FillRect(texture, 48, 38, 13, 3, cyan);
            }

            OutlineAlpha(texture, outline);
            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.4f), TileSize);
            sprite.name = key;
            propCache[key] = sprite;
            return sprite;
        }

        public static Sprite CreateTockSprite()
        {
            const string key = "tock_companion";
            if (propCache.TryGetValue(key, out var cached)) return cached;

            var texture = NewTexture(18, 22, key);
            Clear(texture);
            var outline = new Color32(28, 31, 36, 255);
            var shell = new Color32(226, 216, 185, 255);
            var cyan = new Color32(69, 221, 236, 255);
            FillRect(texture, 4, 5, 10, 11, outline);
            FillRect(texture, 5, 6, 8, 9, shell);
            FillRect(texture, 6, 16, 6, 3, outline);
            FillRect(texture, 7, 17, 4, 1, cyan);
            FillRect(texture, 6, 11, 2, 2, cyan);
            FillRect(texture, 10, 11, 2, 2, cyan);
            FillRect(texture, 7, 3, 4, 3, outline);
            FillRect(texture, 8, 2, 2, 2, cyan);
            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, 18, 22), new Vector2(0.5f, 0.25f), TileSize);
            propCache[key] = sprite;
            return sprite;
        }

        public static Sprite CreateMonolithSprite(int year)
        {
            var key = $"monolith_{year}";
            if (propCache.TryGetValue(key, out var cached)) return cached;
            var texture = NewTexture(20, 44, key);
            Clear(texture);
            var outline = new Color32(14, 16, 21, 255);
            var core = year == 2096 ? new Color32(28, 30, 40, 255) : new Color32(42, 45, 48, 255);
            var glow = year == 2096 ? new Color32(80, 232, 224, 255) : new Color32(92, 154, 173, 255);
            FillRect(texture, 4, 2, 12, 39, outline);
            FillRect(texture, 6, 4, 8, 35, core);
            FillRect(texture, 8, 31, 4, 2, glow);
            FillRect(texture, 8, 20, 4, 1, glow);
            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, 20, 44), new Vector2(0.5f, 0.08f), TileSize);
            propCache[key] = sprite;
            return sprite;
        }

        private static void AddGrassNoise(Texture2D texture, Palette p, int year)
        {
            var random = new System.Random(year * 17);
            for (var i = 0; i < 1700; i++)
            {
                var x = random.Next(0, texture.width);
                var y = random.Next(0, texture.height);
                if (random.NextDouble() > 0.4) texture.SetPixel(x, y, p.grassDetail);
            }
        }

        private static void DrawRoadNetwork(Texture2D texture, Palette p)
        {
            DrawTileRect(texture, 0, 18, 56, 6, p.road);
            DrawTileRect(texture, 25, 0, 6, 40, p.road);
            DrawTileRect(texture, 0, 30, 25, 4, p.road);
            DrawTileRect(texture, 31, 30, 25, 4, p.road);
            DrawTileRect(texture, 10, 0, 4, 18, p.road);
            DrawTileRect(texture, 43, 24, 4, 16, p.road);

            DrawSidewalk(texture, 0, 17, 56, 1, p);
            DrawSidewalk(texture, 0, 24, 56, 1, p);
            DrawSidewalk(texture, 24, 0, 1, 40, p);
            DrawSidewalk(texture, 31, 0, 1, 40, p);
            DrawSidewalk(texture, 0, 29, 25, 1, p);
            DrawSidewalk(texture, 31, 29, 25, 1, p);
            DrawSidewalk(texture, 0, 34, 25, 1, p);
            DrawSidewalk(texture, 31, 34, 25, 1, p);

            for (var x = 1; x < 55; x += 4)
                FillRect(texture, x * TileSize + 4, 20 * TileSize + 7, 10, 2, p.roadMark);
            for (var y = 1; y < 39; y += 4)
                FillRect(texture, 28 * TileSize + 7, y * TileSize + 4, 2, 10, p.roadMark);
        }

        private static void DrawBuildings(Texture2D texture, Palette p, int year)
        {
            for (var i = 0; i < buildingRects.Length; i++)
            {
                var r = buildingRects[i];
                var wall = i % 2 == 0 ? p.buildingA : p.buildingB;
                DrawBuilding(texture, r.x, r.y, r.width, r.height, wall, p, year, i);
            }

            DrawWorkshopSign(texture, 47, 31, p, year);
            DrawMuseumSign(texture, 35, 32, p);
        }

        private static void DrawBuilding(Texture2D texture, int tx, int ty, int tw, int th, Color32 wall, Palette p, int year, int index)
        {
            var outline = p.outline;
            DrawTileRect(texture, tx, ty, tw, th, outline);
            FillRect(texture, tx * TileSize + 4, ty * TileSize + 4, tw * TileSize - 8, th * TileSize - 8, wall);
            var roof = index % 3 == 0 ? p.roofA : p.roofB;
            FillRect(texture, tx * TileSize + 2, (ty + th - 2) * TileSize, tw * TileSize - 4, TileSize + 8, roof);

            for (var x = tx + 1; x < tx + tw - 1; x += 2)
            {
                var wx = x * TileSize + 4;
                var wy = ty * TileSize + 10;
                FillRect(texture, wx - 1, wy - 1, 10, 10, outline);
                FillRect(texture, wx, wy, 8, 8, p.window);
                FillRect(texture, wx + 1, wy + 5, 6, 2, p.windowShine);
            }

            var doorX = (tx + tw / 2) * TileSize + 3;
            FillRect(texture, doorX - 1, ty * TileSize + 1, 12, 17, outline);
            FillRect(texture, doorX, ty * TileSize + 2, 10, 15, p.door);
            texture.SetPixel(doorX + 7, ty * TileSize + 9, p.accent);

            if (year == 2096)
                FillRect(texture, tx * TileSize + 6, (ty + th) * TileSize - 5, tw * TileSize - 12, 2, p.accent);
        }

        private static void DrawWorkshopSign(Texture2D texture, int tx, int ty, Palette p, int year)
        {
            var x = tx * TileSize + 14;
            var y = ty * TileSize + 58;
            FillRect(texture, x, y, 68, 14, p.outline);
            FillRect(texture, x + 2, y + 2, 64, 10, year == 2096 ? p.accent : p.roofA);
            DrawPixelLetterW(texture, x + 7, y + 4, p.windowShine);
            DrawPixelLetterT(texture, x + 25, y + 4, p.windowShine);
        }

        private static void DrawMuseumSign(Texture2D texture, int tx, int ty, Palette p)
        {
            var x = tx * TileSize + 16;
            var y = ty * TileSize + 56;
            FillRect(texture, x, y, 55, 12, p.outline);
            FillRect(texture, x + 2, y + 2, 51, 8, p.roofB);
            DrawPixelLetterM(texture, x + 6, y + 3, p.windowShine);
        }

        private static void DrawClockPlaza(Texture2D texture, Palette p, int year)
        {
            DrawTileRect(texture, 21, 26, 14, 10, p.plaza);
            for (var x = 21; x < 35; x++)
            for (var y = 26; y < 36; y++)
            {
                if ((x + y) % 2 == 0)
                    FillRect(texture, x * TileSize, y * TileSize, TileSize, TileSize, p.plazaAlt);
            }

            var cx = 28 * TileSize;
            var cy = 31 * TileSize;
            FillRect(texture, cx - 12, cy - 12, 24, 24, p.outline);
            FillRect(texture, cx - 9, cy - 9, 18, 18, p.fountain);
            FillRect(texture, cx - 2, cy - 2, 4, 18, p.outline);
            FillRect(texture, cx - 4, cy + 14, 8, 6, p.clock);
            if (year == 2096) FillRect(texture, cx - 7, cy - 7, 14, 2, p.accent);
        }

        private static void DrawPark(Texture2D texture, Palette p, int year)
        {
            DrawTileRect(texture, 2, 27, 7, 4, p.park);
            var trees = new[] { new Vector2Int(2, 29), new Vector2Int(9, 28), new Vector2Int(18, 27), new Vector2Int(38, 27), new Vector2Int(52, 27), new Vector2Int(6, 14), new Vector2Int(50, 14), new Vector2Int(38, 6) };
            foreach (var t in trees) DrawTree(texture, t.x, t.y, p, year);
        }

        private static void DrawStation(Texture2D texture, Palette p, int year)
        {
            var x = 3 * TileSize;
            var y = 19 * TileSize;
            FillRect(texture, x, y, 90, 5, p.outline);
            for (var i = 0; i < 5; i++) FillRect(texture, x + 8 + i * 17, y + 5, 3, 18, p.post);
            FillRect(texture, x + 4, y + 23, 82, 5, p.roofB);
            if (year == 2096) FillRect(texture, x + 8, y + 26, 74, 2, p.accent);
        }

        private static void DrawStreetProps(Texture2D texture, Palette p, int year)
        {
            var lamps = new[] { new Vector2Int(5, 17), new Vector2Int(18, 17), new Vector2Int(38, 17), new Vector2Int(51, 17), new Vector2Int(5, 24), new Vector2Int(18, 24), new Vector2Int(38, 24), new Vector2Int(51, 24), new Vector2Int(24, 6), new Vector2Int(31, 6), new Vector2Int(24, 37), new Vector2Int(31, 37) };
            foreach (var lamp in lamps)
            {
                var x = lamp.x * TileSize + 7;
                var y = lamp.y * TileSize + 2;
                FillRect(texture, x, y, 2, 11, p.post);
                FillRect(texture, x - 3, y + 9, 8, 4, p.outline);
                FillRect(texture, x - 2, y + 10, 6, 2, year == 2096 ? p.accent : p.lamp);
            }

            for (var x = 34; x < 43; x += 3)
            {
                FillRect(texture, x * TileSize + 3, 16 * TileSize + 8, 10, 4, p.outline);
                FillRect(texture, x * TileSize + 4, 16 * TileSize + 9, 8, 2, p.bench);
            }
        }

        private static void DrawEraDetails(Texture2D texture, Palette p, int year)
        {
            if (year == 1956)
            {
                FillRect(texture, 46 * TileSize, 27 * TileSize, 4, 28, new Color32(94, 70, 48, 255));
                FillRect(texture, 45 * TileSize + 8, 28 * TileSize + 12, 20, 5, new Color32(224, 183, 102, 255));
            }
            else if (year == 2096)
            {
                for (var y = 3; y < 38; y += 7)
                    FillRect(texture, 30 * TileSize + 2, y * TileSize, 2, 18, p.accent);
                FillRect(texture, 45 * TileSize + 4, 28 * TileSize, 12, 4, p.accent);
            }
        }

        private static void DrawTree(Texture2D texture, int tx, int ty, Palette p, int year)
        {
            var x = tx * TileSize + 3;
            var y = ty * TileSize + 1;
            var trunk = year == 2096 ? new Color32(86, 72, 96, 255) : p.trunk;
            FillRect(texture, x + 5, y, 4, 10, trunk);
            FillRect(texture, x + 1, y + 7, 12, 9, Darken(p.tree, 0.22f));
            FillRect(texture, x, y + 11, 14, 7, p.tree);
            DrawRectOutline(texture, x, y + 10, 14, 8, p.outline);
        }

        private static void DrawSidewalk(Texture2D texture, int tx, int ty, int tw, int th, Palette p)
        {
            DrawTileRect(texture, tx, ty, tw, th, p.sidewalk);
            for (var x = tx; x < tx + tw; x++)
            for (var y = ty; y < ty + th; y++)
            {
                if ((x + y) % 2 == 0)
                    FillRect(texture, x * TileSize + 1, y * TileSize + 1, TileSize - 2, TileSize - 2, p.sidewalkAlt);
            }
        }

        private static void DrawPixelLetterW(Texture2D t, int x, int y, Color32 c)
        {
            FillRect(t, x, y, 2, 6, c); FillRect(t, x + 6, y, 2, 6, c); FillRect(t, x + 3, y, 2, 4, c); FillRect(t, x + 1, y, 2, 2, c); FillRect(t, x + 5, y, 2, 2, c);
        }

        private static void DrawPixelLetterT(Texture2D t, int x, int y, Color32 c)
        {
            FillRect(t, x, y + 4, 8, 2, c); FillRect(t, x + 3, y, 2, 6, c);
        }

        private static void DrawPixelLetterM(Texture2D t, int x, int y, Color32 c)
        {
            FillRect(t, x, y, 2, 6, c); FillRect(t, x + 8, y, 2, 6, c); FillRect(t, x + 3, y + 3, 2, 3, c); FillRect(t, x + 6, y + 3, 2, 3, c);
        }

        private static Texture2D NewTexture(int width, int height, string name)
        {
            return new Texture2D(width, height, TextureFormat.RGBA32, false) { name = name, filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
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
            for (var x = 0; x < texture.width; x++)
            {
                var i = y * texture.width + x;
                if (source[i].a != 0) continue;
                var neighbor = false;
                for (var oy = -1; oy <= 1 && !neighbor; oy++)
                for (var ox = -1; ox <= 1; ox++)
                {
                    if (ox == 0 && oy == 0) continue;
                    var nx = x + ox; var ny = y + oy;
                    if (nx < 0 || ny < 0 || nx >= texture.width || ny >= texture.height) continue;
                    if (source[ny * texture.width + nx].a != 0) { neighbor = true; break; }
                }
                if (neighbor) result[i] = color;
            }
            texture.SetPixels32(result);
        }

        private static Color32 Darken(Color32 color, float amount)
        {
            return new Color32((byte)(color.r * (1f - amount)), (byte)(color.g * (1f - amount)), (byte)(color.b * (1f - amount)), color.a);
        }

        private static Palette PaletteFor(int year)
        {
            if (year == 1956)
                return new Palette(new(113, 157, 89, 255), new(93, 141, 78, 255), new(57, 57, 54, 255), new(188, 174, 144, 255), new(168, 154, 128, 255), new(213, 106, 65, 255), new(221, 172, 91, 255), new(171, 86, 53, 255), new(196, 135, 67, 255), new(215, 234, 224, 255), new(242, 249, 242, 255), new(92, 57, 38, 255), new(39, 44, 42, 255), new(75, 67, 55, 255), new(255, 220, 111, 255), new(47, 145, 153, 255), new(48, 117, 60, 255), new(93, 62, 35, 255), new(207, 196, 168, 255), new(194, 181, 151, 255), new(119, 178, 188, 255), new(230, 207, 121, 255), new(105, 78, 48, 255));
            if (year == 2096)
                return new Palette(new(49, 78, 73, 255), new(41, 67, 63, 255), new(24, 25, 35, 255), new(70, 74, 83, 255), new(60, 65, 74, 255), new(81, 56, 110, 255), new(45, 83, 103, 255), new(53, 42, 75, 255), new(37, 69, 92, 255), new(69, 173, 190, 255), new(113, 229, 237, 255), new(72, 59, 82, 255), new(19, 22, 29, 255), new(55, 60, 70, 255), new(80, 232, 224, 255), new(80, 232, 224, 255), new(44, 124, 104, 255), new(76, 69, 88, 255), new(75, 78, 90, 255), new(65, 68, 80, 255), new(48, 121, 132, 255), new(80, 232, 224, 255), new(54, 59, 70, 255));
            return new Palette(new(96, 163, 95, 255), new(80, 145, 80, 255), new(53, 58, 64, 255), new(187, 190, 185, 255), new(158, 163, 159, 255), new(216, 107, 65, 255), new(77, 137, 165, 255), new(164, 74, 50, 255), new(51, 103, 131, 255), new(211, 235, 239, 255), new(241, 250, 252, 255), new(75, 72, 67, 255), new(31, 35, 39, 255), new(67, 73, 78, 255), new(249, 219, 116, 255), new(46, 177, 207, 255), new(43, 128, 69, 255), new(88, 60, 36, 255), new(194, 199, 195, 255), new(176, 183, 179, 255), new(94, 154, 174, 255), new(239, 217, 126, 255), new(78, 84, 88, 255));
        }

        private readonly struct Palette
        {
            public readonly Color32 grass, grassDetail, road, sidewalk, sidewalkAlt, buildingA, buildingB, roofA, roofB, window, windowShine, door, outline, post, lamp, accent, tree, trunk, plaza, plazaAlt, fountain, clock, bench, park;

            public Palette(Color32 grass, Color32 grassDetail, Color32 road, Color32 sidewalk, Color32 sidewalkAlt, Color32 buildingA, Color32 buildingB, Color32 roofA, Color32 roofB, Color32 window, Color32 windowShine, Color32 door, Color32 outline, Color32 post, Color32 lamp, Color32 accent, Color32 tree, Color32 trunk, Color32 plaza, Color32 plazaAlt, Color32 fountain, Color32 clock, Color32 bench)
            {
                this.grass = grass; this.grassDetail = grassDetail; this.road = road; this.sidewalk = sidewalk; this.sidewalkAlt = sidewalkAlt; this.buildingA = buildingA; this.buildingB = buildingB; this.roofA = roofA; this.roofB = roofB; this.window = window; this.windowShine = windowShine; this.door = door; this.outline = outline; this.post = post; this.lamp = lamp; this.accent = accent; this.tree = tree; this.trunk = trunk; this.plaza = plaza; this.plazaAlt = plazaAlt; this.fountain = fountain; this.clock = clock; this.bench = bench; park = grass;
            }
        }
    }
}

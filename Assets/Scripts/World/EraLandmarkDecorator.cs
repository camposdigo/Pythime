using UnityEngine;

namespace Pythime
{
    public static class EraLandmarkDecorator
    {
        public static void Decorate(Transform parent, int year)
        {
            if (parent == null) return;

            if (year == 1956)
            {
                Create(parent, "1956_ConstructionSite", StoryWorldFactory.TileToWorld(42f, 39f), BuildConstructionSite(), 8);
                Create(parent, "1956_OldBillboard", StoryWorldFactory.TileToWorld(15f, 35f), BuildBillboard1956(), 9);
                Create(parent, "1956_PowerPoles", StoryWorldFactory.TileToWorld(51f, 15f), BuildPowerPoles(), 8);
                Create(parent, "1956_WoodCrates", StoryWorldFactory.TileToWorld(22f, 10f), BuildCrates(), 9);
            }
            else if (year == 2096)
            {
                Create(parent, "2096_Hologate", StoryWorldFactory.TileToWorld(40f, 39f), BuildHoloGate(), 10);
                Create(parent, "2096_EnergyPylon", StoryWorldFactory.TileToWorld(15f, 34f), BuildEnergyPylon(), 10);
                Create(parent, "2096_HoloBillboard", StoryWorldFactory.TileToWorld(53f, 17f), BuildBillboard2096(), 11);
                Create(parent, "2096_SyntheticGarden", StoryWorldFactory.TileToWorld(21f, 9f), BuildSyntheticGarden(), 9);
            }
            else
            {
                Create(parent, "2026_BusShelter", StoryWorldFactory.TileToWorld(41f, 25f), BuildBusShelter(), 9);
                Create(parent, "2026_CitySign", StoryWorldFactory.TileToWorld(15f, 35f), BuildCitySign(), 9);
                Create(parent, "2026_Planters", StoryWorldFactory.TileToWorld(51f, 15f), BuildPlanters(), 8);
            }
        }

        private static void Create(Transform parent, string name, Vector2 position, Sprite sprite, int sortingOrder)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.localPosition = position;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
        }

        private static Sprite BuildConstructionSite()
        {
            var t = NewTexture(72, 48, "Construction1956");
            var outline = C(44, 39, 34);
            var wood = C(121, 82, 48);
            var woodLight = C(176, 125, 72);
            var cloth = C(218, 176, 88);
            FillRect(t, 4, 4, 64, 4, outline);
            FillRect(t, 6, 6, 60, 3, wood);
            for (var x = 8; x <= 60; x += 13)
            {
                FillRect(t, x, 8, 4, 32, outline);
                FillRect(t, x + 1, 9, 2, 30, wood);
            }
            FillRect(t, 7, 22, 56, 4, outline);
            FillRect(t, 8, 23, 54, 2, woodLight);
            FillRect(t, 18, 31, 34, 12, outline);
            FillRect(t, 20, 33, 30, 8, cloth);
            FillRect(t, 24, 35, 22, 2, C(238, 207, 129));
            return Finish(t, 0.5f, 0.05f);
        }

        private static Sprite BuildBillboard1956()
        {
            var t = NewTexture(50, 36, "Billboard1956");
            var outline = C(45, 40, 34);
            var red = C(174, 69, 50);
            var cream = C(236, 215, 167);
            FillRect(t, 5, 14, 40, 18, outline);
            FillRect(t, 7, 16, 36, 14, cream);
            FillRect(t, 10, 19, 30, 4, red);
            FillRect(t, 12, 25, 18, 2, C(116, 83, 56));
            FillRect(t, 12, 2, 4, 13, outline);
            FillRect(t, 34, 2, 4, 13, outline);
            return Finish(t, 0.5f, 0.05f);
        }

        private static Sprite BuildPowerPoles()
        {
            var t = NewTexture(58, 50, "PowerPoles1956");
            var outline = C(45, 40, 34);
            var wood = C(96, 65, 42);
            for (var baseX = 8; baseX <= 42; baseX += 34)
            {
                FillRect(t, baseX, 3, 5, 40, outline);
                FillRect(t, baseX + 1, 4, 3, 38, wood);
                FillRect(t, baseX - 7, 35, 19, 4, outline);
                FillRect(t, baseX - 5, 36, 15, 2, wood);
            }
            FillRect(t, 12, 42, 34, 2, C(32, 34, 34));
            FillRect(t, 12, 34, 34, 1, C(32, 34, 34));
            return Finish(t, 0.5f, 0.03f);
        }

        private static Sprite BuildCrates()
        {
            var t = NewTexture(48, 28, "Crates1956");
            var outline = C(47, 39, 31);
            var wood = C(139, 91, 49);
            for (var i = 0; i < 3; i++)
            {
                var x = 3 + i * 14;
                var y = i == 1 ? 8 : 3;
                FillRect(t, x, y, 13, 13, outline);
                FillRect(t, x + 2, y + 2, 9, 9, wood);
                FillRect(t, x + 5, y + 2, 2, 9, C(174, 121, 70));
            }
            return Finish(t, 0.5f, 0.05f);
        }

        private static Sprite BuildBusShelter()
        {
            var t = NewTexture(68, 38, "BusShelter2026");
            var outline = C(30, 35, 40);
            var glass = C(110, 174, 194, 190);
            var metal = C(91, 100, 108);
            FillRect(t, 4, 8, 60, 5, outline);
            FillRect(t, 6, 10, 56, 2, metal);
            FillRect(t, 8, 13, 4, 20, outline);
            FillRect(t, 56, 13, 4, 20, outline);
            FillRect(t, 12, 14, 44, 16, glass);
            FillRect(t, 16, 5, 36, 4, outline);
            FillRect(t, 18, 6, 32, 2, C(48, 140, 166));
            return Finish(t, 0.5f, 0.05f);
        }

        private static Sprite BuildCitySign()
        {
            var t = NewTexture(52, 36, "CitySign2026");
            var outline = C(30, 35, 40);
            var blue = C(54, 120, 170);
            FillRect(t, 5, 14, 42, 17, outline);
            FillRect(t, 7, 16, 38, 13, blue);
            FillRect(t, 11, 20, 30, 2, C(221, 235, 241));
            FillRect(t, 14, 24, 24, 2, C(221, 235, 241));
            FillRect(t, 12, 2, 4, 13, outline);
            FillRect(t, 36, 2, 4, 13, outline);
            return Finish(t, 0.5f, 0.05f);
        }

        private static Sprite BuildPlanters()
        {
            var t = NewTexture(58, 30, "Planters2026");
            var outline = C(31, 36, 40);
            var concrete = C(153, 159, 157);
            var green = C(50, 133, 72);
            for (var i = 0; i < 3; i++)
            {
                var x = 3 + i * 18;
                FillRect(t, x, 3, 16, 8, outline);
                FillRect(t, x + 2, 5, 12, 4, concrete);
                FillRect(t, x + 5, 10, 6, 12, outline);
                FillRect(t, x + 6, 11, 4, 10, green);
                FillRect(t, x + 3, 16, 10, 6, green);
            }
            return Finish(t, 0.5f, 0.05f);
        }

        private static Sprite BuildHoloGate()
        {
            var t = NewTexture(82, 56, "HoloGate2096");
            var outline = C(19, 22, 31);
            var cyan = C(70, 230, 226);
            var violet = C(129, 76, 184);
            FillRect(t, 6, 3, 8, 47, outline);
            FillRect(t, 68, 3, 8, 47, outline);
            FillRect(t, 8, 6, 4, 41, violet);
            FillRect(t, 70, 6, 4, 41, violet);
            FillRect(t, 12, 43, 58, 7, outline);
            FillRect(t, 15, 45, 52, 3, cyan);
            FillRect(t, 22, 27, 38, 3, cyan);
            FillRect(t, 28, 17, 26, 2, C(109, 244, 239));
            return Finish(t, 0.5f, 0.04f);
        }

        private static Sprite BuildEnergyPylon()
        {
            var t = NewTexture(48, 66, "EnergyPylon2096");
            var outline = C(18, 22, 31);
            var cyan = C(69, 231, 226);
            var violet = C(117, 71, 174);
            FillRect(t, 17, 4, 14, 54, outline);
            FillRect(t, 20, 8, 8, 46, violet);
            FillRect(t, 22, 12, 4, 38, cyan);
            FillRect(t, 10, 52, 28, 8, outline);
            FillRect(t, 13, 54, 22, 4, cyan);
            FillRect(t, 6, 2, 36, 5, outline);
            FillRect(t, 10, 3, 28, 2, violet);
            return Finish(t, 0.5f, 0.03f);
        }

        private static Sprite BuildBillboard2096()
        {
            var t = NewTexture(58, 42, "Billboard2096");
            var outline = C(17, 21, 30);
            var cyan = C(68, 230, 226);
            FillRect(t, 4, 14, 50, 22, outline);
            FillRect(t, 7, 17, 44, 16, C(50, 58, 88));
            FillRect(t, 11, 20, 36, 2, cyan);
            FillRect(t, 15, 25, 28, 2, C(164, 108, 224));
            FillRect(t, 9, 30, 40, 1, cyan);
            FillRect(t, 13, 2, 4, 13, outline);
            FillRect(t, 41, 2, 4, 13, outline);
            return Finish(t, 0.5f, 0.04f);
        }

        private static Sprite BuildSyntheticGarden()
        {
            var t = NewTexture(64, 38, "SyntheticGarden2096");
            var outline = C(18, 22, 31);
            var cyan = C(68, 230, 226);
            var green = C(46, 143, 112);
            FillRect(t, 4, 3, 56, 8, outline);
            FillRect(t, 6, 5, 52, 4, C(57, 64, 78));
            for (var i = 0; i < 4; i++)
            {
                var x = 7 + i * 14;
                FillRect(t, x + 4, 10, 3, 16, cyan);
                FillRect(t, x, 18, 11, 8, green);
                FillRect(t, x + 2, 25, 7, 5, C(75, 195, 153));
            }
            return Finish(t, 0.5f, 0.05f);
        }

        private static Texture2D NewTexture(int width, int height, string name)
        {
            var t = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = name
            };
            var clear = new Color32(0, 0, 0, 0);
            var pixels = new Color32[width * height];
            for (var i = 0; i < pixels.Length; i++) pixels[i] = clear;
            t.SetPixels32(pixels);
            return t;
        }

        private static Sprite Finish(Texture2D texture, float pivotX, float pivotY)
        {
            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(pivotX, pivotY), 16f);
            sprite.name = texture.name;
            return sprite;
        }

        private static Color32 C(byte r, byte g, byte b)
        {
            return new Color32(r, g, b, 255);
        }

        private static void FillRect(Texture2D texture, int x, int y, int width, int height, Color32 color)
        {
            for (var py = y; py < y + height; py++)
            for (var px = x; px < x + width; px++)
                if (px >= 0 && py >= 0 && px < texture.width && py < texture.height)
                    texture.SetPixel(px, py, color);
        }
    }
}

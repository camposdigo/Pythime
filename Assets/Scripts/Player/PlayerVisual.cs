using System.Collections.Generic;
using UnityEngine;

namespace Pythime
{
    public sealed class PlayerVisual : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private PlayerController controller;
        private readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

        private int skinIndex = 1;
        private int hairStyleIndex;
        private int hairColorIndex = 1;
        private int shirtIndex = 7;
        private int jacketIndex = 6;
        private int pantsIndex;
        private int shoesIndex;
        private int accessoryIndex;
        private int presetIndex = 1;

        private float walkClock;
        private int walkFrame;
        private Direction facing = Direction.Down;
        private Vector2 lastPosition;
        private float movingGrace;

        private static readonly string[] SkinNames = { "Claro", "Médio", "Bronze", "Marrom", "Escuro" };
        private static readonly Color32[] Skins =
        {
            new Color32(244, 202, 166, 255), new Color32(220, 169, 129, 255), new Color32(190, 133, 91, 255),
            new Color32(139, 89, 59, 255), new Color32(82, 52, 41, 255)
        };

        private static readonly string[] HairStyleNames = { "Curto", "Bagunçado", "Topete", "Lateral", "Cacheado", "Comprido", "Ondulado" };
        private static readonly string[] HairColorNames = { "Preto", "Castanho", "Loiro", "Ruivo", "Rosa", "Azul", "Prata", "Castanho claro" };
        private static readonly Color32[] HairColors =
        {
            new Color32(35, 29, 27, 255), new Color32(91, 50, 30, 255), new Color32(211, 166, 72, 255),
            new Color32(161, 72, 41, 255), new Color32(169, 70, 116, 255), new Color32(54, 93, 156, 255),
            new Color32(184, 186, 191, 255), new Color32(125, 79, 48, 255)
        };

        private static readonly string[] ShirtNames =
        {
            "Azul", "Branca", "Creme", "Verde", "Preta", "Roxa", "Amarela", "Xadrez 85", "Listrada", "Vinho"
        };

        private static readonly Color32[] Shirts =
        {
            new Color32(53, 104, 204, 255), new Color32(226, 226, 218, 255), new Color32(226, 214, 194, 255),
            new Color32(62, 145, 92, 255), new Color32(49, 52, 59, 255), new Color32(126, 75, 168, 255),
            new Color32(224, 161, 55, 255), new Color32(226, 214, 194, 255), new Color32(90, 127, 174, 255),
            new Color32(132, 54, 63, 255)
        };

        private static readonly string[] JacketNames =
        {
            "Sem jaqueta", "Colete vermelho", "Jeans azul", "Jaqueta preta", "Jaleco", "Jaqueta 2096", "Marty: colete + jeans", "Bomber"
        };

        private static readonly string[] PantsNames = { "Jeans azul", "Jeans escuro", "Preta", "Bege", "Cinza", "Marrom", "Roxa 2096" };
        private static readonly Color32[] Pants =
        {
            new Color32(55, 82, 126, 255), new Color32(43, 55, 78, 255), new Color32(40, 43, 50, 255),
            new Color32(143, 124, 91, 255), new Color32(91, 98, 107, 255), new Color32(106, 77, 55, 255),
            new Color32(85, 65, 128, 255)
        };

        private static readonly string[] ShoeNames = { "Tênis branco", "Tênis escuro", "Bota", "Tênis vermelho", "Tênis azul", "Sapato social" };
        private static readonly string[] AccessoryNames =
        {
            "Nenhum", "Mochila", "Boné", "Fones", "Bolsa temporal", "Relógio digital", "Óculos", "Cachecol"
        };

        private static readonly string[] PresetNames =
        {
            "Clássico", "Marty 1985", "Retro 1956", "Future 2096", "Lab Tech", "Street 2026", "Temporal Runner"
        };

        public string SkinName => SkinNames[skinIndex];
        public string HairStyleName => HairStyleNames[hairStyleIndex];
        public string HairColorName => HairColorNames[hairColorIndex];
        public string ShirtName => ShirtNames[shirtIndex];
        public string JacketName => JacketNames[jacketIndex];
        public string PantsName => PantsNames[pantsIndex];
        public string ShoesName => ShoeNames[shoesIndex];
        public string AccessoryName => AccessoryNames[accessoryIndex];
        public string PresetName => PresetNames[presetIndex];
        public int PresetCount => PresetNames.Length;

        public void Initialize(SpriteRenderer target)
        {
            spriteRenderer = target;
            controller = GetComponent<PlayerController>();
            lastPosition = transform.position;
            ApplyPreset(1);
        }

        private void Update()
        {
            UpdateDirectionAndAnimation();
            if (spriteRenderer != null)
                spriteRenderer.sortingOrder = 80 - Mathf.RoundToInt(transform.position.y * 3f);
        }

        private void UpdateDirectionAndAnimation()
        {
            var current = (Vector2)transform.position;
            var travelled = current - lastPosition;
            lastPosition = current;

            if (travelled.sqrMagnitude > 0.000004f) movingGrace = 0.09f;
            else movingGrace = Mathf.Max(0f, movingGrace - Time.deltaTime);

            var input = controller != null ? controller.MoveInput : Vector2.zero;
            if (input.sqrMagnitude > 0.01f)
            {
                if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) facing = input.x > 0f ? Direction.Right : Direction.Left;
                else facing = input.y > 0f ? Direction.Up : Direction.Down;
            }

            if (movingGrace > 0f)
            {
                walkClock += Time.deltaTime;
                if (walkClock >= 0.11f)
                {
                    walkClock = 0f;
                    walkFrame = (walkFrame + 1) % 4;
                    ApplySprite(true);
                }
            }
            else if (walkFrame != 0)
            {
                walkFrame = 0;
                walkClock = 0f;
                ApplySprite(false);
            }
        }

        public void CycleSkin(int direction) { skinIndex = Wrap(skinIndex + direction, Skins.Length); Refresh(); }
        public void CycleHairStyle(int direction) { hairStyleIndex = Wrap(hairStyleIndex + direction, HairStyleNames.Length); Refresh(); }
        public void CycleHairColor(int direction) { hairColorIndex = Wrap(hairColorIndex + direction, HairColors.Length); Refresh(); }
        public void CycleShirt(int direction) { shirtIndex = Wrap(shirtIndex + direction, Shirts.Length); Refresh(); }
        public void CycleJacket(int direction) { jacketIndex = Wrap(jacketIndex + direction, JacketNames.Length); Refresh(); }
        public void CyclePants(int direction) { pantsIndex = Wrap(pantsIndex + direction, Pants.Length); Refresh(); }
        public void CycleShoes(int direction) { shoesIndex = Wrap(shoesIndex + direction, ShoeNames.Length); Refresh(); }
        public void CycleAccessory(int direction) { accessoryIndex = Wrap(accessoryIndex + direction, AccessoryNames.Length); Refresh(); }
        public void CyclePreset(int direction) { ApplyPreset(Wrap(presetIndex + direction, PresetNames.Length)); }

        public void ApplyPreset(int index)
        {
            presetIndex = Wrap(index, PresetNames.Length);

            switch (presetIndex)
            {
                case 0:
                    skinIndex = 1; hairStyleIndex = 0; hairColorIndex = 0; shirtIndex = 0; jacketIndex = 0; pantsIndex = 0; shoesIndex = 0; accessoryIndex = 4;
                    break;
                case 1:
                    skinIndex = 1; hairStyleIndex = 1; hairColorIndex = 1; shirtIndex = 7; jacketIndex = 6; pantsIndex = 0; shoesIndex = 0; accessoryIndex = 5;
                    break;
                case 2:
                    skinIndex = 1; hairStyleIndex = 3; hairColorIndex = 1; shirtIndex = 1; jacketIndex = 3; pantsIndex = 3; shoesIndex = 2; accessoryIndex = 2;
                    break;
                case 3:
                    skinIndex = 1; hairStyleIndex = 2; hairColorIndex = 6; shirtIndex = 5; jacketIndex = 5; pantsIndex = 6; shoesIndex = 1; accessoryIndex = 4;
                    break;
                case 4:
                    skinIndex = 1; hairStyleIndex = 0; hairColorIndex = 1; shirtIndex = 1; jacketIndex = 4; pantsIndex = 4; shoesIndex = 0; accessoryIndex = 4;
                    break;
                case 5:
                    skinIndex = 2; hairStyleIndex = 6; hairColorIndex = 0; shirtIndex = 8; jacketIndex = 2; pantsIndex = 1; shoesIndex = 4; accessoryIndex = 1;
                    break;
                default:
                    skinIndex = 3; hairStyleIndex = 4; hairColorIndex = 5; shirtIndex = 4; jacketIndex = 5; pantsIndex = 2; shoesIndex = 1; accessoryIndex = 4;
                    break;
            }

            Refresh();
        }

        private void Refresh()
        {
            cache.Clear();
            ApplySprite(movingGrace > 0f);
        }

        private void ApplySprite(bool moving)
        {
            if (spriteRenderer == null) return;

            var key = facing + "_" + walkFrame + "_" + skinIndex + "_" + hairStyleIndex + "_" + hairColorIndex + "_" +
                      shirtIndex + "_" + jacketIndex + "_" + pantsIndex + "_" + shoesIndex + "_" + accessoryIndex;

            Sprite sprite;
            if (!cache.TryGetValue(key, out sprite))
            {
                sprite = BuildSprite(facing, walkFrame);
                cache[key] = sprite;
            }

            spriteRenderer.sprite = sprite;
            var bounce = moving && (walkFrame == 1 || walkFrame == 3) ? 0.04f : 0f;
            spriteRenderer.transform.localPosition = new Vector3(0f, bounce, 0f);
        }

        private Sprite BuildSprite(Direction direction, int frame)
        {
            const int width = 24;
            const int height = 32;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = "PythimeAvatar"
            };
            Clear(texture);

            var outline = new Color32(23, 27, 31, 255);
            var skin = Skins[skinIndex];
            var skinDark = Darken(skin, 0.16f);
            var shirt = Shirts[shirtIndex];
            var pants = Pants[pantsIndex];
            var hair = HairColors[hairColorIndex];
            var shoe = ShoeColor();

            var leftStep = frame == 1 ? 1 : frame == 3 ? -1 : 0;
            DrawLeg(texture, 7, 2 + Mathf.Max(0, leftStep), pants, shoe, outline);
            DrawLeg(texture, 13, 2 + Mathf.Max(0, -leftStep), pants, shoe, outline);

            DrawTorso(texture, shirt, outline);
            DrawShirtPattern(texture);
            DrawJacket(texture, outline);

            var armSwing = frame == 1 ? 1 : frame == 3 ? -1 : 0;
            var sleeve = SleeveColor();
            DrawArm(texture, 4, 11 + armSwing, sleeve, skin, outline);
            DrawArm(texture, 17, 11 - armSwing, Darken(sleeve, 0.08f), skinDark, outline);

            FillEllipse(texture, 12, 24, 8, 7, outline);
            FillEllipse(texture, 12, 24, 7, 6, skin);
            if (direction == Direction.Left) FillRect(texture, 5, 22, 2, 4, skinDark);
            if (direction == Direction.Right) FillRect(texture, 17, 22, 2, 4, skin);

            DrawHair(texture, direction, hair, outline);
            DrawFace(texture, direction, outline);
            DrawAccessory(texture, direction, outline);

            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.07f), 16f);
            sprite.name = "Avatar_" + presetIndex + "_" + jacketIndex + "_" + hairStyleIndex;
            return sprite;
        }

        private void DrawTorso(Texture2D t, Color32 shirt, Color32 outline)
        {
            FillRect(t, 7, 9, 10, 9, outline);
            FillRect(t, 6, 11, 12, 5, outline);
            FillRect(t, 8, 10, 8, 7, shirt);
            FillRect(t, 7, 12, 10, 4, shirt);
            FillRect(t, 9, 17, 6, 2, outline);
            FillRect(t, 10, 17, 4, 1, shirt);
        }

        private void DrawShirtPattern(Texture2D t)
        {
            if (shirtIndex == 7)
            {
                var red = new Color32(144, 62, 58, 255);
                var blue = new Color32(67, 91, 120, 255);
                FillRect(t, 8, 11, 8, 1, red);
                FillRect(t, 8, 14, 8, 1, blue);
                FillRect(t, 10, 10, 1, 7, red);
                FillRect(t, 14, 10, 1, 7, blue);
            }
            else if (shirtIndex == 8)
            {
                FillRect(t, 8, 12, 8, 1, Lighten(Shirts[shirtIndex], 0.35f));
                FillRect(t, 8, 15, 8, 1, Darken(Shirts[shirtIndex], 0.18f));
            }
        }

        private void DrawLeg(Texture2D t, int x, int y, Color32 pants, Color32 shoe, Color32 outline)
        {
            FillRect(t, x, y + 2, 4, 7, outline);
            FillRect(t, x + 1, y + 3, 2, 5, pants);
            FillRect(t, x - 1, y, 5, 3, outline);
            FillRect(t, x, y + 1, 4, 1, shoe);
            t.SetPixel(x + 3, y, shoe);
        }

        private void DrawArm(Texture2D t, int x, int y, Color32 sleeve, Color32 skin, Color32 outline)
        {
            FillRect(t, x, y, 3, 7, outline);
            FillRect(t, x + 1, y + 2, 2, 4, sleeve);
            t.SetPixel(x + 1, y + 1, skin);
            t.SetPixel(x + 2, y + 1, skin);
            FillEllipse(t, x + 1, y, 2, 2, outline);
            t.SetPixel(x + 1, y, skin);
        }

        private Color32 SleeveColor()
        {
            if (jacketIndex == 2 || jacketIndex == 6) return new Color32(73, 105, 155, 255);
            if (jacketIndex == 3) return new Color32(43, 45, 51, 255);
            if (jacketIndex == 4) return new Color32(226, 229, 225, 255);
            if (jacketIndex == 5) return new Color32(68, 62, 113, 255);
            if (jacketIndex == 7) return new Color32(95, 78, 61, 255);
            return Shirts[shirtIndex];
        }

        private void DrawJacket(Texture2D t, Color32 outline)
        {
            if (jacketIndex == 0) return;

            if (jacketIndex == 6)
            {
                var denim = new Color32(73, 105, 155, 255);
                var denimLight = new Color32(112, 146, 196, 255);
                var redVest = new Color32(188, 52, 48, 255);
                FillRect(t, 7, 9, 10, 9, outline);
                FillRect(t, 8, 10, 8, 7, redVest);
                FillRect(t, 11, 10, 2, 7, Shirts[shirtIndex]);
                FillRect(t, 7, 11, 2, 5, denim);
                FillRect(t, 15, 11, 2, 5, denim);
                t.SetPixel(8, 15, denimLight);
                t.SetPixel(15, 15, denimLight);
                FillRect(t, 8, 10, 1, 6, Lighten(redVest, 0.12f));
                FillRect(t, 15, 10, 1, 6, Darken(redVest, 0.16f));
                return;
            }

            Color32 jacket;
            if (jacketIndex == 1) jacket = new Color32(183, 54, 52, 255);
            else if (jacketIndex == 2) jacket = new Color32(67, 102, 150, 255);
            else if (jacketIndex == 3) jacket = new Color32(43, 45, 51, 255);
            else if (jacketIndex == 4) jacket = new Color32(226, 229, 225, 255);
            else if (jacketIndex == 5) jacket = new Color32(68, 62, 113, 255);
            else jacket = new Color32(95, 78, 61, 255);

            FillRect(t, 7, 9, 10, 9, outline);
            FillRect(t, 8, 10, 8, 7, jacket);
            FillRect(t, 11, 10, 2, 7, Shirts[shirtIndex]);

            if (jacketIndex == 1)
            {
                FillRect(t, 8, 11, 2, 5, Lighten(jacket, 0.13f));
                FillRect(t, 14, 11, 2, 5, Darken(jacket, 0.16f));
            }
            else if (jacketIndex == 2)
            {
                t.SetPixel(9, 14, Lighten(jacket, 0.22f));
                t.SetPixel(14, 14, Lighten(jacket, 0.22f));
                FillRect(t, 8, 16, 8, 1, Darken(jacket, 0.22f));
            }
            else if (jacketIndex == 5)
            {
                FillRect(t, 8, 10, 8, 1, new Color32(70, 220, 226, 255));
                t.SetPixel(9, 12, new Color32(70, 220, 226, 255));
                t.SetPixel(14, 12, new Color32(70, 220, 226, 255));
            }
        }

        private void DrawHair(Texture2D t, Direction direction, Color32 hair, Color32 outline)
        {
            FillEllipse(t, 12, 28, 8, 4, outline);
            FillEllipse(t, 12, 28, 7, 3, hair);

            if (hairStyleIndex == 0)
            {
                FillRect(t, 6, 25, 3, 3, hair);
            }
            else if (hairStyleIndex == 1)
            {
                FillRect(t, 5, 25, 4, 4, hair);
                FillRect(t, 14, 27, 4, 3, hair);
                t.SetPixel(8, 30, hair);
                t.SetPixel(15, 30, hair);
                t.SetPixel(10, 31, hair);
            }
            else if (hairStyleIndex == 2)
            {
                FillRect(t, 8, 28, 8, 3, hair);
                FillRect(t, 14, 29, 4, 2, hair);
            }
            else if (hairStyleIndex == 3)
            {
                FillRect(t, 5, 24, 3, 5, hair);
                FillRect(t, 15, 26, 3, 3, Darken(hair, 0.18f));
            }
            else if (hairStyleIndex == 4)
            {
                FillEllipse(t, 7, 26, 4, 4, hair);
                FillEllipse(t, 16, 26, 4, 4, hair);
                FillEllipse(t, 11, 29, 4, 3, hair);
            }
            else if (hairStyleIndex == 5)
            {
                FillRect(t, 5, 23, 3, 7, hair);
                FillRect(t, 16, 23, 3, 7, hair);
            }
            else
            {
                FillEllipse(t, 7, 27, 4, 3, hair);
                FillEllipse(t, 12, 29, 5, 3, hair);
                FillEllipse(t, 17, 27, 3, 3, hair);
            }

            if (direction == Direction.Up)
            {
                FillEllipse(t, 12, 25, 7, 6, hair);
                FillRect(t, 7, 23, 10, 4, hair);
            }
        }

        private static void DrawFace(Texture2D t, Direction direction, Color32 outline)
        {
            var white = new Color32(242, 242, 235, 255);
            var iris = new Color32(54, 39, 31, 255);
            var mouth = new Color32(157, 88, 77, 255);

            if (direction == Direction.Down)
            {
                FillRect(t, 8, 23, 3, 2, outline);
                FillRect(t, 13, 23, 3, 2, outline);
                t.SetPixel(8, 24, white);
                t.SetPixel(13, 24, white);
                t.SetPixel(10, 23, iris);
                t.SetPixel(15, 23, iris);
                t.SetPixel(9, 25, Darken(outline, 0.05f));
                t.SetPixel(14, 25, Darken(outline, 0.05f));
                t.SetPixel(11, 20, mouth);
                t.SetPixel(12, 20, mouth);
            }
            else if (direction == Direction.Left)
            {
                FillRect(t, 6, 23, 3, 2, outline);
                t.SetPixel(6, 24, white);
                t.SetPixel(8, 23, iris);
                t.SetPixel(6, 20, mouth);
            }
            else if (direction == Direction.Right)
            {
                FillRect(t, 15, 23, 3, 2, outline);
                t.SetPixel(15, 24, white);
                t.SetPixel(17, 23, iris);
                t.SetPixel(17, 20, mouth);
            }
        }

        private void DrawAccessory(Texture2D t, Direction direction, Color32 outline)
        {
            if (accessoryIndex == 0) return;

            if (accessoryIndex == 1)
            {
                var bag = new Color32(92, 61, 40, 255);
                if (direction == Direction.Up)
                {
                    FillRect(t, 9, 11, 6, 5, outline);
                    FillRect(t, 10, 12, 4, 3, bag);
                }
                else
                {
                    var x = direction == Direction.Left ? 18 : 3;
                    FillRect(t, x, 12, 3, 5, outline);
                    FillRect(t, x + 1, 13, 2, 3, bag);
                }
            }
            else if (accessoryIndex == 2)
            {
                var cap = new Color32(165, 54, 54, 255);
                FillRect(t, 6, 29, 12, 2, outline);
                FillRect(t, 7, 30, 10, 1, cap);
                if (direction == Direction.Down) FillRect(t, 11, 28, 7, 2, cap);
            }
            else if (accessoryIndex == 3)
            {
                var phones = new Color32(55, 60, 69, 255);
                FillRect(t, 5, 23, 2, 4, phones);
                FillRect(t, 17, 23, 2, 4, phones);
            }
            else if (accessoryIndex == 4)
            {
                var chrono = new Color32(66, 219, 230, 255);
                if (direction == Direction.Up)
                {
                    FillRect(t, 10, 12, 4, 3, outline);
                    FillRect(t, 11, 13, 2, 1, chrono);
                }
                else
                {
                    var x = direction == Direction.Left ? 18 : 4;
                    FillRect(t, x, 12, 2, 4, outline);
                    t.SetPixel(x + 1, 13, chrono);
                    t.SetPixel(x + 1, 14, chrono);
                }
            }
            else if (accessoryIndex == 5)
            {
                var watch = new Color32(54, 62, 72, 255);
                FillRect(t, 4, 12, 2, 2, watch);
                t.SetPixel(4, 13, new Color32(76, 225, 232, 255));
            }
            else if (accessoryIndex == 6 && direction != Direction.Up)
            {
                FillRect(t, direction == Direction.Left ? 6 : 8, 23, direction == Direction.Left || direction == Direction.Right ? 3 : 8, 2, outline);
                if (direction == Direction.Down)
                {
                    t.SetPixel(9, 23, new Color32(88, 148, 176, 255));
                    t.SetPixel(14, 23, new Color32(88, 148, 176, 255));
                }
            }
            else if (accessoryIndex == 7)
            {
                var scarf = new Color32(181, 63, 55, 255);
                FillRect(t, 8, 18, 8, 2, outline);
                FillRect(t, 9, 18, 6, 1, scarf);
                FillRect(t, 15, 16, 2, 4, scarf);
            }
        }

        private Color32 ShoeColor()
        {
            if (shoesIndex == 0) return new Color32(231, 231, 225, 255);
            if (shoesIndex == 1) return new Color32(34, 37, 43, 255);
            if (shoesIndex == 2) return new Color32(91, 63, 43, 255);
            if (shoesIndex == 3) return new Color32(180, 53, 52, 255);
            if (shoesIndex == 4) return new Color32(58, 92, 154, 255);
            return new Color32(71, 54, 43, 255);
        }

        private static int Wrap(int value, int length)
        {
            if (length <= 0) return 0;
            value %= length;
            if (value < 0) value += length;
            return value;
        }

        private static void Clear(Texture2D texture)
        {
            var pixels = new Color32[texture.width * texture.height];
            var clear = new Color32(0, 0, 0, 0);
            for (var i = 0; i < pixels.Length; i++) pixels[i] = clear;
            texture.SetPixels32(pixels);
        }

        private static void FillRect(Texture2D texture, int x, int y, int width, int height, Color32 color)
        {
            for (var py = y; py < y + height; py++)
            for (var px = x; px < x + width; px++)
                if (px >= 0 && py >= 0 && px < texture.width && py < texture.height)
                    texture.SetPixel(px, py, color);
        }

        private static void FillEllipse(Texture2D texture, int centerX, int centerY, int radiusX, int radiusY, Color32 color)
        {
            if (radiusX <= 0 || radiusY <= 0) return;
            for (var y = centerY - radiusY; y <= centerY + radiusY; y++)
            {
                for (var x = centerX - radiusX; x <= centerX + radiusX; x++)
                {
                    var nx = (x - centerX) / (float)radiusX;
                    var ny = (y - centerY) / (float)radiusY;
                    if (nx * nx + ny * ny > 1f) continue;
                    if (x >= 0 && y >= 0 && x < texture.width && y < texture.height)
                        texture.SetPixel(x, y, color);
                }
            }
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

        private enum Direction
        {
            Down,
            Up,
            Left,
            Right
        }
    }
}

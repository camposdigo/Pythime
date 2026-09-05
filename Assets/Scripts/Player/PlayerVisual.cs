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
        private int hairColorIndex;
        private int shirtIndex;
        private int jacketIndex;
        private int pantsIndex;
        private int shoesIndex;
        private int accessoryIndex;
        private int presetIndex;

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

        private static readonly string[] HairStyleNames = { "Curto", "Bagunçado", "Topete", "Lateral", "Cacheado", "Comprido" };
        private static readonly string[] HairColorNames = { "Preto", "Castanho", "Loiro", "Ruivo", "Rosa", "Azul", "Prata" };
        private static readonly Color32[] HairColors =
        {
            new Color32(35, 29, 27, 255), new Color32(91, 50, 30, 255), new Color32(211, 166, 72, 255),
            new Color32(161, 72, 41, 255), new Color32(169, 70, 116, 255), new Color32(54, 93, 156, 255), new Color32(184, 186, 191, 255)
        };

        private static readonly string[] ShirtNames = { "Azul", "Branca", "Xadrez clara", "Verde", "Preta", "Roxa", "Amarela" };
        private static readonly Color32[] Shirts =
        {
            new Color32(53, 104, 204, 255), new Color32(226, 226, 218, 255), new Color32(226, 214, 194, 255),
            new Color32(62, 145, 92, 255), new Color32(49, 52, 59, 255), new Color32(126, 75, 168, 255), new Color32(224, 161, 55, 255)
        };

        private static readonly string[] JacketNames = { "Sem jaqueta", "Colete vermelho", "Jeans azul", "Jaqueta preta", "Jaleco", "Jaqueta 2096" };
        private static readonly string[] PantsNames = { "Jeans azul", "Jeans escuro", "Preta", "Bege", "Cinza" };
        private static readonly Color32[] Pants =
        {
            new Color32(55, 82, 126, 255), new Color32(43, 55, 78, 255), new Color32(40, 43, 50, 255),
            new Color32(143, 124, 91, 255), new Color32(91, 98, 107, 255)
        };

        private static readonly string[] ShoeNames = { "Tênis branco", "Tênis escuro", "Bota", "Tênis vermelho" };
        private static readonly string[] AccessoryNames = { "Nenhum", "Mochila", "Boné", "Fones", "Bolsa temporal" };
        private static readonly string[] PresetNames = { "Clássico", "Time Traveler 85", "Retro 1956", "Future 2096", "Lab Tech" };

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
            ApplyPreset(0);
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

            var actuallyMoving = movingGrace > 0f;
            if (actuallyMoving)
            {
                walkClock += Time.deltaTime;
                if (walkClock >= 0.12f)
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
            if (presetIndex == 0)
            {
                hairStyleIndex = 0; hairColorIndex = 0; shirtIndex = 0; jacketIndex = 0; pantsIndex = 0; shoesIndex = 0; accessoryIndex = 4;
            }
            else if (presetIndex == 1)
            {
                hairStyleIndex = 1; hairColorIndex = 1; shirtIndex = 2; jacketIndex = 1; pantsIndex = 0; shoesIndex = 0; accessoryIndex = 0;
            }
            else if (presetIndex == 2)
            {
                hairStyleIndex = 3; hairColorIndex = 1; shirtIndex = 1; jacketIndex = 3; pantsIndex = 3; shoesIndex = 2; accessoryIndex = 2;
            }
            else if (presetIndex == 3)
            {
                hairStyleIndex = 2; hairColorIndex = 6; shirtIndex = 5; jacketIndex = 5; pantsIndex = 4; shoesIndex = 1; accessoryIndex = 4;
            }
            else
            {
                hairStyleIndex = 0; hairColorIndex = 1; shirtIndex = 1; jacketIndex = 4; pantsIndex = 4; shoesIndex = 0; accessoryIndex = 4;
            }
            Refresh();
        }

        private void Refresh()
        {
            cache.Clear();
            ApplySprite(movingGrace > 0f);
        }

        private void ApplySprite(bool moving = false)
        {
            if (spriteRenderer == null) return;
            var key = facing + "_" + walkFrame + "_" + skinIndex + "_" + hairStyleIndex + "_" + hairColorIndex + "_" + shirtIndex + "_" + jacketIndex + "_" + pantsIndex + "_" + shoesIndex + "_" + accessoryIndex;
            Sprite sprite;
            if (!cache.TryGetValue(key, out sprite))
            {
                sprite = BuildSprite(facing, walkFrame);
                cache[key] = sprite;
            }
            spriteRenderer.sprite = sprite;
            spriteRenderer.transform.localPosition = new Vector3(0f, moving && (walkFrame == 1 || walkFrame == 3) ? 0.03f : 0f, 0f);
        }

        private Sprite BuildSprite(Direction direction, int frame)
        {
            const int width = 18;
            const int height = 26;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = "PythimeAvatar"
            };
            Clear(texture);

            var outline = new Color32(24, 27, 31, 255);
            var skin = Skins[skinIndex];
            var skinDark = Darken(skin, 0.18f);
            var hair = HairColors[hairColorIndex];
            var shirt = Shirts[shirtIndex];
            var pants = Pants[pantsIndex];
            var shoe = shoesIndex == 0 ? new Color32(231, 231, 225, 255) : shoesIndex == 1 ? new Color32(34, 37, 43, 255) : shoesIndex == 2 ? new Color32(91, 63, 43, 255) : new Color32(180, 53, 52, 255);

            var legShift = frame == 1 ? 1 : frame == 3 ? -1 : 0;
            DrawLeg(texture, 4, 1 + Mathf.Max(0, legShift), pants, shoe, outline);
            DrawLeg(texture, 10, 1 + Mathf.Max(0, -legShift), pants, shoe, outline);

            FillRect(texture, 4, 7, 10, 8, outline);
            FillRect(texture, 5, 8, 8, 6, shirt);
            DrawJacket(texture, jacketIndex, outline);

            var swing = frame == 1 ? 1 : frame == 3 ? -1 : 0;
            FillRect(texture, 2, 8 + swing, 3, 7, outline);
            FillRect(texture, 3, 9 + swing, 2, 5, skin);
            FillRect(texture, 13, 8 - swing, 3, 7, outline);
            FillRect(texture, 13, 9 - swing, 2, 5, skinDark);

            FillRect(texture, 4, 14, 10, 10, outline);
            FillRect(texture, 5, 15, 8, 8, skin);
            FillRect(texture, 5, 15, 2, 8, skinDark);
            DrawHair(texture, direction, hair, outline);
            DrawFace(texture, direction, outline);
            DrawAccessory(texture, direction, outline);

            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.08f), 16f);
            sprite.name = keyName();
            return sprite;
        }

        private string keyName()
        {
            return "Avatar_" + presetIndex + "_" + jacketIndex + "_" + hairStyleIndex;
        }

        private void DrawLeg(Texture2D t, int x, int y, Color32 pants, Color32 shoe, Color32 outline)
        {
            FillRect(t, x, y, 4, 7, outline);
            FillRect(t, x + 1, y + 2, 2, 4, pants);
            FillRect(t, x, y, 4, 2, shoe);
        }

        private void DrawJacket(Texture2D t, int style, Color32 outline)
        {
            if (style == 0) return;

            Color32 jacket;
            if (style == 1) jacket = new Color32(183, 54, 52, 255);
            else if (style == 2) jacket = new Color32(67, 102, 150, 255);
            else if (style == 3) jacket = new Color32(43, 45, 51, 255);
            else if (style == 4) jacket = new Color32(226, 229, 225, 255);
            else jacket = new Color32(68, 62, 113, 255);

            FillRect(t, 4, 7, 10, 8, outline);
            FillRect(t, 5, 8, 8, 6, jacket);
            FillRect(t, 8, 8, 2, 6, Shirts[shirtIndex]);

            if (style == 1)
            {
                FillRect(t, 5, 9, 2, 5, Lighten(jacket, 0.12f));
                FillRect(t, 11, 9, 2, 5, Darken(jacket, 0.16f));
            }
            else if (style == 2)
            {
                t.SetPixel(6, 12, Lighten(jacket, 0.2f));
                t.SetPixel(11, 12, Lighten(jacket, 0.2f));
                FillRect(t, 5, 13, 8, 1, Darken(jacket, 0.25f));
            }
            else if (style == 5)
            {
                FillRect(t, 5, 8, 8, 1, new Color32(70, 220, 226, 255));
                t.SetPixel(6, 10, new Color32(70, 220, 226, 255));
                t.SetPixel(11, 10, new Color32(70, 220, 226, 255));
            }
        }

        private void DrawHair(Texture2D t, Direction direction, Color32 hair, Color32 outline)
        {
            FillRect(t, 4, 21, 10, 4, outline);
            FillRect(t, 5, 21, 8, 3, hair);

            if (hairStyleIndex == 0) FillRect(t, 5, 19, 3, 3, hair);
            else if (hairStyleIndex == 1) { FillRect(t, 4, 20, 4, 4, hair); FillRect(t, 10, 22, 4, 3, hair); }
            else if (hairStyleIndex == 2) { FillRect(t, 5, 20, 8, 3, hair); FillRect(t, 10, 23, 4, 2, hair); }
            else if (hairStyleIndex == 3) { FillRect(t, 4, 19, 3, 5, hair); FillRect(t, 11, 21, 3, 3, Darken(hair, 0.18f)); }
            else if (hairStyleIndex == 4) { FillRect(t, 4, 19, 4, 5, hair); FillRect(t, 10, 19, 4, 5, hair); }
            else { FillRect(t, 4, 18, 3, 6, hair); FillRect(t, 11, 18, 3, 6, hair); }

            if (direction == Direction.Up) FillRect(t, 5, 18, 8, 5, hair);
        }

        private static void DrawFace(Texture2D t, Direction direction, Color32 outline)
        {
            if (direction == Direction.Down)
            {
                t.SetPixel(7, 19, outline);
                t.SetPixel(11, 19, outline);
                FillRect(t, 8, 16, 3, 1, new Color32(161, 96, 79, 255));
            }
            else if (direction == Direction.Left) t.SetPixel(5, 19, outline);
            else if (direction == Direction.Right) t.SetPixel(12, 19, outline);
        }

        private void DrawAccessory(Texture2D t, Direction direction, Color32 outline)
        {
            if (accessoryIndex == 1)
            {
                var bag = new Color32(92, 61, 40, 255);
                if (direction == Direction.Up) { FillRect(t, 6, 9, 6, 5, outline); FillRect(t, 7, 10, 4, 3, bag); }
                else FillRect(t, direction == Direction.Left ? 13 : 2, 10, 3, 5, bag);
            }
            else if (accessoryIndex == 2)
            {
                var cap = new Color32(165, 54, 54, 255);
                FillRect(t, 4, 22, 10, 2, cap);
                if (direction == Direction.Down) FillRect(t, 7, 21, 6, 2, cap);
            }
            else if (accessoryIndex == 3)
            {
                var phones = new Color32(55, 60, 69, 255);
                FillRect(t, 3, 18, 2, 4, phones);
                FillRect(t, 13, 18, 2, 4, phones);
            }
            else if (accessoryIndex == 4)
            {
                var chrono = new Color32(66, 219, 230, 255);
                if (direction == Direction.Up) FillRect(t, 7, 10, 4, 3, chrono);
                else FillRect(t, direction == Direction.Left ? 13 : 3, 10, 2, 4, chrono);
            }
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

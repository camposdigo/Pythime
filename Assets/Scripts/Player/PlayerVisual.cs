using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pythime
{
    public sealed class PlayerVisual : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D body;
        private int outfitIndex;
        private int hairIndex;
        private float walkClock;
        private int walkFrame;
        private Direction facing = Direction.Down;

        private readonly Dictionary<string, Sprite> cache = new();

        private readonly Color32[] outfits =
        {
            new(47, 105, 213, 255),
            new(204, 67, 62, 255),
            new(52, 156, 102, 255),
            new(137, 80, 188, 255),
            new(225, 149, 44, 255)
        };

        private readonly Color32[] hairs =
        {
            new(38, 28, 24, 255),
            new(91, 45, 25, 255),
            new(205, 154, 65, 255),
            new(53, 36, 30, 255),
            new(151, 67, 103, 255)
        };

        public void Initialize(SpriteRenderer target)
        {
            spriteRenderer = target;
            body = GetComponent<Rigidbody2D>();
            ApplySprite();
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.cKey.wasPressedThisFrame)
                {
                    outfitIndex = (outfitIndex + 1) % outfits.Length;
                    cache.Clear();
                    ApplySprite();
                }

                if (keyboard.hKey.wasPressedThisFrame)
                {
                    hairIndex = (hairIndex + 1) % hairs.Length;
                    cache.Clear();
                    ApplySprite();
                }
            }

            UpdateDirectionAndAnimation();
        }

        private void UpdateDirectionAndAnimation()
        {
            if (body == null) return;
            var velocity = body.linearVelocity;

            if (velocity.sqrMagnitude > 0.02f)
            {
                if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
                    facing = velocity.x > 0 ? Direction.Right : Direction.Left;
                else
                    facing = velocity.y > 0 ? Direction.Up : Direction.Down;

                walkClock += Time.deltaTime;
                if (walkClock >= 0.16f)
                {
                    walkClock = 0f;
                    walkFrame = 1 - walkFrame;
                    ApplySprite();
                }
            }
            else if (walkFrame != 0)
            {
                walkFrame = 0;
                walkClock = 0f;
                ApplySprite();
            }
        }

        private void ApplySprite()
        {
            if (spriteRenderer == null) return;
            var key = $"{facing}_{walkFrame}_{outfitIndex}_{hairIndex}";
            if (!cache.TryGetValue(key, out var sprite))
            {
                sprite = BuildSprite(facing, walkFrame);
                cache[key] = sprite;
            }
            spriteRenderer.sprite = sprite;
        }

        private Sprite BuildSprite(Direction direction, int frame)
        {
            const int width = 16;
            const int height = 24;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = $"Avatar_{direction}_{frame}_{outfitIndex}_{hairIndex}"
            };

            var pixels = new Color32[width * height];
            var clear = new Color32(0, 0, 0, 0);
            for (var i = 0; i < pixels.Length; i++) pixels[i] = clear;
            texture.SetPixels32(pixels);

            var outline = new Color32(28, 31, 34, 255);
            var skin = new Color32(218, 166, 128, 255);
            var shirt = outfits[outfitIndex];
            var shirtDark = Darken(shirt, 0.28f);
            var hair = hairs[hairIndex];
            var pants = new Color32(55, 61, 72, 255);
            var shoes = new Color32(31, 33, 38, 255);
            var device = new Color32(55, 210, 224, 255);

            FillRect(texture, 4, 12, 8, 9, outline);
            FillRect(texture, 5, 13, 6, 7, skin);

            if (direction == Direction.Up)
            {
                FillRect(texture, 5, 17, 6, 4, hair);
                FillRect(texture, 4, 19, 8, 3, outline);
                FillRect(texture, 5, 19, 6, 2, hair);
            }
            else
            {
                FillRect(texture, 4, 19, 8, 3, outline);
                FillRect(texture, 5, 19, 6, 2, hair);
                FillRect(texture, 5, 18, 2, 2, hair);

                if (direction == Direction.Down)
                {
                    texture.SetPixel(6, 16, outline);
                    texture.SetPixel(9, 16, outline);
                }
                else if (direction == Direction.Left)
                {
                    texture.SetPixel(5, 16, outline);
                }
                else
                {
                    texture.SetPixel(10, 16, outline);
                }
            }

            FillRect(texture, 4, 6, 8, 7, outline);
            FillRect(texture, 5, 7, 6, 5, shirt);
            FillRect(texture, 5, 7, 2, 5, shirtDark);

            if (direction == Direction.Left)
                FillRect(texture, 11, 8, 2, 4, device);
            else
                FillRect(texture, 3, 8, 2, 4, device);

            var leftLegY = frame == 0 ? 2 : 3;
            var rightLegY = frame == 0 ? 3 : 2;
            FillRect(texture, 4, leftLegY, 4, 5, outline);
            FillRect(texture, 5, leftLegY + 1, 2, 3, pants);
            FillRect(texture, 8, rightLegY, 4, 5, outline);
            FillRect(texture, 9, rightLegY + 1, 2, 3, pants);
            FillRect(texture, 4, leftLegY, 4, 2, shoes);
            FillRect(texture, 8, rightLegY, 4, 2, shoes);

            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.12f), 16f);
            sprite.name = texture.name;
            return sprite;
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
            return new Color32(
                (byte)(color.r * (1f - amount)),
                (byte)(color.g * (1f - amount)),
                (byte)(color.b * (1f - amount)),
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

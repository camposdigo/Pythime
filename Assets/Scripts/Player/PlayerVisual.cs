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

        private readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

        private readonly Color32[] outfits =
        {
            new Color32(47, 105, 213, 255),
            new Color32(204, 67, 62, 255),
            new Color32(52, 156, 102, 255),
            new Color32(137, 80, 188, 255),
            new Color32(225, 149, 44, 255)
        };

        private readonly Color32[] hairs =
        {
            new Color32(38, 28, 24, 255),
            new Color32(91, 45, 25, 255),
            new Color32(205, 154, 65, 255),
            new Color32(53, 36, 30, 255),
            new Color32(151, 67, 103, 255)
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
            if (spriteRenderer != null)
                spriteRenderer.sortingOrder = 80 - Mathf.RoundToInt(transform.position.y * 3f);
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
                if (walkClock >= 0.13f)
                {
                    walkClock = 0f;
                    walkFrame = (walkFrame + 1) % 4;
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
            var key = facing + "_" + walkFrame + "_" + outfitIndex + "_" + hairIndex;
            Sprite sprite;
            if (!cache.TryGetValue(key, out sprite))
            {
                sprite = BuildSprite(facing, walkFrame);
                cache[key] = sprite;
            }
            spriteRenderer.sprite = sprite;
            spriteRenderer.transform.localPosition = new Vector3(0f, walkFrame == 1 || walkFrame == 3 ? 0.025f : 0f, 0f);
        }

        private Sprite BuildSprite(Direction direction, int frame)
        {
            const int width = 20;
            const int height = 28;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = "Avatar_" + direction + "_" + frame + "_" + outfitIndex + "_" + hairIndex
            };

            var pixels = new Color32[width * height];
            var clear = new Color32(0, 0, 0, 0);
            for (var i = 0; i < pixels.Length; i++) pixels[i] = clear;
            texture.SetPixels32(pixels);

            var outline = new Color32(25, 29, 34, 255);
            var skin = new Color32(220, 169, 129, 255);
            var skinDark = new Color32(190, 137, 102, 255);
            var shirt = outfits[outfitIndex];
            var shirtDark = Darken(shirt, 0.25f);
            var shirtLight = Lighten(shirt, 0.18f);
            var hair = hairs[hairIndex];
            var hairDark = Darken(hair, 0.25f);
            var pants = new Color32(52, 59, 72, 255);
            var shoes = new Color32(28, 31, 37, 255);
            var device = new Color32(57, 214, 230, 255);

            var step = frame == 1 ? -1 : frame == 3 ? 1 : 0;
            var leftLegY = 2 + Mathf.Max(0, step);
            var rightLegY = 2 + Mathf.Max(0, -step);

            FillRect(texture, 5, leftLegY, 5, 7, outline);
            FillRect(texture, 6, leftLegY + 2, 3, 4, pants);
            FillRect(texture, 10, rightLegY, 5, 7, outline);
            FillRect(texture, 11, rightLegY + 2, 3, 4, pants);
            FillRect(texture, 5, leftLegY, 5, 2, shoes);
            FillRect(texture, 10, rightLegY, 5, 2, shoes);

            FillRect(texture, 4, 8, 12, 10, outline);
            FillRect(texture, 5, 9, 10, 8, shirt);
            FillRect(texture, 5, 9, 2, 8, shirtDark);
            FillRect(texture, 7, 15, 6, 2, shirtLight);

            var armSwing = frame == 1 ? 1 : frame == 3 ? -1 : 0;
            FillRect(texture, 2, 10 + armSwing, 3, 7, outline);
            FillRect(texture, 3, 11 + armSwing, 2, 5, skin);
            FillRect(texture, 15, 10 - armSwing, 3, 7, outline);
            FillRect(texture, 15, 11 - armSwing, 2, 5, skinDark);

            FillRect(texture, 5, 16, 10, 10, outline);
            FillRect(texture, 6, 17, 8, 8, skin);
            FillRect(texture, 6, 17, 2, 8, skinDark);

            DrawHair(texture, direction, hair, hairDark, outline);

            if (direction == Direction.Down)
            {
                texture.SetPixel(8, 21, outline);
                texture.SetPixel(12, 21, outline);
                texture.SetPixel(10, 18, new Color32(163, 103, 82, 255));
                FillRect(texture, 9, 19, 3, 1, new Color32(190, 116, 93, 255));
            }
            else if (direction == Direction.Left)
            {
                texture.SetPixel(6, 21, outline);
                texture.SetPixel(6, 19, new Color32(164, 104, 83, 255));
            }
            else if (direction == Direction.Right)
            {
                texture.SetPixel(13, 21, outline);
                texture.SetPixel(13, 19, new Color32(164, 104, 83, 255));
            }

            if (direction == Direction.Up)
            {
                FillRect(texture, 7, 10, 6, 5, shirtDark);
                FillRect(texture, 8, 11, 4, 3, device);
                texture.SetPixel(9, 12, new Color32(220, 250, 255, 255));
            }
            else if (direction == Direction.Left)
            {
                FillRect(texture, 15, 11, 2, 5, device);
            }
            else
            {
                FillRect(texture, 3, 11, 2, 5, device);
            }

            texture.Apply(false, false);
            var sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.10f), 16f);
            sprite.name = texture.name;
            return sprite;
        }

        private void DrawHair(Texture2D texture, Direction direction, Color32 hair, Color32 hairDark, Color32 outline)
        {
            if (direction == Direction.Up)
            {
                FillRect(texture, 5, 22, 10, 5, outline);
                FillRect(texture, 6, 22, 8, 4, hair);
                FillRect(texture, 6, 22, 2, 4, hairDark);
                return;
            }

            FillRect(texture, 5, 23, 10, 4, outline);
            FillRect(texture, 6, 23, 8, 3, hair);

            if (hairIndex == 0)
            {
                FillRect(texture, 6, 21, 3, 3, hair);
            }
            else if (hairIndex == 1)
            {
                FillRect(texture, 6, 20, 4, 4, hair);
                FillRect(texture, 12, 22, 2, 3, hairDark);
            }
            else if (hairIndex == 2)
            {
                FillRect(texture, 6, 21, 8, 3, hair);
                FillRect(texture, 5, 20, 3, 3, hair);
            }
            else if (hairIndex == 3)
            {
                FillRect(texture, 6, 20, 3, 4, hairDark);
                FillRect(texture, 11, 21, 3, 3, hair);
            }
            else
            {
                FillRect(texture, 5, 21, 4, 4, hair);
                FillRect(texture, 11, 20, 4, 5, hair);
            }
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

using System.Collections;
using UnityEngine;

namespace Pythime
{
    public sealed class HandmadePlayerVisualRuntime : MonoBehaviour
    {
        private enum Facing
        {
            Down,
            Up,
            Left,
            Right
        }

        private SpriteRenderer avatar;
        private PlayerController controller;
        private Sprite[] idle;
        private Sprite[] walkDown;
        private Sprite[] walkUp;
        private Sprite[] walkLeft;
        private Sprite[] walkRight;
        private Facing facing = Facing.Down;
        private Vector2 lastPosition;
        private float moveGrace;
        private float frameClock;
        private int frame;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (!EmbeddedArtLoader.HasAsset("player_marty_sheet")) return;
            if (GameObject.Find("PythimeHandmadePlayerVisual") != null) return;

            var root = new GameObject("PythimeHandmadePlayerVisual");
            root.AddComponent<HandmadePlayerVisualRuntime>();
        }

        private IEnumerator Start()
        {
            for (var i = 0; i < 180; i++)
            {
                var player = GameObject.Find("Player");
                if (player != null)
                {
                    controller = player.GetComponent<PlayerController>();
                    var avatarTransform = player.transform.Find("Avatar");
                    if (avatarTransform != null)
                        avatar = avatarTransform.GetComponent<SpriteRenderer>();

                    if (avatar != null && BuildFrames())
                    {
                        var procedural = player.GetComponent<PlayerVisual>();
                        if (procedural != null) procedural.enabled = false;

                        var runtime = GameObject.Find("PythimeRuntime");
                        if (runtime != null)
                        {
                            var customizer = runtime.GetComponent<CharacterCustomizerOverlay>();
                            if (customizer != null) customizer.enabled = false;
                        }

                        avatar.transform.localPosition = Vector3.zero;
                        avatar.transform.localScale = Vector3.one;
                        lastPosition = player.transform.position;
                        ApplySprite(false);
                        yield break;
                    }
                }
                yield return null;
            }
        }

        private bool BuildFrames()
        {
            var sheet = EmbeddedArtLoader.LoadTexture("player_marty_sheet");
            if (sheet == null || sheet.width < 96 || sheet.height < 180) return false;

            idle = new Sprite[4];
            walkDown = new Sprite[4];
            walkUp = new Sprite[4];
            walkLeft = new Sprite[4];
            walkRight = new Sprite[4];

            const int cellWidth = 24;
            const int cellHeight = 36;
            const float ppu = 14.5f;

            for (var column = 0; column < 4; column++)
            {
                idle[column] = Slice(sheet, column, 4, cellWidth, cellHeight, ppu, "Idle_" + column);
                walkDown[column] = Slice(sheet, column, 3, cellWidth, cellHeight, ppu, "Down_" + column);
                walkUp[column] = Slice(sheet, column, 2, cellWidth, cellHeight, ppu, "Up_" + column);
                walkLeft[column] = Slice(sheet, column, 1, cellWidth, cellHeight, ppu, "Left_" + column);
                walkRight[column] = Slice(sheet, column, 0, cellWidth, cellHeight, ppu, "Right_" + column);
            }

            return true;
        }

        private static Sprite Slice(Texture2D sheet, int column, int rowFromBottom, int width, int height, float ppu, string name)
        {
            var rect = new Rect(column * width, rowFromBottom * height, width, height);
            var sprite = Sprite.Create(sheet, rect, new Vector2(0.5f, 0.08f), ppu, 0, SpriteMeshType.FullRect);
            sprite.name = "Marty_" + name;
            return sprite;
        }

        private void Update()
        {
            if (avatar == null || controller == null) return;

            var playerPosition = (Vector2)controller.transform.position;
            var travelled = playerPosition - lastPosition;
            lastPosition = playerPosition;

            if (travelled.sqrMagnitude > 0.000004f) moveGrace = 0.11f;
            else moveGrace = Mathf.Max(0f, moveGrace - Time.deltaTime);

            var input = controller.MoveInput;
            if (input.sqrMagnitude > 0.01f)
            {
                if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                    facing = input.x < 0f ? Facing.Left : Facing.Right;
                else
                    facing = input.y > 0f ? Facing.Up : Facing.Down;
            }

            if (moveGrace > 0f)
            {
                frameClock += Time.deltaTime;
                if (frameClock >= 0.115f)
                {
                    frameClock = 0f;
                    frame = (frame + 1) % 4;
                    ApplySprite(true);
                }
            }
            else
            {
                if (frame != 0 || frameClock != 0f)
                {
                    frame = 0;
                    frameClock = 0f;
                    ApplySprite(false);
                }
                else
                {
                    ApplySprite(false);
                }
            }

            avatar.sortingOrder = 90 - Mathf.RoundToInt(controller.transform.position.y * 3f);
        }

        private void ApplySprite(bool moving)
        {
            if (avatar == null || idle == null) return;

            if (!moving)
            {
                avatar.sprite = facing switch
                {
                    Facing.Down => idle[0],
                    Facing.Up => idle[1],
                    Facing.Left => idle[2],
                    Facing.Right => idle[3],
                    _ => idle[0]
                };
                return;
            }

            var index = Mathf.Clamp(frame, 0, 3);
            avatar.sprite = facing switch
            {
                Facing.Down => walkDown[index],
                Facing.Up => walkUp[index],
                Facing.Left => walkLeft[index],
                Facing.Right => walkRight[index],
                _ => walkDown[index]
            };
        }
    }
}

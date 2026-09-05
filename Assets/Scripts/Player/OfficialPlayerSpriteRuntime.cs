using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pythime
{
    [DefaultExecutionOrder(-2000)]
    public sealed class OfficialPlayerSpriteRuntime : MonoBehaviour
    {
        private const string ResourcePath = "Characters/player_marty_sheet";
        private const int Columns = 4;
        private const int Rows = 5;
        private const float PixelsPerUnit = 20f;
        private const float FrameDuration = 0.115f;

        private readonly Dictionary<string, Sprite> frames = new Dictionary<string, Sprite>();
        private SpriteRenderer target;
        private PlayerController controller;
        private Transform player;
        private Vector2 lastPosition;
        private float moveGrace;
        private float animationClock;
        private int walkFrame;
        private Direction facing = Direction.Down;
        private bool usingOfficialSheet;

        public bool UsingOfficialSheet => usingOfficialSheet;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (GameObject.Find("PythimeOfficialPlayer") != null) return;
            var root = new GameObject("PythimeOfficialPlayer");
            root.AddComponent<OfficialPlayerSpriteRuntime>();
        }

        private IEnumerator Start()
        {
            for (var frame = 0; frame < 180; frame++)
            {
                var playerObject = GameObject.Find("Player");
                if (playerObject != null)
                {
                    var avatar = playerObject.transform.Find("Avatar");
                    if (avatar != null)
                    {
                        player = playerObject.transform;
                        target = avatar.GetComponent<SpriteRenderer>();
                        controller = playerObject.GetComponent<PlayerController>();
                        break;
                    }
                }
                yield return null;
            }

            if (player == null || target == null)
            {
                Destroy(gameObject);
                yield break;
            }

            var source = LoadSource();
            if (!source.IsValid)
            {
                Debug.LogWarning("Pythime: player_marty_sheet.png não encontrado. Caminho esperado: Assets/Resources/Characters/player_marty_sheet.png");
                Destroy(gameObject);
                yield break;
            }

            source.Texture.filterMode = FilterMode.Point;
            source.Texture.wrapMode = TextureWrapMode.Clamp;

            BuildFrames(source);
            usingOfficialSheet = frames.Count == 20;

            if (!usingOfficialSheet)
            {
                Debug.LogWarning("Pythime: player_marty_sheet.png foi encontrado, mas não consegui montar os 20 frames. Mantendo fallback temporário.");
                Destroy(gameObject);
                yield break;
            }

            var procedural = player.GetComponent<PlayerVisual>();
            if (procedural != null) procedural.enabled = false;

            var eyePatch = GameObject.Find("PythimePlayerEyeStyle");
            if (eyePatch != null) eyePatch.SetActive(false);

            var runtime = GameObject.Find("PythimeRuntime");
            if (runtime != null)
            {
                var customizer = runtime.GetComponent<CharacterCustomizerOverlay>();
                if (customizer != null) customizer.enabled = false;
            }

            target.transform.localScale = Vector3.one * 1.55f;
            lastPosition = player.position;
            ApplyIdle();
        }

        private static SheetSource LoadSource()
        {
            var sprite = Resources.Load<Sprite>(ResourcePath);
            if (sprite != null && sprite.texture != null)
                return new SheetSource(sprite.texture, sprite.textureRect);

            var texture = Resources.Load<Texture2D>(ResourcePath);
            if (texture != null)
                return new SheetSource(texture, new Rect(0f, 0f, texture.width, texture.height));

            return default;
        }

        private void Update()
        {
            if (!usingOfficialSheet || target == null || player == null) return;

            var current = (Vector2)player.position;
            var travelled = current - lastPosition;
            lastPosition = current;

            if (travelled.sqrMagnitude > 0.000004f) moveGrace = 0.10f;
            else moveGrace = Mathf.Max(0f, moveGrace - Time.deltaTime);

            var input = controller != null ? controller.MoveInput : Vector2.zero;
            if (input.sqrMagnitude > 0.01f)
            {
                if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                    facing = input.x > 0f ? Direction.Right : Direction.Left;
                else
                    facing = input.y > 0f ? Direction.Up : Direction.Down;
            }

            if (moveGrace > 0f)
            {
                animationClock += Time.deltaTime;
                if (animationClock >= FrameDuration)
                {
                    animationClock -= FrameDuration;
                    walkFrame = (walkFrame + 1) % Columns;
                    ApplyWalk();
                }
            }
            else
            {
                animationClock = 0f;
                walkFrame = 0;
                ApplyIdle();
            }

            target.sortingOrder = 80 - Mathf.RoundToInt(player.position.y * 3f);
        }

        private void BuildFrames(SheetSource source)
        {
            frames.Clear();
            var cellWidth = source.Rect.width / Columns;
            var cellHeight = source.Rect.height / Rows;

            if (cellWidth < 8f || cellHeight < 8f) return;

            for (var rowFromTop = 0; rowFromTop < Rows; rowFromTop++)
            {
                for (var column = 0; column < Columns; column++)
                {
                    var x = Mathf.RoundToInt(source.Rect.x + column * cellWidth);
                    var y = Mathf.RoundToInt(source.Rect.y + source.Rect.height - (rowFromTop + 1) * cellHeight);
                    var w = Mathf.RoundToInt(cellWidth);
                    var h = Mathf.RoundToInt(cellHeight);

                    if (x < 0 || y < 0 || x + w > source.Texture.width || y + h > source.Texture.height)
                        continue;

                    var sprite = Sprite.Create(
                        source.Texture,
                        new Rect(x, y, w, h),
                        new Vector2(0.5f, 0.08f),
                        PixelsPerUnit,
                        0,
                        SpriteMeshType.FullRect);
                    sprite.name = $"Marty_r{rowFromTop}_c{column}";
                    frames[$"{rowFromTop}:{column}"] = sprite;
                }
            }
        }

        private void ApplyIdle()
        {
            if (target == null) return;
            var column = facing == Direction.Down ? 0 : facing == Direction.Up ? 1 : facing == Direction.Left ? 2 : 3;
            target.sprite = GetFrame(0, column);
            target.transform.localPosition = Vector3.zero;
        }

        private void ApplyWalk()
        {
            if (target == null) return;
            var row = facing == Direction.Down ? 1 : facing == Direction.Up ? 2 : facing == Direction.Left ? 3 : 4;
            target.sprite = GetFrame(row, walkFrame);
            target.transform.localPosition = Vector3.zero;
        }

        private Sprite GetFrame(int row, int column)
        {
            Sprite sprite;
            return frames.TryGetValue($"{row}:{column}", out sprite) ? sprite : null;
        }

        private readonly struct SheetSource
        {
            public readonly Texture2D Texture;
            public readonly Rect Rect;
            public bool IsValid => Texture != null && Rect.width > 0f && Rect.height > 0f;

            public SheetSource(Texture2D texture, Rect rect)
            {
                Texture = texture;
                Rect = rect;
            }
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

using System.Collections.Generic;
using UnityEngine;

namespace Pythime
{
    public sealed class OfficialPlayerSpriteRuntime : MonoBehaviour
    {
        private const string ResourcePath = "Characters/player_marty_sheet";
        private const int Columns = 4;
        private const int Rows = 5;
        private const float PixelsPerUnit = 28f;
        private const float FrameDuration = 0.115f;

        private readonly Dictionary<string, Sprite> frames = new Dictionary<string, Sprite>();
        private SpriteRenderer target;
        private PlayerController controller;
        private Vector2 lastPosition;
        private float moveGrace;
        private float animationClock;
        private int walkFrame;
        private Direction facing = Direction.Down;
        private bool usingOfficialSheet;

        public bool UsingOfficialSheet => usingOfficialSheet;

        public void Initialize(SpriteRenderer renderer)
        {
            target = renderer;
            controller = GetComponent<PlayerController>();
            lastPosition = transform.position;

            var texture = Resources.Load<Texture2D>(ResourcePath);
            if (texture == null || texture.width < Columns || texture.height < Rows)
            {
                EnableProceduralFallback();
                return;
            }

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            BuildFrames(texture);
            usingOfficialSheet = frames.Count == 20;

            if (!usingOfficialSheet)
            {
                EnableProceduralFallback();
                return;
            }

            ApplyIdle();
        }

        private void Update()
        {
            if (!usingOfficialSheet || target == null) return;

            var current = (Vector2)transform.position;
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

            target.sortingOrder = 80 - Mathf.RoundToInt(transform.position.y * 3f);
        }

        private void BuildFrames(Texture2D texture)
        {
            frames.Clear();
            var cellWidth = texture.width / Columns;
            var cellHeight = texture.height / Rows;

            for (var rowFromTop = 0; rowFromTop < Rows; rowFromTop++)
            {
                for (var column = 0; column < Columns; column++)
                {
                    var y = texture.height - (rowFromTop + 1) * cellHeight;
                    var rect = new Rect(column * cellWidth, y, cellWidth, cellHeight);
                    var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.06f), PixelsPerUnit, 0, SpriteMeshType.FullRect);
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

        private void EnableProceduralFallback()
        {
            usingOfficialSheet = false;
            var fallback = GetComponent<PlayerVisual>();
            if (fallback == null) fallback = gameObject.AddComponent<PlayerVisual>();
            fallback.Initialize(target);
            Debug.LogWarning("Pythime: player_marty_sheet.png não encontrado em Assets/Resources/Characters. Usando o visual procedural temporariamente.");
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

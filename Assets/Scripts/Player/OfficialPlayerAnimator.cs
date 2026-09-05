using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pythime
{
    // Runs after PlayerController.Update so a direction change is visible this frame.
    [DefaultExecutionOrder(100)]
    public sealed class OfficialPlayerAnimator : MonoBehaviour
    {
        public const string ResourcePath = "Characters/player_marty_sheet";
        public const string AssetPath = "Assets/Resources/Characters/player_marty_sheet.png";
        public const float CharacterHeight = 2.1f;
        private readonly Sprite[,] frames = new Sprite[5, 4];
        private Texture2D sheet;
        private SpriteRenderer target;
        private PlayerController controller;
        private int facing;
        private float clock;
        private bool whiteBackground;
        public bool UsingOfficialSheet { get; private set; }

        public void Initialize(SpriteRenderer renderer)
        {
            target = renderer;
            controller = GetComponent<PlayerController>();
            target.sprite = null;
            try
            {
#if UNITY_EDITOR
                OfficialPngValidation.ValidateFile(AssetPath);
#endif
                var importedSprite = Resources.Load<Sprite>(ResourcePath);
                var source = importedSprite != null ? importedSprite.texture : Resources.Load<Texture2D>(ResourcePath);
                if (source == null) throw new InvalidOperationException("A textura não foi importada em Resources. Reimporte o PNG oficial.");
                BuildFrames(source);
                target.transform.localPosition = Vector3.zero;
                target.transform.localScale = Vector3.one;
                target.color = Color.white;
                UsingOfficialSheet = true;
                ApplyFrame(Vector2.zero, 0f);
            }
            catch (Exception ex)
            {
                Debug.LogError(OfficialPngValidation.DescribeFile(AssetPath, ex.Message), this);
                enabled = false;
            }
        }

        private void LateUpdate()
        {
            if (UsingOfficialSheet) ApplyFrame(controller.MoveInput, Time.deltaTime);
        }

        public void ApplyFrame(Vector2 input, float deltaTime)
        {
            if (!UsingOfficialSheet) return;
            bool moving = input.sqrMagnitude > 0.01f;
            if (moving)
            {
                int direction = Mathf.Abs(input.x) > Mathf.Abs(input.y)
                    ? (input.x > 0f ? 3 : 2) : (input.y > 0f ? 1 : 0);
                if (direction != facing) clock = 0f;
                facing = direction;
                clock += deltaTime;
            }
            else clock = 0f;

            // The supplied white-backed artwork repeats left-facing art in idle column 4.
            // Mirror idle-left for right; the dedicated right-walk row needs no mirroring.
            target.flipX = !moving && whiteBackground && facing == 3;
            target.sprite = moving ? frames[facing + 1, (int)(clock / 0.115f) % 4]
                : frames[0, target.flipX ? 2 : facing];
            target.sortingOrder = 80 - Mathf.RoundToInt(transform.position.y * 3f);
        }

        private void BuildFrames(Texture2D source)
        {
            var pixels = source.GetPixels32();
            int w = source.width, h = source.height;
            whiteBackground = IsWhite(pixels[0]) && IsWhite(pixels[w - 1])
                && IsWhite(pixels[(h - 1) * w]) && IsWhite(pixels[w * h - 1]);
            if (whiteBackground) ClearConnectedBackground(pixels, w, h);
            sheet = new Texture2D(w, h, TextureFormat.RGBA32, false)
            {
                name = "Marty_RuntimeSheet", filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp
            };
            sheet.SetPixels32(pixels);
            sheet.Apply(false, false);

            // Scan occupied bands rather than dividing the image including its wide margins.
            var occupiedRows = new bool[h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    if (pixels[y * w + x].a > 128) occupiedRows[y] = true;
            var rows = Bands(occupiedRows);
            if (rows.Count != 5) throw new InvalidOperationException($"Esperadas 5 linhas separadas; encontradas {rows.Count}.");
            var rects = new RectInt[5, 4];
            int maxHeight = 0;
            for (int row = 0; row < 5; row++)
            {
                var band = rows[4 - row]; // Unity pixels start at bottom-left.
                var occupiedColumns = new bool[w];
                for (int y = band.x; y < band.y; y++)
                    for (int x = 0; x < w; x++)
                        if (pixels[y * w + x].a > 128) occupiedColumns[x] = true;
                var columns = Bands(occupiedColumns);
                if (columns.Count != 4) throw new InvalidOperationException($"Linha {row + 1}: esperadas 4 colunas; encontradas {columns.Count}.");
                for (int col = 0; col < 4; col++)
                {
                    int bottom = h, top = 0;
                    for (int y = band.x; y < band.y; y++)
                        for (int x = columns[col].x; x < columns[col].y; x++)
                            if (pixels[y * w + x].a > 128) { bottom = Mathf.Min(bottom, y); top = Mathf.Max(top, y + 1); }
                    rects[row, col] = new RectInt(columns[col].x, bottom, columns[col].y - columns[col].x, top - bottom);
                    maxHeight = Mathf.Max(maxHeight, top - bottom);
                }
            }
            float ppu = maxHeight / CharacterHeight;
            for (int row = 0; row < 5; row++)
                for (int col = 0; col < 4; col++)
                {
                    var r = rects[row, col];
                    frames[row, col] = Sprite.Create(sheet, new Rect(r.x, r.y, r.width, r.height), new Vector2(0.5f, 0.02f), ppu, 0, SpriteMeshType.FullRect);
                    frames[row, col].name = $"Marty_r{row}_c{col}";
                }
        }

        private static bool IsWhite(Color32 c) => c.a < 128 || (c.r >= 235 && c.g >= 235 && c.b >= 235);

        private static void ClearConnectedBackground(Color32[] pixels, int w, int h)
        {
            var visited = new bool[pixels.Length];
            var queue = new Queue<int>();
            void Enqueue(int index)
            {
                if (visited[index] || !IsWhite(pixels[index])) return;
                visited[index] = true;
                queue.Enqueue(index);
            }
            for (int x = 0; x < w; x++) { Enqueue(x); Enqueue((h - 1) * w + x); }
            for (int y = 0; y < h; y++) { Enqueue(y * w); Enqueue(y * w + w - 1); }
            while (queue.Count > 0)
            {
                int i = queue.Dequeue();
                pixels[i].a = 0;
                int x = i % w, y = i / w;
                if (x > 0) Enqueue(i - 1);
                if (x < w - 1) Enqueue(i + 1);
                if (y > 0) Enqueue(i - w);
                if (y < h - 1) Enqueue(i + w);
            }
        }

        private static List<Vector2Int> Bands(bool[] occupied)
        {
            var result = new List<Vector2Int>();
            int start = -1;
            for (int i = 0; i <= occupied.Length; i++)
            {
                bool filled = i < occupied.Length && occupied[i];
                if (filled && start < 0) start = i;
                if (!filled && start >= 0) { result.Add(new Vector2Int(start, i)); start = -1; }
            }
            return result;
        }

        private void OnDestroy()
        {
            foreach (var frame in frames) if (frame != null) Destroy(frame);
            if (sheet != null) Destroy(sheet);
        }
    }
}

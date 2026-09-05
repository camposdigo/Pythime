using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Pythime
{
    [DefaultExecutionOrder(-2000)]
    public sealed class OfficialPlayerSpriteRuntime : MonoBehaviour
    {
        private const string ResourcePath = "Characters/player_marty_sheet";
        private const string DiskRelativePath = "Resources/Characters/player_marty_sheet.png";
        private const int Columns = 4;
        private const int Rows = 5;
        private const float PixelsPerUnit = 18f;
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

            var texture = LoadPlayerTexture();
            if (texture == null)
            {
                Debug.LogWarning("Pythime: player_marty_sheet.png não carregou. Use exatamente Assets/Resources/Characters/player_marty_sheet.png");
                Destroy(gameObject);
                yield break;
            }

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            BuildFrames(texture);
            usingOfficialSheet = frames.Count == Columns * Rows;

            if (!usingOfficialSheet)
            {
                Debug.LogWarning($"Pythime: spritesheet carregou, mas os frames não foram criados. Tamanho: {texture.width}x{texture.height}");
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

            target.transform.localScale = Vector3.one * 1.25f;
            lastPosition = player.position;
            ApplyIdle();
            Debug.Log($"Pythime: player oficial carregado com sucesso ({texture.width}x{texture.height}).");
        }

        private static Texture2D LoadPlayerTexture()
        {
            var directTexture = Resources.Load<Texture2D>(ResourcePath);
            if (directTexture != null)
                return directTexture;

            var sprite = Resources.Load<Sprite>(ResourcePath);
            if (sprite != null && sprite.texture != null)
                return sprite.texture;

            var sprites = Resources.LoadAll<Sprite>(ResourcePath);
            if (sprites != null && sprites.Length > 0 && sprites[0] != null && sprites[0].texture != null)
                return sprites[0].texture;

            var characterSprites = Resources.LoadAll<Sprite>("Characters");
            foreach (var candidate in characterSprites)
            {
                if (candidate == null || candidate.texture == null) continue;
                if (candidate.name.ToLowerInvariant().Contains("player_marty_sheet"))
                    return candidate.texture;
            }

            var diskPath = Path.Combine(Application.dataPath, DiskRelativePath.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(diskPath))
            {
                var bytes = File.ReadAllBytes(diskPath);
                var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false)
                {
                    name = "player_marty_sheet_disk",
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp
                };

                if (texture.LoadImage(bytes, false))
                    return texture;
            }

            DebugAvailableCharacterResources(diskPath);
            return null;
        }

        private static void DebugAvailableCharacterResources(string diskPath)
        {
            var textures = Resources.LoadAll<Texture2D>("Characters");
            var sprites = Resources.LoadAll<Sprite>("Characters");
            var textureNames = textures == null || textures.Length == 0 ? "nenhuma texture" : string.Join(", ", System.Array.ConvertAll(textures, t => t != null ? t.name : "null"));
            var spriteNames = sprites == null || sprites.Length == 0 ? "nenhum sprite" : string.Join(", ", System.Array.ConvertAll(sprites, s => s != null ? s.name : "null"));
            Debug.LogWarning($"Pythime: Resources/Characters encontrados -> Textures: {textureNames} | Sprites: {spriteNames} | Disco esperado: {diskPath} | Existe no disco: {File.Exists(diskPath)}");
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

        private void BuildFrames(Texture2D texture)
        {
            frames.Clear();
            var cellWidth = texture.width / (float)Columns;
            var cellHeight = texture.height / (float)Rows;

            for (var rowFromTop = 0; rowFromTop < Rows; rowFromTop++)
            {
                for (var column = 0; column < Columns; column++)
                {
                    var x = Mathf.RoundToInt(column * cellWidth);
                    var y = Mathf.RoundToInt(texture.height - (rowFromTop + 1) * cellHeight);
                    var w = Mathf.RoundToInt((column + 1) * cellWidth) - x;
                    var h = Mathf.RoundToInt(texture.height - rowFromTop * cellHeight) - y;

                    if (w <= 0 || h <= 0) continue;

                    var rect = new Rect(x, y, w, h);
                    var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.08f), PixelsPerUnit, 0, SpriteMeshType.FullRect);
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

        private enum Direction
        {
            Down,
            Up,
            Left,
            Right
        }
    }
}

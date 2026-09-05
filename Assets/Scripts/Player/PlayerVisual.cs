using UnityEngine;
using UnityEngine.InputSystem;

namespace Pythime
{
    public sealed class PlayerVisual : MonoBehaviour
    {
        private SpriteRenderer bodyRenderer;
        private SpriteRenderer hairRenderer;
        private SpriteRenderer accentRenderer;
        private int outfitIndex;
        private int hairIndex;

        private readonly Color[] outfits =
        {
            new(0.18f, 0.47f, 0.92f),
            new(0.87f, 0.30f, 0.28f),
            new(0.25f, 0.72f, 0.48f),
            new(0.65f, 0.38f, 0.86f),
            new(0.95f, 0.66f, 0.18f)
        };

        private readonly Color[] hairs =
        {
            new(0.10f, 0.07f, 0.05f),
            new(0.35f, 0.16f, 0.07f),
            new(0.92f, 0.73f, 0.28f),
            new(0.18f, 0.12f, 0.10f),
            new(0.74f, 0.32f, 0.49f)
        };

        public void Initialize(SpriteRenderer body, SpriteRenderer hair, SpriteRenderer accent)
        {
            bodyRenderer = body;
            hairRenderer = hair;
            accentRenderer = accent;
            Apply();
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.cKey.wasPressedThisFrame)
            {
                outfitIndex = (outfitIndex + 1) % outfits.Length;
                Apply();
            }

            if (keyboard.hKey.wasPressedThisFrame)
            {
                hairIndex = (hairIndex + 1) % hairs.Length;
                Apply();
            }
        }

        private void Apply()
        {
            if (bodyRenderer != null) bodyRenderer.color = outfits[outfitIndex];
            if (hairRenderer != null) hairRenderer.color = hairs[hairIndex];
            if (accentRenderer != null) accentRenderer.color = Color.Lerp(outfits[outfitIndex], Color.white, 0.35f);
        }
    }
}

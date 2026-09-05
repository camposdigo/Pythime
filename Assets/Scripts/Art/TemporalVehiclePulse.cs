using UnityEngine;

namespace Pythime
{
    public sealed class TemporalVehiclePulse : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Vector3 baseScale;
        private float phase;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            baseScale = transform.localScale;
            phase = Random.Range(0f, Mathf.PI * 2f);
        }

        private void Update()
        {
            var pulse = (Mathf.Sin(Time.time * 2.2f + phase) + 1f) * 0.5f;
            transform.localScale = baseScale * Mathf.Lerp(0.96f, 1.08f, pulse);

            if (spriteRenderer != null)
            {
                var c = spriteRenderer.color;
                c.a = Mathf.Lerp(0.24f, 0.58f, pulse);
                spriteRenderer.color = c;
            }
        }
    }
}

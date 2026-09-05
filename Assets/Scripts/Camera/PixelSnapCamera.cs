using UnityEngine;

namespace Pythime
{
    [DefaultExecutionOrder(1000)]
    public sealed class PixelSnapCamera : MonoBehaviour
    {
        [SerializeField] private float pixelsPerUnit = 16f;

        private void LateUpdate()
        {
            if (pixelsPerUnit <= 0f) return;
            var p = transform.position;
            var step = 1f / pixelsPerUnit;
            p.x = Mathf.Round(p.x / step) * step;
            p.y = Mathf.Round(p.y / step) * step;
            transform.position = p;
        }
    }
}

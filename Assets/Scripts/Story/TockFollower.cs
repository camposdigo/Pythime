using UnityEngine;

namespace Pythime
{
    public sealed class TockFollower : MonoBehaviour
    {
        private Transform target;
        private Vector3 velocity;

        public void Initialize(Transform followTarget)
        {
            target = followTarget;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            var side = Mathf.Sin(Time.time * 1.2f) >= 0f ? 1f : -1f;
            var bob = Mathf.Sin(Time.time * 3.2f) * 0.08f;
            var desired = target.position + new Vector3(0.85f * side, 0.85f + bob, 0f);
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, 0.22f);

            var renderer = GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 10f) + 30;
        }
    }
}

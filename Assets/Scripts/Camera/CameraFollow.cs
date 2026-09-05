using UnityEngine;

namespace Pythime
{
    public sealed class CameraFollow : MonoBehaviour
    {
        private Transform target;
        private Vector3 velocity;
        private float smoothTime = 0.12f;
        private Rect? mapBounds;

        public void SetMapBounds(Rect bounds) => mapBounds = bounds;

        public void Initialize(Transform followTarget, float smoothing = 0.12f)
        {
            target = followTarget;
            smoothTime = smoothing;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            var destination = new Vector3(target.position.x, target.position.y, transform.position.z);
            destination = ClampToMap(destination);
            transform.position = ClampToMap(Vector3.SmoothDamp(transform.position, destination, ref velocity, smoothTime));
        }

        private Vector3 ClampToMap(Vector3 position)
        {
            if (!mapBounds.HasValue) return position;
            var camera = GetComponent<Camera>();
            var bounds = mapBounds.Value;
            camera.orthographicSize = Mathf.Min(camera.orthographicSize, bounds.height * .5f, bounds.width / (2f * camera.aspect));
            float halfHeight = camera.orthographicSize;
            float halfWidth = halfHeight * camera.aspect;
            position.x = Mathf.Clamp(position.x, bounds.xMin + halfWidth, bounds.xMax - halfWidth);
            position.y = Mathf.Clamp(position.y, bounds.yMin + halfHeight, bounds.yMax - halfHeight);
            return position;
        }
    }
}

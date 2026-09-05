using UnityEngine;

namespace Pythime
{
    public sealed class CameraFollow : MonoBehaviour
    {
        private Transform target;
        private Vector3 velocity;
        private float smoothTime = 0.12f;

        public void Initialize(Transform followTarget, float smoothing = 0.12f)
        {
            target = followTarget;
            smoothTime = smoothing;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            var destination = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, smoothTime);
        }
    }
}

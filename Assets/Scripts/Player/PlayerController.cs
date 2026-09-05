using UnityEngine;
using UnityEngine.InputSystem;

namespace Pythime
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        private Rigidbody2D body;
        private Vector2 movement;

        public float MoveSpeed
        {
            get => moveSpeed;
            set => moveSpeed = Mathf.Clamp(value, 2f, 12f);
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                movement = Vector2.zero;
                return;
            }

            movement = Vector2.zero;

            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) movement.y += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) movement.y -= 1f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) movement.x -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) movement.x += 1f;

            movement = movement.normalized;
        }

        private void FixedUpdate()
        {
            body.linearVelocity = movement * moveSpeed;
        }

        private void OnDisable()
        {
            if (body != null) body.linearVelocity = Vector2.zero;
        }
    }
}

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
            body.bodyType = RigidbodyType2D.Dynamic;
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.sleepMode = RigidbodySleepMode2D.NeverSleep;
        }

        private void OnEnable()
        {
            movement = Vector2.zero;
            if (body != null) body.WakeUp();
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            movement = Vector2.zero;

            if (keyboard == null) return;

            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) movement.y += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) movement.y -= 1f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) movement.x -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) movement.x += 1f;

            if (movement.sqrMagnitude > 1f) movement.Normalize();
        }

        private void FixedUpdate()
        {
            if (body == null) return;
            var delta = movement * moveSpeed * Time.fixedDeltaTime;
            body.MovePosition(body.position + delta);
        }

        private void OnDisable()
        {
            movement = Vector2.zero;
            if (body != null) body.linearVelocity = Vector2.zero;
        }
    }
}

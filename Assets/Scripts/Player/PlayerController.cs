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

        public Vector2 MoveInput => movement;
        public bool IsMoving => movement.sqrMagnitude > 0.01f;
        public bool InputLocked { get; set; }

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
            movement = Vector2.zero;
            if (InputLocked) return;

            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) movement.y += 1f;
                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) movement.y -= 1f;
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) movement.x -= 1f;
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) movement.x += 1f;
            }

            var gamepad = Gamepad.current;
            if (gamepad != null && movement.sqrMagnitude < 0.01f)
            {
                var stick = gamepad.leftStick.ReadValue();
                if (stick.sqrMagnitude > 0.08f) movement = stick;
            }

            if (movement.sqrMagnitude > 1f) movement.Normalize();
        }

        private void FixedUpdate()
        {
            if (body == null || InputLocked) return;
            var delta = movement * moveSpeed * Time.fixedDeltaTime;
            body.MovePosition(body.position + delta);
        }

        public void StopImmediately()
        {
            movement = Vector2.zero;
            if (body != null)
            {
                body.linearVelocity = Vector2.zero;
                body.angularVelocity = 0f;
            }
        }

        private void OnDisable()
        {
            StopImmediately();
        }
    }
}

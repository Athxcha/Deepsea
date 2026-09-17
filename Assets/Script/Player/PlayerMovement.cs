using UnityEngine;
using UnityEngine.InputSystem;

namespace DeepScan
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 5f;

        [SerializeField]
        private float verticalSpeed = 3f;

        private CharacterController controller;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            // =========================
            // KEYBOARD
            // =========================

            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            float y = 0f;

            // Space = ขึ้น
            if (Input.GetKey(KeyCode.Space))
            {
                y += 1f;
            }

            // Left Ctrl = ลง
            if (Input.GetKey(KeyCode.LeftControl))
            {
                y -= 1f;
            }


            // =========================
            // XBOX CONTROLLER
            // =========================

            if (Gamepad.current != null)
            {
                // Left Stick
                Vector2 stick =
                    Gamepad.current.leftStick.ReadValue();

                x += stick.x;
                z += stick.y;

                // A = ขึ้น
                if (Gamepad.current.aButton.isPressed)
                {
                    y += 1f;
                }

                // B = ลง
                if (Gamepad.current.bButton.isPressed)
                {
                    y -= 1f;
                }
            }


            // =========================
            // MOVEMENT
            // =========================

            Vector3 horizontal =
                transform.right * x +
                transform.forward * z;

            horizontal =
                Vector3.ClampMagnitude(horizontal, 1f);

            Vector3 movement =
                horizontal * moveSpeed;

            movement +=
                Vector3.up *
                y *
                verticalSpeed;

            controller.Move(
                movement * Time.deltaTime
            );
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

namespace DeepScan
{
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField]
        private Transform playerBody;

        [Header("Mouse")]
        [SerializeField]
        private float sensitivity = 150f;

        [Header("Xbox Controller")]
        [SerializeField]
        private float controllerSensitivity = 120f;

        private float verticalRotation;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            // =========================
            // MOUSE
            // =========================

            float lookX =
                Input.GetAxis("Mouse X") *
                sensitivity *
                Time.deltaTime;

            float lookY =
                Input.GetAxis("Mouse Y") *
                sensitivity *
                Time.deltaTime;


            // =========================
            // XBOX RIGHT STICK
            // =========================

            if (Gamepad.current != null)
            {
                Vector2 rightStick =
                    Gamepad.current.rightStick.ReadValue();

                lookX +=
                    rightStick.x *
                    controllerSensitivity *
                    Time.deltaTime;

                lookY +=
                    rightStick.y *
                    controllerSensitivity *
                    Time.deltaTime;
            }


            // =========================
            // LOOK UP / DOWN
            // =========================

            verticalRotation -= lookY;

            verticalRotation =
                Mathf.Clamp(
                    verticalRotation,
                    -80f,
                    80f
                );

            transform.localRotation =
                Quaternion.Euler(
                    verticalRotation,
                    0f,
                    0f
                );


            // =========================
            // LOOK LEFT / RIGHT
            // =========================

            playerBody.Rotate(
                Vector3.up * lookX
            );
        }
    }
}
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

        [SerializeField]
        private bool lockCursorOnStart = false;

        [SerializeField]
        private KeyCode toggleCursorLockKey = KeyCode.Tab;

        [Header("Xbox Controller")]
        [SerializeField]
        private float controllerSensitivity = 120f;

        [Header("ESP32 MPU6050")]
        [SerializeField]
        private Mpu6050SerialReader mpu6050Input;

        [SerializeField]
        private bool autoFindMpu6050Input = true;

        [SerializeField]
        private bool useMpu6050Yaw = true;

        [SerializeField]
        private float mpu6050YawSensitivity = 1f;

        [SerializeField]
        private float mpu6050PitchSensitivity = 1f;

        private float verticalRotation;

        private void Awake()
        {
            if (mpu6050Input == null && autoFindMpu6050Input)
            {
                mpu6050Input = FindObjectOfType<Mpu6050SerialReader>();
            }
        }

        private void Start()
        {
            SetCursorLocked(lockCursorOnStart);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetCursorLocked(false);
            }
            else if (Input.GetKeyDown(toggleCursorLockKey))
            {
                SetCursorLocked(Cursor.lockState != CursorLockMode.Locked);
            }

            // =========================
            // MOUSE
            // =========================

            float lookX = 0f;
            float lookY = 0f;

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                lookX =
                    Input.GetAxis("Mouse X") *
                    sensitivity *
                    Time.deltaTime;

                lookY =
                    Input.GetAxis("Mouse Y") *
                    sensitivity *
                    Time.deltaTime;
            }


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
            // ESP32 MPU6050 YAW
            // =========================

            if (useMpu6050Yaw && mpu6050Input != null)
            {
                lookX +=
                    mpu6050Input.GetTurnDeltaDegrees(Time.deltaTime) *
                    mpu6050YawSensitivity;

                lookY +=
                    mpu6050Input.GetLookDeltaDegrees(Time.deltaTime) *
                    mpu6050PitchSensitivity;
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

        private static void SetCursorLocked(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}

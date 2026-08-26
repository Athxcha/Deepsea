using UnityEngine;

namespace DeepScan
{
    public class PlayerLook :
        MonoBehaviour
    {
        [SerializeField]
        private Transform playerBody;

        [SerializeField]
        private float sensitivity = 150f;


        private float verticalRotation;


        private void Start()
        {
            Cursor.lockState =
                CursorLockMode.Locked;

            Cursor.visible = false;
        }


        private void Update()
        {
            float mouseX =
                Input.GetAxis("Mouse X") *
                sensitivity *
                Time.deltaTime;

            float mouseY =
                Input.GetAxis("Mouse Y") *
                sensitivity *
                Time.deltaTime;


            verticalRotation -=
                mouseY;

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


            playerBody.Rotate(
                Vector3.up *
                mouseX
            );
        }
    }
}
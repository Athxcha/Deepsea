using UnityEngine;

namespace DeepScan
{
    [RequireComponent(
        typeof(CharacterController)
    )]
    public class PlayerMovement :
        MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 5f;

        [SerializeField]
        private float verticalSpeed = 3f;


        private CharacterController
            controller;


        private void Awake()
        {
            controller =
                GetComponent<
                    CharacterController
                >();
        }


        private void Update()
        {
            float x =
                Input.GetAxisRaw(
                    "Horizontal"
                );

            float z =
                Input.GetAxisRaw(
                    "Vertical"
                );

            float y = 0f;


            if (Input.GetKey(
                KeyCode.Space))
            {
                y += 1f;
            }

            if (Input.GetKey(
                KeyCode.LeftControl))
            {
                y -= 1f;
            }


            Vector3 horizontal =
                transform.right * x +
                transform.forward * z;


            horizontal =
                Vector3.ClampMagnitude(
                    horizontal,
                    1f
                );


            Vector3 movement =
                horizontal * moveSpeed;

            movement +=
                Vector3.up *
                y *
                verticalSpeed;


            controller.Move(
                movement *
                Time.deltaTime
            );
        }
    }
}
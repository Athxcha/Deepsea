using UnityEngine;

namespace DeepScan
{
    public class Billboard :
        MonoBehaviour
    {
        private Camera targetCamera;


        private void Start()
        {
            targetCamera =
                Camera.main;
        }


        private void LateUpdate()
        {
            if (targetCamera == null)
                return;


            Vector3 direction =
                targetCamera.transform.position -
                transform.position;


            direction.y = 0f;


            if (direction.sqrMagnitude <
                0.001f)
                return;


            transform.rotation =
                Quaternion.LookRotation(
                    -direction
                );
        }
    }
}
using UnityEngine;

namespace DeepScan
{
    public class FishMovement : MonoBehaviour
    {
        private FishMovementData data;

        private Vector3 startPosition;

        private float movementTime;
        private float offset;

        private bool isPaused;


        [Header("Swimming Bounds")]

        [SerializeField]
        private Vector3 minBounds =
            new Vector3(-8f, 1f, -8f);

        [SerializeField]
        private Vector3 maxBounds =
            new Vector3(8f, 8f, 8f);


        public void Configure(
            FishMovementData movementData)
        {
            data = movementData;

            startPosition =
                transform.position;

            movementTime = 0f;

            offset =
                Random.Range(
                    0f,
                    100f
                );

            isPaused = false;
        }


        public void SetPaused(bool paused)
        {
            isPaused = paused;
        }


        private void Update()
        {
            if (data == null)
                return;

            if (isPaused)
                return;


            movementTime +=
                Time.deltaTime;


            float t =
                movementTime *
                data.Speed +
                offset;


            float x =
                Mathf.Sin(t) *
                data.HorizontalRange;


            float y =
                Mathf.Sin(t * 0.7f) *
                data.VerticalRange;


            // เพิ่มการเคลื่อนที่แกน Z
            // ให้ดูเหมือนว่ายในพื้นที่ 3D
            float z =
                Mathf.Sin(
                    t * 0.45f +
                    offset
                ) *
                data.HorizontalRange *
                0.5f;


            Vector3 targetPosition =
                startPosition +
                new Vector3(
                    x,
                    y,
                    z
                );


            // บังคับไม่ให้ออกนอกพื้นที่
            targetPosition.x =
                Mathf.Clamp(
                    targetPosition.x,
                    minBounds.x,
                    maxBounds.x
                );


            targetPosition.y =
                Mathf.Clamp(
                    targetPosition.y,
                    minBounds.y,
                    maxBounds.y
                );


            targetPosition.z =
                Mathf.Clamp(
                    targetPosition.z,
                    minBounds.z,
                    maxBounds.z
                );


            transform.position =
                targetPosition;
        }
    }
}
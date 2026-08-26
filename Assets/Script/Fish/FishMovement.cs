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


        public void Configure(FishMovementData movementData)
        {
            data = movementData;

            startPosition = transform.position;

            movementTime = 0f;

            offset = Random.Range(
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

            // ถ้าปลาถูก Pause
            // เวลาการว่ายก็หยุดด้วย
            if (isPaused)
                return;


            movementTime += Time.deltaTime;


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


            transform.position =
                startPosition +
                new Vector3(
                    x,
                    y,
                    0f
                );
        }
    }
}
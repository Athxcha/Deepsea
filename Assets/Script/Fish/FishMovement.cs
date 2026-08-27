using UnityEngine;

namespace DeepScan
{
    public class FishMovement : MonoBehaviour
    {
        private FishMovementData data;

        private Vector3 startPosition;
        private float offset;

        private bool isPaused;

        private Rigidbody rb;


        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }


        public void Configure(FishMovementData movementData)
        {
            data = movementData;

            startPosition = transform.position;

            offset = Random.Range(0f, 100f);
        }


        public void SetPaused(bool paused)
        {
            isPaused = paused;
        }


        private void FixedUpdate()
        {
            if (data == null)
                return;

            if (isPaused)
                return;


            float t =
                Time.time *
                data.Speed +
                offset;


            float x =
                Mathf.Sin(t) *
                data.HorizontalRange;

            float y =
                Mathf.Sin(t * 0.7f) *
                data.VerticalRange;


            Vector3 targetPosition =
                startPosition +
                new Vector3(
                    x,
                    y,
                    0f
                );


            rb.MovePosition(targetPosition);
        }
    }
}
using System;
using UnityEngine;

namespace DeepScan
{
    public class DiveTimer :
        MonoBehaviour
    {
        public event Action<float>
            OxygenChanged;


        private float duration;

        private float remaining;

        private bool running;


        public float NormalizedOxygen
        {
            get
            {
                if (duration <= 0f)
                    return 0f;

                return Mathf.Clamp01(
                    remaining /
                    duration
                );
            }
        }


        public void StartTimer(
            float seconds)
        {
            duration = seconds;

            remaining = seconds;

            running = true;

            OxygenChanged?.Invoke(1f);
        }


        private void Update()
        {
            if (!running)
                return;


            remaining -=
                Time.deltaTime;


            OxygenChanged?.Invoke(
                NormalizedOxygen
            );


            if (remaining <= 0f)
            {
                remaining = 0f;

                running = false;

                SceneFlowService.Instance
                    .Surface();
            }
        }
    }
}
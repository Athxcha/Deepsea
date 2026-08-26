using UnityEngine;

namespace DeepScan
{
    public class LidarScanner :
        MonoBehaviour
    {
        [Header("Camera")]

        [SerializeField]
        private Camera playerCamera;


        [Header("Scan")]

        [SerializeField]
        private float range = 20f;

        [SerializeField]
        private int raysPerFrame = 30;

        [SerializeField]
        private float spread = 0.3f;


        [Header("Layers")]

        [SerializeField]
        private LayerMask scanMask;

        [SerializeField]
        private LayerMask fishMask;


        [Header("Point Pools")]

        [SerializeField]
        private ScanPointPool environmentPool;

        [SerializeField]
        private ScanPointPool fishPool;


        [Header("Environment")]

        [SerializeField]
        private float environmentPointLifetime =
            4f;


        public FishActor CurrentFish
        {
            get;
            private set;
        }


        private void Update()
        {
            if (!Input.GetMouseButton(0))
            {
                CurrentFish = null;

                return;
            }


            FindFishAtCrosshair();

            EmitLidar();

            UpdateFishScan();
        }


        private void FindFishAtCrosshair()
        {
            Ray ray =
                new Ray(
                    playerCamera
                        .transform.position,

                    playerCamera
                        .transform.forward
                );


            if (Physics.Raycast(
                ray,
                out RaycastHit hit,
                range,
                fishMask))
            {
                CurrentFish =
                    hit.collider
                        .GetComponentInParent<
                            FishActor
                        >();
            }
            else
            {
                CurrentFish = null;
            }
        }


        private void UpdateFishScan()
        {
            if (CurrentFish == null)
                return;


            CurrentFish.AddScanProgress(
                Time.deltaTime
            );
        }


        private void EmitLidar()
        {
            for (int i = 0;
                 i < raysPerFrame;
                 i++)
            {
                Vector3 direction =
                    playerCamera
                        .transform.forward;


                direction +=
                    playerCamera
                        .transform.right *
                    Random.Range(
                        -spread,
                        spread
                    );


                direction +=
                    playerCamera
                        .transform.up *
                    Random.Range(
                        -spread,
                        spread
                    );


                direction.Normalize();


                Ray ray =
                    new Ray(
                        playerCamera
                            .transform.position,
                        direction
                    );


                if (!Physics.Raycast(
                    ray,
                    out RaycastHit hit,
                    range,
                    scanMask))
                {
                    continue;
                }


                CreatePoint(hit);
            }
        }


        private void CreatePoint(
            RaycastHit hit)
        {
            bool hitFish =
                (
                    fishMask.value &
                    (1 << hit.collider
                        .gameObject.layer)
                ) != 0;


            if (hitFish)
            {
                FishActor fish =
                    hit.collider
                        .GetComponentInParent<
                            FishActor
                        >();


                if (fish == null)
                    return;


                ScanPoint point =
                    fishPool.Get(
                        hit.point,
                        fish.ScanPointRoot,
                        0f
                    );


                fish.RegisterScanPoint(
                    point
                );
            }
            else
            {
                environmentPool.Get(
                    hit.point,
                    null,
                    environmentPointLifetime
                );
            }
        }
    }
}
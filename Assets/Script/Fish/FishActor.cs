using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepScan
{
    public class FishActor : MonoBehaviour
    {
        [Header("References")]

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Transform scanShapeRoot;

        [SerializeField]
        private Transform scanPointRoot;

        [SerializeField]
        private FishMovement movement;

        [SerializeField]
        private FishInfoWorldUI infoUI;


        [Header("Scan Complete")]

        [SerializeField]
        private float revealDelay = 5f;


        private FishData data;

        private float scanProgress;

        private bool scanCompleted;


        private readonly List<ScanPoint>
            scanPoints =
                new List<ScanPoint>();


        public FishData Data =>
            data;

        public float ScanProgress =>
            scanProgress;

        public Transform ScanPointRoot =>
            scanPointRoot;


       public void Initialize(FishData fishData)
{
    data = fishData;

    scanProgress = 0f;
    scanCompleted = false;

    spriteRenderer.sprite =
        data.Sprite;

    spriteRenderer.enabled =
        false;

    if (infoUI != null)
    {
        infoUI.Hide();
    }

    movement.Configure(
        data.Movement
    );

    CreateScanShape();
}


        private void CreateScanShape()
        {
            if (data.ScanShapePrefab == null)
            {
                Debug.LogWarning(
                    data.FishName +
                    " has no ScanShapePrefab."
                );

                return;
            }


            GameObject shape =
                Instantiate(
                    data.ScanShapePrefab,
                    scanShapeRoot
                );


            SetLayerRecursive(
                shape,
                LayerMask.NameToLayer(
                    "FishScan"
                )
            );
        }


        private void SetLayerRecursive(
            GameObject obj,
            int layer)
        {
            obj.layer = layer;

            foreach (
                Transform child
                in obj.transform)
            {
                SetLayerRecursive(
                    child.gameObject,
                    layer
                );
            }
        }


        public void AddScanProgress(
            float deltaTime)
        {
            if (data == null)
                return;

            if (scanCompleted)
                return;


            scanProgress +=
                deltaTime /
                data.ScanDuration;


            scanProgress =
                Mathf.Clamp01(
                    scanProgress
                );


            if (scanProgress >= 1f)
            {
                scanCompleted = true;

                StartCoroutine(
                    CompleteScanSequence()
                );
            }
        }


        private IEnumerator
            CompleteScanSequence()
        {
            Debug.Log(
                "SCAN 100%: " +
                data.FishName
            );


            // หยุดปลา
            movement.SetPaused(true);


            // ยังให้จุดแดงค้างอยู่ 5 วิ
            yield return
                new WaitForSeconds(
                    revealDelay
                );


            // จำข้อมูลปลา
            GameSession.Instance
                .RegisterFish(data);


            // ปิด ScanShape
            scanShapeRoot
                .gameObject
                .SetActive(false);


            // ล้างจุดแดง
            ClearScanPoints();


            // เปิด Sprite ปลา 2D
            spriteRenderer.enabled =
                true;


            Debug.Log(
                "REVEALED: " +
                data.FishName
            );


            // ให้ปลาว่ายต่อ
            movement.SetPaused(false);
        }


        public void RegisterScanPoint(
            ScanPoint point)
        {
            if (point == null)
                return;

            if (scanPoints.Contains(point))
                return;

            scanPoints.Add(point);
        }


        private void ClearScanPoints()
        {
            foreach (
                ScanPoint point
                in scanPoints)
            {
                if (point != null)
                {
                    point.Release();
                }
            }

            scanPoints.Clear();
        }
    }
}
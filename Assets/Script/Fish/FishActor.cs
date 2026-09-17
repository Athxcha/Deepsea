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
    private SpriteRenderer infoSpriteRenderer;

    [SerializeField]
    private Transform scanShapeRoot;

    [SerializeField]
    private Transform scanPointRoot;

    [SerializeField]
    private FishMovement movement;


    [Header("Reveal")]

    [SerializeField]
    private float revealDuration = 5f;

    [SerializeField]
    private float revealAnimationDuration = 0.5f;

    [SerializeField]
    private float revealStartScale = 0.3f;


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


    public void Initialize(
        FishData fishData)
    {
        data = fishData;

        scanProgress = 0f;

        scanCompleted = false;


        // รูปปลา 2D
        spriteRenderer.sprite =
            data.Sprite;

        spriteRenderer.enabled =
            false;


        // รูปข้อมูลปลา
        infoSpriteRenderer.sprite =
            data.InfoImage;

        infoSpriteRenderer.enabled =
            false;


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
        // ==========================
        // 1. Scan ครบ 100%
        // ==========================

        Debug.Log(
            "SCAN 100%: " +
            data.FishName
        );


        // หยุดปลา
        movement.SetPaused(true);


        // ==========================
        // 2. บันทึกข้อมูลปลา
        // ==========================

        GameSession.Instance
            .RegisterFish(data);


        // ==========================
        // 3. จุดแดงหาย
        // ==========================

        ClearScanPoints();


        // ปิด ScanShape 3D
        scanShapeRoot
            .gameObject
            .SetActive(false);


        // ==========================
        // 4. ปลา Sprite 2D โผล่
        // พร้อม Reveal Animation
        // ==========================

        yield return StartCoroutine(
            PlayRevealAnimation()
        );


        // ==========================
        // 5. รูปข้อมูลโผล่
        // ==========================

        if (data.InfoImage != null)
        {
            infoSpriteRenderer.enabled =
                true;
        }


        // ==========================
        // 6. ค้างตามเวลาที่กำหนด
        // ==========================

        yield return
            new WaitForSeconds(
                revealDuration
            );


        // ==========================
        // 7. รูปข้อมูลหาย
        // ==========================

        infoSpriteRenderer.enabled =
            false;


        // ==========================
        // 8. ปลาว่ายต่อ
        // ==========================

        movement.SetPaused(false);


        Debug.Log(
            "Fish resumed: " +
            data.FishName
        );
    }


    // ==========================
    // Reveal Animation
    // ==========================

    private IEnumerator
        PlayRevealAnimation()
    {
        spriteRenderer.enabled =
            true;


        // จำ Scale เดิม
        Vector3 originalScale =
            spriteRenderer
                .transform
                .localScale;


        // Scale ตอนเริ่ม
        Vector3 startScale =
            originalScale *
            revealStartScale;


        spriteRenderer
            .transform
            .localScale =
                startScale;


        // เริ่มแบบโปร่งใส
        Color color =
            spriteRenderer.color;

        color.a = 0f;

        spriteRenderer.color =
            color;


        float time = 0f;


        while (
            time <
            revealAnimationDuration)
        {
            time +=
                Time.deltaTime;


            float t =
                Mathf.Clamp01(
                    time /
                    revealAnimationDuration
                );


            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );


            // ค่อย ๆ ขยาย
            spriteRenderer
                .transform
                .localScale =
                    Vector3.Lerp(
                        startScale,
                        originalScale,
                        smoothT
                    );


            // ค่อย ๆ Fade In
            color.a =
                smoothT;

            spriteRenderer.color =
                color;


            yield return null;
        }


        // บังคับค่าตอนจบ
        spriteRenderer
            .transform
            .localScale =
                originalScale;


        color.a = 1f;

        spriteRenderer.color =
            color;
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

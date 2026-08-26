using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeepScan
{
    public class DiveHUD :
        MonoBehaviour
    {
        [Header("References")]

        [SerializeField]
        private DiveTimer diveTimer;

        [SerializeField]
        private LidarScanner scanner;


        [Header("Oxygen")]

        [SerializeField]
        private Slider oxygenSlider;

        [SerializeField]
        private TMP_Text oxygenText;


        [Header("Scan")]

        [SerializeField]
        private GameObject scanUI;

        [SerializeField]
        private Slider scanSlider;

        [SerializeField]
        private TMP_Text fishNameText;

        [SerializeField]
        private TMP_Text percentageText;


        private void OnEnable()
        {
            diveTimer.OxygenChanged +=
                UpdateOxygen;
        }


        private void OnDisable()
        {
            diveTimer.OxygenChanged -=
                UpdateOxygen;
        }


        private void Update()
        {
            UpdateScanUI();
        }


        private void UpdateOxygen(
            float value)
        {
            oxygenSlider.value =
                value;


            int percent =
                Mathf.RoundToInt(
                    value * 100f
                );


            oxygenText.text =
                "OXYGEN " +
                percent +
                "%";
        }


        private void UpdateScanUI()
        {
            FishActor fish =
                scanner.CurrentFish;


            if (fish == null ||
                !Input.GetMouseButton(0))
            {
                scanUI.SetActive(false);

                return;
            }


            scanUI.SetActive(true);


            float progress =
                fish.ScanProgress;


            scanSlider.value =
                progress;


            int percent =
                Mathf.RoundToInt(
                    progress * 100f
                );


            percentageText.text =
                percent + "%";


            if (progress >= 1f)
            {
                fishNameText.text =
                    fish.Data.FishName;
            }
            else
            {
                fishNameText.text =
                    "UNKNOWN LIFEFORM";
            }
        }
    }
}
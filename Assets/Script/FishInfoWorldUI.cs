using TMPro;
using UnityEngine;

namespace DeepScan
{
    public class FishInfoWorldUI : MonoBehaviour
    {
        [Header("UI")]

        [SerializeField]
        private TMP_Text nameText;

        [SerializeField]
        private TMP_Text descriptionText;


        private Camera targetCamera;


        private void Awake()
        {
            targetCamera = Camera.main;
        }


        public void Show(FishData fish)
        {
            if (fish == null)
                return;

            nameText.text =
                fish.FishName;

            descriptionText.text =
                fish.Description;

            gameObject.SetActive(true);
        }


        public void Hide()
        {
            gameObject.SetActive(false);
        }


        private void LateUpdate()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            if (targetCamera == null)
                return;

            transform.rotation =
                Quaternion.LookRotation(
                    transform.position -
                    targetCamera.transform.position
                );
        }
    }
}
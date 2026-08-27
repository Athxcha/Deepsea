using System.Collections;
using TMPro;
using UnityEngine;

namespace DeepScan
{
    public class SubmitUI : MonoBehaviour
    {
        [Header("Total Score")]

        [SerializeField]
        private TMP_Text totalPointText;


        [Header("Point Popup")]

        [SerializeField]
        private GameObject pointPanel;

        [SerializeField]
        private TMP_Text pointText;


        [Header("Settings")]

        [SerializeField]
        private float showDuration = 1.5f;


        private Coroutine showRoutine;


        private void Start()
        {
            if (pointPanel != null)
            {
                pointPanel.SetActive(false);
            }

            UpdateTotalScore();
        }


        public void ShowPoint(int amount)
        {
            UpdateTotalScore();

            ShowPopup(
                "+" + amount
            );
        }


        public void ShowBonus(int amount)
        {
            UpdateTotalScore();

            ShowPopup(
                 " BONUS" +  "+" + amount 
            );
        }


        private void UpdateTotalScore()
        {
            if (totalPointText == null)
                return;

            if (GameSession.Instance == null)
            {
                totalPointText.text =
                    " 0";

                return;
            }

            totalPointText.text =
                "POINT " +
                GameSession.Instance.Score;
        }


        private void ShowPopup(string message)
        {
            if (showRoutine != null)
            {
                StopCoroutine(showRoutine);
            }

            showRoutine =
                StartCoroutine(
                    ShowRoutine(message)
                );
        }


        private IEnumerator ShowRoutine(
            string message)
        {
            if (pointPanel == null ||
                pointText == null)
            {
                yield break;
            }


            pointPanel.SetActive(true);

            pointText.text =
                message;


            yield return new WaitForSeconds(
                showDuration
            );


            pointPanel.SetActive(false);

            showRoutine = null;
        }
    }
}
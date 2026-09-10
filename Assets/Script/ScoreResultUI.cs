using TMPro;
using UnityEngine;

namespace DeepScan
{
    public class ScoreResultUI : MonoBehaviour
    {
        [Header("UI")]

        [SerializeField]
        private TMP_Text baseScoreText;

        [SerializeField]
        private TMP_Text quizText;

        [SerializeField]
        private TMP_Text multiplierText;

        [SerializeField]
        private TMP_Text finalScoreText;


        public void Refresh()
        {
            if (GameSession.Instance == null)
            {
                Debug.LogError(
                    "GameSession does not exist."
                );

                return;
            }


            if (baseScoreText != null)
            {
                baseScoreText.text =
                    GameSession.Instance
                        .BaseScore
                        .ToString();
            }


            if (quizText != null)
            {
                quizText.text =
                    GameSession.Instance
                        .CorrectAnswers +
                    " / 3";
            }


            if (multiplierText != null)
            {
                multiplierText.text =
                    "x" +
                    GameSession.Instance
                        .Multiplier;
            }


            if (finalScoreText != null)
            {
                finalScoreText.text =
                    GameSession.Instance
                        .FinalScore
                        .ToString();
            }
        }
    }
}
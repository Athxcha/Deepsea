using TMPro;
using UnityEngine;

namespace DeepScan
{
    public class ScoreResultUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text finalScoreText;
        [SerializeField] private TMP_Text categoryText;
        [SerializeField] private TMP_Text correctText;
        [SerializeField] private TMP_Text streakText;

        private void Start()
        {
            Refresh();
        }

        public void Refresh()
        {
            if (GameSession.Instance == null)
            {
                Debug.LogError("GameSession does not exist.");
                return;
            }

            // POINT : 1200
            if (finalScoreText != null)
            {
                finalScoreText.text =
                    "POINT : " +
                    GameSession.Instance.FinalScore;
            }

            // Category
            if (categoryText != null)
            {
                categoryText.text =
                    "Category : sea life easy mode";
            }

            // Correct : 3
            if (correctText != null)
            {
                correctText.text =
                    "Correct : " +
                    GameSession.Instance.CorrectAnswers;
            }

            // X2 : Streak 3
            if (streakText != null)
            {
                streakText.text =
                    "X" +
                    GameSession.Instance.Multiplier +
                    " : Streak " +
                    GameSession.Instance.CorrectAnswers;
            }
        }
    }
}
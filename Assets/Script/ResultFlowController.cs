using UnityEngine;

namespace DeepScan
{
    public class ResultFlowController :
        MonoBehaviour
    {
        [Header("Panels")]

        [SerializeField]
        private GameObject fishResultPanel;

        [SerializeField]
        private GameObject quizPanel;

        [SerializeField]
        private GameObject scorePanel;


        [Header("References")]

        [SerializeField]
        private QuizUI quizUI;

        [SerializeField]
        private ScoreResultUI scoreResultUI;


        private void Start()
        {
            ShowFishResult();
        }


        public void ShowFishResult()
        {
            fishResultPanel.SetActive(
                true
            );

            quizPanel.SetActive(
                false
            );

            scorePanel.SetActive(
                false
            );
        }


        // ปุ่ม SEND FISH
        public void SendFish()
        {
            if (GameSession.Instance == null)
                return;


            GameSession.Instance
                .SubmitFish();


            fishResultPanel.SetActive(
                false
            );

            quizPanel.SetActive(
                true
            );

            scorePanel.SetActive(
                false
            );


            quizUI.BeginQuiz();
        }


        public void ShowScore()
        {
            fishResultPanel.SetActive(
                false
            );

            quizPanel.SetActive(
                false
            );

            scorePanel.SetActive(
                true
            );


            scoreResultUI
                .Refresh();
        }
    }
}
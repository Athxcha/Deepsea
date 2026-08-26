using UnityEngine;

namespace DeepScan
{
    public class SubmitTerminal :
        MonoBehaviour
    {
        [SerializeField]
        private QuizController quizController;


        private bool playerInside;


        private void Update()
        {
            if (!playerInside)
                return;


            if (Input.GetKeyDown(
                KeyCode.E))
            {
                Submit();
            }
        }


        private void Submit()
        {
            GameSession session =
                GameSession.Instance;


            if (session.ScannedFish.Count ==
                0)
            {
                Debug.Log(
                    "No data to submit."
                );

                return;
            }


            int gained =
                session.SubmitData();


            Debug.Log(
                "DATA SUBMITTED +" +
                gained
            );


            quizController
                .BuildQuestions();


            Debug.Log(
                "Quiz Count: " +
                quizController
                    .Questions.Count
            );
        }


        private void OnTriggerEnter(
            Collider other)
        {
            if (other.CompareTag(
                "Player"))
            {
                playerInside = true;
            }
        }


        private void OnTriggerExit(
            Collider other)
        {
            if (other.CompareTag(
                "Player"))
            {
                playerInside = false;
            }
        }
    }
}
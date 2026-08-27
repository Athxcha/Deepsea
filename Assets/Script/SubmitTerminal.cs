using UnityEngine;

namespace DeepScan
{
    public class SubmitTerminal : MonoBehaviour
    {
        [Header("Quiz")]

        [SerializeField]
        private QuizController quizController;

        [SerializeField]
        private QuizUI quizUI;


        [Header("UI")]

        [SerializeField]
        private SubmitUI submitUI;


        private bool playerInside;


        private void Update()
        {
            if (!playerInside)
                return;


            if (Input.GetKeyDown(KeyCode.E))
            {
                Submit();
            }
        }


        private void Submit()
        {
            GameSession session =
                GameSession.Instance;


            if (session == null)
            {
                Debug.LogError(
                    "GameSession does not exist."
                );

                return;
            }


            // ยังไม่ได้สแกนปลาเลย
            if (session.ScannedFish.Count == 0)
            {
                Debug.Log(
                    "No data to submit."
                );

                return;
            }


            // กันกด Submit ซ้ำ
            if (session.DataSubmitted)
            {
                Debug.Log(
                    "Data already submitted."
                );

                return;
            }


            // ส่งข้อมูลปลาและรับคะแนน
            int gained =
                session.SubmitData();


            // ขึ้น +200 POINT มุมขวาบน
            if (submitUI != null)
            {
                submitUI.ShowPoint(gained);
            }


            // สร้างคำถามจากปลาที่ผู้เล่นสแกน
            quizController.BuildQuestions();


            // ถ้ามีคำถาม ให้เปิด Quiz
            if (quizController.Questions.Count > 0)
            {
                quizUI.OpenQuiz(
                    quizController
                );
            }
            else
            {
                Debug.Log(
                    "No quiz available."
                );
            }
        }


        private void OnTriggerEnter(
            Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInside = true;

                Debug.Log(
                    "Press E to submit data."
                );
            }
        }


        private void OnTriggerExit(
            Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInside = false;
            }
        }
    }
}
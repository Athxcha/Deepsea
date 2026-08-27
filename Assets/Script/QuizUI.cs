using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeepScan
{
    public class QuizUI : MonoBehaviour
    {
        [Header("Main UI")]

        [SerializeField]
        private GameObject quizPanel;

        [SerializeField]
        private TMP_Text questionText;

        [SerializeField]
        private Image fishImage;


        [Header("Answers")]

        [SerializeField]
        private Button[] answerButtons;

        [SerializeField]
        private TMP_Text[] answerTexts;


        [Header("Feedback")]

        [SerializeField]
        private TMP_Text feedbackText;

        [SerializeField]
        private SubmitUI submitUI;


        [Header("Settings")]

        [SerializeField]
        private float nextQuestionDelay = 1f;


        private QuizController quizController;

        private int currentQuestionIndex;

        private bool answering;


        private void Start()
        {
            quizPanel.SetActive(false);
        }


        public void OpenQuiz(
            QuizController controller)
        {
            quizController =
                controller;


            if (quizController == null)
                return;


            if (quizController.Questions.Count == 0)
                return;


            currentQuestionIndex = 0;

            answering = false;


            quizPanel.SetActive(true);


            Cursor.visible = true;

            Cursor.lockState =
                CursorLockMode.None;


            ShowQuestion();
        }


        private void ShowQuestion()
        {
            if (currentQuestionIndex >=
                quizController.Questions.Count)
            {
                FinishQuiz();

                return;
            }


            QuizData quiz =
                quizController
                    .Questions[
                        currentQuestionIndex
                    ];


            questionText.text =
                quiz.Question;


           // ใช้รูปที่กำหนดเองใน QuizData
            
            if (quiz.QuestionImage != null)
            {
                fishImage.gameObject
                    .SetActive(true);

                fishImage.sprite =
                    quiz.QuestionImage;
            }
            else
            {
                fishImage.gameObject
                    .SetActive(false);
            }

            if (feedbackText != null)
            {
                feedbackText.text = "";
            }


            string[] answers =
                quiz.Answers;


            for (int i = 0;
                 i < answerButtons.Length;
                 i++)
            {
                if (i >= answers.Length)
                {
                    answerButtons[i]
                        .gameObject
                        .SetActive(false);

                    continue;
                }


                answerButtons[i]
                    .gameObject
                    .SetActive(true);


                answerButtons[i]
                    .interactable = true;


                answerTexts[i].text =
                    answers[i];


                answerButtons[i]
                    .onClick
                    .RemoveAllListeners();


                int answerIndex = i;


                answerButtons[i]
                    .onClick
                    .AddListener(
                        () =>
                            SelectAnswer(
                                answerIndex
                            )
                    );
            }


            answering = true;
        }


        private void SelectAnswer(
            int answerIndex)
        {
            if (!answering)
                return;


            answering = false;


            DisableButtons();


            QuizData quiz =
                quizController
                    .Questions[
                        currentQuestionIndex
                    ];


            bool correct =
                quizController.Answer(
                    quiz,
                    answerIndex
                );


            if (correct)
            {
                if (feedbackText != null)
                {
                    feedbackText.text =
                        "CORRECT!";
                }


                if (submitUI != null)
                {
                    submitUI.ShowBonus(
                        quiz.BonusScore
                    );
                }
            }
            else
            {
                if (feedbackText != null)
                {
                    feedbackText.text =
                        "WRONG";
                }
            }


            StartCoroutine(
                NextQuestion()
            );
        }


        private void DisableButtons()
        {
            foreach (Button button
                     in answerButtons)
            {
                button.interactable =
                    false;
            }
        }


        private IEnumerator NextQuestion()
        {
            yield return
                new WaitForSeconds(
                    nextQuestionDelay
                );


            currentQuestionIndex++;


            ShowQuestion();
        }


        private void FinishQuiz()
        {
            quizPanel.SetActive(false);


            Cursor.visible = false;

            Cursor.lockState =
                CursorLockMode.Locked;


            Debug.Log(
                "Quiz Finished! Total Score: " +
                GameSession.Instance.Score
            );
        }
    }
}
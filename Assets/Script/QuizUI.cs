using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeepScan
{
    public class QuizUI :
        MonoBehaviour
    {
        [Header("Logic")]

        [SerializeField]
        private QuizController quizController;


        [SerializeField]
        private ResultFlowController resultFlow;


        [Header("UI")]

        [SerializeField]
        private TMP_Text questionNumberText;

        [SerializeField]
        private TMP_Text questionText;

        [SerializeField]
        private Image questionImage;

        [SerializeField]
        private Button[] answerButtons;

        [SerializeField]
        private TMP_Text[] answerTexts;


        private int currentIndex;


        public void BeginQuiz()
        {
            quizController
                .BuildQuestions();


            currentIndex = 0;


            if (quizController
                .Questions.Count == 0)
            {
                FinishQuiz();
                return;
            }


            ShowQuestion();
        }


        private void ShowQuestion()
        {
            QuizData quiz =
                quizController
                    .Questions[
                        currentIndex
                    ];


            questionNumberText.text =
                "QUESTION " +
                (currentIndex + 1) +
                " / " +
                quizController
                    .Questions.Count;


            questionText.text =
                quiz.Question;


            if (questionImage != null)
            {
                questionImage.sprite =
                    quiz.QuestionImage;


                questionImage.gameObject
                    .SetActive(
                        quiz.QuestionImage != null
                    );
            }


            for (
                int i = 0;
                i < answerButtons.Length;
                i++)
            {
                int index = i;


                bool hasAnswer =
                    quiz.Answers != null &&
                    i < quiz.Answers.Length;


                answerButtons[i]
                    .gameObject
                    .SetActive(
                        hasAnswer
                    );


                if (!hasAnswer)
                    continue;


                answerTexts[i].text =
                    quiz.Answers[i];


                answerButtons[i]
                    .onClick
                    .RemoveAllListeners();


                answerButtons[i]
                    .onClick
                    .AddListener(
                        () =>
                        {
                            SelectAnswer(
                                index
                            );
                        }
                    );
            }
        }


        private void SelectAnswer(
            int answerIndex)
        {
            QuizData quiz =
                quizController
                    .Questions[
                        currentIndex
                    ];


            quizController.Answer(
                quiz,
                answerIndex
            );


            currentIndex++;


            if (currentIndex >=
                quizController
                    .Questions.Count)
            {
                FinishQuiz();
                return;
            }


            ShowQuestion();
        }


        private void FinishQuiz()
        {
            GameSession.Instance
                .CalculateFinalScore();


            resultFlow
                .ShowScore();
        }
    }
}
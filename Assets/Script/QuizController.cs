using System.Collections.Generic;
using UnityEngine;

namespace DeepScan
{
    public class QuizController :
        MonoBehaviour
    {
        [Header("Quiz Pool")]

        [SerializeField]
        private QuizData[] quizPool;


        [SerializeField]
        private int maximumQuestions = 3;


        private readonly List<QuizData>
            questions = new();


        public IReadOnlyList<QuizData>
            Questions => questions;


        public void BuildQuestions()
        {
            questions.Clear();


            if (GameSession.Instance == null)
                return;


            foreach (
                QuizData quiz
                in quizPool)
            {
                if (quiz == null)
                    continue;


                // เอาเฉพาะคำถามของ
                // ชนิดปลาที่จับได้
                if (!GameSession.Instance.HasCaught(
                    quiz.RelatedFish))
                {
                    continue;
                }


                questions.Add(
                    quiz
                );
            }


            Shuffle();


            while (
                questions.Count >
                maximumQuestions)
            {
                questions.RemoveAt(
                    questions.Count - 1
                );
            }
        }


        private void Shuffle()
        {
            for (
                int i = questions.Count - 1;
                i > 0;
                i--)
            {
                int random =
                    Random.Range(
                        0,
                        i + 1
                    );


                QuizData temp =
                    questions[i];


                questions[i] =
                    questions[random];


                questions[random] =
                    temp;
            }
        }


        public bool Answer(
            QuizData quiz,
            int answerIndex)
        {
            if (quiz == null)
                return false;


            bool correct =
                quiz.IsCorrect(
                    answerIndex
                );


            if (correct)
            {
                GameSession.Instance
                    .RegisterCorrectAnswer();
            }


            return correct;
        }
    }
}
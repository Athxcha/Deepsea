using System.Collections.Generic;
using UnityEngine;

namespace DeepScan
{
    public class QuizController :
        MonoBehaviour
    {
        private readonly List<QuizData>
            questions = new();


        public IReadOnlyList<QuizData>
            Questions => questions;


        public void BuildQuestions()
        {
            questions.Clear();


            GameSession session =
                GameSession.Instance;


            StageData stage =
                session.CurrentStage;


            if (stage == null)
                return;


            foreach (QuizData quiz
                     in stage.Quizzes)
            {
                if (quiz == null)
                    continue;


                if (!session.HasScanned(
                    quiz.RelatedFish))
                {
                    continue;
                }


                questions.Add(quiz);
            }


            Shuffle();


            while (questions.Count >
                   stage.MaximumQuestions)
            {
                questions.RemoveAt(
                    questions.Count - 1
                );
            }
        }


        private void Shuffle()
        {
            for (int i =
                     questions.Count - 1;
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
            bool correct =
                quiz.IsCorrect(
                    answerIndex
                );


            if (correct)
            {
                GameSession.Instance
                    .AddQuizScore(
                        quiz.BonusScore
                    );
            }


            return correct;
        }
    }
}
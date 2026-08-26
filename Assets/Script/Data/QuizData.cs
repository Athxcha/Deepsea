using UnityEngine;

namespace DeepScan
{
    [CreateAssetMenu(
        fileName = "Quiz_New",
        menuName = "DeepScan/Quiz Data"
    )]
    public class QuizData : GameData
    {
        [Header("Related Fish")]

        [SerializeField]
        private FishData relatedFish;


        [Header("Question")]

        [TextArea]
        [SerializeField]
        private string question;

        [SerializeField]
        private string[] answers;

        [SerializeField]
        private int correctAnswerIndex;


        [Header("Score")]

        [SerializeField]
        private int bonusScore = 20;


        public FishData RelatedFish =>
            relatedFish;

        public string Question =>
            question;

        public string[] Answers =>
            answers;

        public int BonusScore =>
            bonusScore;


        public bool IsCorrect(int index)
        {
            return index ==
                   correctAnswerIndex;
        }
    }
}
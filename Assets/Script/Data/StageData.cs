using UnityEngine;

namespace DeepScan
{
    [System.Serializable]
    public class FishSpawnEntry
    {
        public FishData fish;

        [Min(1)]
        public int amount = 1;
    }


    [CreateAssetMenu(
        fileName = "Stage_New",
        menuName = "DeepScan/Stage Data"
    )]
    public class StageData : GameData
    {
        [Header("Display")]

        [SerializeField]
        private string stageName;

        [TextArea]
        [SerializeField]
        private string description;


        [Header("Dive")]

        [SerializeField]
        private float diveDuration = 60f;


        [Header("Fish")]

        [SerializeField]
        private FishSpawnEntry[] fish;


        [Header("Quiz")]

        [SerializeField]
        private QuizData[] quizzes;

        [SerializeField]
        private int maximumQuestions = 3;


        [Header("Rewards")]

        [SerializeField]
        private RewardData[] rewards;


        public string StageName =>
            stageName;

        public string Description =>
            description;

        public float DiveDuration =>
            diveDuration;

        public FishSpawnEntry[] Fish =>
            fish;

        public QuizData[] Quizzes =>
            quizzes;

        public int MaximumQuestions =>
            maximumQuestions;

        public RewardData[] Rewards =>
            rewards;


        public bool ContainsFish(FishData fishData)
        {
            foreach (FishSpawnEntry entry
                     in fish)
            {
                if (entry.fish == fishData)
                    return true;
            }

            return false;
        }
    }
}
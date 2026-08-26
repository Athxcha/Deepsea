using UnityEngine;

namespace DeepScan
{
    [CreateAssetMenu(
        fileName = "Reward_New",
        menuName = "DeepScan/Reward Data"
    )]
    public class RewardData : GameData
    {
        [SerializeField]
        private string rewardName;

        [SerializeField]
        private Sprite icon;

        [SerializeField]
        private int requiredScore;


        public string RewardName =>
            rewardName;

        public Sprite Icon =>
            icon;

        public int RequiredScore =>
            requiredScore;
    }
}
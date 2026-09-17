using UnityEngine;
using UnityEngine.SceneManagement;

namespace DeepScan
{
    public class RewardButtons : MonoBehaviour
    {
        [Header("Scene Names")]

        [SerializeField]
        private string leaderboardScene = "Leaderboard";

        [SerializeField]
        private string mainMenuScene = "Lobby";


        public void GoToLeaderboard()
        {
            SceneManager.LoadScene(
                leaderboardScene
            );
        }


        public void GoToMainMenu()
        {
            SceneManager.LoadScene(
                mainMenuScene
            );
        }
    }
}
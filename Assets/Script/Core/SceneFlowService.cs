using UnityEngine;
using UnityEngine.SceneManagement;

namespace DeepScan
{
    public class SceneFlowService : MonoBehaviour
    {
        public static SceneFlowService Instance
        {
            get;
            private set;
        }


        [Header("Scene Names")]

        [SerializeField]
        private string lobbyScene =
            "Lobby";

        [SerializeField]
        private string loadingScene =
            "Loading";

        [SerializeField]
        private string underwaterScene =
            "Underwater";

        [SerializeField]
        private string resultScene =
            "Result";

        [SerializeField]
        private string leaderboardScene =
            "Leaderboard";


        public string PendingScene
        {
            get;
            private set;
        }


        public string LoadingMessage
        {
            get;
            private set;
        }


        // =====================================================
        // UNITY
        // =====================================================

        private void Awake()
        {
            if (Instance != null &&
                Instance != this)
            {
                Destroy(gameObject);
                return;
            }


            Instance = this;

            DontDestroyOnLoad(gameObject);
        }


        // =====================================================
        // ระบบใหม่
        //
        // Lobby -> Loading -> Underwater
        // =====================================================

        public void StartGame()
        {
            if (GameSession.Instance == null)
            {
                Debug.LogError(
                    "GameSession does not exist."
                );

                return;
            }


            GameSession.Instance
                .StartNewGame();


            PendingScene =
                underwaterScene;


            LoadingMessage =
                "DESCENDING...";


            SceneManager.LoadScene(
                loadingScene
            );
        }


        // =====================================================
        // ระบบเก่า Compatibility
        //
        // StageTerminal.cs ยังเรียก:
        // SceneFlowService.Instance.StartDive(stage);
        // =====================================================

        public void StartDive(StageData stage)
        {
            if (GameSession.Instance == null)
            {
                Debug.LogError(
                    "GameSession does not exist."
                );

                return;
            }


            GameSession.Instance
                .StartStage(stage);


            PendingScene =
                underwaterScene;


            LoadingMessage =
                "DESCENDING...";


            SceneManager.LoadScene(
                loadingScene
            );
        }


        // =====================================================
        // Oxygen หมด
        //
        // Underwater -> Loading -> Result
        // =====================================================

        public void Surface()
        {
            PendingScene =
                resultScene;


            LoadingMessage =
                "SURFACING...";


            SceneManager.LoadScene(
                loadingScene
            );
        }


        // =====================================================
        // LEADERBOARD
        // =====================================================

        public void OpenLeaderboard()
        {
            SceneManager.LoadScene(
                leaderboardScene
            );
        }


        // =====================================================
        // LOBBY
        // =====================================================

        public void ReturnToLobby()
        {
            SceneManager.LoadScene(
                lobbyScene
            );
        }


        // =====================================================
        // LOADING COMPLETE
        // LoadingController เรียกตัวนี้
        // =====================================================

        public void CompleteLoading()
        {
            if (string.IsNullOrEmpty(
                PendingScene))
            {
                Debug.LogError(
                    "PendingScene is empty."
                );

                return;
            }


            SceneManager.LoadScene(
                PendingScene
            );
        }
    }
}